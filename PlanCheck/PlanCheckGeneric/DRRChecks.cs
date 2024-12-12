using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMS.TPS.Common.Model.API;

namespace PlanCheck.Checks
{
    public class DRRChecks : PlanCheckGeneric
	{
        protected override List<string> MachineExemptions => new List<string> { };

        public DRRChecks(PlanSetup plan) : base(plan) { }

        public override void RunTest(PlanSetup plan)
		{
			DisplayName = "Setup Fields and DRRs";
			Result = "Generated";
			ResultDetails = "";
			ResultColor = ResultColorChoices.Pass;
			TestExplanation = "Checks that at least one setup field was created and\nDRRs are created and attached as a reference image for all fields";

			if (!plan.Beams.Where(x => x.IsSetupField).Any() && plan.Beams.Where(x => !x.EnergyModeDisplayName.ToLower().Contains('e')).Any())
            {
                Result = "Warning";
                ResultDetails += "No setup fields created\n";
				ResultColor = ResultColorChoices.Warn;
			}

			foreach (Beam field in plan.Beams)
			{
				if (field.ReferenceImage == null)
                {
                    Result = "Warning";
                    ResultDetails += field.Id + " has no reference image\n";
					ResultColor = ResultColorChoices.Warn;
				}
			}

			ResultDetails = ResultDetails.TrimEnd('\n');
		}
    }
}
