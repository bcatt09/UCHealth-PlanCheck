using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMS.TPS.Common.Model.API;

namespace PlanCheck.Checks
{
    public class ToleranceTableChecks : PlanCheckBase
	{
		protected override List<string> MachineExemptions => new List<string> { };

		public ToleranceTableChecks(PlanSetup plan) : base(plan) { }

        public override void RunTest(PlanSetup plan)
        {
			DisplayName = "Tolerance Table";
			Result = "";
			ResultDetails = "";
			TestExplanation = "Checks that all fields use the correct tolerance table based on department standards";

			#region Poudre Valley
			// SRS/SRT for plans with 1mm slices
			// SBRT for plans with "_4"
			// Electron for electron plans
			// TB Photon for all other plans
			if (Department == Department.PVH)
			{
				string tolTable;
				string badFields = "";

				if (plan.StructureSet.Image.ZRes == 1) // Plan has 1 mm slices (likely a brain SRS or SRT)
					tolTable = "PVH SRS";
				else if (plan.Beams.Where(x => !x.IsSetupField).Where(x => x.EnergyModeDisplayName.Contains("E", StringComparison.CurrentCultureIgnoreCase)).Count() > 0)
					tolTable = "PVH Electrons";
				else
					tolTable = "PVH IGRT";

				// Check each field to make sure they're the same
				foreach (Beam field in plan.Beams)
				{
					if (ResultDetails == "")
						ResultDetails = field.ToleranceTableLabel;

					// Wrong tolerance table
					if (field.ToleranceTableLabel != tolTable)
					{
						Result = "Warning";
						ResultDetails = $"Not all fields use the {tolTable} tolerance table: ";
						badFields += field.Id + ", ";
						DisplayColor = ResultColorChoices.Warn;
					}
				}

				ResultDetails += badFields;
				ResultDetails = ResultDetails.TrimEnd(' ');
				ResultDetails = ResultDetails.TrimEnd(',');

				// No issues found
				if (Result == "")
				{
					Result = "";
					DisplayColor = ResultColorChoices.Pass;
				}
			}
			#endregion

			else
				TestNotImplemented();
		}
    }
}
