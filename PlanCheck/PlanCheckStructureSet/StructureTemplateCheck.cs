using PlanCheck.Helpers;
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

            if (TreatmentClassifier.IsClinicalPlan(structureSet))
            {
                ResultDetails = "2D plan";
                ResultColor = ResultColorChoices.Pass;
            }
            else if (structureSet.Structures.Count() > 1)
            {
                ResultDetails = "Structures added";
                ResultColor = ResultColorChoices.Pass;
            }
            else
            {
                ResultDetails = "No structures added";
                ResultColor = ResultColorChoices.Fail;
            }
        }
    }
}
