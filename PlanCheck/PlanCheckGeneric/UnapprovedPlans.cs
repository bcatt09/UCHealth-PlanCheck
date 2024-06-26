﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;

namespace PlanCheck.Checks
{
    public class UnapprovedPlans : PlanCheckGeneric
    {
        protected override List<string> MachineExemptions => new List<string> { };

        public UnapprovedPlans(PlanSetup plan) : base(plan) { }

        public override void RunTest(PlanSetup plan)
        {
            DisplayName = "No Unapproved Plans";
            TestExplanation = "Checks that no plans have been left unapproved in the treatment course";
            
            var unapproved = plan.Course.PlanSetups.Where(x => x.ApprovalStatus == PlanSetupApprovalStatus.UnApproved);

            if (unapproved.Count(x => x.RTPrescription == null) > 0)
            {
                Result = String.Join("\n", unapproved.Where(x => x.RTPrescription == null).Select(x => $"{x.Id} is Unapproved"));
                ResultColor = ResultColorChoices.Warn;
            }
            else
            {
                Result = "Pass";
                ResultColor = ResultColorChoices.Pass;
            }
        }
    }
}
