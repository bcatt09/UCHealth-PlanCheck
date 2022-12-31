using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Configuration;
using System.Text;
using System.Threading.Tasks;
using VMS.TPS.Common.Model.API;

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

            var startTime = DateTime.Now;

            var verificationPlans = plan.Course.Patient.Courses.SelectMany(x => x.PlanSetups).Where(x => x.VerifiedPlan == plan);

            foreach (var vPlan in verificationPlans)
            {
                ResultDetails += $"{vPlan.Course.Id}/{vPlan.Id} ({vPlan.StructureSet.Id})\n";
            }

            ResultDetails = ResultDetails.TrimEnd('\n');

            var endTime = DateTime.Now;

            ResultDetails += $"\nTest took {Math.Round((endTime - startTime).TotalSeconds, 1)} seconds\n(I can delete this if it's too slow)";
        }
    }
}
