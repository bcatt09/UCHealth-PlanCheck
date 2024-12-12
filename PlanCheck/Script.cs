using System;
using System.Linq;
using System.Text;
using System.Windows;
using System.Collections.Generic;
using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;
using System.Windows.Input;
using PlanCheck;
using PVH_Log;
using System.Reflection;

namespace VMS.TPS
{
	public class Script
	{
        private static PVH_Logger instance;

        private static string logName = "PVH_Scripts";
        private static string logFilename = logName + ".log";
        private static string logRelativePath = $@"PVH\{logFilename}";
        private string logFullPath;

        private static string oldLogFilename = logName + ".old.log";
        private static string oldLogRelativePath = $@"PVH\{oldLogFilename}";
        private string oldLogFullPath;

        private string scriptName;
        private string userName;
        private string patientName;
        private string planOrImageName;

        public void Initialize(string scriptName, ScriptContext context, string exePath)
        {
            this.scriptName = scriptName;
            userName = context.CurrentUser.Name;
            patientName = $"{context.Patient.LastName}, {context.Patient.FirstName} ({context.Patient.Id})";
            planOrImageName = context.PlanSetup != null ? $"{context.PlanSetup.Id} ({context.Course})" : context.Image.Id;

            logFullPath = System.IO.Path.Combine(exePath, logRelativePath);
            oldLogFullPath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), oldLogRelativePath);

            // Clear the log every day and save yesterday's log in case there were errors that need to be looked into
            
                System.IO.File.Copy(logFullPath, oldLogFullPath, true);
                System.IO.File.Delete(logFullPath);
        }
        public void Execute(ScriptContext context)
		{
            //Initialize("test", context, System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
            var window = new MainWindow();
            window.KeyDown += (object sender, KeyEventArgs e) => { if (e.Key == Key.Escape) window.Close(); };

            if (context.StructureSet == null)
            {
                MessageBox.Show("Image must have a Structure Set before running this script", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                throw new ApplicationException("Image must have a Structure Set before running this script", new NullReferenceException());
            }
            window.SizeToContent = SizeToContent.WidthAndHeight;
            window.MaxHeight = SystemParameters.WorkArea.Height * 0.95;
            window.MaxWidth = SystemParameters.WorkArea.Width * 0.95;
            window.Title = $"PVH - Plan Check";

            ViewModel viewModel = new ViewModel(context);
            window.DataContext = viewModel;

            window.ShowDialog();
        }
	}
}

