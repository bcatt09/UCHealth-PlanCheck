using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;

namespace PlanCheck.Checks
{
    public class ElectronCalcModelTabChecks : PlanCheckLinac
    {
        protected override List<string> MachineExemptions => new List<string> { };

        public ElectronCalcModelTabChecks(PlanSetup plan) : base(plan) { }

        public override void RunTestLinac(ExternalPlanSetup plan)
        {
            string ElectronModel = "EMC_15605";

            DisplayName = "Calculation Model Tab";
            Result = "";
            ResultDetails = "";
            ResultColor = ResultColorChoices.Pass;

            var calcOptions = plan.ElectronCalculationOptions;

            // Calc Model
            var volModel = plan.ElectronCalculationModel;
            if (volModel != ElectronModel)
            {
                Result = "Failure";
                ResultColor = ResultColorChoices.Fail;
                ResultDetails = $"Incorrect volume dose model selected - ({calcOptions["VolumeDose"]})\n";
            }

            // Calc Grid
            var grid = calcOptions["CalculationGridSizeInCM"];
            if (grid != "0.10")
            {
                Result = "Failure";
                ResultColor = ResultColorChoices.Fail;
                ResultDetails = $"Calculation grid size not set to 1 mm - ({calcOptions["CalculationGridSizeInCM"]} cm)\n";
            }

            // Statistical Uncertainty
            var unc = calcOptions["StatisticalUncertainty"];
            if (unc != "2")
            {
                Result = "Failure";
                ResultColor = ResultColorChoices.Fail;
                ResultDetails = $"Statistical Uncertainty not set to 2 - ({calcOptions["StatisticalUncertainty"]})\n";
            }

            // RNG Seed
            var seed = calcOptions["RandomGeneratorSeedNumber"];
            if (seed != "0")
            {
                Result = "Failure";
                ResultColor = ResultColorChoices.Fail;
                ResultDetails = $"Random Number Generator Seed not set to 0 - ({calcOptions["RandomGeneratorSeedNumber"]})\n";
            }

            // Uncertainty Threshold
            var thresh = calcOptions["DoseThresholdForUncertainty"];
            if (thresh != "50")
            {
                Result = "Failure";
                ResultColor = ResultColorChoices.Fail;
                ResultDetails = $"Random Number Generator Seed not set to 0 - ({calcOptions["DoseThresholdForUncertainty"]})\n";
            }

            // Smoothing Method
            var method = calcOptions["SmoothingMethod"];
            if (method != "3-D_Gaussian")
            {
                Result = "Failure";
                ResultColor = ResultColorChoices.Fail;
                ResultDetails = $"Smoothing Method not set to 3D Gaussian - ({calcOptions["SmoothingMethod"]})\n";
            }

            // Smoothing Level
            var level = calcOptions["SmoothingLevel"];
            if (level != "Low")
            {
                Result = "Failure";
                ResultColor = ResultColorChoices.Fail;
                ResultDetails = $"Smoothing Level not set to Low - ({calcOptions["SmoothingLevel"]})\n";
            }

            // Normalization Method
            var norm = calcOptions["NormalizationMethod"];
            // Global Dmax for gynecomastia
            if (plan.Course.Diagnoses.Any(x => x.ClinicalDescription.Contains("Hypertrophy of breast")))
            {
                if (norm != "Global Dmax")
                {
                    Result = "Failure";
                    ResultColor = ResultColorChoices.Fail;
                    ResultDetails = $"Normalization Method should be set to \"Global Dmax\" for gynecomastia - ({calcOptions["NormalizationMethod"]})\n";
                }
            }
            // Central axid Dmax otherwise
            else
            {
                if (norm != "Central axis Dmax")
                {
                    Result = "Failure";
                    ResultColor = ResultColorChoices.Fail;
                    ResultDetails = $"Normalization Method not set to \"Central axis Dmax\" - ({calcOptions["NormalizationMethod"]})\n";
                }
            }

            // Final result
            if (ResultDetails != "")
            {
                Result = "Failure";
                ResultDetails += '\n';
            }
            else
                Result = "Pass";
            ResultDetails += "Click to see full model options";
            
            // Full list of options
            TestExplanation += $"Volume Dose: {plan.PhotonCalculationModel}\n";
            TestExplanation += String.Join("\n", calcOptions.Select(x => $"{AddSpaces(x.Key)}: {x.Value}"));

            TestExplanation += "\n\nChecks that:\n" +
                               "Volume Dose = EMC_15605\n" +
                               "Statistical Uncertainty = 2\n" +
                               "Calculation Grid Size in CM = 0.10\n" +
                               "Random Number Generator Seed = 0\n" +
                               "Number Of Particle Histories = 0\n" +
                               "Dose Threshold For Uncertainty = 50\n" +
                               "Smoothing Method = 3-D_Gaussian\n" +
                               "Smoothing Level = Low\n" +
                               "Normalization Method = Central axis Dmax (Global Dmax for gynecomastia)\n";
        }
    }
}
