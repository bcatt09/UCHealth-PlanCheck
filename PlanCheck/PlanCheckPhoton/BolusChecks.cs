﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMS.TPS.Common.Model.API;

namespace PlanCheck.Checks
{
    public class BolusChecks : PlanCheckLinac
    {
        protected override List<string> MachineExemptions => new List<string> { };

        public BolusChecks(PlanSetup plan) : base(plan) { }

        public override void RunTestLinac(ExternalPlanSetup plan)
		{
			DisplayName = "Bolus";
			Result = "";
			ResultDetails = "";
			TestExplanation = "Checks that each field has a linked bolus if a bolus exists";

			string bolus = "";
			bool containsBolus = false;
			bool containsMultiple = false;
			string resultDetailsMultiPerFieldLine = "";
			string resultDetailsMultiPerPlanLine = "";

			// Check to see if plan contains a bolus
			foreach (Structure struc in plan.StructureSet.Structures)
			{
				if (struc.DicomType == "BOLUS")
				{
					// If it's already found one bolus, then there are multiple
					if (containsBolus)
						containsMultiple = true;

					containsBolus = true;
				}
			}

			// Check each field to make sure it has a bolus attached
			foreach (Beam field in plan.Beams)
			{
				if (!containsBolus)
					break;
				if (!field.IsSetupField)
				{
					// No bolus
					if (field.Boluses.Count() == 0)
					{
						// Set up "no bolus linked" string
						if (ResultDetails == "")
						{
							Result = "Warning";
							ResultDetails = "Some fields do not have a linked bolus: ";
							ResultColor = ResultColorChoices.Warn;
						}
						ResultDetails += field.Id + ", ";
					}
					// More than 1 bolus
					else if (field.Boluses.Count() > 1)
					{
						// Set up "multiple boluses" string
						if (resultDetailsMultiPerFieldLine == "")
						{
							Result = "Warning";
							resultDetailsMultiPerFieldLine = "Some fields have more than one bolus linked: ";
							ResultColor = ResultColorChoices.Warn;
						}
						resultDetailsMultiPerFieldLine += field.Id + ", ";
					}
					// Just one bolus
					else
					{
						// If this is the first bolus found, save it
						if (bolus == "")
						{
							bolus = field.Boluses.First().Id;
						}
						// If not make sure it's the same bolus used on other fields
						else if (field.Boluses.First().Id != bolus)
						{
							// Set up "multiple boluses" string
							if (resultDetailsMultiPerPlanLine == "")
							{
								Result = "Warning";
								resultDetailsMultiPerPlanLine = $"Multiple bolus structures linked in plan: {bolus}, ";
								ResultColor = ResultColorChoices.Warn;
							}
							resultDetailsMultiPerPlanLine += field.Boluses.First().Id + ", ";
						}
					}
				}
			}

			// No bolus in plan so it's good
			if (!containsBolus)
			{
				Result = "";
				ResultDetails = "No bolus in structure set";
				ResultColor = ResultColorChoices.Pass;
			}
			// No issues found
			else if (Result == "")
			{
				Result = "";
				ResultDetails = $"{bolus} attached to all fields";
				ResultColor = ResultColorChoices.Pass;
			}

			// Clean up strings
			ResultDetails = ResultDetails.TrimEnd(' ');
			ResultDetails = ResultDetails.TrimEnd(',');
			resultDetailsMultiPerFieldLine = resultDetailsMultiPerFieldLine.TrimEnd(' ');
			resultDetailsMultiPerFieldLine = resultDetailsMultiPerFieldLine.TrimEnd(',');
			resultDetailsMultiPerPlanLine = resultDetailsMultiPerPlanLine.TrimEnd(' ');
			resultDetailsMultiPerPlanLine = resultDetailsMultiPerPlanLine.TrimEnd(',');
			if (resultDetailsMultiPerFieldLine != "")
				ResultDetails += '\n' + resultDetailsMultiPerFieldLine;
			if (resultDetailsMultiPerPlanLine != "")
				ResultDetails += '\n' + resultDetailsMultiPerPlanLine;
			ResultDetails = ResultDetails.TrimStart('\n');
			ResultDetails = ResultDetails.TrimStart('\n');

			// Multiple boluses in structure set, so put a warning at the end
			if (containsMultiple)
			{
				Result = "Warning";
				ResultDetails += "\nMultiple bolus structures in the structure set, please ensure that the correct one is used";
				ResultColor = ResultColorChoices.Warn;
			}
		}
    }
}
