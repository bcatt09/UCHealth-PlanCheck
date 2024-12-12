using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using VMS.TPS.Common.Model.API;

namespace PlanCheck.Checks
{
    public class DensityOverrides : PlanCheckGeneric
    {
        protected override List<string> MachineExemptions => new List<string> { };

        public DensityOverrides(PlanSetup plan) : base(plan) { }

        public override void RunTest(PlanSetup plan)
        {
            DisplayName = "Density Overrides";
            TestExplanation = "Lists all non-couch density overrides";
            Result = "";
            ResultColor = ResultColorChoices.Pass;

            foreach (var s in plan.StructureSet.Structures.Where(x => x.DicomType != "SUPPORT"))
            {
                if (s.GetAssignedHU(out double HU))
                {
                    Result += $"{s.Id}: {Math.Round(HU,1)} HU\n";
                }
            }

            if (Result == "")
            {
                Result = "No HU overrides";
            }

            Result = Result.TrimEnd('\n');
        }
    }
}
