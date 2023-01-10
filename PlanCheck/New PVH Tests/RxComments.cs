using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMS.TPS.Common.Model.API;

namespace PlanCheck.Checks
{
    public class RxComments : PlanCheckGeneric
    {
        public RxComments(PlanSetup plan) : base(plan) { }

        public override void RunTest(PlanSetup plan)
        {
            DisplayName = "Special Orders";
            TestExplanation = "Displays prescription comment\nIf the comment contains \"bolus\" or the plan utilizes a bolus, it checks that the plan and rx are in agreement";
            DisplayColor = ResultColorChoices.Pass;
            ResultDetails = "";

            var rx = plan.RTPrescription;

            Result = rx.Notes == "" ? "None" : rx.Notes;

            var bolusBeams = plan.Beams.Where(x => !x.IsSetupField).Where(x => x.Boluses.Any());

            // Prescription calls for bolus
            if (rx.Notes.ToLower().Contains("bolus"))
            {
                if (bolusBeams.Any())
                {
                    ResultDetails += getBolusInfo(plan);
                }
                else
                {
                    ResultDetails += "No bolus used on any beams";
                    DisplayColor = ResultColorChoices.Fail;
                }
            }
            // Not prescribed but bolus is used
            else if (bolusBeams.Any())
            {
                ResultDetails += getBolusInfo(plan);

                Result += "\nBolus was not prescribed";
                DisplayColor = ResultColorChoices.Fail;
            }
        }

        private string getBolusInfo(PlanSetup plan)
        {
            var result = "";

            foreach (var beam in plan.Beams.Where(x => !x.IsSetupField))
            {
                foreach (var bol in beam.Boluses)
                {
                    if (beam.Boluses.Any())
                    {
                        result += $"{beam.Id} - {bol.Id}\n";
                    }
                    else
                    {
                        result += $"{beam.Id} - no bolus\n";
                    }
                }
            }

            result = result.TrimEnd('\n');

            return result;
        }
    }
}
