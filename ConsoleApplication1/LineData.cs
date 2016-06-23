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

        public string FieldName = "";
        public string ReferenceId = "";
        public string LineNumber = "";
        public DataType DataType;
        public bool AllowNegative;
        public string Description = "";
        public string TaCalcNotes = "";
        public string Calculation = "";
        public string InternalFieldName = "";
        public string allowNegative = "false";
        public string precisionType = ".Zero";
        public string Category = "Category";
        public bool isChangeable = false;
        private string dataTypeString;

        public LineData(string[] stringInput, string _category)
        {
            
            

            if (_category.Length > 0)
                Category = _category;

            int i = 0;
            FieldName = stringInput[i++];
            isChangeable = CheckForChangeable(FieldName);
            ReferenceId = stringInput[i++];
            LineNumber = stringInput[i++];
            dataTypeString = stringInput[i++];
            Description = stringInput[i++];
            TaCalcNotes = stringInput[i++];
            Calculation = FormatCalculation(stringInput[i++]);
            InternalFieldName = FormatInternalFieldName(FieldName);
            //precisionType = precisionType;

            switch (dataTypeString)
            {
                case "Text":
                    DataType = DataType.Text;
                    break;
                case "Date":
                    DataType = DataType.Date;
                    break;
                case "Whole number (non-negative)":
                    DataType = DataType.WholeNumber;
                    AllowNegative = false;
                    allowNegative = "false";
                    break;
                case "Whole number (allow negative)":
                    DataType = DataType.WholeNumber;
                    AllowNegative = true;
                    allowNegative = "true";
                    break;
                case "Ratio/Percentage":
                    DataType = DataType.Ratio;
                    break;
                case "Yes/No":
                    DataType = DataType.YesNo;
                    break;
                case "Money (allow negative)":
                    DataType = DataType.Money;
                    AllowNegative = true;
                    allowNegative = "true";
                    break;
                case "Money (non-negative)":
                default:
                    DataType = DataType.Money;
                    AllowNegative = false;
                    allowNegative = "false";
                    break;


            }


        }

        private string FormatInternalFieldName(string _FieldName)
        {
            return "_" + _FieldName.Substring(0, 1).ToLower() + FieldName.Substring(1, FieldName.Length - 1);
        }

        private string FormatCalculation(string Calculation)
        {
            return "//" + Calculation.Replace("<br />", Environment.NewLine + "// ");
        }

        private bool CheckForChangeable(string _FieldName)
        {
            if (_FieldName.StartsWith(changeableIndicator))
            {
                FieldName = FieldName.TrimStart(changeableIndicator.ToCharArray());
                return true;
            }

            return false;
        }
    }
}
