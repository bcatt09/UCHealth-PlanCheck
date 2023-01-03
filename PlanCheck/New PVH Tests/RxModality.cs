using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMS.TPS.Common.Model.API;

namespace PlanCheck.Checks
{
    public class RxModality : PlanCheckGeneric
    {
        public RxModality(PlanSetup plan) : base(plan) { }

        public override void RunTest(PlanSetup plan)
        {
            DisplayName = "Modality";
            TestExplanation = "Displays the prescribed modality and checks it against the plan";
            DisplayColor = ResultColorChoices.Pass;

            bool prescribedPhoton = false;
            bool prescribedElectron = false;
            bool prescribedBrachy = false;
            bool plannedPhoton = false;
            bool plannedElectron = false;
            bool plannedBrachy = false;

            var prescribedModalities = plan.RTPrescription.EnergyModes;
            var plannedModalities = plan.Beams.Where(x => !x.IsSetupField).Select(x => x.EnergyModeDisplayName);

            // Get prescribed modalities
            foreach (string mode in prescribedModalities)
            {
                switch(mode)
                {
                    case "Photon":
                        prescribedPhoton = true; break;
                    case "Electron":
                        prescribedElectron = true; break;
                    case "Brachytherapy":
                        prescribedBrachy = true; break;
                }
            }

            // Get planned modalities
            foreach (string mode in plannedModalities)
            {
                if (mode.Contains("X"))
                    prescribedPhoton = true;
                if (mode.Contains("e"))
                    prescribedElectron = true;
            }
            if (plan is BrachyPlanSetup)
            {
                System.Windows.MessageBox.Show(String.Join("\n", (plan as BrachyPlanSetup).Beams.Select(x => x.EnergyModeDisplayName)), "Check Rx Modality for brachy");
                plannedBrachy = true;
            }

            // Check planned vs prescribed
            if ((prescribedPhoton != plannedPhoton) || (prescribedElectron != plannedElectron) || (prescribedBrachy != plannedBrachy))
            {
                Result = "Warning";
                DisplayColor = ResultColorChoices.Warn;
                ResultDetails = $"Prescription: {String.Join(", ", prescribedModalities)}\nCheck that prescription matches plan";
            }
            else
            {
                Result = String.Join(", ", prescribedModalities);
            }
        }
    }
}
