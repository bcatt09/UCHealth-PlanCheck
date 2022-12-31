using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMS.TPS.Common.Model.API;

namespace PlanCheck.Checks
{
    public class RxApproval : PlanCheckGeneric
    {
        public RxApproval(PlanSetup plan) : base(plan) { }

        public override void RunTest(PlanSetup plan)
        {
            DisplayName = "Approval";
            TestExplanation = "";
            DisplayColor = ResultColorChoices.Pass;

            var rx = plan.RTPrescription;

            Result = rx.Status;
            ResultDetails = $"by {rx.HistoryUserDisplayName} at {rx.HistoryDateTime.ToString("MM/dd H:mm tt")}";
        }
    }
}
