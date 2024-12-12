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

            CategoriesTabControl.MaxHeight = SystemParameters.WorkArea.Height * 0.7;
            CategoriesTabControl.MaxWidth = SystemParameters.WorkArea.Width * 0.4;
        }

        // toggle the row details when clicking on the same row
        private void dataGridMouseLeftButton(object sender, MouseButtonEventArgs e)
		{
			PlanCheckDataGrid dg = sender as PlanCheckDataGrid;
			if (dg == null)
				return;

			if (dg.SelectedIndex == prevSelectedRow)
            {
                dg.RowDetailsVisibilityMode = DataGridRowDetailsVisibilityMode.Collapsed;
                dg.SelectedIndex = -1;
				prevSelectedRow = -1;
			}
			else
            {
                dg.RowDetailsVisibilityMode = DataGridRowDetailsVisibilityMode.VisibleWhenSelected;
                prevSelectedRow = dg.SelectedIndex;
			}
		}
    }
}
