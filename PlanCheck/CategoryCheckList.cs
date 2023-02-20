using PlanCheck.Checks;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanCheck
{
    public class CategoryCheckList : INotifyPropertyChanged
    {
        public ObservableCollection<BaseClass.PlanCheckBase> CheckList { get; protected set; }
        public string Header { get; set; }
        public string TabColor { get; protected set; }

        public CategoryCheckList(string header, ObservableCollection<BaseClass.PlanCheckBase> checkList)
        {
            Header = header;
            CheckList = checkList;

            // Remove any plan checks that were not run
            foreach (var check in CheckList.Where(x => x.MachineExempt).ToList())
                CheckList.Remove(check);


            // Set tab color based on highest degree of failure out of the tests run
            TabColor = DisplayColors.ColorLookup[ResultColorChoices.Pass];
            foreach (var check in CheckList)
            {
                if (TabColor == DisplayColors.ColorLookup[ResultColorChoices.Fail]) { }
                else if (TabColor == DisplayColors.ColorLookup[ResultColorChoices.Warn])
                {
                    if (check.DisplayColor == DisplayColors.ColorLookup[ResultColorChoices.Fail])
                        TabColor = check.DisplayColor;
                }
                else
                {
                    if (check.DisplayColor == DisplayColors.ColorLookup[ResultColorChoices.Warn])
                        TabColor = check.DisplayColor;
                    if (check.DisplayColor == DisplayColors.ColorLookup[ResultColorChoices.Fail])
                        TabColor = check.DisplayColor;
                }
            }
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
