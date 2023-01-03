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

            var rx = String.Join(", ", plan.RTPrescription.Energies.OrderBy(x => x));
            var planned = String.Join(", ", plan.Beams
                                                .Where(x => !x.IsSetupField)
                                                .Select(x => x.EnergyModeDisplayName.Replace("X-FFF", "FFF")) // Format FFF energies to match Rx formatting
                                                .Distinct()
                                                .OrderBy(x => x));

            // Energies do not match
            if (rx != planned)
            {
                Result = "Failure";
                DisplayColor = ResultColorChoices.Fail;
                ResultDetails = $"Energy mismatch\nPlan: {planned}\nPrescription: {rx}";
            }
            else
            {
                Result = String.Join(", ", plan.RTPrescription.Energies.OrderBy(x => x));
            }
        }
    }
}
