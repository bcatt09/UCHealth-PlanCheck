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
            TestExplanation = "";
            DisplayColor = ResultColorChoices.Pass;

            Result = String.Join(", ", plan.RTPrescription.Energies.OrderBy(x => x));
        }
    }
}
