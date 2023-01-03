using System;
using System.Linq;
using System.Text;
using System.Windows;
using System.Collections.Generic;
using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;
using System.Windows.Input;
using PlanCheck;

namespace VMS.TPS
{
	public class Script
	{
		public void Execute(ScriptContext context)
		{
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

