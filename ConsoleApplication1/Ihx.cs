using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ihx;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using System.Reflection;

namespace Ihx
{
    public class Ihx
    {
        static int categoryIndex = 1;
        private static readonly string defaultCategory = "1 - Category";
        static string category = defaultCategory;

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

            ConsoleExtensions.Initizialize();

            if (args != null && args.Length > 0)
            {
                File.WriteAllText(inputFileStr, args[0]);
            }


            

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
                }
                else if (lineValues.Length == excelCellCount - 1 && lineValues[0] != string.Empty)   //FieldName is missing
                {

                    ConsoleExtensions.PrintConsoleWarning(currentTextLine, "No developer name");
                    //Console.WriteLine("Line {0} - Warning!... {1} has no Developer created name", currentRow, lineValues[0]);
                    LineData ld = new LineData(lineValues, category, false);
                    LineDataList.Add(ld);
                }
                else
                {
                    GetCategoryFromLine(lineValues, currentTextLine);
                    Debug.WriteLine("Could not process " + inputFileStr 
                        + " line number: " + currentTextLine
                        + " expected cell count: " + excelCellCount
                        + " actual cell count: " + lineValues.Length);
                }

                if (category == defaultCategory)
                    ConsoleExtensions.PrintConsoleWarning(currentTextLine, "Using default category");

                currentTextLine++;
            }


            //translate TA calc
            foreach(LineData outterLd in LineDataList)
            {

                string find = outterLd.referenceId;
                string replace = outterLd.devFieldName;

                foreach(LineData innerLd in LineDataList)
                {
                    innerLd.calculationTranslation = innerLd.calculationTranslation.Replace(find, replace);
                }

            }


            //aw translation test
            string line2;
            foreach(LineData outterLd in LineDataList)
            {
                System.IO.StreamReader awFile = new System.IO.StreamReader("../../aw.txt");
                while ((line2 = awFile.ReadLine()) != null)
                {
                    string[] unit = line2.Split(':');
                    string findAw = ":" + unit[0] + "]";
                    string findPynr = ":PYNR" + unit[0] + "]";
                    string replaceAw =   "(" + unit[1] + ")]";
                    string replacePynr = "(" + unit[1] + ")]";

                    //outterLd.calculationTranslation = outterLd.calculationTranslation.Replace(unit[0], unit[1]);
                    outterLd.calculationTranslation = Regex.Replace(outterLd.calculationTranslation,  findAw, findAw + replaceAw);
                    outterLd.calculationTranslation = Regex.Replace(outterLd.calculationTranslation, findPynr, findPynr + replacePynr);
                }
            }

            //print warnings and errors
            foreach (LineData ld in LineDataList)
            {
                if (ld.calculationTaxAnalysisString.ToLower().Contains("direct entry") && !ld.isChangeable)
                    ConsoleExtensions.PrintConsoleWarning(100, ld.devFieldName + " Contains phrase 'direct entry' ");
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


        //abandoned
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
                category = newCategory;
            }
        }

   
    }

   

   
}
