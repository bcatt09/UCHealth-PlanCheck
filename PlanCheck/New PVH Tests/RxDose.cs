using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMS.TPS.Common.Model.API;

namespace PlanCheck.Checks
{
    public class RxDose : PlanCheckGeneric
    {
        public RxDose(PlanSetup plan) : base(plan) { }

        public override void RunTest(PlanSetup plan)
        {
            DisplayName = "Dose";
            DisplayColor = ResultColorChoices.Pass;
            TestExplanation = "Checks the total dose and dose per fraction of the plan against the presciption";

            var rx = plan.RTPrescription;

            Result = $"{rx.Targets.First().DosePerFraction} x {rx.Targets.First().NumberOfFractions} = {rx.Targets.First().DosePerFraction * rx.Targets.First().NumberOfFractions}\nTo-do: the actual checking part";
        }
    }
}
