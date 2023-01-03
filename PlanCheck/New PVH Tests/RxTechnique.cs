using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.TextFormatting;
using VMS.TPS.Common.Model.API;

namespace PlanCheck.Checks
{
    public class RxTechnique : PlanCheckGeneric
    {
        public RxTechnique(PlanSetup plan) : base(plan) { }

        public override void RunTest(PlanSetup plan)
        {
            DisplayName = "Technique";
            TestExplanation = "Displays the prescribed technique and makes sure the use of SRS technique beams matches";
            DisplayColor = ResultColorChoices.Pass;

            var tech = plan.RTPrescription.Technique;

            // SRS/SBRT was prescribed
            if (tech == "SRS" || tech == "SBRT")
            {
                var nonSrsBeams = plan.Beams
                                    .Where(x => !x.IsSetupField)
                                    .Where(x => !x.Technique.Id.Contains("SRS"))
                                    .Select(x => $"{x.Id} ({x.Name}) - {x.Technique}");

                // Non-SRS beams used
                if (nonSrsBeams.Any())
                {
                    Result = $"Prescibed technique is {tech}\nSome beams do not use SRS technique";
                    DisplayColor = ResultColorChoices.Warn;
                    ResultDetails = $"{String.Join("\n", nonSrsBeams)}";
                }
                else
                {
                    Result = tech;
                }
            }
            // Non-SRS/SBRT was prescribed
            else
            {
                var srsBeams = plan.Beams
                                    .Where(x => !x.IsSetupField)
                                    .Where(x => x.Technique.Name.Contains("SRS"))
                                    .Select(x => $"{x.Id} ({x.Name}) - {x.Technique}");

                // SRS beams used
                if (srsBeams.Any())
                {
                    Result = $"Prescibed technique is {tech}\nSome beams use SRS technique";
                    DisplayColor = ResultColorChoices.Warn;
                    ResultDetails = $"{String.Join("\n", srsBeams)}";
                }
                else
                {
                    Result = tech;
                }
            }
        }
    }
}
