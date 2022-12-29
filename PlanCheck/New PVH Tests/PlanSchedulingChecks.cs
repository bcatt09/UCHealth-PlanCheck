﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMS.TPS.Common.Model.API;

namespace PlanCheck.Checks
{
    public class PlanSchedulingChecks : PlanCheckBase
    {
        protected override List<string> MachineExemptions => new List<string> { };

        public PlanSchedulingChecks(PlanSetup plan) : base(plan) { }

        public override void RunTest(PlanSetup plan)
        {
            DisplayName = "Plan Scheduling Checks";
            TestExplanation = "Correct number of fractions scheduled";
            Result = "";
            DisplayColor = ResultColorChoices.Pass;

            var numSessions = plan.TreatmentSessions.Count();

            var overlappingSessions =
                    plan.TreatmentSessions
                    .SelectMany(session => session.TreatmentSession.SessionPlans
                        .Where(sessionPlan => sessionPlan.PlanSetup.Id != plan.Id)
                        .Select(sessionPlan => sessionPlan.PlanSetup))
                    .GroupBy(x => x);

            ResultDetails = $"{numSessions} scheduled sessions";

            if (numSessions != plan.NumberOfFractions)
            {
                ResultDetails += $"\nScheduled sessions doesn't equal planned fractions";
                DisplayColor = ResultColorChoices.Warn;
            }

            foreach (var sessionGroup in overlappingSessions)
            {
                ResultDetails += $"\n{plan.Id} is scheduled with {sessionGroup.Key} for {sessionGroup.Count()} sessions";
                DisplayColor = ResultColorChoices.Warn;
            }
        }
    }
}