using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;

namespace PlanCheck.Checks
{
    public class FieldTechniqueChecks : PlanCheckLinac
    {
        protected override List<string> MachineExemptions => new List<string> { };

        public FieldTechniqueChecks(PlanSetup plan) : base(plan) { }

        public override void RunTestLinac(ExternalPlanSetup plan)
        {
            DisplayName = "Field Technique";
            Result = "";
            ResultDetails = "";
            TestExplanation = "Checks that SRS ARC / SRS STATIC is used appropriately (> 500 cGy / fx)";
            ResultColor = ResultColorChoices.Pass;

            var SBRT = plan.DosePerFraction > new DoseValue(500, DoseValue.DoseUnit.cGy);

            if (SBRT)
            {
                foreach (var beam in plan.Beams.Where(x => !x.IsSetupField && !x.Technique.ToString().Contains("SRS")))
                {
                    Result = "The daily dose is > 500 cGy\nShould the following fields be SRS technique?";
                    ResultColor = ResultColorChoices.Warn;
                    ResultDetails += $"{beam.Id}\n";
                }
            }
            else
            {
                foreach (var beam in plan.Beams.Where(x => !x.IsSetupField && x.Technique.ToString().Contains("SRS")))
                {
                    Result = "The following fields use an SRS technique when the daily dose is <= 500 cGy";
                    ResultColor = ResultColorChoices.Fail;
                    ResultDetails += $"{beam.Id}\n";
                }
            }

            ResultDetails = ResultDetails.TrimEnd('\n');
        }
    }
}
