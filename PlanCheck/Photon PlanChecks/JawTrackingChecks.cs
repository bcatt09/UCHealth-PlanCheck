using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;

namespace PlanCheck.Checks
{
    public class JawTrackingChecks : PlanCheckPhoton
	{
		protected override List<string> MachineExemptions => new List<string> { };

		public JawTrackingChecks(PlanSetup plan) : base(plan) { }

        public override void RunTestPhoton(ExternalPlanSetup plan)
        {
			bool IMRT = false;

			DisplayName = "Jaw Tracking";
			Result = "";
			TestExplanation = "Checks to see if jaw tracking is enabled for IMRT/VMAT plans\n" +
							  "Doesn't do any additional checks for jaw tracking during small SRS fields";

            // Use jaw tracking
            if (Department == Department.PVH)
			{
				// VMAT plan
				if ((from s in plan.Beams where s.MLCPlanType == MLCPlanType.VMAT select s).Count() > 0)
				{
					IMRT = true;

					foreach (Beam field in plan.Beams)
					{
						// Ignore setup fields
						if (!field.IsSetupField)
						{
							// For VMAT fields, check that there are different jaw positions in the control points
							if (field.MLCPlanType == MLCPlanType.VMAT)
							{
								// Get initial jaw positions
								double x1 = field.ControlPoints.First().JawPositions.X1;
								double x2 = field.ControlPoints.First().JawPositions.X2;
								double y1 = field.ControlPoints.First().JawPositions.Y1;
								double y2 = field.ControlPoints.First().JawPositions.Y2;

								// If they change at any of the control points, then jaw tracking must be on
								foreach (ControlPoint point in field.ControlPoints)
								{
									if (x1 != point.JawPositions.X1 || x2 != point.JawPositions.X2 || y1 != point.JawPositions.Y1 || y2 != point.JawPositions.Y2)
									{
										Result = "Enabled";
										DisplayColor = ResultColorChoices.Pass;
										break;
									}
								}
							}
						}
					}
				}
				// IMRT plan
				else if ((from s in plan.Beams where s.MLCPlanType == MLCPlanType.DoseDynamic select s).Count() > 0)
				{
					foreach (Beam field in plan.Beams)
					{
						// Ignore setup fields
						if (!field.IsSetupField)
						{
							// For IMRT fields that have more control points than step and shoot, check that there are different jaw positions in the control points
							if (field.MLCPlanType == MLCPlanType.DoseDynamic && field.ControlPoints.Count > 18)
							{
								IMRT = true;

								// Get initial jaw positions
								double x1 = field.ControlPoints.First().JawPositions.X1;
								double x2 = field.ControlPoints.First().JawPositions.X2;
								double y1 = field.ControlPoints.First().JawPositions.Y1;
								double y2 = field.ControlPoints.First().JawPositions.Y2;

								// If they change at any of the control points, then jaw tracking must be on
								foreach (ControlPoint point in field.ControlPoints)
								{
									if (x1 != point.JawPositions.X1 || x2 != point.JawPositions.X2 || y1 != point.JawPositions.Y1 || y2 != point.JawPositions.Y2)
									{
										Result = "Enabled";
										DisplayColor = ResultColorChoices.Pass;
										break;
									}
								}
							}
						}
					}
				}

				// Static fields
				if (!IMRT)
				{
					Result = "N/A";
					DisplayColor = ResultColorChoices.Pass;
				}

				// No jaw tracking detected
				else if (Result == "")
				{
					Result = "Warning";
					ResultDetails = "Please check that jaw tracking is enabled in the optimization window or leaf motion calculator";
					DisplayColor = ResultColorChoices.Warn;
				}
			}

			else
				TestNotImplemented();
		}
    }
}
