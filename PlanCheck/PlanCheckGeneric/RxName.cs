using NLog.Targets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Security;
using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;

namespace PlanCheck.Checks
{
    public class RxName : PlanCheckGeneric
    {
        public RxName(PlanSetup plan) : base(plan) { }

        public override void RunTest(PlanSetup plan)
        {
            DisplayName = "Prescription Name";
            TestExplanation = "Displays the prescription name and tries to check laterality if possible";
            ResultColor = ResultColorChoices.Pass;

            var rx = plan.RTPrescription;

            if (rx == null)
            {
                Result = "No Prescription Attached";
                ResultColor = ResultColorChoices.Fail;

                return;
            }

            Result = rx.Name;

            var leftLateralityPattern = new Regex(@"(^(L(t)?|Left) .*)|(.*(_L)$)");
            var rightLateralityPattern = new Regex(@"(^(R(t)?|Right) .*)|(.*(_R)$)");

            // Prescription has laterality
            if (leftLateralityPattern.IsMatch(rx.Name) || rightLateralityPattern.IsMatch(rx.Name) && !rx.Name.ToLower().Contains("spine"))
            {
                VVector targetLoc;
                String targetVerbiage;

                var body = plan.StructureSet.Structures
                                .Where(x => x.DicomType.ToUpper() == "BODY" || x.DicomType.ToUpper() == "EXTERNAL")
                                .OrderByDescending(x => x.Volume)
                                .First();

                // These are invalid plan targets to check for laterality
                if (CheckForNoTarget(plan) || CheckForBodyStructureTarget(plan))
                {
                    targetLoc = plan.Beams.First(x => !x.IsSetupField).IsocenterPosition;
                    targetVerbiage = "Isocenter Location";
                }
                else
                {
                    var target = plan.StructureSet.Structures.FirstOrDefault(x => x.Id == plan.TargetVolumeID);
                    targetLoc = target.CenterPoint;
                    targetVerbiage = target.Id;
                }

                var offset = targetLoc.x - body.CenterPoint.x;

                if (leftLateralityPattern.IsMatch(rx.Name))
                {
                    // Right sided target structure
                    if (offset < 0)
                    {
                        ResultDetails = $"Laterality does not match\nPrescription: Left\n{targetVerbiage}: Right";
                        ResultColor = ResultColorChoices.Warn;
                    }
                }
                else if (rightLateralityPattern.IsMatch(rx.Name))
                {
                    // Left sided target structure
                    if (offset > 0)
                    {
                        ResultDetails = $"Laterality does not match\nPrescription: Right\n{targetVerbiage}: Left";
                        ResultColor = ResultColorChoices.Warn;
                    }
                }
            }
        }

        private bool CheckForNoTarget(PlanSetup plan)
        {
            if (plan.TargetVolumeID == "")
            {
                ResultDetails = "No target volume for laterality check\nUsing isocenter location instead of target to check laterality";
                ResultColor = ResultColorChoices.Warn;

                return true;
            }

            var target = plan.StructureSet.Structures.FirstOrDefault(x => x.Id == plan.TargetVolumeID);

            if (target == null)
            {
                Result = $"Structure does not exist matching plan target ({plan.TargetVolumeID})\nUsing isocenter location instead of target to check laterality";
                ResultColor = ResultColorChoices.Fail;

                return true;
            }

            return false;
        }

        private bool CheckForBodyStructureTarget(PlanSetup plan)
        {
            var target = plan.StructureSet.Structures.Single(x => x.Id == plan.TargetVolumeID);

            if (target.DicomType.ToUpper() == "BODY" || target.DicomType.ToUpper() == "EXTERNAL")
            {
                ResultDetails = $"{target.Id} is target\nUsing isocenter location instead of target to check laterality";
                ResultColor = ResultColorChoices.Warn;
                return true;
            }

            return false;
        }
    }
}
