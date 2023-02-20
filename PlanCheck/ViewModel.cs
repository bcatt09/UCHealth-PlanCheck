using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMS.TPS.Common.Model.Types;
using VMS.TPS.Common.Model.API;
using System.Windows;
using System.Collections.ObjectModel;
using PlanCheck.Checks;
using NLog;
using NLog.Fluent;
using System.Reflection;
using System.IO;
using System.ServiceModel.Configuration;
using PlanCheck.BaseClass;
using PVH_Log;

namespace PlanCheck
{
    class ViewModel : INotifyPropertyChanged
    {
        private ScriptContext _context;                                             // ScriptContext from Aria

        private ObservableCollection<CategoryCheckList> _checkLists;
        public ObservableCollection<CategoryCheckList> CheckLists { get { return _checkLists; } set { _checkLists = value; OnPropertyChanged("CheckLists"); } }




        private string _patientName;                                                // Patient name
        public string PatientName { get { return _patientName; } set { _patientName = value; OnPropertyChanged("PatientName"); } }

        private string _planID;                                                     // Plan id
        public string PlanID { get { return _planID; } set { _planID = value; OnPropertyChanged("PlanID"); } }

        private string _courseID;                                                   // Course id
        public string CourseID { get { return _courseID; } set { _courseID = value; OnPropertyChanged("CourseID"); } }

        private string _slices;                                                     // Number of CT slices
        public string Slices { get { return _slices; } set { _slices = value; OnPropertyChanged("Slices"); } }

        private static readonly PVH_Logger logger = PVH_Logger.Logger;

        public ViewModel(ScriptContext context)
        {
            _context = context;

            // Plan information
            PatientName = context.Patient.Name;
            CourseID = context.Course?.Id;
            PlanID = context.PlanSetup?.Id;
            Slices = context.Image.ZSize.ToString();

            logger.Initialize("PVH_PlanCheck", context, Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));

            RunPlanChecks();

            logger.Log();
        }

        // Run all tests
        private void RunPlanChecks()
        {
            var plan = _context.PlanSetup;
            var ss = _context.StructureSet;

            CheckLists = new ObservableCollection<CategoryCheckList>
            {
                new CategoryCheckList("CT Import", new ObservableCollection<PlanCheckBase>
                {
                    new BodyContour(ss),
                    new CouchStructuresChecks(ss),
                    new UserOrigin(ss),
                    new StructureTemplateCheck(ss),
                    new ImportNamingConventions(ss)
                }),

                new CategoryCheckList("Pre - MD Review", new ObservableCollection<PlanCheckBase>
                {
                    new CourseChecks(plan),
                    // normal contours complete (no empty contours?)
                    // rename/delete opti (don't think I can check)
                    new MachineChecks(plan),
                    new CouchStructuresChecks(ss),
                    // delta couch (can't seem to check)
                    new DensityOverrides(plan),
                    new DoseGrid(plan),
                    new BolusChecks(plan),
                    new DPVChecks(plan),


                    new HotspotChecks(plan),
                    new DoseRateChecks(plan),
                    new CTSimChecks(plan),
                    new OrientationChecks(plan),
                    new IsocenterChecks(plan),
                    new MLCChecks(plan),
                    new JawTrackingChecks(plan),
                    new TargetChecks(plan)
                }),

                new CategoryCheckList("Tx Prep - Dosi", new ObservableCollection<PlanCheckBase>
                {
                    new FieldNameChecks(plan),
                    new ToleranceTableChecks(plan),
                    // treatment time calculation
                    new CouchValueChecks(plan),
                    // setup notes (can't do)
                    new Shifts(plan),
                    new DRRChecks(plan),
                    new VerificationPlan(plan),
                    new UnapprovedPlans(plan)
                }),

                new CategoryCheckList("Rx", new ObservableCollection<PlanCheckBase>
                {
                    new RxName(plan),
                    new RxSite(plan),
                    new RxDose(plan),
                    new RxPhase(plan),
                    new RxModality(plan),
                    new RxTechnique(plan),
                    new RxEnergy(plan),
                    // Frequency (can't do)
                    new RxComments(plan),
                    new RxApproval(plan)
                }),

                plan.Beams.Where(x => x.EnergyModeDisplayName.ToUpper().Contains('E')).Any()
                ?   // Electron plan
                new CategoryCheckList("Electron Plan", new ObservableCollection<PlanCheckBase>
                {
                    new PhotonDoseTabChecks(plan),
                    new UserOrigin(ss),
                    new CouchStructuresChecks(ss),
                    // body contour?
                    new FieldNameChecks(plan),
                    new DensityOverrides(plan),
                    new DoseGrid(plan),
                    new ElectronCalcModelTabChecks(plan),
                    new ElectronBlockChecks(plan),
                    new PhotonFieldTabChecks(plan),
                    new HotspotChecks(plan)
                    // DVH/scorecard
                    // delta couch
                })
                :   // Photon plan
                new CategoryCheckList("Photon Plan", new ObservableCollection<PlanCheckBase>
                {
                    new PhotonDoseTabChecks(plan),
                    new UserOrigin(ss),
                    new CouchStructuresChecks(ss),
                    // body contour?
                    new FieldNameChecks(plan),
                    new DensityOverrides(plan),
                    new DoseGrid(plan),
                    new PhotonCalcModelTabChecks(plan),
                    new PhotonFieldTabChecks(plan),
                    new HotspotChecks(plan)
                    // DVH/scorecard
                    // delta couch
                }),

                new CategoryCheckList("Tx Prep - Physics", new ObservableCollection<PlanCheckBase>
                {
                    new PlanSchedulingChecks(plan),
                    new CouchValueChecks(plan),
                    // tx time and multiplication factor
                    new ToleranceTableChecks(plan),
                    // set up notes (can't do)
                    new DRRChecks(plan),
                    new DPVChecks(plan),
                    new BolusChecks(plan),
                    new PlanApprovalChecks(plan)
                }),

                new CategoryCheckList("Other Misc", new ObservableCollection<PlanCheckBase>
                {
                    // Other misc checks (maybe add them in other tabs?)
                    new PrecriptionChecks(plan),
                    new UseGatedChecks(plan),
                    new TreatmentTimeCalculation(plan),
                    new CalcParametersChecks(plan)
                })
            };
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
