using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMS.TPS.Common.Model.API;

namespace PlanCheck.Checks
{
    public class IllegalCharactersCheck : PlanCheckGeneric
    {
        List<char> illegalChars = new List<char> { '(', ')', ',', '<', '>', '{', '}', '[', ']', '\\', '\t', '!', '$', '^', '&', '*', '=', '%', '"', ';', '?', '@' };

        protected override List<string> MachineExemptions => new List<string> { };

        public IllegalCharactersCheck(PlanSetup plan) : base(plan) { }

        public override void RunTest(PlanSetup plan)
        {
            DisplayName = "Illegal Characters";
            ResultDetails = "";
            TestExplanation = "Checks that field, plan, image, and series IDs do not contain any of the following:\n( ) , . > < { } [ ] / \\ ! $ ^ & * - = % \"; ? @ +";
            ResultColor = ResultColorChoices.Pass;

            if (ContainsIllegal(plan.Id))
            {
                ResultDetails += "Plan ID contains illegal character\n";
                ResultColor = ResultColorChoices.Fail;
            }
            if (ContainsIllegal(plan.StructureSet.Image.Id))
            {
                ResultDetails += "Image ID contains illegal character\n";
                ResultColor = ResultColorChoices.Fail;
            }
            if (ContainsIllegal(plan.StructureSet.Image.Series.Id))
            {
                ResultDetails += "Image Series ID contains illegal character\n";
                ResultColor = ResultColorChoices.Fail;
            }
            foreach (var beam in plan.Beams)
            {
                if (ContainsIllegal(beam.Id))
                {
                    ResultDetails += $"{beam.Id} ID contains illegal character\n";
                    ResultColor = ResultColorChoices.Fail;
                }
            }

            if (ResultDetails == "")
                ResultDetails = "Pass";
            else
                ResultDetails += "Click here for more details";
        }

        private bool ContainsIllegal(string input)
        {
            return input.Any(x => illegalChars.Contains(x));
        }
    }
}
