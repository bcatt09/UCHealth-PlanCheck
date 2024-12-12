using NLog.LayoutRenderers;
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
    public class VerificationPlan : PlanCheckLinac
    {
        protected override List<string> MachineExemptions => new List<string> { };

        public VerificationPlan(PlanSetup plan) : base(plan) { }

        public override void RunTestLinac(ExternalPlanSetup plan)
        {
            DisplayName = "Verification Plan";
            TestExplanation = "Lists all verification plans created from this plan and the corresponding Structure Set names";
            ResultColor = ResultColorChoices.Pass;
            ResultDetails = "";

            var verificationPlans = plan.Course.Patient.Courses.SelectMany(x => x.PlanSetups).Where(x => tryVerifiedPlan(x, plan));

            // No couch kicks on verification plans
            if (verificationPlans.SelectMany(x => x.Beams).Where(x => x.ControlPoints.FirstOrDefault().PatientSupportAngle != 0.0).Any())
            {
                Result = "Make sure there are no couch kicks for verification plans";
                ResultColor = ResultColorChoices.Warn;
            }

            // Check if one was made for VMAT/IMRT plans
            var firstBeam = plan.Beams.Where(x => !x.IsSetupField).First();
            if (firstBeam.MLCPlanType == MLCPlanType.VMAT || (firstBeam.MLCPlanType == MLCPlanType.DoseDynamic && firstBeam.ControlPoints.Count > 21))
            {
                if (!verificationPlans.Any())
                {
                    Result = "No verification plans created for VMAT/IMRT plan";
                    ResultColor = ResultColorChoices.Fail;
                }
            }
            else if (verificationPlans.Any())
            {
                Result = "Was a verification plan necessary?";             
                ResultColor = ResultColorChoices.Warn;
            }
            else
            {
                Result = "N/A";
            }

            // Display the created verification plans
            foreach (var vPlan in verificationPlans)
            {
                var portal = "Portal Dosimetry";
                ResultDetails += $"{vPlan.Course.Id}/{vPlan.Id} ({vPlan.StructureSet?.Id ?? portal})\n";
            }

            ResultDetails = ResultDetails.TrimEnd('\n');
        }

        private bool tryVerifiedPlan(PlanSetup testPlan, PlanSetup checkedPlan)
        {
            try
            {
                return testPlan.VerifiedPlan == checkedPlan;
            }
            catch 
            {
                return false;
            }
        }
    }
}
