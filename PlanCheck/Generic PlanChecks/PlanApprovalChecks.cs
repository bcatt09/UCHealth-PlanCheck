using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;

namespace PlanCheck.Checks
{
    public class PlanApprovalChecks : PlanCheckBase
    {
        protected override List<string> MachineExemptions => new List<string> { };

        public PlanApprovalChecks(PlanSetup plan) : base(plan) { }

        public override void RunTest(PlanSetup plan)
        {
            PlanSetupApprovalStatus approvalStatus = plan.ApprovalStatus;

            DisplayName = "Plan Approval";
            Result = "";
            ResultDetails = $"Status: {AddSpaces(approvalStatus.ToString())}";
            DisplayColor = ResultColorChoices.Pass;
            TestExplanation = "Displays plan approval\nAlso checks that plan has been Planning Approved by a physician";

            // Not Approved yet
            if (approvalStatus != PlanSetupApprovalStatus.ExternallyApproved && approvalStatus != PlanSetupApprovalStatus.PlanningApproved && approvalStatus != PlanSetupApprovalStatus.Reviewed && approvalStatus != PlanSetupApprovalStatus.TreatmentApproved)
            {
                Result = "";
                ResultDetails = "No Plan Approvals Found";
                DisplayColor = ResultColorChoices.Warn;

            }

            // Has been Approved or Reviewed
            else
            {
                // Hasn't been "Reviewed"
                if (plan.ApprovalHistory.Where(x => x.ApprovalStatus == PlanSetupApprovalStatus.PlanningApproved).Count() < 1)
                {
                    Result = "Warning";
                    ResultDetails += "Plan has not been Planning Approved\nVerify that a physician has approved the plan";
                }
                // Has been Planning Approved
                if (plan.ApprovalHistory.Where(x => x.ApprovalStatus == PlanSetupApprovalStatus.PlanningApproved).Count() > 0)
                {
                    // Get user who marked plan as PlanningApproved last
                    ApprovalHistoryEntry planningApprovedHistoryEntry = plan.ApprovalHistory.Where(x => x.ApprovalStatus == PlanSetupApprovalStatus.PlanningApproved).Last();
                    string planningApprovedUserDisplayName = planningApprovedHistoryEntry.UserDisplayName;
                    string planningApprovedUserName = planningApprovedHistoryEntry.UserId;
                    string planningApprovedDateTime = planningApprovedHistoryEntry.ApprovalDateTime.ToString("dddd, MMMM d, yyyy H:mm:ss tt");
                    string planningApprovedUserNameMinusDomain = planningApprovedUserName.Substring(planningApprovedUserName.IndexOf('\\') + 1);


                    if (DepartmentInfo.GetRadOncUserNames(Department).Count > 0)
                    {
                        // Check approval user name against physician list
                        if (!DepartmentInfo.GetRadOncUserNames(Department).Contains(planningApprovedUserNameMinusDomain))
                        {
                            Result = "Warning";
                            DisplayColor = ResultColorChoices.Warn;
                            ResultDetails += $"\n\"Reviewed\" by {planningApprovedUserDisplayName} at {planningApprovedDateTime}\n Plan Reviewer not on Physician List for Center";
                            //System.Windows.MessageBox.Show($"runmd: {reviewedUserNameMinusDomain}, dept: {Department}");
                        }
                        else
                        {
                            DisplayColor = ResultColorChoices.Pass;
                            ResultDetails += $"\nReviewed by: {planningApprovedUserDisplayName} at {planningApprovedDateTime}";
                        }
                    }
                    // No physician user names have been defined for this site
                    else
                        TestNotImplemented();
                }
                // Has been Treatment Approved
                if (plan.ApprovalHistory.Where(x => x.ApprovalStatus == PlanSetupApprovalStatus.TreatmentApproved).Count() > 0)
                {
                    // Get user who marked plan as "Treatment Approved" last
                    ApprovalHistoryEntry treatApprovedHistoryEntry = plan.ApprovalHistory.Where(x => x.ApprovalStatus == PlanSetupApprovalStatus.TreatmentApproved).Last();
                    string treatApprovedUserDisplayName = treatApprovedHistoryEntry.UserDisplayName;
                    string treatApprovedDateTime = treatApprovedHistoryEntry.ApprovalDateTime.ToString("dddd, MMMM d, yyyy H:mm:ss tt");

                    ResultDetails += $"\nTreatment Approved by: {treatApprovedUserDisplayName} at {treatApprovedDateTime}";
                }
            }
        }
    }
}
