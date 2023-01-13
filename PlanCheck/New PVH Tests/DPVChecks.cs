using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMS.TPS.Common.Model.API;

namespace PlanCheck.Checks
{
    public class DPVChecks : PlanCheckBase
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
                DisplayColor = ResultColorChoices.Warn;
            }

            if (refPoint.PatientVolumeId != plan.TargetVolumeID)
            {
                Result += $"DPV Volume ({refPoint.PatientVolumeId}) does not\nmatch Target Volume ({plan.TargetVolumeID})\n";
                DisplayColor = ResultColorChoices.Fail;
            }

            if(refPoint.TotalDoseLimit != plan.TotalDose || refPoint.SessionDoseLimit != plan.DosePerFraction)
            {
                Result += "Please check reference point limits\n";
                DisplayColor = ResultColorChoices.Fail;
            }

            Result = Result.TrimEnd('\n');
            ResultDetails = $"{refPoint.Id}\nTotal: {refPoint.TotalDoseLimit}\nDaily: {refPoint.DailyDoseLimit}\nSession: {refPoint.SessionDoseLimit}";
        }
    }
}
