using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using FileLoggingLib;

namespace UserSettingsLib
{
   
   
    public class UserSettings
    {
        //  static string UserSettingsFileName = "user.config";

        // define a record as: "setting name string", "setting value string" \n
        // the file will contain as many lines as records
        static string m_UserSettingsPath = null;
        static object m_singleton=null;
        static string m_ApplicationRedirectedAppPath = null;

        static void GetSingleton()
        {
            if (m_singleton == null) m_singleton = new object();
        }

        /// <summary>
        /// used to allow the configure PSS app to override its own OS assigned path and create the same path as the LPR service useses
        /// </summary>
        /// <param name="appPath"></param>
        public static void SetNewApplicationPath(string appPath)
        {
            m_ApplicationRedirectedAppPath = appPath;
        }

        public static void Set(string userSettingPath, string setting, string value)
        {
            GetSingleton();

            lock (m_singleton)
            {
                m_UserSettingsPath = userSettingPath;
                Set(setting, value);
                m_UserSettingsPath = null;
            }
        }

        public static string GetAppPath()
        {
            return "C:\\FirstEvidence\\user.config";
        }

        public static void Set(string setting, string value)
        {
            GetSingleton();

            lock (m_singleton)
            {
                string inputString;
                string[] inputFields;
                inputFields = new string[2];
                bool settingDone = false;


                string UserSettingsFileName;
                UserSettingsFileName  = GetAppPath();


                // does file exist already ?
                try
                {
                    if (File.Exists(UserSettingsFileName))
                    {
                        // load the records into memory
                        //    open the file for reading
                        StreamReader fileReader;
                        StreamWriter fileWriter;
                        string NewSettingsName;
                        string NewSettingsValue;

                        //  COUNT THE ENTRIES

                        fileReader = new StreamReader(UserSettingsFileName);

                        int i = 0;
                        while ((inputString = fileReader.ReadLine()) != null)
                        {
                            if (inputString.Contains(",")) i++;
                        }
                        if (i == 0) // error
                        {
                            // MessageBox.Show("file user.config exists but is empty");
                        }

                        // ALLOCATE MEMORY

                        int numLines = i;
                        //     MessageBox.Show("numLines = "+ numLines.ToString());

                        fileReader.Close();

                        // LOAD THE LINES

                        fileReader = new StreamReader(UserSettingsFileName);

                        i = 0;
                        string[] SettingsName;
                        string[] SettingsValue;
                        SettingsName = new string[numLines];
                        SettingsValue = new string[numLines];

                        while ((inputString = fileReader.ReadLine()) != null)
                        {

                            if (!inputString.Contains(","))
                                break;

                            inputFields = inputString.Split(',');

                            SettingsName[i] = inputFields[0];
                            SettingsValue[i] = inputFields[1];
                            if (SettingsName[i] == setting)
                            {
                                SettingsValue[i] = value;
                                settingDone = true;
                            }
                            i++;
                        } // end while


                        // now we have either changed an existing setting or will add a new one, 
                        //        write all settings to the file

                        fileReader.Close();

                        // delete the old file and write the new one

                        File.Delete(UserSettingsFileName);

                        fileWriter = new StreamWriter(UserSettingsFileName);

                        for (i = 0; i < numLines; i++)
                        {
                            fileWriter.WriteLine(SettingsName[i] + "," + SettingsValue[i]);
                        }

                        if (!settingDone)  // we did not find an existing setting to change, add a new one
                        {
                            NewSettingsName = setting;
                            NewSettingsValue = value;
                            fileWriter.WriteLine(NewSettingsName + "," + NewSettingsValue);
                        }


                        fileWriter.Close();

                    }// end if file exists
                    else // the files has not been created, so creat it and store the settings
                    {

                        StreamWriter fileWriter;
                        string NewSettingsName;
                        string NewSettingsValue;

                        NewSettingsName = setting;
                        NewSettingsValue = value;

                        fileWriter = new StreamWriter(UserSettingsFileName);

                        fileWriter.WriteLine(NewSettingsName + "," + NewSettingsValue);

                        fileWriter.Close();
                    }
                }// end try

                catch (ArgumentException)
                {
                    FileLogging.Set("ArgumentException.. " + UserSettingsFileName);
                } // end catch
                catch (IOException)
                {
                    FileLogging.Set("IOException.. " + UserSettingsFileName);
                } // end catch
                catch (UnauthorizedAccessException)
                {
                    FileLogging.Set("UnauthorizedAccessException.. " + UserSettingsFileName);
                } // end catch

            }
        } // end Set



