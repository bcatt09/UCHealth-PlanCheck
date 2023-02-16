using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMS.TPS.Common.Model.API;

namespace PlanCheck.Checks
{
    public class RxEnergy : PlanCheckGeneric
    {
        public RxEnergy(PlanSetup plan) : base(plan) { }

        public override void RunTest(PlanSetup plan)
        {
            DisplayName = "Energy";
            TestExplanation = "Checks prescribed energies against plan";
            DisplayColor = ResultColorChoices.Pass;

            if (plan.RTPrescription == null)
            {
                Result = "No Prescription Attached";
                DisplayColor = ResultColorChoices.Fail;

                return;
            }

            // Adds a 0 in front of the energy if it doesn't start with a 1 or 2
            Func<string, string> add0 = x => (x[0] > '2' && x[0] <= '9') ? "0" + x : x;
            var rx = String.Join(", ", plan.RTPrescription.Energies.OrderBy(x => add0(x)));
            var planned = String.Join(", ", plan.Beams
                                                .Where(x => !x.IsSetupField)
                                                .Select(x => x.EnergyModeDisplayName.Replace("X-FFF", "FFF")) // Format FFF energies to match Rx formatting
                                                .Distinct()
                                                .OrderBy(x => add0(x)));

            // Energies do not match
            if (rx != planned)
            {
                Result = "Failure";
                DisplayColor = ResultColorChoices.Fail;
                ResultDetails = $"Energy mismatch\nPlan: {planned}\nPrescription: {rx}";
            }
            else
            {
                Result = rx;
            }
        }
    }
}
