using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;
using System.Threading;
using System.IO;
using System.Windows.Forms;
using ApplicationDataClass;


namespace GPSLib
{
    public class GPSDev
    {

        SerialPortControl m_sport;
        internal GPSInfo m_Info;
        string m_CommPort;
        Thread m_PollDevice;

        public class GPSInfo
        {
            public bool HaveSatData
            {
                get { lock (this) { return _HaveSatData; } }
                set { lock (this) { _HaveSatData = value; } }
            }

            public bool DetectedReceiver
            {
                get { lock (this) { return _DetectedReceiver; } }
                set { lock (this) { _DetectedReceiver = value; } }
            }

            public string GoogleMapString
            {
                get { lock (this) { return _GoogleMapString; } }
                set { lock (this) { _GoogleMapString = value; } }
            }

            public string CleanPositionString
            {
                get { lock (this) { return _NMEA0183String; } }
                set { lock (this) { _NMEA0183String = value; } }
            }


            public string Longitude
            {
                get { lock (this) { return _Longitude; } }
                set { lock (this) { _Longitude = value; } }
            }

            public string Latitude
            {
                get { lock (this) { return _Latitude; } }
                set { lock (this) { _Latitude = value; } }
            }

            bool _HaveSatData;
            bool _DetectedReceiver;
            string _GoogleMapString;
            string _NMEA0183String;
            string _Latitude;
            string _Longitude;
           

            public GPSInfo Clone()
            {
                lock (this)
                {
                    GPSInfo newInfo = new GPSInfo();
                    newInfo.HaveSatData = this.HaveSatData;

                    if (this._GoogleMapString == null)
                        newInfo._GoogleMapString = null;
                    else
                        newInfo._GoogleMapString = this._GoogleMapString.ToString();

                    if (this._NMEA0183String == null)
                        newInfo._NMEA0183String = null;
                    else
                        newInfo._NMEA0183String = this._NMEA0183String.ToString();

                    if (this._Latitude == null)
                        newInfo._Latitude = null;
                    else
                        newInfo._Latitude = this._Latitude;

                    if (this._Longitude == null)
                        newInfo._Longitude = null;
                    else
                        newInfo._Longitude = this._Longitude;


                    newInfo._HaveSatData = this._HaveSatData;
                    newInfo._DetectedReceiver = this._DetectedReceiver;

                    return (newInfo);
                }
            }

            public void Clear()
            {
                lock (this)
                {
                   _HaveSatData = false;
                   _DetectedReceiver = false;
                   _GoogleMapString = m_NoPositionAvailable;
                   _NMEA0183String = m_NoPositionAvailable;
                   _Latitude = m_NoPositionAvailable;
                   _Longitude = m_NoPositionAvailable;
                }
            }
        }

        const string m_NoPositionAvailable = "no position available";
        public delegate void NotifyDeviceFailure();
        NotifyDeviceFailure m_NotifyDeviceFailure;
        APPLICATION_DATA m_AppData;

        public GPSDev(string commPort, NotifyDeviceFailure notifyDeviceFailure, APPLICATION_DATA appData )
        {

            m_Info = new GPSInfo();
            m_Info.HaveSatData = false;

            m_AppData = appData;
            m_AppData.AddOnClosing(Close, APPLICATION_DATA.CLOSE_ORDER.FIRST);

            m_NotifyDeviceFailure = notifyDeviceFailure;

            m_Info.CleanPositionString = m_NoPositionAvailable;
            m_Info.Latitude = m_NoPositionAvailable;
            m_Info.Longitude = m_NoPositionAvailable;
            m_Info.GoogleMapString = m_NoPositionAvailable;
            m_Info.HaveSatData = false;
            m_Info.DetectedReceiver = false;

         //   m_CommPort = string.Format("COM{0:0}", m_Info.commport);

            m_CommPort = commPort;

            m_Stop = false;

            m_sport = new SerialPortControl(this);
            m_sport.Open();

            //  is there a receiver there spewing data?
            Thread.Sleep(100);
            if (m_sport.BytesToRead ()== 0)
            {
              // we failed to find the device
                _OpenSucess = false;
                m_sport.Close();
                return;
            }
            _OpenSucess = true;

            m_Info.DetectedReceiver = true;

            m_PollDevice = new Thread(PollLoop);
            m_PollDevice.Start();
        }

        bool _OpenSucess = false;

        public bool OpenSucess
        {
            set { }
            get { return _OpenSucess; }
        }

        bool m_Stop;
        public void Close()
        {
            m_Stop = true;
            if (m_sport != null)
            {
                m_sport.Close();
                m_sport = null;
            }
        }


