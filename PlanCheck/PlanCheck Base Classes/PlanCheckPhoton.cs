using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMS.TPS.Common.Model.API;

namespace PlanCheck.Checks
{
    public abstract class PlanCheckPhoton : PlanCheckBase
    {
        public PlanCheckPhoton(PlanSetup plan) : base(plan) { }

        public override void RunTest(PlanSetup plan)
        {
            MachineExemptions.Concat(DepartmentInfo.ProtonGantries);
            RunTestPhoton(plan as ExternalPlanSetup);
        }

        public override void RunTest(StructureSet structureSet)
        {
            throw new NotImplementedException();
        }

        public abstract void RunTestPhoton(ExternalPlanSetup plan);
    }
}
