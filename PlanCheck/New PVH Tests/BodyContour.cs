using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMS.TPS.Common.Model.API;

namespace PlanCheck.Checks
{
    public class BodyContour : PlanCheckStructureSet
    {
        protected override List<string> MachineExemptions => new List<string> { };
        public BodyContour(StructureSet structureSet) : base(structureSet) { }

        public override void RunTestStructureSet(StructureSet structureSet)
        {
            DisplayName = "Body Contour";
            TestExplanation = "Checks that the Body structure exists";

            if (structureSet.Structures.Count(s => s.DicomType.ToUpper() == "BODY" || s.DicomType.ToUpper() == "EXTERNAL") > 0)
            {
                ResultDetails = "Exists";
                DisplayColor = ResultColorChoices.Pass;
            }
            else
            {
                ResultDetails = "Body structure not generated";
                DisplayColor = ResultColorChoices.Fail;
            }
        }
    }
}
