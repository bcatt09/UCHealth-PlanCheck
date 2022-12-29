using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;

namespace PlanCheck.Checks
{
    class FieldNameChecks : PlanCheckPhoton
	{
		protected override List<string> MachineExemptions => new List<string> { };

		public FieldNameChecks(PlanSetup plan) : base(plan) { }

        public override void RunTestPhoton(ExternalPlanSetup plan)
		{
			DisplayName = "Field Names";
			ResultDetails = "";
			TestExplanation = "Checks that field names follow OneAria naming conventions";

			foreach (Beam field in plan.Beams)
			{
				// Ignore setup fields
				if (!field.IsSetupField)
                {
                    if (field.Technique.ToString().ToUpper().Contains("STATIC"))
					{

					}
                    else if (field.Technique.ToString().ToUpper().Contains("ARC"))
					{
                        if (!field.Name.Contains($"{Math.Round(field.ControlPoints.FirstOrDefault().GantryAngle)}-{Math.Round(field.ControlPoints.LastOrDefault().GantryAngle)}"))
                        {
                            Result = "Warning";
                            ResultDetails += $"Field name mismatch  —  Field: {field.Id} - {field.Name}\nGantry Start: {field.ControlPoints.FirstOrDefault().GantryAngle}\nGantry Stop: {field.ControlPoints.LastOrDefault().GantryAngle}\n";
                            DisplayColor = ResultColorChoices.Warn;
                        }
                    }
                    // Check for pedestal kicks
                    if (field.ControlPoints.First().PatientSupportAngle != 0)
                    {
                        // Field name matching pattern: t270 with or without a space or "_" between
                        string fieldNamePedestal = "(?i)(t)_? ?" + Math.Round(field.PatientSupportAngleToUser(field.ControlPoints.First().PatientSupportAngle), 0).ToString();

                        if (!Regex.IsMatch(field.Name, fieldNamePedestal))
                        {
                            Result = "Warning";
                            ResultDetails += $"Field name mismatch  —  Field: {field.Id} - {field.Name}\nPedestal Angle: {field.PatientSupportAngleToUser(field.ControlPoints.First().PatientSupportAngle)}\n";
                            DisplayColor = ResultColorChoices.Warn;
                        }
                    }

                    #region Field ID Naming
                    //#region Static Gantry Angles
                    //// For static fields, check that the gantry angle is contained in the field name
                    //if (field.Technique.ToString() == "STATIC" || field.Technique.ToString() == "SRS STATIC")
                    //{
                    //	// Field name matching pattern: g125 with or without a space or "_" between
                    //	string fieldNameGantry = "(?i)g_? ?" + Math.Round(field.GantryAngleToUser(field.ControlPoints.First().GantryAngle), 0).ToString();

                    //	if (!Regex.IsMatch(field.Id, fieldNameGantry))
                    //	{
                    //		Result = "Warning";
                    //		ResultDetails += "Field name mismatch  —  Field: " + field.Id + "  Gantry Angle: " + field.GantryAngleToUser(field.ControlPoints.FirstOrDefault().GantryAngle).ToString() + "\n";
                    //		DisplayColor = ResultColorChoices.Warn;
                    //	}
                    //}
                    //               #endregion

                    //               #region Arcs
                    //               // For arc fields, check that cw vs ccw matches rotation direction
                    //               else if (field.Technique.ToString() == "ARC" || field.Technique.ToString() == "SRS ARC")
                    //{
                    //	if (!field.Name.Contains($"{Math.Round(field.ControlPoints.FirstOrDefault().GantryAngle)}-{Math.Round(field.ControlPoints.LastOrDefault().GantryAngle)}"))
                    //                   {
                    //                       Result = "Warning";
                    //                       ResultDetails += $"Field name mismatch  —  Field: {field.Id}\nGantry Start: {field.ControlPoints.FirstOrDefault().GantryAngle}\nGantry Stop: {field.ControlPoints.LastOrDefault().GantryAngle}\n";
                    //                       DisplayColor = ResultColorChoices.Warn;
                    //                   }
                    //}
                    //               #endregion

                    //               #region Couch Kicks
                    //               // Check for pedestal kicks
                    //               if (field.ControlPoints.First().PatientSupportAngle != 0)
                    //{
                    //	// Field name matching pattern: g125 with or without a space or "_" between
                    //	string fieldNamePedestal = "(?i)(p|t)_? ?" + Math.Round(field.PatientSupportAngleToUser(field.ControlPoints.First().PatientSupportAngle), 0).ToString();

                    //	if (!Regex.IsMatch(field.Name, fieldNamePedestal))
                    //	{
                    //		Result = "Warning";
                    //		ResultDetails += $"Field name mismatch  —  Field: {field.Id}\nPedestal Angle: {field.PatientSupportAngleToUser(field.ControlPoints.First().PatientSupportAngle)}\n";
                    //		DisplayColor = ResultColorChoices.Warn;
                    //	}
                    //}
                    //               #endregion
                    #endregion
                }
            }

			ResultDetails = ResultDetails.TrimEnd('\n');

			// No problems found
			if (ResultDetails == "")
			{
				Result = "Pass";
                ResultDetails = String.Join("\n", plan.Beams.Select(x => $"{x.Id} - {x.Name} - {x.Technique}"));
				DisplayColor = ResultColorChoices.Pass;
			}
		}
    }
}
