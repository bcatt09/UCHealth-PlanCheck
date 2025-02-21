using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;

namespace PlanCheck.Helpers
{
    public static class TreatmentClassifier
    {
        public static bool IsBreastAPBI (PlanSetup plan)
        {
            var rxTarg = plan.RTPrescription?.Targets?.FirstOrDefault();

            return (plan.RTPrescription?.Site == "Breast") && 
                   (rxTarg?.NumberOfFractions <= 5) &&
                   (rxTarg?.DosePerFraction * rxTarg?.NumberOfFractions > new DoseValue(2500, DoseValue.DoseUnit.cGy));
        }

        public static bool IsProstSIB (PlanSetup plan)
        {
            var rxTarg = plan.RTPrescription?.Targets?.FirstOrDefault();

            return plan.Id.ToUpper().Contains("SIB") ||
                   plan.Course.Id.ToUpper().Contains("SIB") ||
                   (plan.Id.ToUpper().Contains("PROST") && (rxTarg?.DosePerFraction * rxTarg?.NumberOfFractions) > new DoseValue(8100, DoseValue.DoseUnit.cGy)) ||
                   (plan.Id.ToUpper().Contains("PROST") && (rxTarg?.NumberOfFractions < 39) && (rxTarg?.DosePerFraction * rxTarg?.NumberOfFractions) > new DoseValue(7250, DoseValue.DoseUnit.cGy));
        }
    }
}
