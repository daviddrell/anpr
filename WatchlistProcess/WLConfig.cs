using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ApplicationDataClass;
using ErrorLoggingLib;
using LPREngineLib;
using System.Threading;
using Utilities;
using FrameGeneratorLib;
using System.IO;
using UserSettingsLib;

namespace WatchlistLib
{


    public partial class WatchLists
    {

        object m_LoadListSingleton;

        public List<WatchListControl> MasterList
        {
            set
            {
                lock (m_LoadListSingleton)
                {
                    m_WatchLists = value;
                    if (m_WatchLists == null)
                        m_WatchLists = new List<WatchListControl>();
                }
            }
            get
            {
                lock (m_LoadListSingleton)
                {
                    return (m_WatchLists);
                }
            }
        }

        public int ListCount
        {
            get
            {
                lock (m_LoadListSingleton)
                {
                    return (m_WatchLists.Count);
                }
            }
        }

        public int GetNextIndex()
        {
            int counter = 0;

            lock (m_LoadListSingleton)
            {
                foreach (WatchListControl list in m_WatchLists)
                {
                    if (list.Index == counter)
                    {
                        counter++;
                    }
                }
                return (counter);
            }
        }

        public void SaveList(WatchListControl list)
        {
            lock (m_LoadListSingleton)
            {
                UserSettings.Set(m_SettingsTags.WatchLists_UserAssignedGroupName[list.Index], list.UserAssignedGroupName);
                UserSettings.Set(m_SettingsTags.WatchLists_DataFilePath[list.Index], list.DataFileCompletePath);
                UserSettings.Set(m_SettingsTags.WatchLists_EmailFilePath[list.Index], list.EmailFileCompletePath);
                UserSettings.Set(m_SettingsTags.WatchLists_AlertThresh[list.Index], list.AlertThreshold.ToString());
            }
        }

        public void DeleteList(WatchListControl list)
        {
            lock (m_LoadListSingleton)
            {

                UserSettings.Remove(m_SettingsTags.WatchLists_UserAssignedGroupName[list.Index]);
                UserSettings.Remove(m_SettingsTags.WatchLists_DataFilePath[list.Index]);
                UserSettings.Remove(m_SettingsTags.WatchLists_EmailFilePath[list.Index]);
                UserSettings.Remove(m_SettingsTags.WatchLists_AlertThresh[list.Index]);
            }
        }

