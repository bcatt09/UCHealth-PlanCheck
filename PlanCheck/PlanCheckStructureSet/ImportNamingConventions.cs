using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using VMS.TPS.Common.Model.API;

namespace PlanCheck.Checks
{
    internal class ImportNamingConventions : PlanCheckStructureSet
    {
        protected override List<string> MachineExemptions => new List<string> { };

        public ImportNamingConventions(StructureSet structureSet) : base(structureSet) { }

        public override void RunTestStructureSet(StructureSet structureSet)
        {
            DisplayName = "SS/Image/Series Naming";
            ResultDetails = "";
            TestExplanation = "Checks that the Series, 3D Image, and Structure Set follow the naming convention \"Site MMYY\"";

            var regex = new Regex(@".*( |_)\d{4}");

            var names = new List<string>
            {
                structureSet.Id,
                structureSet.Image.Id,
                structureSet.Image.Series.Id
            };

            if (!regex.IsMatch(names[0]))
            {
                ResultDetails += $"Structure Set is not named appropriately ({names[0]})\n";
            }
            if (!regex.IsMatch(names[1]))
            {
                ResultDetails += $"3D Image is not named appropriately ({names[1]})\n";
            }
            if (!regex.IsMatch(names[2]))
            {
                ResultDetails += $"Series is not named appropriately ({names[2]})\n";
            }

            if (ResultDetails != "")
            {
                Result = "Warning";
                ResultColor = ResultColorChoices.Warn;
            }
            else
            {
                if (!(names[0] == names[1] && names[1] == names[2]))
                {
                    Result = "Warning";
                    ResultColor = ResultColorChoices.Warn;
                    ResultDetails += $"Names do not all match\nStructure Set: {names[0]}\n3D Image: {names[1]}\nSeries: {names[2]}";
                }
                else
                {
                    Result = "Pass";
                    ResultColor = ResultColorChoices.Pass;
                }
            }

            ResultDetails = ResultDetails.TrimEnd('\n');
        }
    }
}
