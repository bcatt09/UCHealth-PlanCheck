using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using VMS.TPS.Common.Model.API;

namespace PlanCheck.Checks
{
    public class UseGatedChecks : PlanCheckBasePhoton
    {
        protected override List<string> MachineExemptions => DepartmentInfo.LinearAccelerators;

        public UseGatedChecks(PlanSetup plan) : base(plan) { }
        
        public override void RunTestPhoton(ExternalPlanSetup plan)
        {
            DisplayName = "\"Use Gated\"?";
            TestExplanation = "Checks to see if \"Use Gated\" should be checked off in plan properties based on department standards";

            //#region Macomb Group
            //if (MachineID == DepartmentInfo.MachineNames.CLA_TB ||
            //    MachineID == DepartmentInfo.MachineNames.MPH_TB ||
            //    MachineID == DepartmentInfo.MachineNames.MAC_TB)
            //{
            //    string planningImageComment = plan.StructureSet.Image.Comment;
            //    string planningImageId = plan.StructureSet.Image.Id;
            //    int? numberOfFractions = plan.NumberOfFractions;

            //    if (numberOfFractions < 10 && (planningImageComment.Contains("AIP", StringComparison.CurrentCultureIgnoreCase) || planningImageComment.Contains("avg", StringComparison.CurrentCultureIgnoreCase) || planningImageId.Contains("AIP", StringComparison.CurrentCultureIgnoreCase) || planningImageId.Contains("avg", StringComparison.CurrentCultureIgnoreCase) || planningImageId.Contains("ave", StringComparison.CurrentCultureIgnoreCase) || planningImageComment.Contains("%")))
            //    {
            //        if (plan.UseGating)
            //        {
            //            Result = "";
            //            ResultDetails = "\"Use Gating\" is checked";
            //            DisplayColor = ResultColorChoices.Pass;
            //        }
            //        else
            //        {
            //            Result = "Warning";
            //            ResultDetails = "Plan has a low number of fractions and looks to contain a 4D image.  Should \"Use Gated\" be checked?";
            //            DisplayColor = ResultColorChoices.Warn;
            //        }
            //    }
            //    else
            //    {
            //        Result = "Pass";
            //        DisplayColor = ResultColorChoices.Pass;
            //    }
            //}
            //#endregion

            //else
            //    TestNotImplemented();
        }
    }
}
