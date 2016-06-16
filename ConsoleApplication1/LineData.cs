using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExcelSpecExtractor
{

    class LineData
    {
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

        public LineData(string[] stringInput)
        {
            int i = 0;
            FieldName = stringInput[i++];
            ReferenceId = stringInput[i++];
            LineNumber = stringInput[i++];


            switch (stringInput[i++])
            {

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

            Description = stringInput[i++];
            TaCalcNotes = stringInput[i++];
            Calculation = FormatCalculation(stringInput[i++]);
            InternalFieldName = FormatInternalFieldName(FieldName);
            //precisionType = precisionType;
        }

        private string FormatInternalFieldName(string FieldName)
        {
            return "_" + FieldName.Substring(0, 1).ToLower() + FieldName.Substring(1, FieldName.Length - 1);
        }

        private string FormatCalculation(string Calculation)
        {
            return "//" + Calculation.Replace("<br />", Environment.NewLine + "// ");
        }
    }
}
