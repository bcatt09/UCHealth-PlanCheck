using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMS.TPS.Common.Model.API;

namespace PlanCheck.Checks
{
    public class ReferencePointChecks : PlanCheckGeneric
    {
        protected override List<string> MachineExemptions => new List<string> { };

        public ReferencePointChecks(PlanSetup plan) : base(plan) { }

        public override void RunTest(PlanSetup plan)
        {
            DisplayName = "Reference Point Checks";
            TestExplanation = "Checks that the primary reference point ID is PlanID_MMYY if possible\n" +
                              "Checks total, daily, and session limits against the Rx";
            Result = "";

            var refPoint = plan.PrimaryReferencePoint;

            if (plan.Id.Length <= 11)
            {
                if(refPoint.Id.ToUpper() != plan.Id.ToUpper()+'_'+plan.StructureSet.Image.CreationDateTime?.ToString("MMyy"))
                {
                    Result += $"Primary reference point should be called {plan.Id + '_' + plan.StructureSet.Image.CreationDateTime?.ToString("MMyy")}";
                    ResultColor = ResultColorChoices.Warn;
                }
            }
            else
            {
                if(!refPoint.Id.Contains('_'+ plan.StructureSet.Image.CreationDateTime?.ToString("MMyy")))
                {
                    Result += $"Primary reference point should be include _{plan.StructureSet.Image.CreationDateTime?.ToString("MMyy")}";
                    ResultColor = ResultColorChoices.Warn;
                }
            }

            if (Math.Round(refPoint.DailyDoseLimit.Dose, 1) != Math.Round(plan.DosePerFraction.Dose, 1))
            {
                Result += "Please check daily reference point limits\n";
                ResultColor = ResultColorChoices.Warn;
            }

            if (refPoint.HasLocation(plan))
            {
                Result += $"Primary reference point has a volume (I thought that wasn't possible anymore)\n";
                ResultColor = ResultColorChoices.Warn;
            }

            if (Math.Round(refPoint.TotalDoseLimit.Dose,1) != Math.Round(plan.TotalDose.Dose,1) || Math.Round(refPoint.SessionDoseLimit.Dose,1) != Math.Round(plan.DosePerFraction.Dose,1))
            {
                Result += "Please check reference point limits\n";
                ResultColor = ResultColorChoices.Fail;
            }

            Result = Result.TrimEnd('\n');
            ResultDetails = $"{refPoint.Id}\nTotal: {refPoint.TotalDoseLimit}\nDaily: {refPoint.DailyDoseLimit}\nSession: {refPoint.SessionDoseLimit}";
        }
    }
}