        void ConnectionError()
        {
            _OpenSucess = false;
            m_Info.DetectedReceiver = false;
            m_Info.HaveSatData = false;
            m_NotifyDeviceFailure();
            
        }
        
        DateTime m_TimeLastSatString;// time when the last satellite list was received from the device
        DateTime m_TimeLastGPSDeviceCommunication;

        void PollLoop()
        {
            string location = null;
            
            TimeSpan m_SatLossTimer = new TimeSpan(0, 3, 0);
            TimeSpan m_DeviceCommunicationTimer = new TimeSpan(0, 0, 30);


            while (!m_Stop)
            {
                location = ReadString();// loops on bytes until the UART buffer has been cleared and processinges '$' terminated strings
                                             // ReadString also sets DetectedReceiver=true based on received characters
                                             // ReadString also spin's off parsing on received messages, and parsing detertines Satellite sync
                

                if ( DateTime.Now.Subtract( m_TimeLastSatString).CompareTo(m_SatLossTimer) > 0 )
                    m_Info.HaveSatData = false;// have not received a message in a while, we do not have any satellites

                if (DateTime.Now.Subtract(m_TimeLastGPSDeviceCommunication).CompareTo(m_DeviceCommunicationTimer) > 0)
                {
                    ConnectionError();
                }

                Thread.Sleep(10);
            }
        }


        public bool GetReciverDetected()
        {

            return (m_Info.DetectedReceiver);

        }


        public bool HaveSatelliteData()
        {

            return (m_Info.HaveSatData);

        }


      



        public string GetLocation(out GPSInfo info)
        {

            info = m_Info.Clone();
            if (m_Info.HaveSatData)
                return (m_Info.CleanPositionString);
            else
                return (null);

        }


        List<byte> buffer = null;

        string ReadString()
        {
            string result = null;

            if (m_sport.IsOpen)
            {
                if (m_sport.BytesToRead() > 2000)
                {  // too many bytes in buffer, just flush it
                    while (m_sport.BytesToRead() > 0)
                    {
                        m_sport.FlushBuffer();
                    }
                }

                while (m_sport.BytesToRead() > 0)
                {
                    m_TimeLastGPSDeviceCommunication = DateTime.Now;

                    byte b = m_sport.ReadByte();

                    // $ starts a new string and terminates a previous one
                    if (b == '$' && buffer != null)
                    {   // term the previous one and start a new one
                        result = System.Text.ASCIIEncoding.ASCII.GetString(buffer.ToArray());
                        buffer = new List<byte>();
                        break;
                    }
                    // we have not started one yet, so start one
                    else if (b == '$' && buffer == null)
                    {
                        buffer = new List<byte>();
                    }
                    else if (b != '$' && buffer != null)
                    {
                        buffer.Add(b);
                    }
                    else
                    {
                       // we are waiting for the start of a string to start a new buffer
                    }

                }
                if (result != null)
                {
                    ParseSatelliteString(result);

                    string location = ParseLocationString(result);
                    if (location != null)
                    {
                        return (location);
                    }
                }

               
            }
            return (null);
        }

       

        string ParseLocationString(string data)
        {
            string Latitude = null;

            string Longitude = null;

            string NMEA0183String = null;

            string[] lineArr = data.Split(',');

            if (lineArr[0] == "GPGGA" && lineArr.Length > 5)
            {

                try
                {
                    //Latitude

                    Double dLat = Convert.ToDouble(lineArr[2]);

                    int pt = dLat.ToString().IndexOf('.');

                    double degreesLat = Convert.ToDouble(dLat.ToString().Substring(0, pt - 2));

                    double minutesLat = Convert.ToDouble(dLat.ToString().Substring(pt - 2));

                    double DecDegsLat = degreesLat + (minutesLat / 60.0);

                    Latitude = lineArr[3].ToString() + DecDegsLat;

                    //Longitude

                    Double dLon = Convert.ToDouble(lineArr[4]);

                    pt = dLon.ToString().IndexOf('.');

                    double degreesLon = Convert.ToDouble(dLon.ToString().Substring(0, pt - 2));

                    double minutesLon = Convert.ToDouble(dLon.ToString().Substring(pt - 2));

                    double DecDegsLon = degreesLon + (minutesLon / 60.0);

                    Longitude = lineArr[5].ToString() + DecDegsLon;

                    NMEA0183String = data.ToString();

                    // break;// we found a valid position string, so stop
                }

                catch
                {
                    Latitude = null;
                    Longitude = null;
                }

            }



            if (Latitude == null || Longitude == null)
                return (null);



            lock (m_Info)
            {
                m_Info.Latitude = Latitude;
                m_Info.Longitude = Longitude;


                m_Info.CleanPositionString = Latitude + "," + Longitude;
            }



            return (m_Info.CleanPositionString);
        }


