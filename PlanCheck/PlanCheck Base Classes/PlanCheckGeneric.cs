using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Configuration;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using VMS.TPS.Common.Model.API;

namespace PlanCheck.Checks
{
    public abstract class PlanCheckGeneric : PlanCheckBase
    {
        protected override List<string> MachineExemptions => new List<string> { };

        public PlanCheckGeneric(PlanSetup plan) : base(plan) { }
    }
}
