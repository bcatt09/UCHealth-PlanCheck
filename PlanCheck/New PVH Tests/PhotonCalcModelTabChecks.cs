using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;

namespace PlanCheck.Checks
{
    public class PhotonCalcModelTabChecks : PlanCheckBase
    {
        protected override List<string> MachineExemptions => new List<string> { };

        public PhotonCalcModelTabChecks(PlanSetup plan) : base(plan) { }

        public override void RunTest(PlanSetup plan)
        {
            string PhotonModel = "AAA_15605";
            string POModel = "PO_15605";

            DisplayName = "Calculation Model Tab";
            Result = "";
            ResultDetails = "";
            DisplayColor = ResultColorChoices.Pass;

            bool IMRT = false;
            bool VMAT = false;

            var calcOptions = plan.PhotonCalculationOptions;

            #region Get IMRT/VMAT usage
            // Loop through beams to see what needs to be displayed
            foreach (Beam beam in plan.Beams.Where(x => !x.IsSetupField))
            {
                if (beam.MLCPlanType == MLCPlanType.DoseDynamic && beam.ControlPoints.Count > 18)
                    IMRT = true;
                else if (beam.MLCPlanType == MLCPlanType.VMAT)
                    VMAT = true;
            }
            #endregion

            // Field normalization
            var norm = calcOptions["FieldNormalizationType"];
            if (norm != "100% to isocenter")
            {
                Result = "Failure";
                DisplayColor = ResultColorChoices.Fail;
                ResultDetails = $"Field Normalization Type not set to \"100% to isocenter\" - ({calcOptions["FieldNormalizationType"]})\n";
            }

            // Heterogeneity correction
            var het = calcOptions["HeterogeneityCorrection"];
            if (het != "ON")
            {
                Result = "Failure";
                DisplayColor = ResultColorChoices.Fail;
                ResultDetails = "Heterogeneity corrections not turned on\n";
            }

            // Calc model
            var volModel = plan.PhotonCalculationModel;
            if (volModel != PhotonModel)
            {
                Result = "Failure";
                DisplayColor = ResultColorChoices.Fail;
                ResultDetails = $"Incorrect volume dose model selected ({calcOptions["VolumeDose"]})\n";
            }

            // PO model
            var poModel = plan.GetCalculationModel(CalculationType.PhotonIMRTOptimization);
            if (poModel != POModel)
            {
                Result = "Failure";
                DisplayColor = ResultColorChoices.Fail;
                ResultDetails = $"Incorrect PO model selected ({poModel})\n";
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

            if (IMRT)
            {
                TestExplanation += $"\n\nIMRT Optimization: {plan.GetCalculationModel(CalculationType.PhotonIMRTOptimization)}\n";
                TestExplanation += String.Join("\n", plan.GetCalculationOptions(plan.GetCalculationModel(CalculationType.PhotonIMRTOptimization)).Select(x => $"{AddSpaces(x.Key)}: {x.Value}"));
                TestExplanation += $"\n\nLeaf Motion: {plan.GetCalculationModel(CalculationType.PhotonLeafMotions)}\n";
                TestExplanation += String.Join("\n", plan.GetCalculationOptions(plan.GetCalculationModel(CalculationType.PhotonLeafMotions)).Select(x => $"{AddSpaces(x.Key)}: {x.Value}"));
            }
            if (VMAT)
            {
                TestExplanation += $"\n\nVMAT Optimization: {plan.GetCalculationModel(CalculationType.PhotonVMATOptimization)}\n";
                TestExplanation += String.Join("\n", plan.GetCalculationOptions(plan.GetCalculationModel(CalculationType.PhotonVMATOptimization)).Select(x => $"{AddSpaces(x.Key)}: {x.Value}"));
            }


            TestExplanation += "\n\nAlso checks that:\n" +
                               "Field Normalization Type = 100% to isocenter\n" +
                               "Heterogeneity Correctionsn = On\n" +
                               "Calculation Model = AAA_15605\n" +
                               "Optimization Model = PO_15605\n";
        }
    }
}
