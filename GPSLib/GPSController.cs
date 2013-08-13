using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using UserSettingsLib;
using ApplicationDataClass;
using ErrorLoggingLib;

namespace GPSLib
{


    public class GPSController
    {

        GPSDev m_GPSDevice;
        Thread m_PollForDataThread;

        GPSDev.GPSInfo m_CurrentLocationInfo;

        public enum GPS_USE_MODES { USE_RECEIVER, USE_FIXED, USE_NONE };
        GPS_USE_MODES m_GPSUseModes;

        string m_FixedCommPort = null;
        GPSDev.GPSInfo m_FixedSiteCoordiates;

        bool m_ResetGPSDriver = false;
        int m_PollLoopSleepTime = 1000;
        APPLICATION_DATA m_AppData;

        /// design

        /// this object in instantiated on startup and the controller thread starts
        /// 
        /// if the initial config is either un-configured or USE_NONE for GPS mode,
        /// the thread will do nothing but loop on the use mode flags. A call to LoadGPSConfiguration() will
        ///  cause the flags to change and the operation of the loop to change.

        public GPSController(PutNewGPSData putNewGPSData, APPLICATION_DATA appData)
        {
            m_putNewGPSData = putNewGPSData;

            m_AppData = appData;
            m_AppData.AddOnClosing(Close, APPLICATION_DATA.CLOSE_ORDER.FIRST);
            m_Log = (ErrorLog)m_AppData.Logger;

            m_FixedSiteCoordiates = new GPSDev.GPSInfo();

            m_CurrentLocationInfo = new GPSDev.GPSInfo();

            // load any static/stored positions in case a GPS device is not found
            m_GPSUseModes = GPS_USE_MODES.USE_FIXED;

            LoadGPSConfiguration();

            // start a new thread
            m_PollForDataThread = new Thread(Poller);
            m_PollForDataThread.Start();

        }

        ErrorLog m_Log;

        bool m_ReceiverDetected = false;

        public delegate void PutNewGPSData(string position, bool deviceFound, bool satellitesFound);
        PutNewGPSData m_putNewGPSData;
      

        public void LoadGPSConfiguration()
        {
          
            m_FixedSiteCoordiates.Clear();

            GetFixedSiteCoordinates();

        }


        void GetFixedSiteCoordinates()
        {
            string latDegrees = UserSettings.Get(UserSettingTags.GPSLocationFixedLatDegrees);
            string latDirection = UserSettings.Get(UserSettingTags.GPSLocationFixedLatDirection);
            string latMinutes = UserSettings.Get(UserSettingTags.GPSLocationFixedLatMinutes);
            if (latDegrees == null || latDirection == null || latMinutes == null)
            {
                m_googleString = UserSettings.Get(UserSettingTags.GPSMAPURL);
                if (m_googleString == null)
                {
                    m_googleString = "http://maps.google.com/maps?q=";
                }

                m_FixedSiteCoordiates.CleanPositionString = "config error: fixed position not set";

                m_FixedSiteCoordiates.GoogleMapString = BuildURL("config error: fixed position not set", "config error: fixed position not set", m_googleString);

                m_FixedSiteCoordiates.DetectedReceiver = false;
                m_FixedSiteCoordiates.HaveSatData = false;
                return;
            }

            if (latDirection.Contains("NORTH"))
                latDirection = "N";
            else
                latDirection = "S";



            string lonDegrees = UserSettings.Get(UserSettingTags.GPSLocationFixedLonDegrees);
            string lonDirection = UserSettings.Get(UserSettingTags.GPSLocationFixedLonDirection);
            string lonMinutes = UserSettings.Get(UserSettingTags.GPSLocationFixedLonMinutes);

            if (lonDegrees == null || lonDirection == null || lonMinutes == null)
            {
                m_googleString = UserSettings.Get(UserSettingTags.GPSMAPURL);
                if (m_googleString == null)
                {
                    m_googleString = "http://maps.google.com/maps?q=";
                }

                m_FixedSiteCoordiates.CleanPositionString = "config error: fixed position not set";

                m_FixedSiteCoordiates.GoogleMapString = BuildURL("config error: fixed position not set", "config error: fixed position not set", m_googleString);

                m_FixedSiteCoordiates.DetectedReceiver = false;
                m_FixedSiteCoordiates.HaveSatData = false;
                return;
            }

            if (lonDirection.Contains("EAST"))
                lonDirection = "E";
            else
                lonDirection = "W";





            // N 43.80782, W 70.16428
            // needs to have calculated check sum.. to do
            double degreesLat;
            double minsecsLat;
            double lat = 0;

            double degreesLon;
            double minsecsLon;
            double lon = 0;

            try
            {
                degreesLat = Convert.ToDouble(latDegrees);
                minsecsLat = Convert.ToDouble(latMinutes); // the minutes and seconds are stored together as the fractional part 
                lat = degreesLat + (minsecsLat / 60.0);

                degreesLon = Convert.ToDouble(lonDegrees);
                minsecsLon = Convert.ToDouble(lonMinutes); // the minutes and seconds are stored together as the fractional part 
                lon = degreesLon + (minsecsLon / 60);
            }
            catch
            {

            }


            string position = latDirection + " " + lat.ToString() + ", " + lonDirection + " " + lon.ToString();
            position = RemoveSpaces(position);

            m_googleString = UserSettings.Get(UserSettingTags.GPSMAPURL);
            if (m_googleString == null)
            {
                m_googleString = "http://maps.google.com/maps?q=";
            }

            m_FixedSiteCoordiates.CleanPositionString = position;

            m_FixedSiteCoordiates.GoogleMapString = BuildURL(latDirection + " " + lat.ToString(), lonDirection + " " + lon.ToString(), m_googleString);

            m_FixedSiteCoordiates.DetectedReceiver = true;
            m_FixedSiteCoordiates.HaveSatData = true;
        }

