using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;

namespace PlanCheck.Checks
{
    public class DoseGrid : PlanCheckLinac
    {
        protected override List<string> MachineExemptions => new List<string> { };

        public DoseGrid(PlanSetup plan) : base(plan) { }

        public override void RunTestLinac(ExternalPlanSetup plan)
        {
            DisplayName = "Dose Grid";
            TestExplanation = "Displays calculation algorithm and dose grid size\n" +
                              "Guesses at SRS/SBRT status based on number of fractions and dose per fraction (<= 5 fx and > 500 cGy / fx)\n" +
                              "Guesses at Prostate SIB status based on Plan/Course ID and prescribed dose / number of fractions\n" +
                              "Guesses at Breast APBI status based on Rx Site and prescribed dose of > 2500 cGy in <= 5 fx\n" +
                              "If maximum jaw opening of a field is < 5 cm it fails and tells the user to use 1 mm calc grid";
            Result =  plan.Beams.Any(x => x.EnergyModeDisplayName.ToUpper().Contains('E')) ? plan.ElectronCalculationModel : plan.PhotonCalculationModel;
            ResultDetails = $"{plan.Dose.XRes} mm";

            var gridSize = plan.Dose.XRes;

            var prostSIB = Helpers.TreatmentClassifier.IsProstSIB(plan);
            var breastAPBI = Helpers.TreatmentClassifier.IsBreastAPBI(plan);

            if (breastAPBI)
            {
                if (gridSize > 2.0)
                    ResultColor = ResultColorChoices.Fail;
                return;
            }
            // Most likely SRS/SBRT (should be 1 mm)
            if (plan.NumberOfFractions <= 5 && plan.DosePerFraction > new DoseValue(500, DoseValue.DoseUnit.cGy))
            {
                if (gridSize > 1.0)
                {
                    ResultDetails += "\nCheck grid size if this is SRS/SBRT";
                    ResultColor = ResultColorChoices.Warn;
                }
            }
            // Prostate SIB (should be 1 mmm)
            if (prostSIB)
            {
                if (gridSize > 1.0)
                {
                    ResultDetails += "\nCheck grid size if this is a prostate SIB";
                    ResultColor = ResultColorChoices.Warn;
                }
            }
            // Electron (should be 1 mm)
            else if (plan.Beams.Any(b => b.EnergyModeDisplayName.ToUpper().Contains('E')))
            {
                if (gridSize > 1.0)
                    ResultColor = ResultColorChoices.Fail;
            }
            // Jaw opening < 5 cm in X or Y (should be 1 mm)
            else if (plan.Beams.Where(b => !b.IsSetupField).Min(b => Math.Min(b.ControlPoints.Max(c => c.JawPositions.X2) - b.ControlPoints.Min(c => c.JawPositions.X1), b.ControlPoints.Max(c => c.JawPositions.Y2) - b.ControlPoints.Min(c => c.JawPositions.Y1))) < 50.0)
            {
                if (gridSize > 1.0)
                {
                    ResultColor = ResultColorChoices.Fail;
                    var list = plan.Beams.Where(b => !b.IsSetupField).Select(b =>
                                new
                                {
                                    b.Id,
                                    Opening = Math.Min(b.ControlPoints.Max(c => c.JawPositions.X2) - b.ControlPoints.Min(c => c.JawPositions.X1), b.ControlPoints.Max(c => c.JawPositions.Y2) - b.ControlPoints.Min(c => c.JawPositions.Y1))
                                });

                    var smallBeam = list.OrderBy(x => x.Opening).First();

                    ResultDetails += $"\n{smallBeam.Id} - Max jaw positions: {Math.Round(smallBeam.Opening / 10.0, 1)} cm\nPlease use 1 mm calc grid";
                }
            }
            // Regular photon plan
            else
            {
                if (gridSize > 2.0)
                    ResultColor = ResultColorChoices.Fail;
            }
        }
    }
}
