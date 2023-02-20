using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMS.TPS.Common.Model.API;

namespace PlanCheck.Checks
{
    public class CoursesComplete : PlanCheckGeneric
    {
        protected override List<string> MachineExemptions => new List<string> { };

        public CoursesComplete(PlanSetup plan) : base(plan) { }

        public override void RunTest(PlanSetup plan)
        {
            DisplayName = "Courses Completed";
            TestExplanation = "Checks that all other Courses have been marked as Complete";
        }
    }
}
