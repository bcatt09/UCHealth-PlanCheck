﻿using PVH_Log;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.ServiceModel.Channels;
using System.Text;
using System.Windows;
using VMS.TPS.Common.Model.API;
using PlanCheck.Checks;

namespace PlanCheck.BaseClass
{
    /// <summary>
    /// Base class for all new plan checks<br/>
    /// Must add constructor: <br/> <code>public MyNewCheck(PlanSetup plan) : base(plan) { }</code>
    /// </summary>
    public abstract class PlanCheckBase : INotifyPropertyChanged
    {
        /// <summary>
        /// Displayed name of the test that will show in the PlanCheck window
        /// </summary>
        public string DisplayName { get; protected set; }
        /// <summary>
        /// Simple result text (Pass/Warning/Fail) of the test that will show on the first line<br/>
        /// Leave as "" to skip
        /// </summary>
        public string Result { get; protected set; }
        /// <summary>
        /// Detailed results
        /// </summary>
        public string ResultDetails { get; protected set; }
        /// <summary>
        /// Color of the result background used in xaml for display (must be a color from System.Drawing.Color)<br/>
        /// THIS IS USED IN XAML ONLY, YOU MUST SET DisplayColor TO CHANGE THIS<br/>
        /// Pass = LimeGreen<br/>
        /// Warning = Khaki<br/>
        /// Fail = Salmon
        /// </summary>
        public string DisplayColor { get { return DisplayColors.ColorLookup[ResultColor]; } }
        /// <summary>
        /// Explanation of the test that will show in the table when clicked
        /// </summary>
        public string TestExplanation { get; protected set; }
        /// <summary>
        /// Color of the result background (if left blank will default to green)<br/>
        /// Pass = Green<br/>
        /// Warn = Yellow<br/>
        /// Fail = Red
        /// </summary>
        protected ResultColorChoices ResultColor { get; set; }
        /// <summary>
        /// List of machine IDs that this test will not run for<br/>
        /// Example:
        /// <code>protected override List&lt;string&gt; MachineExemptions => new List&lt;string&gt; { Globals.MachineNames.MPH_TB };</code><br/>
        /// To use on all machines, leave list blank:
        /// <code>protected override List&lt;string&gt; MachineExemptions => new List&lt;string&gt; { };</code>
        /// </summary>
        protected abstract List<string> MachineExemptions { get; }
        /// <summary>
        /// ID of the planned treatment machine
        /// </summary>
        protected string MachineID { get; }
        /// <summary>
        /// Department of the planned treatment machine
        /// </summary>
        protected Department Department { get; }

        /// <summary>
        /// Check was not run due to a machine exemption
        /// </summary>
        public bool MachineExempt { get; private set; }

        protected static readonly PVH_Logger logger = PVH_Logger.Logger;

        public PlanCheckBase(PlanSetup plan)
        {
            if (plan == null)
                return;

            if (plan.Beams.Count() < 1)
            {
                MessageBox.Show("Plan must contain at least one beam", "No Beams", MessageBoxButton.OK, MessageBoxImage.Error);
                throw new Exception("Plan must contain at least one beam");
            }

            MachineID = plan.Beams.First().TreatmentUnit.Id;
            Department = DepartmentInfo.GetDepartment(MachineID);

            if (!MachineExemptions.Contains(plan.Beams.First().TreatmentUnit.Id))   // Planned machine isn't in the list of test exceptions
            {
                try
                {
                    if (this is PlanCheckLinac)
                    {
                        if (plan is ExternalPlanSetup)
                             (this as PlanCheckLinac).RunTestLinac(plan as ExternalPlanSetup);
                        else
                            MachineExempt = true;
                    }
                    else if (this is PlanCheckProton)
                    {
                        if (plan is IonPlanSetup)
                             (this as PlanCheckProton).RunTestProton(plan as IonPlanSetup);
                        else
                            MachineExempt = true;
                    }
                    else
                        RunTest(plan);
                }
                catch (Exception e)
                {
                    TestCouldNotComplete(e);
                }
            }
            else
            {
                MachineExempt = true;
            }
        }

        public PlanCheckBase(StructureSet structureSet)
        {
            Department = DepartmentInfo.GetDepartment(structureSet.Image);

            try
            {
                RunTest(structureSet);
            }
            catch (Exception e)
            {
                TestCouldNotComplete(e);
            }
        }

        /// <summary>
        /// Executes test and stores all results
        /// </summary>
        /// <param name="plan">PlanSetup that the test will be run on</param>
        public abstract void RunTest(PlanSetup plan);

        /// <summary>
        /// Executes test and stores all results
        /// </summary>
        /// <param name="image">Image that the test will be run on</param>
        public virtual void RunTest(StructureSet structureSet)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Log that check failed to run
        /// </summary>
        protected void TestCouldNotComplete()
        {
            Result = "Failure - Test could not be run";
            ResultColor = ResultColorChoices.Fail;
        }

        /// <summary>
        /// Log that check failed to run
        /// </summary>
        protected void TestCouldNotComplete(string message)
        {
            logger.Log($"{DisplayName} - Could not complete", Severity.Error);

            ResultDetails = message;
            TestCouldNotComplete();
        }

        /// <summary>
        /// Log that check failed to run
        /// </summary>
        protected void TestCouldNotComplete(Exception e)
        {
            logger.LogError(e);

            TestCouldNotComplete();
        }

        /// <summary>
        /// Log that check failed to run
        /// </summary>
        protected void TestCouldNotComplete(Exception e, string message)
        {
            logger.LogError(e);

            ResultDetails = message;
            TestCouldNotComplete();
        }


        /// <summary>
        /// Test criteria have not been specified by a site for implementation
        /// </summary>
        protected void TestNotImplemented()
        {
            Result = $"Test \"{DisplayName}\" has not been implemented yet for {MachineID}";
            ResultColor = ResultColorChoices.Fail;
            TestExplanation += "\n\nCriteria for test need to be specified";
        }

        /// <summary>
        /// Adds spaces in front of capital letters in text and can attempt to preserve acronyms
        /// </summary>
        /// <param name="text"></param>
        /// <param name="preserveAcronyms"></param>
        /// <returns></returns>
        protected static string AddSpaces(string text, bool preserveAcronyms = true)
        {
            if (string.IsNullOrWhiteSpace(text))
                return string.Empty;
            StringBuilder newText = new StringBuilder(text.Length * 2);
            newText.Append(text[0]);
            for (int i = 1; i < text.Length; i++)
            {
                if (char.IsUpper(text[i]))
                    if ((text[i - 1] != ' ' && !char.IsUpper(text[i - 1])) ||
                        (preserveAcronyms && char.IsUpper(text[i - 1]) &&
                         i < text.Length - 1 && !char.IsUpper(text[i + 1])))
                        newText.Append(' ');
                newText.Append(text[i]);
            }
            return newText.ToString();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }
    }
}
