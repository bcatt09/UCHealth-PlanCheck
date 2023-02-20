using System;
using System.Collections.Generic;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMS.TPS.Common.Model.API;

namespace PlanCheck.Checks
{
    public class PrescriptionName : PlanCheckGeneric
    {
        protected override List<string> MachineExemptions => new List<string> { };

        enum laterality
        {
            Left,
            Right,
            NA
        }

        public PrescriptionName(PlanSetup plan) : base(plan) { }

        public override void RunTest(PlanSetup plan)
        {
            DisplayName = "Prescription Name";
            TestExplanation = "Checks prescription laterality against plan target";
            Result = "";
        }
    }
}
