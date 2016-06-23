using ExcelSpecExtractor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace ExcelSpecExtractor
{
    class DataTranslator
    {
        string eol = Environment.NewLine;
        public string translation;          //protection level

        public DataTranslator(LineData ld)
        {
            XDocument snippet = GetSnippet(ld.DataType, ld.isChangeable);
            translation = GenerateCode(snippet, ld);

                //switch (ld.DataType)
                //    {
                //    case DataType.Text:
                //        translation = Text(ld);
                //        break;
                //    case DataType.Date:
                //            translation = Date(ld);
                //            break;
                //    case DataType.WholeNumber:
                //            translation = WholeNumber(ld);
                //            break;
                //        case DataType.Ratio:
                //            translation = Ratio(ld);
                //            break;
                //        case DataType.YesNo:
                //            translation = YesNo(ld);
                //            break;
                //        case DataType.Money:
                //        default:
                //            translation = Money(ld);
                //            break;
                //    }

            }

        private XDocument GetSnippet(DataType _dataType, bool _isChangeable)
        {
            switch (_dataType)
            {
                case DataType.Text:
                    if (_isChangeable)
                        return XDocument.Load("../../Snippets/InsertChangeableString.Snippet");
                    else
                        return XDocument.Load("../../Snippets/InsertCalculatableString.Snippet");
                case DataType.Money:
                    if (_isChangeable)
                        return XDocument.Load("../../Snippets/InsertChangeableMoney.Snippet");
                    else
                        return XDocument.Load("../../Snippets/InsertCalculatableMoney.Snippet");
                case DataType.WholeNumber:
                     if (_isChangeable)
                        return XDocument.Load("../../Snippets/InsertChangeableNumber.Snippet");
                    else
                        return XDocument.Load("../../Snippets/InsertCalculatableNumber.Snippet");
                case DataType.Ratio:
                    if (_isChangeable)
                        return XDocument.Load("../../Snippets/InsertChangeableRatio.Snippet");
                    else
                        return XDocument.Load("../../Snippets/InsertCalculatableRatio.Snippet");
                case DataType.YesNo:
                   if (_isChangeable)
                        return XDocument.Load("../../Snippets/InsertChangeableYesNo.Snippet");
                    else
                        return XDocument.Load("../../Snippets/InsertCalculatableYesNo.Snippet");
                case DataType.Date:
                default:
                    if (_isChangeable)
                        return XDocument.Load("../../Snippets/InsertChangeable.Snippet");
                    else
                        return XDocument.Load("../../Snippets/InsertCalculatable.Snippet");

            }

        }
        
        private string GenerateCode(XDocument _snipXml, LineData _excelLineData)
        {
            string codeString;
            XNamespace xns = _snipXml.Root.Name.Namespace;
            XElement codeElement = _snipXml.Root.Element(xns + "CodeSnippet").Element(xns + "Snippet").Element(xns + "Code");
            codeString = codeElement.Value;

            //sanitize codeString from snippet
            codeString = Regex.Replace(codeString, @"\r?\n", "\r\n");
            codeString = codeString.Replace("\t", "");
            codeString = Regex.Replace(codeString, @" {2,}", "");

            //insert values into snippet
            codeString = codeString.Replace("//TODO: Enter code for $FieldName$ calculation", _excelLineData.Calculation);
            codeString = codeString.Replace("$FieldName$", _excelLineData.FieldName);
            codeString = codeString.Replace("$LineNumber$", _excelLineData.LineNumber);
            codeString = codeString.Replace("$InternalFieldName$", _excelLineData.InternalFieldName);
            codeString = codeString.Replace("$SummarySection$", _excelLineData.Description);
            codeString = codeString.Replace("$ReferenceId$", _excelLineData.ReferenceId);
            codeString = codeString.Replace("$AttributeCategory$", _excelLineData.Category);
            codeString = codeString.Replace("$AttributeDescription$", _excelLineData.Description);
            codeString = codeString.Replace("$end$", eol);

            return codeString;
        }


        public string Text(LineData input)
        {
            string codeString;

            XDocument snip = XDocument.Load("../../Snippets/InsertChangeableString.Snippet");
            XNamespace xns = snip.Root.Name.Namespace;
            XElement codeElement = snip.Root.Element(xns + "CodeSnippet").Element(xns + "Snippet").Element(xns + "Code");
            codeString = codeElement.Value;

            //sanitize codeString from snippet
            codeString = Regex.Replace(codeString, @"\r?\n", "\r\n");
            codeString = codeString.Replace("\t", "");
            codeString = Regex.Replace(codeString, @" {2,}", "");

            //insert values into snippet
            codeString = codeString.Replace("$FieldName$", input.FieldName);
            codeString = codeString.Replace("$LineNumber$", input.LineNumber);
            codeString = codeString.Replace("$InternalFieldName$", input.InternalFieldName);
            codeString = codeString.Replace("$SummarySection$", input.Description);
            codeString = codeString.Replace("$ReferenceId$", input.ReferenceId);
            codeString = codeString.Replace("$AttributeDescription$", input.Description);

                return codeString;
        }

        public string Date(LineData input)
        {

            return
            "#region Date " + input.FieldName + " (Line " + input.LineNumber + ")" + eol
           + "internal Calculatable<Date, RoundedToTheNearestInteger> " + input.InternalFieldName + ";" + eol
           + "/// <summary> " + eol
           + "/// " + input.Description + "  (Calculatable) " + eol
           + "/// Reference Number " + input.ReferenceId + "" + eol
           + "/// </summary> " + eol
           + "[Description(\"" + input.Description + "\"), Category(\""+input.Category+"\"), ReferenceNumber(\"" + input.ReferenceId + "\"), LineNumber(\"" + input.LineNumber + "\")] " + eol
           + "public Date " + input.FieldName + " { get { return " + input.InternalFieldName + ".Calculate(" + input.FieldName + "_Calculation); } } " + eol
           + "private Date " + input.FieldName + "_Calculation() " + eol
           + "{ " + eol
           + "\t " + input.Calculation + "" + eol
           + "\t //TODO: Enter code for " + input.FieldName + " calculation " + eol
           + "} " + eol
           + "#endregion " + input.FieldName + " " + eol;

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
           + "[Description(\"" + input.Description + "\"), Category(\""+input.Category+"\"), ReferenceNumber(\"" + input.ReferenceId + "\"), LineNumber(\"" + input.LineNumber + "\")] " + eol
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
           + "[Description(\"" + input.Description + "\"), Category(\""+input.Category+"\"), ReferenceNumber(\"" + input.ReferenceId + "\"), LineNumber(\"" + input.LineNumber + "\")] " + eol
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
            + "[Description(\"" + input.Description + "\"), Category(\""+input.Category+"\"), ReferenceNumber(\"" + input.ReferenceId + "\"), LineNumber(\"" + input.LineNumber + "\")] " + eol
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
           + "[Description(\"" + input.Description + "\"), Category(\""+input.Category+"\"), ReferenceNumber(\"" + input.ReferenceId + "\"), LineNumber(\"" + input.LineNumber + "\")] " + eol
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
