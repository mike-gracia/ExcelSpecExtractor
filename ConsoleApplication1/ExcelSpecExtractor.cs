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

            string category = "Category";
            Console.WriteLine("Enter a Category:");
            //category = Console.ReadLine();            

            string inputFile = @"../../input.txt";
            string outputFile = @"../../output.txt";
            //Console.WriteLine(line);

            //clean input file (remove \n and put everything on one line
            string text = File.ReadAllText(inputFile);
            text = Regex.Replace(text, @"(?<!\r)\n", "<br />");
            File.WriteAllText(inputFile, text);

            string[] rowLines = File.ReadAllLines(inputFile);
            
            StreamWriter file = new StreamWriter(outputFile);

            int currentRow = 1;
            foreach (string line in rowLines)
            {
                string sanitizedLine = Regex.Replace(line, @"\t{2,}", "\t");  //remove excess tabs
                string[] lineValues = sanitizedLine.Split('\t');    //split on tabs

                if (lineValues.Length == excelCellCount && lineValues[0] != string.Empty)
                {
                    LineData ld = new LineData(lineValues, category);
                    DataTranslator dt = new DataTranslator(ld);
                    file.WriteLine(dt.translation);
                }
                else
                {
                    file.WriteLine("/* " + line + "*/\r\n");
                    Debug.WriteLine("Could not process " + inputFile 
                        + " line number: " + currentRow
                        + " expected cell count: " + excelCellCount
                        + " actual cell count: " + lineValues.Length);
                }

                currentRow++;


            }
            file.Close();

        }

   
    }

   

   
}
