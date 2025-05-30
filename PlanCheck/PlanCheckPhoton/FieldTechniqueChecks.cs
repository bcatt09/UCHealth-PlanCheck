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
            TestExplanation = "Checks that SRS ARC / SRS STATIC is used appropriately (> 500 cGy / fx)\nAPBI breast should not use SRS ARC";
            ResultColor = ResultColorChoices.Pass;

            var SBRT = plan.DosePerFraction > new DoseValue(500, DoseValue.DoseUnit.cGy);

            var breastAPBI = Helpers.TreatmentClassifier.IsBreastAPBI(plan);

            if (SBRT && !breastAPBI)
            {
                var nonSrsBeams = plan.Beams.Where(x => !x.IsSetupField && !x.Technique.ToString().Contains("SRS"));

                if (!nonSrsBeams.Any())
                    Result = "Pass";
                
                foreach (var beam in nonSrsBeams)
                {
                    Result = "The daily dose is > 500 cGy\nShould the following fields be SRS technique for a signoff at the machine?";
                    ResultColor = ResultColorChoices.Warn;
                    ResultDetails += $"{beam.Id}\n";
                }
            }
            else
            {
                var srsBeams = plan.Beams.Where(x => !x.IsSetupField && x.Technique.ToString().Contains("SRS"));

                if (!srsBeams.Any())
                    Result = "Pass";

                foreach (var beam in plan.Beams.Where(x => !x.IsSetupField && x.Technique.ToString().Contains("SRS")))
                {
                    if (breastAPBI)
                        Result = "The following fields use an SRS technique when the treatment is a breast APBI";
                    else
                        Result = "The following fields use an SRS technique when the daily dose is <= 500 cGy";
                    ResultColor = ResultColorChoices.Fail;
                    ResultDetails += $"{beam.Id}\n";
                }
            }

            ResultDetails = ResultDetails.TrimEnd('\n');
        }
    }
}
