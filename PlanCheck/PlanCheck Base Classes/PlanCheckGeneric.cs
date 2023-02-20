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
    /// <summary>
    /// Base class for all new plan checks that apply to any plan type<br/>
    /// Must add constructor: <br/> <code>public MyNewCheck(PlanSetup plan) : base(plan) { }</code>
    /// </summary>
    public abstract class PlanCheckGeneric : BaseClass.PlanCheckBase
    {
        protected override List<string> MachineExemptions => new List<string> { };

        public PlanCheckGeneric(PlanSetup plan) : base(plan) { }
    }
}
