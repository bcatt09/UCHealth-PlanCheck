﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMS.TPS.Common.Model.API;

namespace PlanCheck.Checks
{
    public class CTSimChecks : PlanCheckGeneric
	{
		protected override List<string> MachineExemptions => new List<string> { };

		public CTSimChecks(PlanSetup plan) : base(plan) { }

        public override void RunTest(PlanSetup plan)
        {
			DisplayName = "CT Sim";
			ResultDetails = "";
			TestExplanation = "Checks that the correct CT is chosen for the image series based on what treatment unit is used for planning\n" +
							  "Warning: This will not correctly account for patients simmed at one facility and treated at another";

			string ct = plan.StructureSet.Image.Series.ImagingDeviceId;

			if (!DepartmentInfo.GetCTIDs(Department).Contains(ct))
			{
				Result = "Warning";
				ResultDetails = $"Patient is being treated on {MachineID}, but the {String.Join(" or ", DepartmentInfo.GetCTIDs(Department))}\nwas not chosen as the imaging device for the series, please check";
				ResultDetails += $"\nSelected CT: {ct}\n";
				ResultColor = ResultColorChoices.Warn;
			}

			if (ResultDetails == "")
			{
				Result = "";
				ResultDetails = ct;
				ResultColor = ResultColorChoices.Pass;
			}

			ResultDetails = ResultDetails.TrimEnd('\n');
		}
    }
}
