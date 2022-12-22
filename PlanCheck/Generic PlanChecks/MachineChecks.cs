using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMS.TPS.Common.Model.API;

namespace PlanCheck.Checks
{
    public class MachineChecks : PlanCheckBase
	{
		protected override List<string> MachineExemptions => new List<string> { };

		public MachineChecks(PlanSetup plan) : base(plan) { }

        public override void RunTest(PlanSetup plan)
		{
			DisplayName = "Machine";
			Result = "";
			ResultDetails = "";
			TestExplanation = "Checks that all fields are planned using the same machine";

			//Check each field to make sure they're the same
			foreach (Beam field in plan.Beams)
			{
				if (field.TreatmentUnit.Id != MachineID)
				{
					Result = "Failure";
					ResultDetails = $"Not all fields use the same machine";
					DisplayColor = ResultColorChoices.Fail;
				}
			}

			if (Result == "")
			{
				Result = "";
				ResultDetails = MachineID;
				DisplayColor = ResultColorChoices.Pass;
			}
		}
    }
}
