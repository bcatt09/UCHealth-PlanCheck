using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;
using System.Windows.Media.TextFormatting;
using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;

namespace PlanCheck.Checks
{
    public class RxTechnique : PlanCheckGeneric
    {
        public RxTechnique(PlanSetup plan) : base(plan) { }

        public override void RunTest(PlanSetup plan)
        {
            DisplayName = "Technique";
            TestExplanation = "Displays the prescribed technique and checks if the planned beams match\n" +
                              "SRS - Looks for SRS technique\n" +
                              "VMAT - Looks for VMAT MLC type\n" +
                              "IMRT - Looks for DoseDynamic MLC type with > 25 control points\n" +
                              "3D - Looks for Static or ArcDynamic MLC type or DoseDynamic MLC type with < 25 control points";
            ResultColor = ResultColorChoices.Pass;

            if (plan.RTPrescription == null)
            {
                Result = "No Prescription Attached";
                ResultColor = ResultColorChoices.Fail;

                return;
            }

            var srsSbrtTechniques = new List<string> { "SRS", "SBRT" };
            var vmatTechniques = new List<string> { "VMAT" };
            var imrtTechniques = new List<string> { "IMRT" };
            var threeDTechniques = new List<string> { "2D", "3D", "AP/PA", "Laterals", "Obliques", "Tangents", "WedgedPair" };

            var tech = plan.RTPrescription.Technique;

            #region SRS/SBRT
            // SRS/SBRT was prescribed
            if (srsSbrtTechniques.Contains(tech))
            {
                var nonSrsBeams = plan.Beams
                                    .Where(x => !x.IsSetupField)
                                    .Where(x => !x.Technique.Id.Contains("SRS"))
                                    .Select(x => $"{x.Id} ({x.Name}) - {x.Technique}");

                // Non-SRS beams used
                if (nonSrsBeams.Any())
                {
                    Result = $"Prescibed technique is {tech}\nSome beams do not use SRS technique";
                    ResultColor = ResultColorChoices.Warn;
                    ResultDetails = $"{String.Join("\n", nonSrsBeams)}";
                }
                else
                {
                    Result = tech;
                }

                return;
            }
            // Non-SRS/SBRT was prescribed
            else
            {
                var srsBeams = plan.Beams
                                    .Where(x => !x.IsSetupField)
                                    .Where(x => x.Technique.Name.Contains("SRS"))
                                    .Select(x => $"{x.Id} ({x.Name}) - {x.Technique}");

                // SRS beams used
                if (srsBeams.Any())
                {
                    Result = $"Prescibed technique is {tech}\nSome beams use SRS technique";
                    ResultColor = ResultColorChoices.Warn;
                    ResultDetails = $"{String.Join("\n", srsBeams)}";
                }
                else
                {
                    Result = tech;
                }
            }
            #endregion

            #region VMAT
            // VMAT was prescribed
            if (vmatTechniques.Contains(tech))
            {
                var nonVmatBeams = plan.Beams
                                    .Where(x => !x.IsSetupField)
                                    .Where(x => x.MLCPlanType != MLCPlanType.VMAT)
                                    .Select(x => $"{x.Id} ({x.Name}) - {x.MLCPlanType}");

                // Non-VMAT beams used
                if (nonVmatBeams.Any())
                {
                    Result = $"Prescibed technique is {tech}\nSome beams are not VMAT";
                    ResultColor = ResultColorChoices.Warn;
                    ResultDetails = $"{String.Join("\n", nonVmatBeams)}";
                }
                else
                {
                    Result = tech;
                }

                return;
            }
            // Non-VMAT was prescribed
            else
            {
                var vmatBeams = plan.Beams
                                .Where(x => !x.IsSetupField)
                                .Where(x => x.MLCPlanType == MLCPlanType.VMAT)
                                .Select(x => $"{x.Id} ({x.Name}) - {x.MLCPlanType}");

                // VMAT beams used
                if (vmatBeams.Any())
                {
                    Result = $"Prescibed technique is {tech}\nSome beams use SRS technique";
                    ResultColor = ResultColorChoices.Warn;
                    ResultDetails = $"{String.Join("\n", vmatBeams)}";
                }
                else
                {
                    Result = tech;
                }
            }
            #endregion

            #region IMRT
            // IMRT was prescribed
            if (imrtTechniques.Contains(tech))
            {
                var nonImrtBeams = plan.Beams
                                    .Where(x => !x.IsSetupField)
                                    .Where(x => (x.MLCPlanType != MLCPlanType.DoseDynamic) || (x.MLCPlanType == MLCPlanType.DoseDynamic && x.ControlPoints.Count < 25))
                                    .Select(x => $"{x.Id} ({x.Name}) - non-IMRT");

                // Non-SRS beams used
                if (nonImrtBeams.Any())
                {
                    Result = $"Prescibed technique is {tech}\nSome beams are not IMRT";
                    ResultColor = ResultColorChoices.Warn;
                    ResultDetails = $"{String.Join("\n", nonImrtBeams)}";
                }
                else
                {
                    Result = tech;
                }

                return;
            }
            // Non-IMRT was prescribed
            else
            {
                var imrtBeams = plan.Beams
                                    .Where(x => !x.IsSetupField)
                                    .Where(x => x.MLCPlanType == MLCPlanType.DoseDynamic && x.ControlPoints.Count > 25)
                                    .Select(x => $"{x.Id} ({x.Name}) - IMRT");

                // IMRT beams used
                if (imrtBeams.Any())
                {
                    Result = $"Prescibed technique is {tech}\nSome beams use IMRT technique";
                    ResultColor = ResultColorChoices.Warn;
                    ResultDetails = $"{String.Join("\n", imrtBeams)}";
                }
                else
                {
                    Result = tech;
                }
            }
            #endregion

            #region Various 3D
            // 3D was prescribed
            if (threeDTechniques.Contains(tech))
            {
                var non3dBeams = plan.Beams
                                    .Where(x => !x.IsSetupField)
                                    .Where(x => (x.MLCPlanType != MLCPlanType.NotDefined && x.MLCPlanType != MLCPlanType.Static && x.MLCPlanType != MLCPlanType.ArcDynamic && x.MLCPlanType != MLCPlanType.DoseDynamic) || (x.MLCPlanType == MLCPlanType.DoseDynamic && x.ControlPoints.Count > 25))
                                    .Select(x => $"{x.Id} ({x.Name}) - not 3D ({x.MLCPlanType} - {x.ControlPoints.Count} control points)");

                // Non-3D beams used
                if (non3dBeams.Any())
                {
                    Result = $"Prescibed technique is {tech}\nSome beams are not 3D";
                    ResultColor = ResultColorChoices.Warn;
                    ResultDetails = $"{String.Join("\n", non3dBeams)}";
                }
                else
                {
                    Result = tech;
                }

                return;
            }
            // Non-3D was prescribed
            else
            {
                var threeDBeams = plan.Beams
                                    .Where(x => !x.IsSetupField)
                                    .Where(x => (x.MLCPlanType == MLCPlanType.Static) || (x.MLCPlanType == MLCPlanType.DoseDynamic && x.ControlPoints.Count < 25))
                                    .Select(x => $"{x.Id} ({x.Name}) - 3D");

                // IMRT beams used
                if (threeDBeams.Any())
                {
                    Result = $"Prescibed technique is {tech}\nSome beams use 3D technique";
                    ResultColor = ResultColorChoices.Warn;
                    ResultDetails = $"{String.Join("\n", threeDBeams)}";
                }
                else
                {
                    Result = tech;
                }
            }
            #endregion
        }
    }
}