        public static void Remove(string userSettingPath, string setting)
        {
            GetSingleton();

            lock (m_singleton)
            {
                m_UserSettingsPath = userSettingPath;
                Remove(setting);
                m_UserSettingsPath = null;
            }
        }


        public static void Remove(string setting)
        {
            GetSingleton();

            lock (m_singleton)
            {
                string inputString;
                string[] inputFields;
                inputFields = new string[2];


                string UserSettingsFileName;
                UserSettingsFileName = GetAppPath();
              

                // does file exist already ?
                try
                {
                    if (File.Exists(UserSettingsFileName))
                    {
                        // load the records into memory
                        //    open the file for reading
                        StreamReader fileReader;
                        StreamWriter fileWriter;

                        //  COUNT THE ENTRIES

                        fileReader = new StreamReader(UserSettingsFileName);

                        int i = 0;
                        while ((inputString = fileReader.ReadLine()) != null)
                        {
                            if (inputString.Contains(",")) i++;
                        }
                        if (i == 0) // error
                        {
                            // MessageBox.Show("file user.config exists but is empty");
                        }

                        // ALLOCATE MEMORY

                        int numLines = i;
                        //     MessageBox.Show("numLines = "+ numLines.ToString());

                        fileReader.Close();

                        // LOAD THE LINES

                        fileReader = new StreamReader(UserSettingsFileName);

                        i = 0;
                        string[] SettingsName;
                        string[] SettingsValue;
                        SettingsName = new string[numLines];
                        SettingsValue = new string[numLines];

                        while ((inputString = fileReader.ReadLine()) != null)
                        {

                            if (!inputString.Contains(","))
                                break;

                            inputFields = inputString.Split(',');

                            SettingsName[i] = inputFields[0];
                            SettingsValue[i] = inputFields[1];
                            i++;
                        } // end while


                        // now we have either changed an existing setting or will add a new one, 
                        //        write all settings to the file except the one being removed

                        fileReader.Close();

                        // delete the old file and write the new one

                        File.Delete(UserSettingsFileName);

                        fileWriter = new StreamWriter(UserSettingsFileName);

                        for (i = 0; i < numLines; i++)
                        {

                            if (SettingsName[i] == setting) continue; // the delete 

                            fileWriter.WriteLine(SettingsName[i] + "," + SettingsValue[i]);
                        }


                        fileWriter.Close();

                    }// end if file exists

                }// end try

                catch (ArgumentException)
                {
                    FileLogging.Set("ArgumentException.. " + UserSettingsFileName);
                } // end catch
                catch (IOException)
                {
                    FileLogging.Set("IOException.. " + UserSettingsFileName);
                } // end catch
                catch (UnauthorizedAccessException)
                {
                    FileLogging.Set("UnauthorizedAccessException.. " + UserSettingsFileName);
                } // end catch

            }
        } // end Remove




        public static string Get(string userSettingPath, string setting)
        {
            GetSingleton();

            lock (m_singleton)
            {
                m_UserSettingsPath = userSettingPath;
                string retVal = Get(setting);
                m_UserSettingsPath = null;

                return (retVal);
            }
        }