        void ParseSatelliteString(string data)
        {
            // C:\Users\David\Pictures\test

            //if (data.Contains("GPGSA"))
            //{
            //    File.AppendAllText("C:\\Users\\David\\Pictures\\test\\gpsstring.txt", data);
            //}

          // good reception:   GPGSA,A,3,06,07,13,20,23,25,32,,,,,,2.3,1.3,1.9*3F
          // poor reception:   GPGSA,A,1,,,,,,,,,,,,,0.0,0.0,0.0*30

            int satCount = 0;

            if (data.Contains("GPGSA") )
            {
                string[] lineArr = data.Split(',');
                if (lineArr.Length < 16) return;// we did not get a complete string

                for (int i = 3; i < lineArr.Length - 3; i++)
                {
                    try
                    {
                        int satNumber = Convert.ToInt32(lineArr[i]);

                        // we dont get to this statement if the field is blank, excption thrown
                        satCount++;
                    }
                    catch
                    {
                        break;
                    }
                }

                lock (m_Info)
                {
                    if (satCount > 2)
                        m_Info.HaveSatData = true;
                    else
                        m_Info.HaveSatData = false;
                }

                m_TimeLastSatString = DateTime.Now;
            }            
        }


         /// <summary>
        /// /////////                          class SerialPortControl
         /// </summary>

        class SerialPortControl
        {
            object singleton;
            SerialPort port;
            GPSDev m_Parent;

            public SerialPortControl(GPSDev parent)
            {
                try
                {
                    m_Parent = parent;
                    singleton = new object();
                    port = new SerialPort();
                   
                }
                catch
                {
                    m_Parent.ConnectionError();

                }
            }

            public void FlushBuffer()
            {
                if (port != null)
                    port.DiscardInBuffer();
            }

            public bool IsOpen
            {

                get
                {
                    lock (singleton)
                    {
                        try
                        {
                            return port.IsOpen;
                        }
                        catch
                        {
                            m_Parent.ConnectionError();

                            return (false);
                        }
                    }

                }
                set { }

            }

            public int BytesToRead()
            {
                lock (singleton)
                {
                    try
                    {
                        if (!port.IsOpen) return (0);

                        return (port.BytesToRead);
                    }
                    catch
                    {
                        m_Parent.ConnectionError();
                        return (0);
                    }
                }

            }

            public byte ReadByte()
            {
                lock (singleton)
                {
                    try
                    {
                        if (!port.IsOpen) return (0);
                        return ((byte)port.ReadByte());
                    }
                    catch
                    {
                        m_Parent.ConnectionError();
                        return (0);
                    }
                }

            }

            //public string ReadExisting()
            //{
            //    lock (singleton)
            //    {
            //        try
            //        {
            //            if (port == null) return (null);
            //            if (!port.IsOpen) return (null);

            //            if (port.BytesToRead < 2)
            //            {    // the receiver has been unplugged
            //                m_Parent.m_Info.DetectedReceiver = false;
            //                m_Parent.m_Info.HaveSatData = false;
            //                return (null);
            //            }

            //            string data = port.ReadExisting();
            //            return (data);
            //        }
            //        catch
            //        {
            //            m_Parent.m_Info.DetectedReceiver = false;
            //            m_Parent.m_Info.HaveSatData = false;
            //            this.Close();
            //            return (null);
            //        }
            //    }
            //}


            public void Close()
            {
                lock (singleton)
                {
                    try
                    {
                        port.Close();
                    }
                    catch
                    {
                        lock (m_Parent.m_Info)
                        {
                            m_Parent.m_Info.DetectedReceiver = false;
                            m_Parent.m_Info.HaveSatData = false;
                            this.Close();
                        }
                    }
                }
            }

            public void Open()
            {
                lock (singleton)
                {

                    try
                    {
                        if (port != null)
                        {
                            port.Close();
                        }

                        port = new SerialPort(m_Parent.m_CommPort);

                        port.BaudRate = 4800;
                        port.DataBits = 8;
                        port.Parity = Parity.None;
                        port.StopBits = StopBits.One;

                        port.Open();

                    }
                    catch
                    {
                        m_Parent.ConnectionError();
                    }
                }
            }

        }


    }
}
