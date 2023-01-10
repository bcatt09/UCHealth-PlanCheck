using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;

namespace PlanCheck.Checks
{
    public class CouchStructuresChecks : PlanCheckStructureSet
	{
        protected override List<string> MachineExemptions => new List<string> { };

        public CouchStructuresChecks(StructureSet structureSet) : base(structureSet) { }

        public override void RunTestStructureSet(StructureSet structureSet)
		{
			DisplayName = "Couch Structures";
			TestExplanation = "Checks that the correct couch structure based on department standards";

			IEnumerable<Structure> couchStructures = null;
			string couchName = "";
			bool couchStructure;
			bool brain = false;
			bool lung = false;

			// Find couch structure if it exists
			if ((from s in structureSet.Structures where s.DicomType == "SUPPORT" select s).Count() > 0)
			{
				couchStructures = (from s in structureSet.Structures where s.DicomType == "SUPPORT" select s);
				couchStructure = true;
				Structure firstCouch = couchStructures.FirstOrDefault();
				if (firstCouch.Name != "" && firstCouch.Name != firstCouch.Id)
					couchName = firstCouch.Name;
				else
					couchName = firstCouch.Comment;
			}
			else
				couchStructure = false;

			// Find brain and lung structures
			foreach (Structure structure in structureSet.Structures)
			{
				if (structure.Id.ToLower().Contains("brain"))
					brain = true;
				if (structure.Id.ToLower().Contains("lung"))
					lung = true;
			}

            #region PVH
            // If a brain structure exists without a lung -> no couch
            // If a brain structure exists with a lung -> ??? highlight and display status
            // Otherwise a couch should exist
            if (Department == Department.PVH)
            {
                if (brain)
                {
                    // No couch
                    if (!lung)
                    {

                        if (couchStructure)
                        {
                            Result = "Warning";
                            ResultDetails = "Couch structures should not be included for cranial plans";
                            DisplayColor = ResultColorChoices.Warn;

                            AddCouchStructureInfo(couchName, couchStructures);
                        }
                        else
                        {
                            Result = "";
                            ResultDetails = "No couch structures";
                            DisplayColor = ResultColorChoices.Pass;
                        }
                    }
                    // Highlight and display couch status
                    else
                    {
                        if (couchStructure)
                        {
                            Result = "Check couch structure appropriateness";
                            DisplayColor = ResultColorChoices.Warn;

                            AddCouchStructureInfo(couchName, couchStructures);
                        }
                        else
                        {
                            Result = "Check couch structure appropriateness";
                            ResultDetails = "No couch structures included";
                            DisplayColor = ResultColorChoices.Warn;
                        }
                    }
                }
                // Should have a couch (IGRT Medium)
                else
                {
                    if (couchName.Contains("IGRT") && couchName.Contains("medium"))
                    {
                        Result = "";
                        DisplayColor = ResultColorChoices.Pass;

                        AddCouchStructureInfo(couchName, couchStructures);
                    }
                    else
                    {
                        Result = "Warning";
                        ResultDetails = "Exact IGRT Couch, medium not inserted, please check that correct couch is inserted";
                        DisplayColor = ResultColorChoices.Warn;

                        AddCouchStructureInfo(couchName, couchStructures);
                    }
                }
            }
            #endregion

            else
                TestNotImplemented();
		}

		/// <summary>
		/// Add couch info to ResultDetails
		/// </summary>
		private void AddCouchStructureInfo(string couchName, IEnumerable<Structure> couchStructures)
		{
			if (ResultDetails == null)
				ResultDetails = couchName;
			else
				ResultDetails += $"\n{couchName}";

			//dislay HU values for all support structures
			foreach (Structure couch in couchStructures)
			{
				if (!couch.GetAssignedHU(out double HU))
					ResultDetails += $"\n{couch.Id}: HU = N/A";

				ResultDetails += $"\n{couch.Id}: HU = {HU}";
			}
		}
	}
}
