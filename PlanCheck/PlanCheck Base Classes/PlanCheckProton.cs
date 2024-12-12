using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMS.TPS.Common.Model.API;

namespace PlanCheck.Checks
{
    /// <summary>
    /// Base class for all new plan checks that apply to proton plans<br/>
    /// Must add constructor: <br/> <code>public MyNewCheck(PlanSetup plan) : base(plan) { }</code>
    /// </summary>
    public abstract class PlanCheckProton : BaseClass.PlanCheckBase
    {
        public PlanCheckProton(PlanSetup plan) : base(plan) { }

        public override void RunTest(PlanSetup plan)
        {
            MachineExemptions.Concat(DepartmentInfo.LinearAccelerators);
            RunTestProton(plan as IonPlanSetup);
        }

        public abstract void RunTestProton(IonPlanSetup plan);
    }
}