        string RemoveSpaces(string s)
        {
            int offset = s.IndexOf (' ');
            while (offset > -1)
            {
                s = s.Remove(offset,1);
                offset = s.IndexOf(' ');
            }
            return (s);
        }

        string m_googleString;

        string BuildURL(string Latitude, string Longitude, string URLBase)
        {
            StringBuilder queryAddress = new StringBuilder();

            queryAddress.Append(URLBase);
            queryAddress.Append(Latitude + "%2C");
            queryAddress.Append(Longitude);
            string url = queryAddress.ToString();
            url = RemoveSpaces(url);


            return (url);
        }



        public bool GetReceiverDetected()
        {
            if (m_GPSUseModes == GPS_USE_MODES.USE_RECEIVER)
            {

                return (m_ReceiverDetected);
            }
            else if (m_GPSUseModes == GPS_USE_MODES.USE_FIXED)
            {
                return (true);
            }
            else return (false);
        }



        bool m_Stop;

        public void Close()
        {
            m_Stop = true;
            if (m_GPSDevice != null)
                m_GPSDevice.Close();
        }


        public string GetCurrentCommPort()
        {
            return (m_FixedCommPort);
        }

        public string FindCommPort()
        {
            return FindDevicePort.GetGPSCommPort();
        }


        public string GetLocation(out GPSDev.GPSInfo info)
        {

            string l = null;

            if (m_GPSUseModes == GPS_USE_MODES.USE_RECEIVER)
            {
                lock (m_CurrentLocationInfo)
                {
                    info = m_CurrentLocationInfo.Clone();
                }

            }
            else
            {
                lock (m_FixedSiteCoordiates)
                {
                    info = m_FixedSiteCoordiates.Clone();
                }
            }

            return (l);
        }


        void Poller()
        {
            GPSDev.GPSInfo gpsInfo = null;

            while (!m_Stop)
            {
           

                Thread.Sleep(m_PollLoopSleepTime);


                // do we have a comm port?
                if (m_FixedCommPort == null)
                {
                    m_FixedCommPort = FindDevicePort.GetGPSCommPort();
                    // may still be null if there is no device
                }

                if (m_ResetGPSDriver)
                {
                    // lost the device connection, start over
                    if (m_GPSDevice != null) m_GPSDevice.Close();
                    m_FixedCommPort = null; // start over, perhaps the user moved the receiver to another port
                    m_GPSDevice = null;
                    m_ResetGPSDriver = false;

                    m_Log.Log("lost GPS Device connection", ErrorLog.LOG_TYPE.FATAL);
                    m_GPSUseModes = GPS_USE_MODES.USE_FIXED;

                    LoadGPSConfiguration();// re-load the static configuration if one is there
                }

                if (m_FixedCommPort != null)
                {

                    // we have found a comm, port, try it....

                    // do we have a device opened ?
                    if (m_GPSDevice == null && !m_Stop)
                        m_GPSDevice = new GPSDev(m_FixedCommPort, HandleLostGPSDeviceConnection, m_AppData);

                    if (m_GPSDevice == null && m_Stop)
                        break;

                    if (!m_GPSDevice.OpenSucess)
                    {
                        m_GPSDevice.Close();
                        m_GPSDevice = null;
                        continue;// try again next time around
                    }
                    else
                    {

                        if (m_ReceiverDetected == false)
                        {
                            m_Log.Log("connected to GPS Device", ErrorLog.LOG_TYPE.FATAL);
                        }

                        m_ReceiverDetected = true;
                        m_GPSUseModes = GPS_USE_MODES.USE_RECEIVER;

                        // everything is working fine, get the latest available sat data

                        m_GPSDevice.GetLocation(out gpsInfo);

                        lock (m_CurrentLocationInfo)
                        {
                            m_CurrentLocationInfo = gpsInfo.Clone();
                        }

                        BuildURL(gpsInfo.Latitude, gpsInfo.Longitude, m_googleString);

                    }
                }

               
                if (m_GPSUseModes == GPS_USE_MODES.USE_RECEIVER)
                {
                    m_putNewGPSData(gpsInfo.CleanPositionString, gpsInfo.DetectedReceiver, gpsInfo.HaveSatData);
                }
                else if (m_GPSUseModes == GPS_USE_MODES.USE_FIXED)
                {
                    m_putNewGPSData(m_FixedSiteCoordiates.CleanPositionString, m_FixedSiteCoordiates.DetectedReceiver, m_FixedSiteCoordiates.HaveSatData);
                }
                else if (m_GPSUseModes == GPS_USE_MODES.USE_NONE)
                {
                    m_putNewGPSData("no position avaialbe", false, false);
                }

            }// end while (! stop)

        

            if (m_GPSDevice != null)
            {
                m_GPSDevice.Close();
                m_GPSDevice = null;
            }
        }


        void HandleLostGPSDeviceConnection()
        {
            m_ReceiverDetected = false;

            m_putNewGPSData("no position available", false, false);
            m_ResetGPSDriver = true;
        }
    }
}
