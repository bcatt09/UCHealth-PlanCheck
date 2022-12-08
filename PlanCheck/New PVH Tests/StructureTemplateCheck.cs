using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMS.TPS.Common.Model.API;

namespace PlanCheck.Checks
{
    public class StructureTemplateCheck : PlanCheckBase
    {
        protected override List<string> MachineExemptions => new List<string> { };

        public StructureTemplateCheck(PlanSetup plan) : base(plan) { }

        public override void RunTest(PlanSetup plan)
        {
            DisplayName = "Structure Template";
            TestExplanation = "Checks that structures have been added (does not check for any specific template)";

            if (plan.StructureSet.Structures.Count() > 1)
            {
                ResultDetails = "Structures added";
                DisplayColor = ResultColorChoices.Pass;
            }
            else
            {
                ResultDetails = "No structures added";
                DisplayColor = ResultColorChoices.Fail;
            }
        }
    }
}
