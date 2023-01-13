using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;

namespace PlanCheck.Checks
{
    public class DoseGrid : PlanCheckBase
    {
        protected override List<string> MachineExemptions => new List<string> { };

        public DoseGrid(PlanSetup plan) : base(plan) { }

        public override void RunTest(PlanSetup plan)
        {
            DisplayName = "Dose Grid";
            TestExplanation = "Displays calculation algorithm and dose grid size\n" +
                              "Guesses at SRS/SBRT status based on number of fractions and dose per fraction (<= 5 fx and > 500 cGy / fx)\n" +
                              "Guesses at Prostate SIB status based on Plan/Course ID and prescribed dose / number of fractions";
            Result = plan.PhotonCalculationModel;
            ResultDetails = $"{plan.Dose.XRes} mm";

            var gridSize = plan.Dose.XRes;

            var rxTarg = plan.RTPrescription.Targets.First();

            var prostSIB = plan.Id.ToUpper().Contains("SIB") ||
                           plan.Course.Id.ToUpper().Contains("SIB") ||
                          (plan.Id.ToUpper().Contains("PROST") && (rxTarg.DosePerFraction * rxTarg.NumberOfFractions) > new DoseValue(8100, DoseValue.DoseUnit.cGy)) ||
                          (plan.Id.ToUpper().Contains("PROST") && (rxTarg.NumberOfFractions < 39) && (rxTarg.DosePerFraction * rxTarg.NumberOfFractions) > new DoseValue(7250, DoseValue.DoseUnit.cGy));

            // Most likely SRS/SBRT (should be 1 mm)
            if (plan.NumberOfFractions <= 5 && plan.DosePerFraction > new DoseValue(500, DoseValue.DoseUnit.cGy))
            {
                if (gridSize > 1.0)
                {
                    ResultDetails += "\nCheck grid size if this is SRS/SBRT";
                    DisplayColor = ResultColorChoices.Warn;
                }
            }
            // Prostate SIB (should be 1 mmm)
            if (prostSIB)
            {
                if (gridSize > 1.0)
                {
                    ResultDetails += "\nCheck grid size if this is a prostate SIB";
                    DisplayColor = ResultColorChoices.Warn;
                }
            }
            // Electron (should be 1 mm)
            else if (plan.Beams.Any(b => b.EnergyModeDisplayName.Contains('e')))
            {
                if (gridSize > 1.0)
                    DisplayColor = ResultColorChoices.Fail;
            }
            // Regular photon plan
            else
            {
                if (gridSize > 2.0)
                    DisplayColor = ResultColorChoices.Fail;
            }
        }
    }
}
