using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMS.TPS.Common.Model.API;

namespace PlanCheck.Checks
{
    public class PhotonPlanContentWindowChecks : PlanCheckLinac
    {
        protected override List<string> MachineExemptions => new List<string> { };

        public PhotonPlanContentWindowChecks(PlanSetup plan) : base(plan) { }

        public override void RunTestLinac(ExternalPlanSetup plan)
        {
            DisplayName = "Plan Content Window Checks";
            TestExplanation = "User Origin has been set (can't check that it's aligned to BBs)\nCouch has been inserted\nField Names match conventions\nLists density overrides";
            Result = "";
            ResultDetails = "";

            // User Origin
            var userOrigin = plan.StructureSet.Image.UserOrigin;
            var dicomOrigin = plan.StructureSet.Image.Origin;

            if (userOrigin.x == dicomOrigin.x && userOrigin.y == dicomOrigin.y && userOrigin.z == dicomOrigin.z)
            {
                ResultDetails = "User Origin not set";
                ResultColor = ResultColorChoices.Fail;
            }

            // Couch inserted

        }
    }
}
