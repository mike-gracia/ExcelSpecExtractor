using ExcelSpecExtractor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExcelSpecExtractor
{
    class DataTranslator
    {
        string eol = Environment.NewLine;
        public string translation;          //think about better protection level

        public DataTranslator(LineData ld)
        { 
                switch (ld.DataType)
                    {
                    case DataType.WholeNumber:
                            translation = WholeNumber(ld);
                            break;
                        case DataType.Ratio:
                            translation = Ratio(ld);
                            break;
                        case DataType.YesNo:
                            translation = YesNo(ld);
                            break;
                        case DataType.Money:
                        default:
                            translation = Money(ld);
                            break;
                    }

            }
        public string WholeNumber(LineData input)
        {

            return
            "#region int " + input.FieldName + " (Line " + input.LineNumber + ")" + eol
           + "internal Calculatable<int, RoundedToTheNearestInteger> " + input.InternalFieldName + ";" + eol
           + "/// <summary> " + eol
           + "/// " + input.Description + "  (Calculatable) " + eol
           + "/// Reference Number " + input.ReferenceId + "" + eol
           + "/// </summary> " + eol
           + "[Number(AllowNegative = " + input.allowNegative + ")]" + eol
           + "[Description(\"" + input.Description + "\"), Category(\"Category\"), ReferenceNumber(\"" + input.ReferenceId + "\"), LineNumber(\"" + input.LineNumber + "\")] " + eol
           + "public int " + input.FieldName + " { get { return " + input.InternalFieldName + ".Calculate(" + input.FieldName + "_Calculation); } } " + eol
           + "private int " + input.FieldName + "_Calculation() " + eol
           + "{ " + eol
           + "\t " + input.Calculation + "" + eol
           + "\t //TODO: Enter code for " + input.FieldName + " calculation " + eol
           + "} " + eol
           + "#endregion " + input.FieldName + " " + eol;
        
        }

        public string Money(LineData input)
        {
            

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
