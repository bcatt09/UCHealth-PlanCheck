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

            #region Poudre Valley
            // Vert = -20
            // Long = 90
            // Lat = 0
            if (Department == Department.PVH)
			{
				//Check each field to see if couch values are NaN
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

				//no issues found
				if (ResultDetails == "")
				{
					Result = "";
					//if (Department == Department.LAP ||
					//	Department == Department.OWO)
					//	ResultDetails = $"Lat: {(ConvertCouchLatToVarianStandardScale(plan.Beams.First().ControlPoints.First().TableTopLateralPosition) / 10.0).ToString("0.0")} cm\nLong: {(plan.Beams.First().ControlPoints.First().TableTopLongitudinalPosition / 10.0).ToString("0.0")} cm\nVert: {(ConvertCouchVertToVarianStandardScale(plan.Beams.First().ControlPoints.First().TableTopVerticalPosition) / 10.0).ToString("0.0")} cm";
					//else
						ResultDetails = $"Lat: {(ConvertCouchLatToVarianIECScale(plan.Beams.First().ControlPoints.First().TableTopLateralPosition) / 10.0).ToString("0.0")} cm\n" +
										$"Long: {(plan.Beams.First().ControlPoints.First().TableTopLongitudinalPosition / 10.0).ToString("0.0")} cm\n" +
										$"Vert: {(ConvertCouchVertToVarianIECScale(plan.Beams.First().ControlPoints.First().TableTopVerticalPosition) / 10.0).ToString("0.0")} cm" +
                                        $"CHECK THAT COORDINATES DON'T NEED TO BE CONVERTED";
					DisplayColor = ResultColorChoices.Pass;
				}
			}
            #endregion

            else
                TestNotImplemented();
		}

		/// <summary>
		/// Converts a given couch lateral to Varian IEC Scale
		/// </summary>
		/// <param name="couch">Couch lateral in mm</param>
		/// <returns>Couch lateral in Varian IEC Scale in mm</returns>
		private double ConvertCouchLatToVarianIECScale(double couch)
		{
			return (couch + 10000) % 10000;
		}

		/// <summary>
		/// Converts a given couch vertical to Varian IEC Scale
		/// </summary>
		/// <param name="couch">Couch vertical in mm</param>
		/// <returns>Couch vertical in Varian IEC Scale in mm</returns>
		private double ConvertCouchVertToVarianIECScale(double couch)
		{
			return (10000 - couch) % 10000;
		}

		/// <summary>
		/// Converts a given couch lateral to Varian Standard Scale
		/// </summary>
		/// <param name="couch">Couch lateral in mm</param>
		/// <returns>Couch lateral in Varian Standard Scale in mm</returns>
		private double ConvertCouchLatToVarianStandardScale(double couch)
		{
			return couch + 1000;
		}

		/// <summary>
		/// Converts a given couch vertical to Varian Standard Scale
		/// </summary>
		/// <param name="couch">Couch vertical in mm</param>
		/// <returns>Couch vertical in Varian Standard Scale in mm</returns>
		private double ConvertCouchVertToVarianStandardScale(double couch)
		{
			return 1000 - couch;
		}
	}
}
