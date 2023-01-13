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
            TestExplanation = "Displays the prescribed phase and if it is \"Boost\" checks if the plan name contains \"Boost\" or \"Bst\" or \"Gy\"";
            DisplayColor = ResultColorChoices.Pass;
            Result = plan.RTPrescription.PhaseType;

            if (plan.RTPrescription.PhaseType == "Boost")
            {
                // Check plan name
                if (!plan.Id.ToUpper().Contains("BOOST") && 
                    !plan.Id.ToUpper().Contains("BST") &&
                    !plan.Id.ToUpper().Contains("GY"))

                {
                    ResultDetails = "Should the plan be named as some form of boost?\nI can remove this check if it's not being helpful";
                    DisplayColor = ResultColorChoices.Warn;
                }
            }
        }
    }
}
