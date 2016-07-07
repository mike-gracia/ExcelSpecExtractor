using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExcelSpecExtractor;
using System.Text.RegularExpressions;

namespace ExcelSpecExtractor
{
    class ExcelSpecExtractor
    {
        const int excelCellCount = 7;

        static void Main(string[] args)
        {

            string inputFileStr = @"../../input.txt";
            string outputFileStr = @"../../output.txt";



            string category = GetCategoryFromUser();
            string[] rowLines = GetAndSanitizeInputFromTxt(inputFileStr);
            
            StreamWriter outFile = new StreamWriter(outputFileStr);

            int currentRow = 1;
            foreach (string line in rowLines)
            {
                string sanitizedLine = Regex.Replace(line, @"\t{2,}", "\t");  //remove excess tabs
                string[] lineValues = sanitizedLine.Split('\t');    //split on tabs

                if (lineValues.Length == excelCellCount && lineValues[0] != string.Empty)
                {
                    LineData ld = new LineData(lineValues, category);
                    DataTranslator dt = new DataTranslator(ld);
                    outFile.WriteLine(dt.GetCode);
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

        }

        public static string[] GetAndSanitizeInputFromTxt(string _inputFileString)
        {
            //clean input file (remove \n and put everything on one line
            string text = File.ReadAllText(_inputFileString);
            text = Regex.Replace(text, @"(?<!\r)\n", "<br />");
            //replace " with '
            text = Regex.Replace(text, "\"", "'");
            File.WriteAllText(_inputFileString, text);

            return File.ReadAllLines(_inputFileString);
        }

        public static string GetCategoryFromUser()
        {
            Console.WriteLine("Enter a Category:");
            return Console.ReadLine();
        }

   
    }

   

   
}
