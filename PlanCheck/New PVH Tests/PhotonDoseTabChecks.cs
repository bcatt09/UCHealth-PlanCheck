using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMS.TPS.Common.Model.API;

namespace PlanCheck.Checks
{
    public class PhotonDoseTabChecks : PlanCheckBase
    {
        protected override List<string> MachineExemptions => new List<string> { };

        public PhotonDoseTabChecks(PlanSetup plan) : base(plan) { }

        public override void RunTest(PlanSetup plan)
        {
            DisplayName = "Dose Tab Checks";
            TestExplanation = "Rx is associate with the plan\nDPV used as primmary reference point (and named appropriately)\nPlan normalization set to Plan Normalization Value";
            Result = "";
            ResultDetails = "";

            // Rx associated with plan
            if (plan.RTPrescription == null)
            {
                Result = "Failure";
                DisplayColor = ResultColorChoices.Fail;
                ResultDetails += "No prescription attached\n";
            }
            // Primary reference point
            if (!plan.PrimaryReferencePoint.Id.ToUpper().Contains("DPV"))
            {
                Result = "Failure";
                DisplayColor = ResultColorChoices.Fail;
                ResultDetails += "Primary Reference Point not set to \"DPV\" (or there is a typo)\n";
            }
            // Plan normalization
            if (!plan.PlanNormalizationMethod.Contains("Plan Normalization Value"))
            {
                Result = "Failure";
                DisplayColor = ResultColorChoices.Fail;
                ResultDetails += $"Plan Normalization Mode not set to Value\n{plan.PlanNormalizationMethod}\n";
            }

            if (Result == "")
            {
                Result = "Pass";
                DisplayColor = ResultColorChoices.Pass;
            }

            ResultDetails = ResultDetails.TrimEnd('\n');
        }
    }
}
