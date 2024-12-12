using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMS.TPS.Common.Model.API;

namespace PlanCheck.Checks
{
    public class RxApproval : PlanCheckGeneric
    {
        public RxApproval(PlanSetup plan) : base(plan) { }

        public override void RunTest(PlanSetup plan)
        {
            DisplayName = "Approval";
            TestExplanation = "Checks for Prescription approval by a physician";
            ResultColor = ResultColorChoices.Pass;

            var rx = plan.RTPrescription;
            var dep = DepartmentInfo.GetDepartment(plan.Beams.First().TreatmentUnit.Id);

            if(rx == null )
            {
                Result = "No Prescription Attached";
                ResultColor = ResultColorChoices.Fail;

                return;
            }

            // Rx is approved
            if (rx.Status == "Approved")
            {
                var fullUser = rx.HistoryUserName;
                var user = fullUser.Substring(fullUser.IndexOf('\\') + 1, fullUser.Length - fullUser.IndexOf('\\') - 1);

                // Not approved by a department physician
                if (!DepartmentInfo.GetRadOncUserNames(dep).Contains(user))
                {
                    Result = "Failure";
                    ResultColor = ResultColorChoices.Fail;
                    ResultDetails = $"Prescription approved by non-physician: {rx.HistoryUserDisplayName} ({fullUser})";
                }
                // Is approved by a department physician
                else
                {
                    Result = rx.Status;
                    ResultDetails = $"by {rx.HistoryUserDisplayName} at {rx.HistoryDateTime.ToString("MM/dd H:mm tt")}";
                }
            }
            // Rx not approved
            else
            {
                Result = rx.Status;
                ResultColor = ResultColorChoices.Fail;
            }

        }
    }
}
