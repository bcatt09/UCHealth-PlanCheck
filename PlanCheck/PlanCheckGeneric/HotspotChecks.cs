﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMS.TPS.Common.Model.API;

namespace PlanCheck.Checks
{
    public class HotspotChecks : PlanCheckGeneric
	{
        protected override List<string> MachineExemptions => new List<string> { };

        public HotspotChecks(PlanSetup plan) : base(plan) { }

        public override void RunTest(PlanSetup plan)
		{
			DisplayName = "Hotspot";
			TestExplanation = "Checks to see if the hotspot is inside of the plan target";

			if (plan.TargetVolumeID == "")
            {
				Result = "";
				ResultDetails = $"No target structure selected";
				ResultColor = ResultColorChoices.Fail;

				return;
			}

			Structure target = plan.StructureSet.Structures.FirstOrDefault(s => s.Id == plan.TargetVolumeID);

			if (target == null)
			{
				Result = $"Structure does not exist matching plan target ({plan.TargetVolumeID})";
                ResultColor = ResultColorChoices.Fail;

				return;
            }

			if (plan.IsDoseValid)
			{
				bool inTarget = target.IsPointInsideSegment(plan.Dose.DoseMax3DLocation);

				Result = "";
				ResultDetails = inTarget ? $"{plan.Dose.DoseMax3D} hotspot is in plan target ({target.Id})" : $"{plan.Dose.DoseMax3D} hotspot is not in plan target ({target.Id})";
				ResultColor = inTarget ? ResultColorChoices.Pass : ResultColorChoices.Warn;
			}
			else
			{
				Result = "";
				ResultDetails = "Dose has not been calculated";
				ResultColor = ResultColorChoices.Fail;
			}
		}
    }
}
