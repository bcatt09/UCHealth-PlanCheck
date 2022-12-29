using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMS.TPS.Common.Model.API;

namespace PlanCheck.Checks
{
    public abstract class PlanCheckStructureSet : PlanCheckBase
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
