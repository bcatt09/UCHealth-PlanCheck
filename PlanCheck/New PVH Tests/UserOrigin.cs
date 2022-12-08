using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMS.TPS.Common.Model.API;

namespace PlanCheck.Checks
{
    public class UserOrigin : PlanCheckBase
    {
        protected override List<string> MachineExemptions => new List<string> { };

        public UserOrigin(PlanSetup plan) : base(plan) { }

        public override void RunTest(PlanSetup plan)
        {
            DisplayName = "User Origin Set";
            TestExplanation = "Checks that the User Origin has been moved (doesn't check that it's actually at BBs)";

            var userOrigin = plan.StructureSet.Image.UserOrigin;
            var dicomOrigin = plan.StructureSet.Image.Origin;

            if (userOrigin.x == dicomOrigin.x && userOrigin.y == dicomOrigin.y && userOrigin.z == dicomOrigin.z)
            {
                ResultDetails = "User Origin not set";
                DisplayColor = ResultColorChoices.Fail;
            }
            else
            {
                ResultDetails = "User Origin has been set";
                DisplayColor = ResultColorChoices.Pass;
            }
        }
    }
}
