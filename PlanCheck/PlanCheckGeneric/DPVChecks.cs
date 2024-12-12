using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMS.TPS.Common.Model.API;

namespace PlanCheck.Checks
{
    public class DPVChecks : PlanCheckGeneric
    {
        protected override List<string> MachineExemptions => new List<string> { };

        public DPVChecks(PlanSetup plan) : base(plan) { }

        public override void RunTest(PlanSetup plan)
        {
            DisplayName = "DPV Checks";
            TestExplanation = "Checks that the primary reference point ID contains \"DPV\"\n" +
                              "Checks total, daily, and session limits against the Rx\n" +
                              "Checks that the DPV volume is the same as the target volume";
            Result = "";

            var refPoint = plan.PrimaryReferencePoint;

            if (!refPoint.Id.ToUpper().Contains("DPV"))
            {
                Result += $"DPV ID does not contain \"DPV\" ({refPoint.Id})";
                ResultColor = ResultColorChoices.Warn;
            }

            if (Math.Round(refPoint.DailyDoseLimit.Dose, 1) != Math.Round(plan.DosePerFraction.Dose, 1))
            {
                Result += "Please check daily reference point limits\n";
                ResultColor = ResultColorChoices.Warn;
            }

            if (Math.Round(refPoint.TotalDoseLimit.Dose,1) != Math.Round(plan.TotalDose.Dose,1) || Math.Round(refPoint.SessionDoseLimit.Dose,1) != Math.Round(plan.DosePerFraction.Dose,1))
            {
                Result += "Please check reference point limits\n";
                ResultColor = ResultColorChoices.Fail;
            }

            if (refPoint.HasLocation(plan))
            {
                Result += "Primary reference point has a physical location (I thought that wasn't possible anymore)\n";
                ResultColor = ResultColorChoices.Fail;
            }

            Result = Result.TrimEnd('\n');
            ResultDetails = $"{refPoint.Id}\nTotal: {refPoint.TotalDoseLimit}\nDaily: {refPoint.DailyDoseLimit}\nSession: {refPoint.SessionDoseLimit}";
        }
    }
}
