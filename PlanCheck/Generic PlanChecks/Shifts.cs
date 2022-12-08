using NLog.LayoutRenderers.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;

namespace PlanCheck.Checks
{
    public class Shifts : PlanCheckBase
	{
        protected override List<string> MachineExemptions => new List<string> { };

        public Shifts(PlanSetup plan) : base(plan) { }

        public override void RunTest(PlanSetup plan)
		{
			DisplayName = "Patient Shifts";
			TestExplanation = "Displays shifts from Marker Structure or User Origin";
			Result = "";
			ResultDetails = "";
			DisplayColor = ResultColorChoices.Pass;

			PatientOrientation orientation = plan.TreatmentOrientation;

			// Get location of user origin and plan isocenter
			VVector tattoos = plan.StructureSet.Image.UserOrigin;
			VVector isocenter = plan.Beams.First().IsocenterPosition;

			// Calculated shift distance from user origin
			VVector shift = isocenter - tattoos;
			string shiftFrom = "User Origin";

			#region Shifts from iso placed at sim (if marker present)
			// These sites set iso at sim and import in a "MARKER" structure that shifts will be based off (also they don't use gold markers, so there's no need to worry about those "MARKER" structures)
			if (Department == Department.PVH)
			{
				// Loop through each patient marker and see if it's closer to the iso than the user origin and if it is use that for the calculated shift
				foreach (Structure point in plan.StructureSet.Structures.Where(x => x.DicomType == "MARKER"))
				{
					if (Math.Round((isocenter - point.CenterPoint).Length, 2) <= Math.Round(shift.Length, 2))
					{
						shift = isocenter - point.CenterPoint;
						shiftFrom = point.Id;
					}
				}
			}
			#endregion

			// Round it off to prevent very small numbers from appearing and convert to cm for shifts
			shift.x = Math.Round(shift.x / 10, 1);
			shift.y = Math.Round(shift.y / 10, 1);
			shift.z = Math.Round(shift.z / 10, 1);

			if (shift.Length == 0)
				ResultDetails = $"No shifts from {shiftFrom}";
			else
			{
				// Set shift verbiage based on department
				string pat, sup, inf, ant, post;
				//if (Department == Department.NOR)
				//{
				//	pat = "Patient";
    //                sup = "superior";
    //                inf = "inferior";
    //                ant = "anterior";
    //                post = "posterior";
				//}
				//else
				//{
					pat = "Table";
					sup = "out";
					inf = "in";
					ant = "down";
					post = "up";
				//}

			// x-axis
			if (shift.x > 0)
					ResultDetails += $"{pat} left: {shift.x:0.0} cm\n";
				else if (shift.x < 0)
					ResultDetails += $"{pat} right: {-shift.x:0.0} cm\n";

				// z-axis
				if (shift.z > 0)
					ResultDetails += $"{pat} {sup}: {shift.z:0.0} cm\n";
				else if (shift.z < 0)
					ResultDetails += $"{pat} {inf}: {-shift.z:0.0} cm\n";

				// y-axis
				if (shift.y > 0)
					ResultDetails += $"{pat} {post}: {shift.y:0.0} cm\n";
				else if (shift.y < 0)
					ResultDetails += $"{pat}  {ant}: {-shift.y:0.0} cm\n";


				// Remove negatives
				ResultDetails.Replace("-", string.Empty);

				ResultDetails = $"Shifts from {shiftFrom}\n" + ResultDetails;
			}

			ResultDetails = ResultDetails.TrimEnd('\n');
		}
    }
}
