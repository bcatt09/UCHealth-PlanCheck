using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Configuration;
using System.Text;
using System.Threading.Tasks;
using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;

namespace PlanCheck.Checks
{
    public class VerificationPlan : PlanCheckPhoton
    {
        protected override List<string> MachineExemptions => new List<string> { };

        public VerificationPlan(PlanSetup plan) : base(plan) { }

        public override void RunTestPhoton(ExternalPlanSetup plan)
        {
            DisplayName = "Verification Plan";
            TestExplanation = "Lists all verification plans created from this plan and the corresponding Structure Set names";
            DisplayColor = ResultColorChoices.Pass;
            ResultDetails = "";

            var verificationPlans = plan.Course.Patient.Courses.SelectMany(x => x.PlanSetups).Where(x => x.VerifiedPlan == plan);

            // Check if one was made for VMAT/IMRT plans
            var firstBeam = plan.Beams.First();
            if (firstBeam.MLCPlanType == MLCPlanType.ArcDynamic || firstBeam.MLCPlanType == MLCPlanType.VMAT || (firstBeam.MLCPlanType == MLCPlanType.DoseDynamic && firstBeam.ControlPoints.Count > 21))
            {
                if (!verificationPlans.Any())
                {
                    Result = "No verification plans created for VMAT/IMRT plan";
                    DisplayColor = ResultColorChoices.Fail;
                }
            }
            else
            {
                Result = "Was a verification plan necessary?";
                DisplayColor = ResultColorChoices.Warn;
            }

            // Display the created verification plans
            foreach (var vPlan in verificationPlans)
            {
                ResultDetails += $"{vPlan.Course.Id}/{vPlan.Id} ({vPlan.StructureSet.Id})\n";
            }

            ResultDetails = ResultDetails.TrimEnd('\n');
        }
    }
}
