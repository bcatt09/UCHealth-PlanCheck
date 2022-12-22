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

namespace PlanCheck
{
    class ViewModel : INotifyPropertyChanged
    {
        private ScriptContext _context;                                             // ScriptContext from Aria

        private ObservableCollection<PlanCheckBase> _planChecks;                    // List of plan checks and results to be displayed
        public ObservableCollection<PlanCheckBase> PlanChecks { get { return _planChecks; } set { _planChecks = value; OnPropertyChanged("PlanChecks"); } }

        private ObservableCollection<PlanCheckBase> _ctImportChecks;                // Plan checks from CT Import Questionnaire
        public ObservableCollection<PlanCheckBase> CTImportChecks { get { return _ctImportChecks; } set { _ctImportChecks = value; OnPropertyChanged("PlanChecks"); } }

        private ObservableCollection<PlanCheckBase> _preMdReviewChecks;             // Plan checks from Pre - MD Review Questionnaire
        public ObservableCollection<PlanCheckBase> PreMdReviewChecks { get { return _preMdReviewChecks; } set { _preMdReviewChecks = value; OnPropertyChanged("PreMdReviewChecks"); } }

        private ObservableCollection<PlanCheckBase> _treatmentPrepDosiChecks;           // Plan checks from Treatment Prep Questionnaire
        public ObservableCollection<PlanCheckBase> TreatmentPrepDosiChecks { get { return _treatmentPrepDosiChecks; } set { _treatmentPrepDosiChecks = value; OnPropertyChanged("TreatmentPrepDosiChecks"); } }

        private ObservableCollection<PlanCheckBase> _rxChecks;           // Plan checks from Treatment Prep Questionnaire
        public ObservableCollection<PlanCheckBase> RxChecks { get { return _rxChecks; } set { _rxChecks = value; OnPropertyChanged("RxChecks"); } }

        private ObservableCollection<PlanCheckBase> _photonChecks;           // Plan checks from Photon Plan Questionnaire
        public ObservableCollection<PlanCheckBase> PhotonChecks { get { return _photonChecks; } set { _photonChecks = value; OnPropertyChanged("PhotonChecks"); } }

        private ObservableCollection<PlanCheckBase> _treatmentPrepPhysicsChecks;           // Plan checks from Photon Plan Questionnaire
        public ObservableCollection<PlanCheckBase> TreatmentPrepPhysicsChecks { get { return _treatmentPrepPhysicsChecks; } set { _treatmentPrepPhysicsChecks = value; OnPropertyChanged("TreatmentPrepPhysicsChecks"); } }


        private string _patientName;                                                // Patient name
        public string PatientName { get { return _patientName; } set { _patientName = value; OnPropertyChanged("PatientName"); } }

        private string _planID;                                                     // Plan id
        public string PlanID { get { return _planID; } set { _planID = value; OnPropertyChanged("PlanID"); } }

        private string _courseID;                                                   // Course id
        public string CourseID { get { return _courseID; } set { _courseID = value; OnPropertyChanged("CourseID"); } }

        private string _slices;                                                     // Number of CT slices
        public string Slices { get { return _slices; } set { _slices = value; OnPropertyChanged("Slices"); } }

        private static readonly Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public ViewModel(ScriptContext context)
        {
            _context = context;

            PlanChecks = new ObservableCollection<PlanCheckBase>();
            CTImportChecks = new ObservableCollection<PlanCheckBase>();
            PreMdReviewChecks = new ObservableCollection<PlanCheckBase>();
            TreatmentPrepDosiChecks = new ObservableCollection<PlanCheckBase>();
            RxChecks = new ObservableCollection<PlanCheckBase>();
            PhotonChecks = new ObservableCollection<PlanCheckBase>();
            TreatmentPrepPhysicsChecks = new ObservableCollection<PlanCheckBase>();

            // Plan information
            PatientName = context.Patient.Name;
            CourseID = context.Course.Id;
            PlanID = context.PlanSetup.Id;
            Slices = context.Image.ZSize.ToString();

            //Log.Initialize(context);

            RunPlanChecks();

            //logger.Info("");

            //LogManager.Shutdown();
        }

