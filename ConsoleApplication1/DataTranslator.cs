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
        private string snippetDir = "../../Snippets/";
        string eol = Environment.NewLine;
        private string codeString;


        public DataTranslator(LineData ld)
        {
            XDocument snippet = GetSnippet(ld.dataType, ld.isChangeable);
            codeString = GenerateCode(snippet, ld);
        }

        private XDocument GetSnippet(DataType _dataType, bool _isChangeable)
        {
            switch (_dataType)
            {
                case DataType.Text:
                    return _isChangeable
                        ? XDocument.Load(snippetDir + "InsertChangeableString.Snippet")
                        : XDocument.Load(snippetDir + "InsertCalculatableString.Snippet");
                case DataType.Money:
                    return _isChangeable
                        ? XDocument.Load(snippetDir + "InsertChangeableMoney.Snippet")
                        : XDocument.Load(snippetDir + "InsertCalculatableMoney.Snippet");
                case DataType.WholeNumber:
                    return _isChangeable
                        ? XDocument.Load(snippetDir + "InsertChangeableNumber.Snippet")
                        : XDocument.Load(snippetDir + "InsertCalculatableNumber.Snippet");
                case DataType.Ratio:
                    return _isChangeable
                        ? XDocument.Load(snippetDir + "InsertChangeableRatio.Snippet")
                        : XDocument.Load(snippetDir + "InsertCalculatableRatio.Snippet");
                case DataType.YesNo:
                   return _isChangeable
                        ? XDocument.Load(snippetDir + "InsertChangeableBoolean.Snippet")
                        : XDocument.Load(snippetDir + "InsertCalculatableBoolean.Snippet");
                case DataType.YesNoBlank:
                   return _isChangeable
                        ? XDocument.Load(snippetDir + "InsertChangeableNullableBoolean.Snippet")
                        : XDocument.Load(snippetDir + "InsertCalculatableNullableBoolean.Snippet");
                default:
                    return _isChangeable
                        ? XDocument.Load(snippetDir + "InsertChangeable.Snippet")
                        : XDocument.Load(snippetDir + "InsertCalculatable.Snippet");

            }

        }
        
        private string GenerateCode(XDocument _snipXml, LineData _txtLineData)
        {
            string codeStr;
            XNamespace xns = _snipXml.Root.Name.Namespace;
            XElement codeElement = _snipXml.Root.Element(xns + "CodeSnippet").Element(xns + "Snippet").Element(xns + "Code");
            codeStr = codeElement.Value;

            //sanitize codeString from .snippet
            codeStr = Regex.Replace(codeStr, @"\r?\n", "\r\n");
            codeStr = codeStr.Replace("\t", "");
            codeStr = Regex.Replace(codeStr, @" {2,}", "");

            //insert values into snippet
            codeStr = codeStr.Replace("//TODO: Enter code for $FieldName$ calculation", _txtLineData.calculationString);
            codeStr = codeStr.Replace("$FieldName$", _txtLineData.fieldName);
            codeStr = codeStr.Replace("$Negative$", _txtLineData.allowNegativeString);
            codeStr = codeStr.Replace("$LineNumber$", _txtLineData.lineNumber);
            codeStr = codeStr.Replace("$InternalFieldName$", _txtLineData.InternalFieldName);
            codeStr = codeStr.Replace("$SummarySection$", _txtLineData.description);
            codeStr = codeStr.Replace("$ReferenceId$", _txtLineData.referenceId);
            codeStr = codeStr.Replace("$AttributeCategory$", _txtLineData.categoryString);
            codeStr = codeStr.Replace("$AttributeDescription$", _txtLineData.description);
            codeStr = codeStr.Replace("$Precision$", _txtLineData.precisionTypeString);
            codeStr = codeStr.Replace("$PrecisionType$", _txtLineData.precisionTypeString);
            codeStr = codeStr.Replace("$Rounding$", _txtLineData.roundingString);
            codeStr = codeStr.Replace("$RoundingType$", _txtLineData.roundingString);
            if (_txtLineData.dataType == DataType.Unknown) codeStr = codeStr.Replace("$DataType$", _txtLineData.dataTypeString);
            codeStr = codeStr.Replace("$end$", "");

            return codeStr;
        }

        public string GetCode
        {
            get { return this.codeString; }
        }

        //old stuff no longer used
        /*
        public string Date(LineData input)
        {

            return
            "#region Date " + input.fieldName + " (Line " + input.lineNumber + ")" + eol
           + "internal Calculatable<Date, RoundedToTheNearestInteger> " + input.internalFieldNameString + ";" + eol
           + "/// <summary> " + eol
           + "/// " + input.description + "  (Calculatable) " + eol
           + "/// Reference Number " + input.referenceId + "" + eol
           + "/// </summary> " + eol
           + "[Description(\"" + input.description + "\"), Category(\""+input.categoryString+"\"), ReferenceNumber(\"" + input.referenceId + "\"), LineNumber(\"" + input.lineNumber + "\")] " + eol
           + "public Date " + input.fieldName + " { get { return " + input.internalFieldNameString + ".Calculate(" + input.fieldName + "_Calculation); } } " + eol
           + "private Date " + input.fieldName + "_Calculation() " + eol
           + "{ " + eol
           + "\t " + input.calculationString + "" + eol
           + "\t //TODO: Enter code for " + input.fieldName + " calculation " + eol
           + "} " + eol
           + "#endregion " + input.fieldName + " " + eol;

        }
        public string WholeNumber(LineData input)
        {

            return
            "#region int " + input.fieldName + " (Line " + input.lineNumber + ")" + eol
           + "internal Calculatable<int, RoundedToTheNearestInteger> " + input.internalFieldNameString + ";" + eol
           + "/// <summary> " + eol
           + "/// " + input.description + "  (Calculatable) " + eol
           + "/// Reference Number " + input.referenceId + "" + eol
           + "/// </summary> " + eol
           + "[Number(AllowNegative = " + input.allowNegativeString + ")]" + eol
           + "[Description(\"" + input.description + "\"), Category(\""+input.categoryString+"\"), ReferenceNumber(\"" + input.referenceId + "\"), LineNumber(\"" + input.lineNumber + "\")] " + eol
           + "public int " + input.fieldName + " { get { return " + input.internalFieldNameString + ".Calculate(" + input.fieldName + "_Calculation); } } " + eol
           + "private int " + input.fieldName + "_Calculation() " + eol
           + "{ " + eol
           + "\t " + input.calculationString + "" + eol
           + "\t //TODO: Enter code for " + input.fieldName + " calculation " + eol
           + "} " + eol
           + "#endregion " + input.fieldName + " " + eol;
        
        }
        public string Money(LineData input)
        {
            

            return
           "#region decimal " + input.fieldName + " (Line " + input.lineNumber + ")" + eol
           + "internal Calculatable<decimal, RoundedToTheNearestInteger> " + input.internalFieldNameString + ";" + eol
           + "/// <summary> " + eol
           + "/// " + input.description + "  (Calculatable) " + eol
           + "/// Reference Number " + input.referenceId + "" + eol
           + "/// </summary> " + eol
           + "[Money(AllowNegative = " + input.allowNegativeString + ", Precision = PrecisionType.Zero)] " + eol
           + "[Description(\"" + input.description + "\"), Category(\""+input.categoryString+"\"), ReferenceNumber(\"" + input.referenceId + "\"), LineNumber(\"" + input.lineNumber + "\")] " + eol
           + "public decimal " + input.fieldName + " { get { return " + input.internalFieldNameString + ".Calculate(" + input.fieldName + "_Calculation); } } " + eol
           + "private decimal " + input.fieldName + "_Calculation() " + eol
           + "{ " + eol
           + "\t " + input.calculationString + "" + eol
           + "\t //TODO: Enter code for " + input.fieldName + " calculation " + eol
           + "} " + eol
           + "#endregion " + input.fieldName + " " + eol;


        }
        public static string Ratio(LineData input)
        {
            string eol = Environment.NewLine;

            return
            "#region decimal " + input.fieldName + " (Line " + input.lineNumber + ")" + eol
            + "internal Calculatable<decimal, RoundedToTwoDecimalPlaces> " + input.internalFieldNameString + ";" + eol
            + "/// <summary> " + eol
            + "/// " + input.description + "  (Calculatable) " + eol
            + "/// Reference Number " + input.referenceId + "" + eol
            + "/// </summary> " + eol
            + "[Ratio(Precision = PrecisionType.Zero)]" + eol
            + "[Description(\"" + input.description + "\"), Category(\""+input.categoryString+"\"), ReferenceNumber(\"" + input.referenceId + "\"), LineNumber(\"" + input.lineNumber + "\")] " + eol
            + "public decimal " + input.fieldName + " { get { return " + input.internalFieldNameString + ".Calculate(" + input.fieldName + "_Calculation); } } " + eol
            + "private decimal " + input.fieldName + "_Calculation() " + eol
            + "{ " + eol
            + "\t " + input.calculationString + "" + eol
            + "\t //TODO: Enter code for " + input.fieldName + " calculation " + eol
            + "} " + eol
            + "#endregion " + input.fieldName + " " + eol;

        }
        public static string YesNo(LineData input)
        {
            string eol = Environment.NewLine;

            return
                "#region bool " + input.fieldName + " (Line " + input.lineNumber + ")" + eol
           + "internal Calculatable<bool> " + input.internalFieldNameString + ";" + eol
           + "/// <summary> " + eol
           + "/// " + input.description + "  (Calculatable) " + eol
           + "/// Reference Number " + input.referenceId + "" + eol
           + "/// </summary> " + eol
           + "[Description(\"" + input.description + "\"), Category(\""+input.categoryString+"\"), ReferenceNumber(\"" + input.referenceId + "\"), LineNumber(\"" + input.lineNumber + "\")] " + eol
           + "public bool " + input.fieldName + " { get { return " + input.internalFieldNameString + ".Calculate(" + input.fieldName + "_Calculation); } } " + eol
           + "private bool " + input.fieldName + "_Calculation() " + eol
           + "{ " + eol
           + "\t " + input.calculationString + "" + eol
           + "\t //TODO: Enter code for " + input.fieldName + " calculation " + eol
           + "} " + eol
           + "#endregion " + input.fieldName + " " + eol;

        }


        */
    }
}
