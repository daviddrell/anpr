using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace LicenseKeyObject
{
    public class LicenseKeys
    {
        //  records of generated keys will be written to a flat file.
        // this will start of using a flat file. if the number of records exceeded thousands I supposs an actual SQL database would be faster.


        public LicenseKeys(string path)
        {

            m_RootPath = path;
            m_RecordFile = path + "\\keyrecords.txt";

            m_lastSerialNumber = 4465;

            GetLastSerialNumberFromRecordDB();
        }

        public enum PRODUCT_CODES :int { PSS_V3_C1, PSS_V3_C2, PSS_V3_C3, PSS_V3_C4, WORKSTATION_V3}

        
        public enum KEY_TYPE {PRODUCTION, DEMO}
       
        public class KEY_OBJECT
        {
            public KEY_TYPE type;
            public PRODUCT_CODES productCode;
            public string productID { get { return Enum.GetNames(typeof(PRODUCT_CODES))[(int)productCode]; } }
            public int serialNumber;
            public int numDemoDays;
            public string customerName;
            public string purchasedFromName;
            public string purchaseDate;

            public KEY_OBJECT(PRODUCT_CODES code)
            {
                productCode = code;
            }
        }

 
         //foreach (string statName in Enum.GetNames(typeof(LPR)))
         //       {
         //           AddStat(statName, (int)(Enum.GetValues(typeof(LPR))).GetValue(index));
         //           index++;
         //       }

        string m_RootPath;
        string m_RecordFile;
        int m_lastSerialNumber;
      
        public void StoreRecord()
        {

        }

        public void GetLastSerialNumber()
        {

        }


        void GetLastSerialNumberFromRecordDB()
        {

            // read all lines and parse them
            string[] lines = File.ReadAllLines(m_RecordFile);

            int serialNumber=0;
            int maxSerialNumber = 0;


            foreach (string line in lines)
            {
                if (!ParseLine(line, ref serialNumber))
                {
                    MessageBox.Show("Parsing error in line: " + line);
                }
                else
                {
                    if (serialNumber > maxSerialNumber) maxSerialNumber = serialNumber;
                }
            }
            m_lastSerialNumber = maxSerialNumber;
        }

        public static string BuildLicenseString() { return null; }

        bool ParseLine(string line, ref int serialNum)
        {

            serialNum = 0;

            string[] sp1 = line.Split(',');

            // productIDString, keytype, serialNum, numDemoDays, customerName, purchasedFromName, purchaseDate

            if (sp1.Length < 3)
                return false;

            try
            {
                serialNum = Convert.ToInt32(sp1[2]);
            }
            catch (Exception ex) 
            {
                MessageBox.Show("int conversion error on serial num: " + line + "  ex: "+ex.Message);
                return (false);
            }

            return true;
        }
    }
}
