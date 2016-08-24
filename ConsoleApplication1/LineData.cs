using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExcelSpecExtractor
{

    class LineData
    {
        private string changeableIndicator = "*";

        public string fieldName = "";
        public string referenceId = "";
        public string lineNumber = "";
        public DataType dataType;
        public bool isChangeable = false;
        public bool allowNegative = false;
        public string dataTypeString = "";
        public string description = "";
        public string taCalcNotes = "";
        public string calculationString = "";
        public string allowNegativeString = "false";
        public string precisionTypeString = ".Zero";
        public string categoryString = "Category";
        public string roundingString = "RoundedToTheNearestInteger";

        public LineData(string[] stringInput, string _category)
        {
            
            

            if (_category.Length > 0)
                categoryString = _category;

            int i = 0;
            fieldName = stringInput[i++];
            isChangeable = CheckForChangeable(fieldName);
            referenceId = stringInput[i++];
            lineNumber = stringInput[i++];
            dataTypeString = stringInput[i++];
            description = stringInput[i++];
            taCalcNotes = stringInput[i++];
            calculationString = FormatCalculation(stringInput[i++]);
            //internalFieldNameString = FormatInternalFieldName(fieldName);
            //precisionType = precisionType;

            switch (dataTypeString)
            {
                case "Text":
                    dataType = DataType.Text;
                    break;
                case "Whole number (non-negative)":
                    dataType = DataType.WholeNumber;
                    allowNegative = false;
                    allowNegativeString = "false";
                    break;
                case "Whole number (allow negative)":
                    dataType = DataType.WholeNumber;
                    allowNegative = true;
                    allowNegativeString = "true";
                    break;
                case "Ratio/Percentage":
                    dataType = DataType.Ratio;
                    break;
                case "Yes/No":
                    dataType = DataType.YesNo;
                    break;
                case "Yes/No/Blank":
                    dataType = DataType.YesNoBlank;
                    break;
                case "Money (allow negative)":
                    dataType = DataType.Money;
                    allowNegative = true;
                    allowNegativeString = "true";
                    break;
                case "Money (non-negative)":
                     dataType = DataType.Money;
                    allowNegative = false;
                    allowNegativeString = "false";
                    break;
                default:
                    dataType = DataType.Unknown;
                    dataTypeString = FormatUnknownDataTypeString(dataTypeString);
                    break;


            }


        }
        private string FormatUnknownDataTypeString(string _dataTypeString)
        {
            //types that have objects
            if (_dataTypeString == "Date") return _dataTypeString;
            if (_dataTypeString == "SSN/ITIN") return "SocialSecurityNumber";
            if (_dataTypeString == "Phone number") return "PhoneNumber";

            //odd balls
            return string.Format("Object/*{0}*/", _dataTypeString);
        }

        public string InternalFieldName
        {
            get { return fieldName.Substring(0, 1).ToLower() + fieldName.Substring(1, fieldName.Length - 1); }
        }

        private string FormatCalculation(string _calculation)
        {
            return "//" + _calculation.Replace("<br />", Environment.NewLine + "// ");
        }

        private bool CheckForChangeable(string _fieldName)
        {
            if (_fieldName.StartsWith(changeableIndicator))
            {
                fieldName = fieldName.TrimStart(changeableIndicator.ToCharArray());
                return true;
            }

            return false;
        }
    }
}
