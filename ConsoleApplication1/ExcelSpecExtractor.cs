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
        static void Main(string[] args)
        {
            //
            
            string inputFile = @"../../input.txt";
            string outputFile = @"../../output.txt";
            //Console.WriteLine(line);

            //clean input file (remove \n and put everything on one line
            string text = File.ReadAllText(inputFile);
            //text = Regex.Replace(text, @"[^\r]\n", "<br />");
            text = Regex.Replace(text, @"(?<!\r)\n", "<br />");
            File.WriteAllText(inputFile, text);

            string[] rowLines = File.ReadAllLines(inputFile);
            
            StreamWriter file = new StreamWriter(outputFile);

            int currentRow = 1;
            foreach (string line in rowLines)
            {
                string sanitizedLine = Regex.Replace(line, @"\t{2,}", "\t");  //remove excess tabs
                string[] lineValues = sanitizedLine.Split('\t');    //split on tabs

                if (lineValues.Length == 7 && lineValues[0] != string.Empty)
                {
                    LineData ld = new LineData(lineValues);

                    switch (ld.DataType)
                    {
                        case DataType.Ratio:
                            file.WriteLine(Ratio(ld));
                            break;
                        case DataType.YesNo:
                            file.WriteLine(YesNo(ld));
                            break;
                        case DataType.Money:
                        default:
                            file.WriteLine(Money(ld));
                            break;
                    }
                }
                else
                {
                    file.WriteLine("//***********" + line + "***********\r\n", currentRow);
                }

                currentRow++;


            }
            file.Close();

        }

        public static string Money(LineData input)
        {
            string eol = Environment.NewLine;

            return
           "#region decimal " + input.FieldName + " (Line " + input.LineNumber + ")" + eol
           + "internal Calculatable<decimal, RoundedToTheNearestInteger> " + input.InternalFieldName + ";" + eol
           + "/// <summary> " + eol
           + "/// " + input.Description + "  (Calculatable) " + eol
           + "/// Reference Number " + input.ReferenceId + "" + eol
           + "/// </summary> " + eol
           + "[Money(AllowNegative = " + input.allowNegative + ", Precision = PrecisionType.Zero)] " + eol
           + "[Description(\"" + input.Description + "\"), Category(\"Category\"), ReferenceNumber(\"" + input.ReferenceId + "\"), LineNumber(\"" + input.LineNumber + "\")] " + eol
           + "public decimal " + input.FieldName + " { get { return " + input.InternalFieldName + ".Calculate(" + input.FieldName + "_Calculation); } } " + eol
           + "private decimal " + input.FieldName + "_Calculation() " + eol
           + "{ " + eol
           + "\t " + input.Calculation + "" + eol
           + "\t //TODO: Enter code for " + input.FieldName + " calculation " + eol
           + "} " + eol
           + "#endregion " + input.FieldName + " " + eol;


        }

        public static string Ratio(LineData input)
        {
            string eol = Environment.NewLine;

            return
            "#region decimal " + input.FieldName + " (Line " + input.LineNumber + ")" + eol
            + "internal Calculatable<decimal, RoundedToTwoDecimalPlaces> " + input.InternalFieldName + ";" + eol
            + "/// <summary> " + eol
            + "/// " + input.Description + "  (Calculatable) " + eol
            + "/// Reference Number " + input.ReferenceId + "" + eol
            + "/// </summary> " + eol
            + "[Ratio(Precision = PrecisionType.Zero)]" + eol
            + "[Description(\"" + input.Description + "\"), Category(\"Category\"), ReferenceNumber(\"" + input.ReferenceId + "\"), LineNumber(\"" + input.LineNumber + "\")] " + eol
            + "public decimal " + input.FieldName + " { get { return " + input.InternalFieldName + ".Calculate(" + input.FieldName + "_Calculation); } } " + eol
            + "private decimal " + input.FieldName + "_Calculation() " + eol
            + "{ " + eol
            + "\t " + input.Calculation + "" + eol
            + "\t //TODO: Enter code for " + input.FieldName + " calculation " + eol
            + "} " + eol
            + "#endregion " + input.FieldName + " " + eol;

        }

        public static string YesNo(LineData input)
        {
            string eol = Environment.NewLine;

            return
                "#region bool " + input.FieldName + " (Line " + input.LineNumber + ")" + eol
           + "internal Calculatable<bool> " + input.InternalFieldName + ";" + eol
           + "/// <summary> " + eol
           + "/// " + input.Description + "  (Calculatable) " + eol
           + "/// Reference Number " + input.ReferenceId + "" + eol
           + "/// </summary> " + eol
           + "[Description(\"" + input.Description + "\"), Category(\"Category\"), ReferenceNumber(\"" + input.ReferenceId + "\"), LineNumber(\"" + input.LineNumber + "\")] " + eol
           + "public bool " + input.FieldName + " { get { return " + input.InternalFieldName + ".Calculate(" + input.FieldName + "_Calculation); } } " + eol
           + "private bool " + input.FieldName + "_Calculation() " + eol
           + "{ " + eol
           + "\t " + input.Calculation + "" + eol
           + "\t //TODO: Enter code for " + input.FieldName + " calculation " + eol
           + "} " + eol
           + "#endregion " + input.FieldName + " " + eol;
        
        }
    }

   

   
}
