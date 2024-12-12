using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMS.TPS.Common.Model.API;

namespace PlanCheck.Checks
{
    /// <summary>
    /// Base class for all new plan checks that do not require a plan to be generated yet<br/>
    /// Must add constructor: <br/> <code>public MyNewCheck(PlanSetup plan) : base(plan) { }</code>
    /// </summary>
    public abstract class PlanCheckStructureSet : BaseClass.PlanCheckBase
    {
        public PlanCheckStructureSet(StructureSet structureSet) : base(structureSet) { }

        public override void RunTest(PlanSetup plan)
        {
            throw new NotImplementedException();
        }

        public override void RunTest(StructureSet structureSet)
        {
            RunTestStructureSet(structureSet);
        }

        public abstract void RunTestStructureSet(StructureSet structureSet);
    }
}
