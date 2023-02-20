using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMS.TPS.Common.Model.API;

namespace PlanCheck.Checks
{
    public abstract class PlanCheckLinac : BaseClass.PlanCheckBase
    {
        /// <summary>
        /// Base class for all new plan checks that apply to linac plans<br/>
        /// Must add constructor: <br/> <code>public MyNewCheck(PlanSetup plan) : base(plan) { }</code>
        /// </summary>
        public PlanCheckLinac(PlanSetup plan) : base(plan) { }

        public override void RunTest(PlanSetup plan)
        {
            MachineExemptions.Concat(DepartmentInfo.ProtonGantries);
            RunTestLinac(plan as ExternalPlanSetup);
        }

        public override void RunTest(StructureSet structureSet)
        {
            throw new NotImplementedException();
        }

        public abstract void RunTestLinac(ExternalPlanSetup plan);
    }
}
