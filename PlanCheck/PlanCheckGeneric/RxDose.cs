using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMS.TPS.Common.Model.API;

namespace PlanCheck.Checks
{
    public class RxDose : PlanCheckGeneric
    {
        public RxDose(PlanSetup plan) : base(plan) { }

        public override void RunTest(PlanSetup plan)
        {
            DisplayName = "Dose";
            ResultColor = ResultColorChoices.Pass;
            TestExplanation = "Checks the total dose and dose per fraction of the plan against the prescription";
            ResultDetails = "";

            var rx = plan.RTPrescription;
            var targRx = rx?.Targets.OrderByDescending(x => x.DosePerFraction.Dose).First();

            if (rx == null)
            {
                Result = "No Prescription Attached";
                ResultColor = ResultColorChoices.Fail;

                return;
            }

            // Number of fractions do not agree
            if (targRx.NumberOfFractions != plan.NumberOfFractions)
            {
                Result = "Failure";
                ResultColor = ResultColorChoices.Fail;
                ResultDetails += $"Number of fractions mismatch\nPlan: {plan.NumberOfFractions} fx\nPrescription: {targRx.NumberOfFractions} fx\n\n";
            }
            // Dose per fraction does agree with a target but not the highest dose one
            if (rx?.Targets.Any(x => x.DosePerFraction == plan.DosePerFraction) == true)
            {
                Result = "Warning";
                ResultColor = ResultColorChoices.Warn;
                ResultDetails += $"Plan dose does not match the highest dose target volume\nPlan: {plan.DosePerFraction}\nPrescription: {targRx.DosePerFraction}\n\n";
            }
            // Dose per fraction does not agree
            else if (targRx.DosePerFraction != plan.DosePerFraction)
            {
                Result = "Failure";
                ResultColor = ResultColorChoices.Fail;
                ResultDetails += $"Dose per fraction mismatch\nPlan: {plan.DosePerFraction}\nPrescription: {targRx.DosePerFraction}\n\n";
            }

            // If 1 target, just display the fractionation
            if (rx.Targets.Count() > 1)
                ResultDetails += String.Join("\n", rx.Targets.OrderByDescending(x => x.DosePerFraction.Dose).Select(x => $"{x.TargetId} - {x.DosePerFraction * x.NumberOfFractions} = {x.DosePerFraction} x {x.NumberOfFractions}"));
            // Otherwise display it per target (along with the name of the target)
            else
                ResultDetails += $"{targRx.DosePerFraction * targRx.NumberOfFractions} = {targRx.DosePerFraction} x {targRx.NumberOfFractions}";

            ResultDetails = ResultDetails.TrimEnd('\n');
        }
    }
}
