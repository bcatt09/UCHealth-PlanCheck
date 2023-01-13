using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;

namespace PlanCheck.Checks
{
    class DoseRateChecks : PlanCheckPhoton
	{
        protected override List<string> MachineExemptions => new List<string> { };

		public DoseRateChecks(PlanSetup plan) : base(plan) { }

        public override void RunTestPhoton(ExternalPlanSetup plan)
        {
			DisplayName = "Dose Rate";
			ResultDetails = "";
			TestExplanation = "Checks that all dose rates are set to maximum";

			// 4X        - 250
			// Flattened - 600
			// 6FFF      - 1400
			// 10FFF     - 2400
			// Electron  - 1000
			if (Department == Department.PVH)
			{
				foreach (Beam field in plan.Beams)
				{
					// Ignore setup fields
					if (!field.IsSetupField)
					{
						string energy = field.EnergyModeDisplayName;

						if (energy == "4X")
                        {
                            if (field.DoseRate < 250)
                            {
                                Result = "Warning";
                                ResultDetails += field.Id + " dose rate set at " + field.DoseRate + "\n";
                                DisplayColor = ResultColorChoices.Warn;
                            }
                        }
						else if (energy == "6X" || energy == "10X" || energy == "15X" || energy == "16X" || energy == "18X" || energy == "23X")
						{
							if (field.DoseRate < 600)
							{
								Result = "Warning";
								ResultDetails += field.Id + " dose rate set at " + field.DoseRate + "\n";
								DisplayColor = ResultColorChoices.Warn;
							}
						}
						else if (energy == "6X-FFF")
						{
							if (field.DoseRate < 1400)
							{
								Result = "Warning";
								ResultDetails += field.Id + " dose rate set at " + field.DoseRate + "\n";
								DisplayColor = ResultColorChoices.Warn;
							}
						}
						else if (energy == "10X-FFF")
						{
							if (field.DoseRate < 2400)
							{
								Result = "Warning";
								ResultDetails += field.Id + " dose rate set at " + field.DoseRate + "\n";
								DisplayColor = ResultColorChoices.Warn;
							}
						}
						else if (energy.Contains("E", StringComparison.CurrentCultureIgnoreCase))
						{
							if (field.DoseRate < 1000)
							{
								Result = "Warning";
								ResultDetails += field.Id + " dose rate set at " + field.DoseRate + "\n";
								DisplayColor = ResultColorChoices.Warn;
							}
						}
					}
				}

				// No problems found
				if (ResultDetails == "")
				{
					Result = "";
					ResultDetails = plan.Beams.Where(x => !x.IsSetupField).First().DoseRate.ToString();
					DisplayColor = ResultColorChoices.Pass;
				}

				ResultDetails = ResultDetails.TrimEnd('\n');
			}

			else
				TestNotImplemented();
		}
    }
}
