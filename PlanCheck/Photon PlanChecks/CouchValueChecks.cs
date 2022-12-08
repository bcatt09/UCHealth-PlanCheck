using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMS.TPS.Common.Model.API;

namespace PlanCheck.Checks
{
    public class CouchValueChecks : PlanCheckBasePhoton
	{
		protected override List<string> MachineExemptions => new List<string> { };

		public CouchValueChecks(PlanSetup plan) : base(plan) { }

        public override void RunTestPhoton(ExternalPlanSetup plan)
        {
			DisplayName = "Couch Values";
			ResultDetails = "";
			TestExplanation = "Checks that couch values are entered for each field based on department standards";

            #region PVH
            // Vert = -20
            // Long = 90
            // Lat = 0
            if (Department == Department.PVH)
			{
				// Check each field to see if couch values are NaN
				foreach (Beam field in plan.Beams)
				{
					if (field.ControlPoints.FirstOrDefault().TableTopLateralPosition != 0 || field.ControlPoints.FirstOrDefault().TableTopLongitudinalPosition != 900 || field.ControlPoints.FirstOrDefault().TableTopVerticalPosition != -200)
					{
						Result = "Warning";
						ResultDetails += "Couch value incorrect for " + field.Id.ToString() + ": ";
						DisplayColor = ResultColorChoices.Warn;

						if (field.ControlPoints.First().TableTopLateralPosition != 0)
							ResultDetails += "lat, ";
						if (field.ControlPoints.First().TableTopLongitudinalPosition != 1000)
							ResultDetails += "long, ";
						if (field.ControlPoints.First().TableTopVerticalPosition != 0)
							ResultDetails += "vert, ";

						ResultDetails = ResultDetails.TrimEnd(' ');
						ResultDetails = ResultDetails.TrimEnd(',');
						ResultDetails += '\n';
					}
				}

				ResultDetails = ResultDetails.TrimEnd('\n');

				// No issues found
				if (ResultDetails == "")
				{
					Result = "";
					ResultDetails = $"Vert: {plan.Beams.First().ControlPoints.First().TableTopVerticalPosition / 10.0:0.0} cm\n" +
                                    $"Long: {plan.Beams.First().ControlPoints.First().TableTopLongitudinalPosition / 10.0:0.0} cm\n" +
                                    $"Lat: {plan.Beams.First().ControlPoints.First().TableTopLateralPosition / 10.0:0.0} cm";
					DisplayColor = ResultColorChoices.Pass;
				}
			}
            #endregion

            else
                TestNotImplemented();
		}
	}
}
