using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;

namespace PlanCheck.Checks
{
    public class PhotonFieldTabChecks : PlanCheckLinac
    {
        protected override List<string> MachineExemptions => new List<string> { };

        public PhotonFieldTabChecks(PlanSetup plan) : base(plan) { }

        public override void RunTestLinac(ExternalPlanSetup plan)
        {
            DisplayName = "Field Tab Checks";
            Result = "";
            ResultDetails = "";
            TestExplanation = "Isocenter the same for all fields\nSSD values populated\nField and collimator arrengement reasonable\n(just checks that IMRT/VMAT does not use collimator 0)";

            var isocenters = plan.Beams.GroupBy(x => x.IsocenterPosition).Count();

            // Isocenters
            if (isocenters > 1)
            {
                Result = "Failure";
                ResultDetails += $"{isocenters} isocenters detected\n";
                ResultColor = ResultColorChoices.Fail;
            }

            // SSDs
            if (plan.Beams.Count(x => Double.IsNaN(x.SSD)) > 0)
            {
                Result = "Failure";
                ResultDetails += $"{String.Join("\n", plan.Beams.Where(x => Double.IsNaN(x.SSD)).Select(x => $"No SSD calculated for {x.Id}"))}\n";
                ResultColor = ResultColorChoices.Fail;
            }
            foreach(var beam in plan.Beams.Where(x => x.EnergyModeDisplayName.ToUpper().Contains('E')))
            {
                if (Math.Round(beam.SSD) != 1000)
                {
                    ResultDetails += $"{beam.Id}: SSD = {Math.Round(beam.SSD / 10, 1)} cm";
                    ResultColor = ResultColorChoices.Warn;
                }
            }

            #region Get IMRT/VMAT usage
            bool IMRT = false;
            bool VMAT = false;

            // Loop through beams to see what needs to be displayed
            foreach (Beam beam in plan.Beams.Where(x => !x.IsSetupField))
            {
                if (beam.MLCPlanType == MLCPlanType.DoseDynamic && beam.ControlPoints.Count > 18)
                    IMRT = true;
                else if (beam.MLCPlanType == MLCPlanType.VMAT)
                    VMAT = true;
            }
            #endregion

            // Gantry and collimator angles
            var coll0 = plan.Beams.Where(x => !x.IsSetupField && x.ControlPoints.First().CollimatorAngle == 0);

            if ((IMRT || VMAT) && coll0.Any())
            {
                Result = "Failure";
                ResultDetails += String.Join("\n", coll0.Select(x => $"{x.Id} uses collimator 0")) + "\n";
                ResultColor = ResultColorChoices.Fail;
            }

            if (ResultDetails == "")
            {
                Result = "Pass";
                ResultColor = ResultColorChoices.Pass;
            }

            ResultDetails = ResultDetails.TrimEnd('\n');
        }
    }
}
