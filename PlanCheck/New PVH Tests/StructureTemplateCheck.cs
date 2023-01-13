using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMS.TPS.Common.Model.API;

namespace PlanCheck.Checks
{
    public class StructureTemplateCheck : PlanCheckStructureSet
    {
        protected override List<string> MachineExemptions => new List<string> { };

        public StructureTemplateCheck(StructureSet structureSet) : base(structureSet) { }

        public override void RunTestStructureSet(StructureSet structureSet)
        {
            DisplayName = "Structure Template";
            TestExplanation = "Checks that more structures than just the Body have been added (does not check for any specific template)";

            if (structureSet.Structures.Count() > 1)
            {
                ResultDetails = "Structures added";
                DisplayColor = ResultColorChoices.Pass;
            }
            else
            {
                ResultDetails = "No structures added";
                DisplayColor = ResultColorChoices.Fail;
            }
        }
    }
}
