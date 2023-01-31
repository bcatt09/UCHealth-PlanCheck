using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PlanCheck
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : MetroWindow
	{
		public int prevSelectedRow = -1;

		public MainWindow()
		{
			InitializeComponent();

            CTImportGrid.MaxHeight = SystemParameters.WorkArea.Height * 0.7;
            CTImportGrid.MaxWidth = SystemParameters.WorkArea.Width * 0.4;

            PreMDGrid.MaxHeight = SystemParameters.WorkArea.Height * 0.7;
            PreMDGrid.MaxWidth = SystemParameters.WorkArea.Width * 0.4;

            TreatmentPrepDosiGrid.MaxHeight = SystemParameters.WorkArea.Height * 0.7;
            TreatmentPrepDosiGrid.MaxWidth = SystemParameters.WorkArea.Width * 0.4;

            RxChecksGrid.MaxHeight = SystemParameters.WorkArea.Height * 0.7;
            RxChecksGrid.MaxWidth = SystemParameters.WorkArea.Width * 0.4;

            PhotonPlanGrid.MaxHeight = SystemParameters.WorkArea.Height * 0.7;
            PhotonPlanGrid.MaxWidth = SystemParameters.WorkArea.Width * 0.4;

            TreatmentPrepPhysicsGrid.MaxHeight = SystemParameters.WorkArea.Height * 0.7;
            TreatmentPrepPhysicsGrid.MaxWidth = SystemParameters.WorkArea.Width * 0.4;

            OldCheckGrid.MaxHeight = SystemParameters.WorkArea.Height * 0.7;
            OldCheckGrid.MaxWidth = SystemParameters.WorkArea.Width * 0.4;
        }

		// toggle the row details when clicking on the same row
		private void dataGridMouseLeftButton(object sender, MouseButtonEventArgs e)
		{
			DataGrid dg = sender as DataGrid;
			if (dg == null)
				return;

			if (dg.SelectedIndex == prevSelectedRow)
            {
                CTImportGrid.RowDetailsVisibilityMode = DataGridRowDetailsVisibilityMode.Collapsed;
                PreMDGrid.RowDetailsVisibilityMode = DataGridRowDetailsVisibilityMode.Collapsed;
                TreatmentPrepDosiGrid.RowDetailsVisibilityMode = DataGridRowDetailsVisibilityMode.Collapsed;
                RxChecksGrid.RowDetailsVisibilityMode = DataGridRowDetailsVisibilityMode.Collapsed;
                PhotonPlanGrid.RowDetailsVisibilityMode = DataGridRowDetailsVisibilityMode.Collapsed;
                TreatmentPrepPhysicsGrid.RowDetailsVisibilityMode = DataGridRowDetailsVisibilityMode.Collapsed;
                OldCheckGrid.RowDetailsVisibilityMode = DataGridRowDetailsVisibilityMode.Collapsed;
                dg.SelectedIndex = -1;
				prevSelectedRow = -1;
			}
			else
            {
                CTImportGrid.RowDetailsVisibilityMode = DataGridRowDetailsVisibilityMode.VisibleWhenSelected;
                PreMDGrid.RowDetailsVisibilityMode = DataGridRowDetailsVisibilityMode.VisibleWhenSelected;
                TreatmentPrepDosiGrid.RowDetailsVisibilityMode = DataGridRowDetailsVisibilityMode.VisibleWhenSelected;
                RxChecksGrid.RowDetailsVisibilityMode = DataGridRowDetailsVisibilityMode.VisibleWhenSelected;
                PhotonPlanGrid.RowDetailsVisibilityMode = DataGridRowDetailsVisibilityMode.VisibleWhenSelected;
                TreatmentPrepPhysicsGrid.RowDetailsVisibilityMode = DataGridRowDetailsVisibilityMode.VisibleWhenSelected;
                OldCheckGrid.RowDetailsVisibilityMode = DataGridRowDetailsVisibilityMode.VisibleWhenSelected;
                prevSelectedRow = dg.SelectedIndex;
			}
		}
    }
}
