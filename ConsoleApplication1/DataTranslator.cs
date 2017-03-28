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
using System.Reflection;
using System.Diagnostics;

namespace ExcelSpecExtractor
{
    class DataTranslator
    {
        private string snippetDir = "~/Snippets/";
        string eol = Environment.NewLine;
        private string codeString;


        public DataTranslator(LineData ld)
        {
            XDocument snippet = GetSnippet(ld.dataType, ld.isChangeable);
            codeString = GenerateCode(snippet, ld);
        }

        public Stream GetResourceTextFile(string filename)
        {

            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "ExcelSpecExtractor.Snippets." + filename;
            //do you see me?
            return assembly.GetManifestResourceStream(resourceName);

            
        }

        private XDocument GetSnippet(DataType _dataType, bool _isChangeable)
        {
            switch (_dataType)
            {
                case DataType.Text:
                    return _isChangeable
                        ? XDocument.Load(GetResourceTextFile("InsertChangeableString.snippet"))
                        : XDocument.Load(GetResourceTextFile("InsertCalculatableString.snippet"));
                case DataType.Money:
                    return _isChangeable
                        ? XDocument.Load(GetResourceTextFile("InsertChangeableMoney.snippet"))
                        : XDocument.Load(GetResourceTextFile("InsertCalculatableMoney.snippet"));
                case DataType.WholeNumber:
                    return _isChangeable
                        ? XDocument.Load(GetResourceTextFile("InsertChangeableNumber.snippet"))
                        : XDocument.Load(GetResourceTextFile("InsertCalculatableNumber.snippet"));
                case DataType.Ratio:
                    return _isChangeable
                        ? XDocument.Load(GetResourceTextFile("InsertChangeableRatio.snippet"))
                        : XDocument.Load(GetResourceTextFile("InsertCalculatableRatio.snippet"));
                case DataType.YesNo:
                   return _isChangeable
                        ? XDocument.Load(GetResourceTextFile("InsertChangeableBoolean.snippet"))
                        : XDocument.Load(GetResourceTextFile("InsertCalculatableBoolean.snippet"));
                case DataType.YesNoBlank:
                   return _isChangeable
                        ? XDocument.Load(GetResourceTextFile("InsertChangeableNullableBoolean.snippet"))
                        : XDocument.Load(GetResourceTextFile("InsertCalculatableNullableBoolean.snippet"));
                default:
                    return _isChangeable
                        ? XDocument.Load(GetResourceTextFile("InsertChangeable.snippet"))
                        : XDocument.Load(GetResourceTextFile("InsertCalculatable.snippet"));

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
            codeStr = codeStr.Replace("$FieldName$", _txtLineData.devFieldName);
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
