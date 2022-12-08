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
            TestExplanation = "Displays calculation algorithm and dose grid size and guesses at SRS/SBRT status based on number of fractions and dose per fraction (<= 5 fx and >= 500 cGy / fx)";
            Result = plan.PhotonCalculationModel;
            ResultDetails = $"{plan.Dose.XRes} mm";

            var gridSize = plan.Dose.XRes;

            // Most likely SRS/SBRT (should be 1 mm)
            if (plan.NumberOfFractions <= 5 && plan.DosePerFraction >= new DoseValue(500, DoseValue.DoseUnit.cGy))
            {
                if (gridSize > 1.0)
                {
                    ResultDetails += "\nCheck grid size if this is SRS/SBRT";
                    DisplayColor = ResultColorChoices.Warn;
                }
            }
            // Electron (should be 1 mm)
            else if (plan.Beams.Count(b => b.EnergyModeDisplayName.Contains('e')) > 0)
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
