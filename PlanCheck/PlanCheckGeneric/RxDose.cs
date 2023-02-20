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
                ResultDetails += $"Number of fractions mismatch\nPlan: {plan.DosePerFraction}\nPrescription: {targRx.DosePerFraction}\n\n";
            }
            // Dose per fraction does not agree
            if (targRx.DosePerFraction != plan.DosePerFraction)
            {
                Result = "Failure";
                ResultColor = ResultColorChoices.Fail;
                ResultDetails += $"Dose per fraction mismatch\nPlan: {plan.DosePerFraction}\nPrescription: {targRx.DosePerFraction}\n\n";
            }

            ResultDetails += $"{targRx.DosePerFraction * targRx.NumberOfFractions} = {targRx.DosePerFraction} x {targRx.NumberOfFractions}";
        }
    }
}