        /// <summary>
        /// Find a specified list(by supplied UserAssignedGroupName)and fills in the list configuration data into the supplied list reference
        /// </summary>
        /// <param name="listName"></param>
        /// <param name="list"></param>
        public void GetListFromUserConfig(string listName, WatchListControl list)
        {
            try
            {
                lock (m_LoadListSingleton)
                {

                    string[] tags = UserSettings.FindValues(listName);

                    // assume that the list name is unique and only one list has this name
                    if (tags.Length != 1) m_Log.Log("GetListFromConfig duplicate list name found", ErrorLog.LOG_TYPE.FATAL);

                    if (tags.Length == 0) return;

                    // get the index off the tag
                    string indexstr = ((tags[0].Split(',')[0]).Split('_'))[2];
                    int index;
                    try
                    {
                        index = Convert.ToInt32(indexstr);
                    }
                    catch
                    {
                        m_Log.Log("GetListFromConfig invalid index format", ErrorLog.LOG_TYPE.FATAL);
                        return;
                    }
                    list.Index = index;

                    list.DataFileCompletePath = UserSettings.Get(m_SettingsTags.WatchLists_DataFilePath[list.Index]);
                    list.EmailFileCompletePath = UserSettings.Get(m_SettingsTags.WatchLists_EmailFilePath[list.Index]);
                    string thresholdstring = UserSettings.Get(m_SettingsTags.WatchLists_AlertThresh[list.Index]);

                    try
                    {
                        list.AlertThreshold = Convert.ToInt32(thresholdstring);
                    }
                    catch
                    {
                        m_Log.Log("GetListFromConfig invalid AlertThreshold format", ErrorLog.LOG_TYPE.FATAL);
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                m_Log.Trace(ex, ErrorLog.LOG_TYPE.FATAL);
            }
        }

       
        public int loadList(WatchListControl list)
        {
            try
            {
                lock (m_LoadListSingleton)
                {
                    lock (list)
                    {
                        int i;
                        int numLines;

                        if (list.DataFileCompletePath == null) return (0);

                        string file = list.DataFileCompletePath;

                        if (!File.Exists(file))
                        {
                            list.WatchListLoaded = false;
                            return (0);
                        }

                        // open the file

                        string[] lines;
                        try
                        {


                            lines = File.ReadAllLines(file);


                            // count the lines
                            numLines = lines.Count();


                        }

                        catch (Exception ex)
                        {
                            m_Log.Log("loadList ArgumentException.. " + ex.Message + "\r\n\r\n" + file, ErrorLog.LOG_TYPE.FATAL);
                            return (0);

                        }



                        // allocate memory

                        list.WatchEntrys = new WatchedNumber[numLines];

                        // process the lines


                        i = 0;
                        foreach (string inputString in lines)
                        {
                            string[] ss;

                            // input should be of the form "123HTY, user comments, with any chars after the first comma"

                            ss = inputString.Split(','); // find the first comma
                            if (ss.Length < 2)
                            {
                                // we did not find a comma, so make one up

                                string[] newsplit = new string[2];
                                newsplit[0] = ss[0];
                                newsplit[1] = "from watch list: "+ Path.GetFileNameWithoutExtension(file);

                                ss = newsplit;
                              
                            }

                            if (ss[0].Length > 10) // max chars to search for 
                            {
                              
                                list.WatchListLoaded = false;
                                m_Log.Log("too many chars in plate string:\n\r" + ss[0] + " \n\r all lines must be of the form:\n\r123XZY, user comment text\n\rmaximum of 10 chars in watch item", ErrorLog.LOG_TYPE.FATAL);

                                return (0);
                            }

                            list.WatchEntrys[i] = new WatchedNumber();
                            list.WatchEntrys[i].Number = ss[0].ToUpper(); // plate number
                            list.WatchEntrys[i].Number = list.WatchEntrys[i].Number.Replace('O', '0'); // replace all ohs with zeros
                            list.WatchEntrys[i].UserComment = ss[1]; // user supplied comment text
                            i++;
                        }


                        list.WatchListLoaded = true;

                        return (numLines);
                    }
                }
            }
            catch (Exception ex)
            {
                m_Log.Trace(ex, ErrorLog.LOG_TYPE.FATAL);
                return (0);
            }

        }

      
        public bool UpdateListFile(int WatchIndex, string FileName)
        {
            try
            {
                lock (m_LoadListSingleton)
                {

                    WatchListControl wlItem = m_WatchLists[WatchIndex];

                    wlItem.DataFileCompletePath = FileName;


                    if (loadList(wlItem) < 1)
                    {
                        return false;
                    }
                    else
                    {  // if the file is changed by the user, re-load it automatically
                        setupFileWatcher(wlItem);
                    }

                    ReplaceItem(WatchIndex, wlItem);

                    return (true);

                }
            }
            catch (Exception ex)
            {
                m_Log.Trace(ex, ErrorLog.LOG_TYPE.FATAL);
                return (false);
            }
        }

        public bool UpdateEmailListFile(int WatchIndex, string FileName)
        {
            try
            {
                lock (m_LoadListSingleton)
                {
                    WatchListControl list = m_WatchLists[WatchIndex];
                    list.EmailFileCompletePath = FileName;

                    if (loadEmails(list) < 1)
                    {
                        return false;
                    }


                    ReplaceItem(WatchIndex, list);

                    return (true);

                }
            }
            catch (Exception ex)
            {
                m_Log.Trace(ex, ErrorLog.LOG_TYPE.FATAL);
                return (false);
            }
 
        }

        public int loadEmails(WatchListControl list)
        {
            try
            {
                lock (m_LoadListSingleton)
                {

                    string inputString;
                    int i;
                    int numLines;
                    StreamReader sr;
                    string file = list.EmailFileCompletePath;

                    if (!File.Exists(file))
                        return (0);

                    // open the file

                    try
                    {
                        sr = new StreamReader(file);

                        // count the lines

                        i = 0;
                        while ((inputString = sr.ReadLine()) != null)
                        {
                            if (inputString.Contains('@'))
                            {
                                i++;
                            }
                            else
                                break; // stop at the first non-address line
                        }
                        numLines = i;

                        sr.Close();
                    }

                    catch (Exception ex)
                    {
                        m_Log.Log("loadEmails ArgumentException.. " + ex.Message + file, ErrorLog.LOG_TYPE.FATAL);
                        return (0);

                    } // end catch


                    // allocate memory

                    list.WatchEmailAddresses = new string[numLines];


                    // open the file

                    sr = new StreamReader(file);

                    // process the lines

                    for (i = 0; i < numLines; i++)
                    {
                        list.WatchEmailAddresses[i] = sr.ReadLine();
                    }

                    sr.Close();


                    list.WatchEmailLoaded = true;

                    return (numLines);
                }
            }
            catch (Exception ex)
            {
                m_Log.Trace(ex, ErrorLog.LOG_TYPE.FATAL);
                return (0);
            }

        }

        public static void setupFileWatcher(WatchListControl list)
        {

            try
            {
                m_WatchFileChangedList = new ThreadSafeList<string>(100);

                FileInfo fi = new FileInfo(list.DataFileCompletePath);

                string fileName = fi.Name;
                string directory = fi.Directory.ToString();

                // Create a new FileSystemWatcher and set its properties.
                list.Watcher = new FileSystemWatcher();
                list.Watcher.Path = directory;
                list.Watcher.Filter = fileName; // filter out all but this one file, its all we want to watch
                /* Watch for changes in LastAccess and LastWrite times */
                //    wlItem.watcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite;
                list.Watcher.NotifyFilter = NotifyFilters.LastWrite;

                // Add event handlers.
                list.Watcher.Changed += new FileSystemEventHandler(OnWatchFileChanged);
                // Begin watching.
                list.Watcher.EnableRaisingEvents = true;

            }
            catch (Exception ex)
            {
                ErrorLog.Trace(ex );
            }

        }

        private static void OnWatchFileChanged(object source, FileSystemEventArgs e)
        {
            String changedFile = e.FullPath;
            // does this exist already - we get two hits per change
            if (!m_WatchFileChangedList.Contains(changedFile))
                m_WatchFileChangedList.Add(changedFile);
        }

        /// <summary>
        /// returns the index of the watch list item in the list of watch lists.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public int GetIndexOf(WatchListControl item)
        {
            try
            {
                lock (m_LoadListSingleton)
                {
                    if (m_WatchLists.Contains(item))
                        return (m_WatchLists.IndexOf(item));
                    else
                        return (-1);
                }
            }
            catch (Exception ex)
            {
                m_Log.Trace(ex, ErrorLog.LOG_TYPE.FATAL);
                return (0);
            }

        }

        /// <summary>
        /// returns the index (from the list of watchlists) of the watch list that has the specified watch data file
        /// </summary>
        /// <param name="name"></param>
        /// <param name="startIndex">index offset in the master list of watchlists</param>
        /// <returns></returns>
        public int GetIndexOfFileName(string name, int startIndex)
        {
            try
            {
                lock (m_LoadListSingleton)
                {
                    for (int i = startIndex; i < m_WatchLists.Count(); i++)
                    {
                        WatchListControl wl = m_WatchLists[i];

                        if (wl.DataFileCompletePath.Contains(name))
                            return (m_WatchLists.IndexOf(wl));

                        if (wl.EmailFileCompletePath.Contains(name))
                            return (m_WatchLists.IndexOf(wl));

                    }
                    return (-1);
                }
            }
            catch (Exception ex)
            {
                m_Log.Trace(ex, ErrorLog.LOG_TYPE.FATAL);
                return (0);
            }

        }

        public int GetIndexOfFileName(string name)
        {
            try
            {
                lock (m_LoadListSingleton)
                {
                    foreach (WatchListControl wl in m_WatchLists)
                    {
                        if (wl.DataFileCompletePath.Contains(name))
                            return (m_WatchLists.IndexOf(wl));

                        if (wl.EmailFileCompletePath.Contains(name))
                            return (m_WatchLists.IndexOf(wl));

                    }
                    return (-1);
                }
            }
            catch (Exception ex)
            {
                m_Log.Trace(ex, ErrorLog.LOG_TYPE.FATAL);
                return (0);
            }

        }

        /// <summary>
        /// returns the index (from the list of watchlists) based on the user supplied list name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public int GetIndexOf(string name)
        {
            try
            {
                lock (m_LoadListSingleton)
                {
                    foreach (WatchListControl wl in m_WatchLists)
                    {
                        if (wl.UserAssignedGroupName.Equals(name))
                            return (m_WatchLists.IndexOf(wl));
                    }
                    return (-1);
                }
            }
            catch (Exception ex)
            {
                m_Log.Trace(ex, ErrorLog.LOG_TYPE.FATAL);
                return (0);
            }
        }

        public bool isDuplicateName(string name)
        {
            try
            {
                lock (m_LoadListSingleton)
                {
                    foreach (WatchListControl wl in m_WatchLists)
                    {
                        if (wl.UserAssignedGroupName.Equals(name))
                            return (true);
                    }
                    return (false);
                }
            }
            catch (Exception ex)
            {
                m_Log.Trace(ex, ErrorLog.LOG_TYPE.FATAL);
                return (false);
            }
        }

        public void ReplaceItem(WatchListControl oldItem, WatchListControl newItem)
        {
            try
            {
                lock (m_LoadListSingleton)
                {
                    if (m_WatchLists.Contains(oldItem))
                    {
                        m_WatchLists.Remove(oldItem);
                        RemoveItemFromFile(oldItem);
                    }

                    m_WatchLists.Add(newItem);

                    SaveItemToFile(newItem);
                }
            }
            catch (Exception ex)
            {
                m_Log.Trace(ex, ErrorLog.LOG_TYPE.FATAL);

            }
        }

        public void ReplaceItem(int index, WatchListControl newItem)
        {
            try
            {
                lock (m_LoadListSingleton)
                {
                    if (index >= 0 && index < m_WatchLists.Count)
                    {
                        m_WatchLists.RemoveAt(index);
                        m_WatchLists.Add(newItem);
                    }

                    SaveItemToFile(newItem);
                }
            }
            catch (Exception ex)
            {
                m_Log.Trace(ex, ErrorLog.LOG_TYPE.FATAL);

            }


        }

        public void AddItem(WatchListControl item)
        {
            try
            {
                lock (m_LoadListSingleton)
                {
                    int index = GetIndexOf(item.UserAssignedGroupName);
                    if (index > -1) return;// this name already exists

                    if (!m_WatchLists.Contains(item))
                    {
                        m_WatchLists.Add(item);
                        SaveItemToFile(item);
                    }
                }
            }
            catch (Exception ex)
            {
                m_Log.Trace(ex, ErrorLog.LOG_TYPE.FATAL);

            }

        }

        /// <summary>
        /// Removes a watch list from the master list by watch list UserAssginedGroupName
        /// </summary>
        /// <param name="name"></param>
        public void RemoveItem(string name)
        {
            try
            {
                lock (m_LoadListSingleton)
                {
                    foreach (WatchListControl list in m_WatchLists)
                    {
                        if (list.UserAssignedGroupName == name)
                        {
                            m_WatchLists.Remove(list);
                            RemoveItemFromFile(list);// remove from the reference from the config file
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                m_Log.Trace(ex, ErrorLog.LOG_TYPE.FATAL);

            }

        }

    


        /// <summary>
        /// Removes a watchlist from the master list by object reference
        /// </summary>
        /// <param name="item"></param>
        public void RemoveItem(WatchListControl item)
        {
            try
            {
                lock (m_LoadListSingleton)
                {
                    if (m_WatchLists.Contains(item))
                    {
                        m_WatchLists.Remove(item);
                    }
                    RemoveItemFromFile(item);
                }
            }
            catch (Exception ex)
            {
                m_Log.Trace(ex, ErrorLog.LOG_TYPE.FATAL);

            }

        }

        void RemoveItemFromFile(WatchListControl list)
        {
            try
            {
                lock (m_LoadListSingleton)
                {
                    if (list == null) return;

                    UserSettings.Remove(m_SettingsTags.WatchLists_UserAssignedGroupName[list.Index]);
                    UserSettings.Remove(m_SettingsTags.WatchLists_DataFilePath[list.Index]);
                    UserSettings.Remove(m_SettingsTags.WatchLists_EmailFilePath[list.Index]);
                    UserSettings.Remove(m_SettingsTags.WatchLists_AlertThresh[list.Index]);

                    // Re-load the lists
                    LoadListsFromUserConfig();
                }
            }
            catch (Exception ex)
            {
                m_Log.Trace(ex, ErrorLog.LOG_TYPE.FATAL);

            }

        }

        void SaveItemToFile(WatchListControl list)
        {
            try
            {
                lock (m_LoadListSingleton)
                {
                    if (list == null) return;


                    UserSettings.Set(m_SettingsTags.WatchLists_UserAssignedGroupName[list.Index], list.UserAssignedGroupName);
                    UserSettings.Set(m_SettingsTags.WatchLists_DataFilePath[list.Index], list.DataFileCompletePath);
                    UserSettings.Set(m_SettingsTags.WatchLists_EmailFilePath[list.Index], list.EmailFileCompletePath);
                    UserSettings.Set(m_SettingsTags.WatchLists_AlertThresh[list.Index], list.AlertThreshold.ToString());


                    LoadListsFromUserConfig();
                }
            }
            catch (Exception ex)
            {
                m_Log.Trace(ex, ErrorLog.LOG_TYPE.FATAL);

            }
        }
       

        public int GetCount()
        {
            try
            {
                lock (m_LoadListSingleton)
                {
                    return (m_WatchLists.Count);
                }
            }
            catch (Exception ex)
            {
                m_Log.Trace(ex, ErrorLog.LOG_TYPE.FATAL);
                return (0);
            }
        }

        public int GetListCount(int index)
        {
            try
            {
                lock (m_LoadListSingleton)
                {
                    if (index >= 0 && index < m_WatchLists.Count)
                    {
                        if (m_WatchLists[index].WatchEntrys == null) return (0);

                        return (m_WatchLists[index].WatchEntrys.Count());
                    }
                    else
                        return (0);
                }
            }
            catch (Exception ex)
            {
                m_Log.Trace(ex, ErrorLog.LOG_TYPE.FATAL);
                return (0);
            }
        }

        public void checkLists()
        {
            try
            {
                int index = 0;
                if (m_WatchFileChangedList == null) return;

                for (int f = m_WatchFileChangedList.Count() - 1; f >= 0; f--)
                {
                    string file = m_WatchFileChangedList[f];
                    if (file == null) continue;

                    index = GetIndexOfFileName(file);
                    if (index >= 0)
                    {
                        loadList(GetListAtIndex(index));
                    }
                    m_WatchFileChangedList.RemoveAt(f);
                }

            }
            catch (Exception ex)
            {
                m_Log.Trace(ex, ErrorLog.LOG_TYPE.FATAL);

            }
        }

        WatchListControl GetListAtIndex(int index)
        {
            try
            {
                lock (m_LoadListSingleton)
                {
                    foreach (WatchListControl list in m_WatchLists)
                    {
                        if (list.Index == index) return (list);
                    }
                    return (null);
                }
            }
            catch (Exception ex)
            {
                m_Log.Trace(ex, ErrorLog.LOG_TYPE.FATAL);
                return (null);
            }
        }

        int GetListIndex(string groupName)
        {
            try
            {
                lock (m_LoadListSingleton)
                {
                    foreach (WatchListControl list in m_WatchLists)
                    {
                        if (list.UserAssignedGroupName == groupName) return (list.Index);
                    }
                    return (-1);
                }
            }
            catch (Exception ex)
            {
                m_Log.Trace(ex, ErrorLog.LOG_TYPE.FATAL);
                return (0);
            }
        }

        public bool NameIsADuplicate(WatchLists.WatchListControl newListInEditMode)
        {
            try
            {
                foreach (WatchLists.WatchListControl list in m_WatchLists)
                {
                    if (list.UserAssignedGroupName.Equals(newListInEditMode.UserAssignedGroupName))
                    {
                        return true;
                    }
                }
                return (false);
            }
            catch (Exception ex)
            {
                m_Log.Trace(ex, ErrorLog.LOG_TYPE.FATAL);
                return (false);
            }
        }

        public List<WatchListControl> LoadListsFromUserConfig()
        {
            lock (m_LoadListSingleton)
            {
                int listIndex;
                List<WatchListControl> watchLists = new List<WatchListControl>();

                try
                {
                    // count how many watch lists are named in the user config file
                    int listCount = UserSettings.GetCountByPartial(m_SettingsTags.WatchLists_UserAssignedGroupName.GetBaseTag());

                    for (int i = 0; i < listCount; i++)
                    {
                        bool returnWholeLine = true;
                        string listNameInputLine = UserSettings.GetNextByPartial(i, m_SettingsTags.WatchLists_UserAssignedGroupName.GetBaseTag(), returnWholeLine);
                        if (listNameInputLine == null) break;

                        try
                        {
                            string[] sp1 = listNameInputLine.Split(',');
                            string[] sp2 = sp1[0].Split('_');
                            listIndex = Convert.ToInt32(sp2[2]);
                        }
                        catch (Exception ex)
                        {
                            m_Log.Log("Exception on convert Watch list index :" + ex.Message, ErrorLog.LOG_TYPE.FATAL);
                            break;
                        }

                        WatchListControl newList = new WatchListControl(listIndex);

                        // look for the data in the user config file

                        newList.UserAssignedGroupName = (listNameInputLine.Split(','))[1];

                        returnWholeLine = false;

                        newList.DataFileCompletePath = UserSettings.Get(m_SettingsTags.WatchLists_DataFilePath[listIndex]);
                        if (newList.DataFileCompletePath == null) newList.DataFileCompletePath = "";

                        newList.EmailFileCompletePath = UserSettings.Get(m_SettingsTags.WatchLists_EmailFilePath[listIndex]);
                        if (newList.EmailFileCompletePath == null) newList.EmailFileCompletePath = "";

                        string alertThreshold = UserSettings.Get(m_SettingsTags.WatchLists_AlertThresh[listIndex]);

                        try
                        {
                            newList.AlertThreshold = Convert.ToInt32(alertThreshold);
                        }
                        catch
                        {
                            newList.AlertThreshold = 70;
                        }

                        newList.EmailsEnabled = false;

                        if (loadEmails(newList) > 0)
                        {
                            newList.EmailsEnabled = true;
                        }

                        watchLists.Add(newList);

                        if (newList.DataFileCompletePath.Length > 2)
                        {
                            if (loadList(newList) < 1)
                            {
                                m_Log.Log("Error loading watch list: no watch numbers found in list: " + newList.UserAssignedGroupName, ErrorLog.LOG_TYPE.FATAL);
                            }
                            else
                                setupFileWatcher(newList);
                        }
                        else
                        {
                            // this a brand new list - the user has not had a chance to select the data file yet - do nothing
                        }
                    }
                }
                catch (Exception ex)
                {
                    m_Log.Log("Watch lists  LoadListsFromUserConfig ex: " + ex.Message, ErrorLog.LOG_TYPE.FATAL);
                    return (null);
                }

                return (watchLists);
            }
        }

    }



}
