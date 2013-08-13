using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UserSettingsLib
{

    public class WatchListDynamicTags
    {
        public WatchListDynamicTags()
        {
            WatchLists_UserAssignedGroupName = new WatchLists_UserAssignedGroupNameClass();
            WatchLists_DataFilePath = new WatchLists_DataFilePathClass();
            WatchLists_EmailFilePath = new WatchLists_EmailFilePathClass();
            WatchLists_AlertThresh = new WatchLists_AlertThreshClass();
        }

        //////////////////////////////////////////////////////
        // Watch Lists
        public WatchLists_UserAssignedGroupNameClass WatchLists_UserAssignedGroupName;
        public class WatchLists_UserAssignedGroupNameClass
        {
            public string this[int index]
            {
                get
                {
                    string tag = "WatchLists_UserAssignedGroupName";
                    return (tag + "_" + index.ToString());
                }
            }
            public string GetBaseTag()
            {
                return("WatchLists_UserAssignedGroupName" );
            }
        }
  

        public WatchLists_DataFilePathClass WatchLists_DataFilePath;
        public class WatchLists_DataFilePathClass
        {
            public string this[int index]
            {
                get
                {
                    string tag = "WatchLists_DataFilePath";
                    return (tag + "_" + index.ToString());
                }
            }
        }

        public WatchLists_EmailFilePathClass WatchLists_EmailFilePath;
        public  class WatchLists_EmailFilePathClass
        {
            public string this[int index]
            {
                get
               {
                   string tag = "WatchLists_EmailFilePath";
                   return (tag + "_" + index.ToString());
               }
           }
       }

        public WatchLists_AlertThreshClass WatchLists_AlertThresh;
        public class WatchLists_AlertThreshClass
        {
            public string this[int index]
            {
                get
                {
                    string tag = "WatchLists_AlertThresh";
                    return (tag + "_" + index.ToString());
                }
            }
        }

    }

    public class UserSettingTags
    {
       
        //////////////////////////////////////////////////////
        // boolian
        public static string BOOL_TRUE = "true";
        public static string BOOL_FALSE = "false";

        //////////////////////////////////////////////////////
        // video setup
        public static string VideoSetup_PAL_NTSC = "VideoSetup_PAL_NTSC";
        public static string VideoSetup_PAL = "PAL";
        public static string VideoSetup_NTSC = "NTSC";

        //////////////////////////////////////////////////////
        // AnalystsWorkstation
        //////////////////////////////////////////////////////
        //

        public static string AW_SingleFileLastLocation = "AW_SingleFileLastLocation";
        public static string AW_LastDirectory = "DVRUseExternalStorage";
        public static string AW_OCRLibSourceDirectory = "AW_OCRLibSourceDirectory";
        public static string AW_OCRLibDestinationDirectory = "AW_OCRLibDestinationDirectory";


        //////////////////////////////////////////////////////
        // DVR
        //////////////////////////////////////////////////////
        //
        public static string DVR_UserSpecifiedStoragePath = "DVR_UserSpecifiedStoragePath";
        public static string DVR_StoreToUserSpecifiedFolder = "DVR_StoreToUserSpecifiedFolder";
        public static string DVR_StorageMode = "DVR_StorageMode";
        public static string DVR_StorageModeValueStoreOnPlate = "DVR_StorageModeValueStoreOnPlate";
        public static string DVR_StorageModeValueStoreOnMotion = "DVR_StorageModeValueStoreOnMotion";

        //////////////////////////////////////////////////////
        //
        //  Archive Selection
        public static string ArchiveLocaton = "ArchiveLocaton";


        //////////////////////////////////////////////////////
        //
        //  Source Channel ID/Names

        public static class ChannelNames
        {
            static string tag = "SrcChanName_";
            public static string Name(int index)
            {
                return (tag + "_" + index.ToString());
            }
        }

        public static string ChannelNotUsed = "NOT USED";

      

        //////////////////////////////////////////////////////
        //
        //  Passwords
        public static string PWControlCenterAdmin = "PWControlCenterAdmin";
        public static string PWControlCenterViewer = "PWControlCenterViewer";

        public static string PWLPRServiceAdmin = "PWLPRServiceAdmin";
        public static string PWLPRServiceViewer = "PWLPRServiceViewer";

        //////////////////////////////////////////////////////
        //
        //  GPS Settings

        public static string GPSLocationFixedLatDegrees = "GPSLocationFixedLatDegrees";
        public static string GPSLocationFixedLatMinutes = "GPSLocationFixedLatMinutes";
        public static string GPSLocationFixedLatDirection = "GPSLocationFixedLatDirection";

        public static string GPSLocationFixedLonDegrees = "GPSLocationFixedLonDegrees";
        public static string GPSLocationFixedLonMinutes = "GPSLocationFixedLonMinutes";
        public static string GPSLocationFixedLonDirection = "GPSLocationFixedLonDirection";

        public static string GPSUseReceiverMode = "GPSUseReceiverMode";
        public class GPSUseReceiverModeValues
        {
            public static string USE_RECEIVER = "USE_RECEIVER";
            public static string USE_FIXED = "USE_FIXED";
            public static string USE_NONE = "USE_NONE";
        }


        public static string GPSUseThisCommPort = "GPSUseThisCommPort";
        public static string GPSMAPURL = "GPSMAPURL";


        //////////////////////////////////////////////////////
        //
        //  S2255 Quad Port Frame Grabber Settings



        public static string S2255DeviceMode(int deviceIndex)
        { return (string.Format("S2255DeviceMode_{0:0}", deviceIndex)); }


        //S2255DeviceMode_0,PortPair
        //S2255DeviceMode_1,PortPair
        public static class S2255DeviceModeValues
        {
            public static string PortPair = "PortPair";
            public static string AllBMPs = "AllBMPs";
        }


        //////////////////////////////////////////////////////
        //
        //  Remote Hosts
        public static string REMOTEHOSTS_LastHostFileUsed = "REMOTEHOSTS_LastHostFileUsed";
    }
}
