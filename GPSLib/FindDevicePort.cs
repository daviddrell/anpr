using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Management;
using System.Management.Instrumentation;
using System.IO;
using FileLoggingLib;

namespace GPSLib
{
    public class FindDevicePort
    {

        public FindDevicePort()
        {

        }

    
        public static string GetGPSCommPort()
        {
            string commPortString = null;

            try
            {
                // Query the device list trough the WMI. If you want to get
                // all the properties listen in the MSDN article mentioned
                // below, use "select * from Win32_PnPEntity" instead!
                ManagementObjectSearcher deviceList =
                    new ManagementObjectSearcher("Select Name, Status from Win32_PnPEntity");

                /// USB to UART Bridge Controller (COM6)

                //string filename = "C:\\Users\\David\\Pictures\\test\\devlist.txt";
                //StreamWriter sw = File.AppendText(filename);

                // Any results? There should be!
                if (deviceList != null)
                    // Enumerate the devices
                    foreach (ManagementObject device in deviceList.Get())
                    {
                        // To make the example more simple,
                        string name = device.GetPropertyValue("Name").ToString();
                        string status = device.GetPropertyValue("Status").ToString();

                        if (name.Contains("USB to UART Bridge Controller") || name.Contains("USB-to-Serial"))
                        {
                            // get the comm number

                            try
                            {
                                int index = name.LastIndexOf("COM");
                                string ss = name.Substring(index);
                                string[] sa = ss.Split(')');

                                // int commPortNum = Convert.ToInt32(ss[1]);

                                commPortString = sa[0];

                                return (commPortString);
                            }
                            catch
                            {
                                return (null);
                            }
                        }

                        // sw.WriteLine(name + "\r\n");

                        // Uncomment these lines and use the "select * query" if you 
                        // want a VERY verbose list
                        // foreach (PropertyData prop in device.Properties)
                        //    Console.WriteLine( "\t" + prop.Name + ": " + prop.Value);

                        // More details on the valid properties:
                        // http://msdn.microsoft.com/en-us/library/aa394353(VS.85).aspx
                        //  Console.WriteLine("Device name: {0}", name);
                        //   Console.WriteLine("\tStatus: {0}", status);

                        // Part II, Evaluate the device status.
                        //    bool working = ((status == "OK") || (status == "Degraded")
                        //        || (status == "Pred Fail"));

                        //   Console.WriteLine("\tWorking?: {0}", working);

                    }
            }
            catch (Exception ex)
            {
                FileLogging.Set("GetGPSCommPort ex: " + ex.Message);   
            }
            return (null);
        }
    }
}
