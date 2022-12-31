using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMS.TPS.Common.Model.API;

namespace PlanCheck.Checks
{
    public class RxComments : PlanCheckGeneric
    {
        public RxComments(PlanSetup plan) : base(plan) { }

        public override void RunTest(PlanSetup plan)
        {
            DisplayName = "Special Orders";
            TestExplanation = "";
            DisplayColor = ResultColorChoices.Pass;

            var rx = plan.RTPrescription;

            Result = rx.Comment == "" ? "None" : rx.Comment;

            // if bolus
            // check plan for bolus usage
        }
    }
}
