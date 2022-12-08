using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMS.TPS.Common.Model.API;

namespace PlanCheck.Checks
{
    public class BodyContour : PlanCheckBase
    {
        protected override List<string> MachineExemptions => new List<string> { };
        public BodyContour(PlanSetup plan) : base(plan) { }

        public override void RunTest(PlanSetup plan)
        {
            DisplayName = "Body Contour";
            TestExplanation = "Checks that the Body structure exists";

            if (plan.StructureSet.Structures.Count(s => s.DicomType.ToUpper() == "BODY" || s.DicomType.ToUpper() == "EXTERNAL") > 0)
            {
                ResultDetails = "Exists";
                DisplayColor = ResultColorChoices.Pass;
            }
            else
            {
                ResultDetails = "Body structure not generated";
                DisplayColor = ResultColorChoices.Fail;
            }
        }
    }
}