        public static string Get(string setting)
        {
            GetSingleton();

            lock (m_singleton)
            {
                string inputString;
                string[] inputFields;
                bool readingDone = false;
                string value = null;

                string UserSettingsFileName;
                UserSettingsFileName = GetAppPath();
               

                // does file exits already ?

                if (File.Exists(UserSettingsFileName))
                {
                    // load the records into memory
                    //    open the file for reading
                    try
                    {
                        StreamReader fileReader;
                        string SettingsName;
                        string SettingsValue;

                        fileReader = new StreamReader(UserSettingsFileName);

                        while ((inputString = fileReader.ReadLine()) != null)
                        {
                            if (!inputString.Contains(","))
                                break;
                            inputFields = inputString.Split(',');
                            SettingsName = inputFields[0];
                            SettingsValue = inputFields[1];
                            if (SettingsName == setting)
                            {
                                value = SettingsValue;
                                readingDone = true;
                                break;
                            }
                        } // end while
                        if (!readingDone)  // we did not find an existing setting to change, add a new one
                        {
                            //         MessageBox.Show("did not find string -" + setting + "- in file");
                            fileReader.Close();
                            return null;
                        }

                        fileReader.Close();

                    }
                    catch (Exception ex)
                    {
                        FileLogging.Set("Get exception : " + ex.Message);
                        return (null);
                    }

                }// end if file exists

                else // the files has not been created, so this is an error
                {
                    // MessageBox.Show("could not open user.config file");
                    return null;
                }

                return value;
            }
        } // end get

        /// <summary>
        /// Returns all tags which contain the search string 
        /// </summary>
        /// <param name="partial"></param>
        /// <returns></returns>
        public static string[] FindTag(string partial)
        {
            GetSingleton();
            List<string> results = new List<string>();

            lock (m_singleton)
            {
                string inputString;
                string[] inputFields;


                string UserSettingsFileName;
                UserSettingsFileName = GetAppPath();

                if (File.Exists(UserSettingsFileName))
                {
                    // load the records into memory
                    //    open the file for reading
                    try
                    {
                        StreamReader fileReader;
                        string SettingsName;
                        string SettingsValue;

                        fileReader = new StreamReader(UserSettingsFileName);

                        while ((inputString = fileReader.ReadLine()) != null)
                        {
                            if (!inputString.Contains(","))
                                continue;
                            inputFields = inputString.Split(',');
                            SettingsName = inputFields[0];
                            SettingsValue = inputFields[1];
                            if (SettingsName.Contains(partial))
                            {
                                results.Add(SettingsName);
                            }
                        } // end while
                        fileReader.Close();

                    }
                    catch (Exception ex)
                    {
                        FileLogging.Set("GetCountByPartial exception : " + ex.Message);
                        return null;
                    } // end try

                }// end if file exists

                else // the files has not been created, so this is an error
                {
                    // MessageBox.Show("could not open user.config file");
                    return null;
                }

                return results.ToArray();
            }
        }


        /// <summary>
        /// Returns all tags for settings in which the value equals the search string
        /// </summary>
        /// <param name="partial"></param>
        /// <returns></returns>
        public static string[] FindValues(string searchString)
        {
            GetSingleton();
            List<string> results = new List<string>();

            lock (m_singleton)
            {
                string inputString;
                string[] inputFields;


                string UserSettingsFileName;
                UserSettingsFileName = GetAppPath();


                if (File.Exists(UserSettingsFileName))
                {
                    // load the records into memory
                    //    open the file for reading
                    try
                    {
                        StreamReader fileReader;
                        string SettingsName;
                        string SettingsValue;

                        fileReader = new StreamReader(UserSettingsFileName);

                        while ((inputString = fileReader.ReadLine()) != null)
                        {
                            if (!inputString.Contains(","))
                                continue;
                            inputFields = inputString.Split(',');
                            SettingsName = inputFields[0];
                            SettingsValue = inputFields[1];
                            if (SettingsValue.Equals(searchString))
                            {
                                results.Add(SettingsName);
                            }
                        } // end while
                        fileReader.Close();

                    }
                    catch (Exception ex)
                    {
                        FileLogging.Set("GetCountByPartial exception : " + ex.Message);
                        return null;
                    } // end try

                }// end if file exists

                else // the files has not been created, so this is an error
                {
                    // MessageBox.Show("could not open user.config file");
                    return null;
                }

                return results.ToArray();
            }
        }



