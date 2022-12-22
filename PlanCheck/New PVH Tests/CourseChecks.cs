using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using VMS.TPS.Common.Model.API;

namespace PlanCheck.Checks
{
    public class CourseChecks : PlanCheckBase
    {
        protected override List<string> MachineExemptions => new List<string> { };

        public CourseChecks(PlanSetup plan) : base(plan) { }

        public override void RunTest(PlanSetup plan)
        {
            DisplayName = "Course Checks";
            ResultDetails = "";
            TestExplanation = "Checks that the Course ID follows naming conventions (Site MMYY) and has an Intent and Diagnosis Code attached";

            var regex = new Regex(@".*( |_)\d{4}");

            if (plan.Course.Intent == "")
            {
                ResultDetails += "Course intent has not been added\n";
                DisplayColor = ResultColorChoices.Warn;
            }
            if (plan.Course.Diagnoses.Count() == 0)
            {
                ResultDetails += "Diagnosis code has not been attached\n";
                DisplayColor = ResultColorChoices.Warn;
            }
            if (!regex.IsMatch(plan.Course.Id))
            {
                ResultDetails += $"Course does not follow naming conventions ({plan.Course.Id})\n";
                DisplayColor = ResultColorChoices.Fail;
            }

            ResultDetails = ResultDetails.TrimEnd('\n');

            if (ResultDetails == "")
                ResultDetails = "Pass";
        }
    }
}
