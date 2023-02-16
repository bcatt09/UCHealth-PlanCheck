using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMS.TPS.Common.Model.API;

namespace PlanCheck.Checks
{
    public class RxSite : PlanCheckGeneric
    {
        public RxSite(PlanSetup plan) : base(plan) { }

        public override void RunTest(PlanSetup plan)
        {
            DisplayName = "Site";
            DisplayColor = ResultColorChoices.Pass;
            TestExplanation = "Displays the Prescription Site";

            var rx = plan.RTPrescription;

            if (rx == null)
            {
                Result = "No Prescription Attached";
                DisplayColor = ResultColorChoices.Fail;

                return;
            }

            Result = rx.Site;
        }
    }
}