        /// <summary>
        /// Gets a count of setting tags that contain the string portion specified. Specify the settings file path.
        /// </summary>
        /// <param name="userSettingPath"></param>
        /// <param name="partial"></param>
        /// <returns></returns>

        public static int GetCountByPartial(string userSettingPath, string partial)
        {
            GetSingleton();

            lock (m_singleton)
            {
                m_UserSettingsPath = userSettingPath;
                int retVal = GetCountByPartial(partial);
                m_UserSettingsPath = null;

                return (retVal);
            }
        }

        /// <summary>
        /// Gets a count of setting tags that contain the string portion specified.
        /// </summary>
        /// <param name="partial"></param>
        /// <returns></returns>
        public static int GetCountByPartial(string partial)
        {
            GetSingleton();

            lock (m_singleton)
            {
                string inputString;
                string[] inputFields;

                int count = 0;


                string UserSettingsFileName;
                UserSettingsFileName = GetAppPath();


                if (File.Exists(UserSettingsFileName))
                {
                    // load the records into memory
                    //    open the file for reading
                    try
                    {
                        StreamReader fileReader;
                        string SettingsName;
                        string SettingsValue;

                        fileReader = new StreamReader(UserSettingsFileName);

                        while ((inputString = fileReader.ReadLine()) != null)
                        {
                            if (!inputString.Contains(","))
                                continue;
                            inputFields = inputString.Split(',');
                            SettingsName = inputFields[0];
                            SettingsValue = inputFields[1];
                            if (SettingsName.Contains(partial))
                            {
                                count++;
                            }
                        } // end while
                        fileReader.Close();

                    }
                    catch (Exception ex)
                    {
                        FileLogging.Set("GetCountByPartial exception : " + ex.Message);
                        return (0);
                    } // end try

                }// end if file exists

                else // the files has not been created, so this is an error
                {
                    // MessageBox.Show("could not open user.config file");
                    return 0;
                }

                return count;
            }
        }


        public static string GetNextByPartial(string userSettingPath, int index, string partial, bool wholeLine)
        {
            GetSingleton();

            lock (m_singleton)
            {
                m_UserSettingsPath = userSettingPath;
                string retVal = GetNextByPartial(index, partial, wholeLine);
                m_UserSettingsPath = null;

                return (retVal);
            }
        }


        public static string GetNextByPartial(int index, string partial, bool wholeLine)
        {
            GetSingleton();

            lock (m_singleton)
            {
                string inputString;
                string[] inputFields;

                int count = 0;


                string UserSettingsFileName;
                UserSettingsFileName = GetAppPath();
               


                if (File.Exists(UserSettingsFileName))
                {
                    // load the records into memory
                    //    open the file for reading
                    try
                    {
                        StreamReader fileReader;
                        string SettingsName;
                        string SettingsValue;

                        fileReader = new StreamReader(UserSettingsFileName);

                        inputString = " ";

                        while ((inputString = fileReader.ReadLine()) != null )
                        {
                            if (!inputString.Contains(",")) continue;
                               
                            inputFields = inputString.Split(',');
                            SettingsName = inputFields[0];
                            SettingsValue = inputFields[1];
                            if (SettingsName.Contains(partial))
                            {
                                if (count == index)
                                {
                                    fileReader.Close();
                                    if (wholeLine)
                                        return (inputString);
                                    else
                                        return(SettingsValue);
                                }
                                count++;
                            }
                        } // end while
                        fileReader.Close();

                    }
                    catch (Exception ex)
                    {
                        FileLogging.Set("GetCountByPartial exception : " + ex.Message);
                        return (null);
                    } // end try

                }// end if file exists

                else // the files has not been created, so this is an error
                {
                    // MessageBox.Show("could not open user.config file");
                    return null;
                }

                return null;
            }
        }




    }// end class userSettings


}
