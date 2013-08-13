using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace GenerateVeryLongWatchList
{
    class Program
    {
        static void Main(string[] args)
        {
            //C:\Users\David\Documents
            string path = "C:\\Users\\David\\Documents\\longwatchlist.txt";
            ranStrings = new GenRadomStrings();

            for (int i = 0; i < 300000; i++)
            {
                string ranString = ranStrings.GetNextString();
                string numStr = ranString + ", comment text fasdfasdfsf" + Environment.NewLine;
                File.AppendAllText(path, numStr);
                if (i % 10000 == 0) Console.WriteLine("progress: " + i.ToString()); 
            }
            
            Console.WriteLine("Done : "+ path);
        }

        static GenRadomStrings ranStrings;
    }

    class GenRadomStrings
    {

        char[] radomCharsBaseArray;
        int charIndex = 0;
       

        public GenRadomStrings()
        {
            Random ran = new Random();
            radomCharsBaseArray = new char[256];
            for (int i = 0; i < 256; i++)
            {
                radomCharsBaseArray[i] = (char)ran.Next(48, 122);
            }
        }

        public string GetNextString()
        {
           
            char[] ranCharStringChars = new char[8];

            for (int i = 0; i < 8; i++)
            {
                ranCharStringChars[i] = radomCharsBaseArray[charIndex];
                charIndex++;
                if (charIndex >= radomCharsBaseArray.Length) charIndex = 0;
            }

            string s = new string(ranCharStringChars);

            return (s);
        }
    }
    
}
