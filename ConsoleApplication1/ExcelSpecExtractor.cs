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
        static int categoryIndex = 1;
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




            //category = GetCategoryFromUser();
            string rowLines = GetAndSanitizeInputFromTxt(File.ReadAllText(inputFileStr));
            File.WriteAllText(inputFileStr, rowLines);
            
           

            List<LineData> LineDataList = new List<LineData>();

            int currentTextLine = 1;
            foreach (string line in rowLines.Split(new string[] { Environment.NewLine }, StringSplitOptions.None))
            {
                string sanitizedLine = Regex.Replace(line, @"\t{2,}", "\t");  //remove excess tabs
                string[] lineValues = sanitizedLine.Split('\t');    //split on tabs

                if (lineValues.Length == excelCellCount && lineValues[0] != string.Empty)
                {
                    LineData ld = new LineData(lineValues, category);
                    LineDataList.Add(ld);
                    DataTranslator dt = new DataTranslator(ld);



                    //outFile.WriteLine(dt.GetCode);
                    //jsonFile.WriteLine(JsonConvert.SerializeObject(ld) + ",");
                }
                else if (lineValues.Length == excelCellCount - 1 && lineValues[0] != string.Empty)   //FieldName is missing
                {

                    ConsoleExtensions.PrintConsoleWarning(currentTextLine, "No developer name");
                    //Console.WriteLine("Line {0} - Warning!... {1} has no Developer created name", currentRow, lineValues[0]);
                    LineData ld = new LineData(lineValues, category, false);
                   

                }
                else
                {
                    GetCategoryFromLine(lineValues, currentTextLine);
                    Debug.WriteLine("Could not process " + inputFileStr 
                        + " line number: " + currentTextLine
                        + " expected cell count: " + excelCellCount
                        + " actual cell count: " + lineValues.Length);
                }


                currentTextLine++;


            }


            //translate TA calc
            foreach(LineData outterLd in LineDataList)
            {

                string find = outterLd.referenceId;
                string replace = outterLd.devFieldName;

                foreach(LineData innerLd in LineDataList)
                {

                   innerLd.calculationTaxAnalysisString =  innerLd.calculationTaxAnalysisString.Replace(find, replace);
                }
            }

            //print warnings and errors
            foreach(LineData ld in LineDataList)
            {
                if (ld.calculationTaxAnalysisString.ToLower().Contains("direct entry") && !ld.isChangeable)
                    ConsoleExtensions.PrintConsoleWarning(100, ld.referenceId + " Contains phrase 'direct entry' ");
            }


            //translate and write out to file
            StreamWriter outFile = new StreamWriter(outputFileStr);
            StreamWriter jsonFile = new StreamWriter(jsonFileLocation);
            foreach(LineData ld in LineDataList)
            {
                DataTranslator dt = new DataTranslator(ld);
                outFile.WriteLine(dt.GetCode);
                jsonFile.WriteLine(JsonConvert.SerializeObject(ld) + ",");
            }

            outFile.Close();
            jsonFile.Close();

            Console.WriteLine("Press any key to close...");
            Console.ReadLine();
        }


        public static string GetAndSanitizeInputFromTxt(string _inputString)
        {
            //clean input (remove \n but preserve as <br /> and put everything on one line
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

        private static void GetCategoryFromLine(string[] _lineValues, int _lineNumber)
        {
            if (_lineValues.Where(s => s != string.Empty).Count() == 1)
            {
                string newCategory = string.Join("", _lineValues.Where(s => s != string.Empty).FirstOrDefault().Take(30));
                //string message = "Changing category from {0} to {1}", category;
                //ConsoleExtensions.PrintConsoleMessage(_lineNumber, message);
                Console.WriteLine("Line {0} - Changing category from {1} to {2}", _lineNumber, category, newCategory);
                category = categoryIndex++ + " - " + newCategory;
            }
        }

   
    }

   

   
}