        // Run all tests
        private void RunPlanChecks()
        {
            var plan = _context.PlanSetup;

            // CT Import Checklist
            CTImportChecks = new ObservableCollection<PlanCheckBase>
            {
                new BodyContour(plan),
                new CouchStructuresChecks(plan),
                new UserOrigin(plan),
                new StructureTemplateCheck(plan),
                new ImportNamingConventions(plan)
            };

            PreMdReviewChecks = new ObservableCollection<PlanCheckBase>
            {
                new CourseChecks(plan),
                // normal contours complete (no empty contours?)
                // rename/delete opti (don't think I can check)
                new MachineChecks(plan),
                new CouchStructuresChecks(plan),
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
                new JawTrackingChecks(plan)
            };

            TreatmentPrepDosiChecks = new ObservableCollection<PlanCheckBase>
            {
                new FieldNameChecks(plan),
                new ToleranceTableChecks(plan),
                // treatment time calculation
                new CouchValueChecks(plan),
                // setup notes (can't do)
                new Shifts(plan),
                new DRRChecks(plan),
                new UnapprovedPlans(plan)
            };

            RxChecks = new ObservableCollection<PlanCheckBase>
            {
                new PrecriptionChecks(plan)
            };

            PhotonChecks = new ObservableCollection<PlanCheckBase>
            {
                new PhotonDoseTabChecks(plan),
                new UserOrigin(plan),
                new CouchStructuresChecks(plan),
                // body contour?
                new FieldNameChecks(plan),
                new DensityOverrides(plan),
                new DoseGrid(plan),
                new PhotonCalcModelTabChecks(plan),
                new PhotonFieldTabChecks(plan),
                new HotspotChecks(plan)
                // DVH/scorecard
                // delta couch
            };

            TreatmentPrepPhysicsChecks = new ObservableCollection<PlanCheckBase>
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
            };

            PlanChecks = new ObservableCollection<PlanCheckBase>
            {
                // Run all plan checks
            };

            PlanChecks.Add(new MachineChecks(_context.PlanSetup));
            PlanChecks.Add(new DoseRateChecks(_context.PlanSetup));
            PlanChecks.Add(new CTSimChecks(_context.PlanSetup));
            PlanChecks.Add(new OrientationChecks(_context.PlanSetup));
            PlanChecks.Add(new PrecriptionChecks(_context.PlanSetup));
            PlanChecks.Add(new TargetChecks(_context.PlanSetup));
            PlanChecks.Add(new HotspotChecks(_context.PlanSetup));
            PlanChecks.Add(new PlanApprovalChecks(_context.PlanSetup));
            PlanChecks.Add(new IsocenterChecks(_context.PlanSetup));
            PlanChecks.Add(new FieldNameChecks(_context.PlanSetup));
            PlanChecks.Add(new JawTrackingChecks(_context.PlanSetup));
            PlanChecks.Add(new CouchStructuresChecks(_context.PlanSetup));
            PlanChecks.Add(new CouchValueChecks(_context.PlanSetup));
            PlanChecks.Add(new Shifts(_context.PlanSetup));
            PlanChecks.Add(new ToleranceTableChecks(_context.PlanSetup));
            PlanChecks.Add(new BolusChecks(_context.PlanSetup));
            PlanChecks.Add(new DRRChecks(_context.PlanSetup));
            PlanChecks.Add(new UseGatedChecks(_context.PlanSetup));
            PlanChecks.Add(new MLCChecks(_context.PlanSetup));
            PlanChecks.Add(new TreatmentTimeCalculation(_context.PlanSetup));
            PlanChecks.Add(new NamingConventionChecks(_context.PlanSetup));
            PlanChecks.Add(new CalcParametersChecks(_context.PlanSetup));


            // Remove any plan checks that were not run
            foreach (var p in PlanChecks.Where(x => x.MachineExempt).ToList())
                PlanChecks.Remove(p);
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
