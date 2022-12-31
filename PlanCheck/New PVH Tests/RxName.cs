using NLog.Targets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Security;
using VMS.TPS.Common.Model.API;

namespace PlanCheck.Checks
{
    public class RxName : PlanCheckGeneric
    {
        public RxName(PlanSetup plan) : base(plan) { }

        public override void RunTest(PlanSetup plan)
        {
            DisplayName = "Prescirption Name";
            TestExplanation = "Displays the prescription name and tries to check laterality if possible";
            DisplayColor = ResultColorChoices.Pass;

            var rx = plan.RTPrescription;

            Result = rx.Name;

            var leftLateralityPattern = new Regex(@"(^(L(t)?|Left) .*)|(.*(_L)$)");
            var rightLateralityPattern = new Regex(@"(^(R(t)?|Right) .*)|(.*(_R)$)");

            // Prescription has laterality
            if (leftLateralityPattern.IsMatch(rx.Name) || rightLateralityPattern.IsMatch(rx.Name))
            {
                // These are invalid plan targets to check for laterality
                if (CheckForNoTarget(plan) || CheckForBodyStructureTarget(plan))
                    return;

                var target = plan.StructureSet.Structures.Single(x => x.Id == plan.TargetVolumeID);
                var body = plan.StructureSet.Structures.Where(x => x.DicomType.ToUpper() == "BODY" || x.DicomType.ToUpper() == "EXTERNAL").OrderByDescending(x => x.Volume).First();

                var offset = target.CenterPoint.x - body.CenterPoint.x;

                if (leftLateralityPattern.IsMatch(rx.Name))
                {
                    // Right sided target structure
                    if (offset < 0)
                    {
                        ResultDetails = $"Laterality does not match\nPrescription: Left\n{target.Id}: Right";
                        DisplayColor = ResultColorChoices.Warn;
                    }
                }
                else if (rightLateralityPattern.IsMatch(rx.Name))
                {
                    // Left sided target structure
                    if (offset > 0)
                    {
                        ResultDetails = $"Laterality does not match\nPrescription: Right\n{target.Id}: Left";
                        DisplayColor = ResultColorChoices.Warn;
                    }
                }
            }
        }

        private bool CheckForNoTarget(PlanSetup plan)
        {
            if (plan.TargetVolumeID == "")
            {
                ResultDetails = "Can't check laterality (no target volume)";
                DisplayColor = ResultColorChoices.Warn;
                return true;
            }

            return false;
        }

        private bool CheckForBodyStructureTarget(PlanSetup plan)
        {
            var target = plan.StructureSet.Structures.Single(x => x.Id == plan.TargetVolumeID);

            if (target.DicomType.ToUpper() == "BODY" || target.DicomType.ToUpper() == "EXTERNAL")
            {
                ResultDetails = $"Can't check laterality ({target.Id} is target)";
                DisplayColor = ResultColorChoices.Warn;
                return true;
            }

            return false;
        }
    }
}
