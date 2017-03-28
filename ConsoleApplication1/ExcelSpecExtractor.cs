using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExcelSpecExtractor;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using System.Reflection;

namespace ExcelSpecExtractor
{
    public class ExcelSpecExtractor
    {
        static string category = "Category1";
        static string jsonFileLocation = @"../../fields.json";
        const int excelCellCount = 7;

        public static string AsService(String input, String category = "Category1")
        {
            string[] a = new String[1];
            a[0] = input;
            Main(a);
            return File.ReadAllText(@"../../output.txt"); ;
        }

        public static string AsServiceJson(String input, String category = "Category1")
        {
            string[] a = new String[1];
            a[0] = input;
            Main(a);
            return File.ReadAllText(@"../../fields.json"); ;
        }

        public static string AsServiceText(String input, String category = "Category1")
        {
            return GetAndSanitizeInputFromTxt(input);
        }



        static void Main(string[] args)
        {

            string inputFileStr = @"../../input.txt";
            string outputFileStr = @"../../output.txt";
            

            if (args != null && args.Length > 0)
            {
                File.WriteAllText(inputFileStr, args[0]);
            }



            string category = "1 - Income";
            //category = GetCategoryFromUser();
            string rowLines = GetAndSanitizeInputFromTxt(File.ReadAllText(inputFileStr));
            File.WriteAllText(inputFileStr, rowLines);
            
            StreamWriter outFile = new StreamWriter(outputFileStr);
            StreamWriter jsonFile = new StreamWriter(jsonFileLocation);

            int currentRow = 1;
            foreach (string line in rowLines.Split(new string[] { Environment.NewLine }, StringSplitOptions.None))
            {
                string sanitizedLine = Regex.Replace(line, @"\t{2,}", "\t");  //remove excess tabs
                string[] lineValues = sanitizedLine.Split('\t');    //split on tabs

                if (lineValues.Length == excelCellCount && lineValues[0] != string.Empty)
                {
                    LineData ld = new LineData(lineValues, category);
                    DataTranslator dt = new DataTranslator(ld);
                    outFile.WriteLine(dt.GetCode);
                    jsonFile.WriteLine(JsonConvert.SerializeObject(ld) + ",");
                }
                else if (lineValues.Length == excelCellCount - 1 && lineValues[0] != string.Empty)   //FieldName is missing
                {
                    Console.WriteLine("Warning!... {0} has no Developer created name", lineValues[0]);
                    LineData ld = new LineData(lineValues, category, false);
                    DataTranslator dt = new DataTranslator(ld);
                    outFile.WriteLine(dt.GetCode);
                    jsonFile.WriteLine(JsonConvert.SerializeObject(ld) + ",");

                }
                else
                {
                    outFile.WriteLine("/* " + line + "*/\r\n");
                    Debug.WriteLine("Could not process " + inputFileStr 
                        + " line number: " + currentRow
                        + " expected cell count: " + excelCellCount
                        + " actual cell count: " + lineValues.Length);
                }

                currentRow++;


            }
            outFile.Close();
            jsonFile.Close();

            Console.WriteLine("Press any key to close...");
            Console.ReadLine();
        }

        //public static string[] GetAndSanitizeInputFromTxt(string _inputFileString)
        //{
        //    //clean input file (remove \n and put everything on one line
        //    string text = File.ReadAllText(_inputFileString);
        //    text = Regex.Replace(text, @"(?<!\r)\n", "<br />");
        //    //replace " with '
        //    text = Regex.Replace(text, "\"", "'");
        //    File.WriteAllText(_inputFileString, text);

        //    return File.ReadAllLines(_inputFileString);
        //}

        public static string GetAndSanitizeInputFromTxt(string _inputString)
        {
            //clean input (remove \n and put everything on one line
            _inputString = Regex.Replace(_inputString, @"(?<!\r)\n", "<br />");
            //replace " with '
            _inputString = Regex.Replace(_inputString, "\"", "'");

            return _inputString;
        }



        public static string GetCategoryFromUser()
        {
            Console.WriteLine("Enter a Category:");
            return Console.ReadLine();
        }

   
    }

   

   
}
