using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMS.TPS.Common.Model.API;

namespace PlanCheck.Checks
{
    public class RxTechnique : PlanCheckGeneric
    {
        public RxTechnique(PlanSetup plan) : base(plan) { }

        public override void RunTest(PlanSetup plan)
        {
            DisplayName = "Technique";
            TestExplanation = "";
            DisplayColor = ResultColorChoices.Pass;

            Result = plan.RTPrescription.Technique;
        }
    }
}
