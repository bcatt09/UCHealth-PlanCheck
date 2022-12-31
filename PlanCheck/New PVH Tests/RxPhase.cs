using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMS.TPS.Common.Model.API;

namespace PlanCheck.Checks
{
    public class RxPhase : PlanCheckGeneric
    {
        public RxPhase(PlanSetup plan) : base(plan) { }

        public override void RunTest(PlanSetup plan)
        {
            DisplayName = "Primary/Boost";
            TestExplanation = "Displays the prescribed phase and if it is \"Boost\" checks if the plan is named accordingly";
            DisplayColor = ResultColorChoices.Pass;
            Result = plan.RTPrescription.PhaseType;

            if (plan.RTPrescription.PhaseType == "Boost")
            {
                if (!plan.Id.ToUpper().Contains("Boost") && !plan.Id.ToUpper().Contains("Bst"))
                {
                    ResultDetails = "Should the plan be named \"boost\"?";
                    DisplayColor = ResultColorChoices.Warn;
                }
            }
        }
    }
}
