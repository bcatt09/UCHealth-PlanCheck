using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMS.TPS.Common.Model.API;

namespace PlanCheck.Checks
{
    struct BlockResults
    {
        public string BeamId;
        public string BlockId;
        public string Material;
        public string Transmission;
        public string Type;
        public string DivergingCut;
        public string Tray;
    }

    struct TrayResults
    {
        public string BeamId;
        public string TrayId;
    }

    struct BeamResults
    {
        public string Id;
        public string Applicator;
        public List<BlockResults> BlockResults;
        public List<TrayResults> TrayResults;
    }

    class ElectronBlockChecks : PlanCheckLinac
    {
        protected override List<string> MachineExemptions => new List<string> { };

        public ElectronBlockChecks(PlanSetup plan) : base(plan) { }

        public override void RunTestLinac(ExternalPlanSetup plan)
        {
            DisplayName = "Block Properties";
            ResultColor = ResultColorChoices.Pass;
            Result = "";
            ResultDetails = "";
            TestExplanation = "Checks that:\n" +
                "Material code = e-cut\n" +
                "Aperture is selected\n" +
                "Diverging Cut is selected\n" +
                "CustomFFDA or CustomFFDA10";

            List<BeamResults> beamResults = new List<BeamResults>();

            foreach (Beam beam in plan.Beams.Where(x => x.EnergyModeDisplayName.ToUpper().Contains('E')))
            {
                beamResults.Add(new BeamResults
                {
                    Id = beam.Id,
                    Applicator = beam.Applicator.Id,
                    BlockResults = beam.Blocks.Select(x => GetBlockResults(x, beam)).ToList(),
                    TrayResults = beam.Trays.Select(x => GetTrayResults(x, beam)).ToList()
                });
            }

            ResultDetails += $"Applicator:\n{String.Join("\n", beamResults.Select(x => x.Id + ": " + x.Applicator))}";
            ResultDetails += $"\n\nBlock:\n{String.Join("\n", beamResults.SelectMany(x => x.BlockResults).Select(x => "" + PrintBlockResults(x, "")))}";
            //ResultDetails += $"\n\nTray:\n{String.Join("\n", beamResults.SelectMany(x => x.TrayResults).Select(x => "" + PrintTrayResults(x, "")))}";
        }

        private BlockResults GetBlockResults(Block block, Beam beam)
        {
            return new BlockResults
            {
                BeamId = beam.Id,
                BlockId = block.Id,
                Material = block.AddOnMaterial.Id,
                Transmission = $"{block.TransmissionFactor * 100:0.0}%",
                Type = block.Type.ToString(),
                DivergingCut = block.IsDiverging.ToString(),
                Tray = block.Tray.Id,
            };
        }

        private TrayResults GetTrayResults(Tray tray, Beam beam)
        {
            return new TrayResults
            {
                BeamId = beam.Id,
                TrayId = tray.Id
            };
        }

        private string PrintBlockResults(BlockResults results, string prefix)
        {
            if (results.Material != "e-cut")
                ResultColor = ResultColorChoices.Fail;
            if (results.Type.ToLower() != "aperture")
                ResultColor = ResultColorChoices.Fail;
            if (results.DivergingCut.ToLower() != "true")
                ResultColor = ResultColorChoices.Fail;
            if (results.Tray != "CustomFFDA" && results.Type != "CustomFFDA10")
                ResultColor = ResultColorChoices.Fail;

            return results.BeamId + ":\n" +
                $"{prefix}Material: {results.Material}\n" +
                $"{prefix}Block Transmission: {results.Transmission}\n" +
                $"{prefix}Type: {results.Type}\n" +
                $"{prefix}Diverging Cut: {results.DivergingCut}\n" +
                $"{prefix}Tray: {results.Tray}";
        }

        private string PrintTrayResults(TrayResults results, string prefix)
        {
            return $"{results.BeamId}: {results.TrayId}";
        }
    }
}
