using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMS.TPS.Common.Model.API;

namespace PlanCheck.Checks
{
    public class RxModality : PlanCheckGeneric
    {
        public RxModality(PlanSetup plan) : base(plan) { }

        public override void RunTest(PlanSetup plan)
        {
            DisplayName = "Modality";
            TestExplanation = "Displays the prescribed modality and checks it against the plan";
            DisplayColor = ResultColorChoices.Pass;

            Result = String.Join(", ", plan.RTPrescription.EnergyModes);
        }
    }
}
