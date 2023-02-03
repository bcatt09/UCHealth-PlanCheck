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
        public ObservableCollection<PlanCheckBase> CheckList { get; protected set; }
        public string Header { get; set; }
        public string TabColor { get; protected set; }

        public CategoryCheckList(string header, ObservableCollection<PlanCheckBase> checkList)
        {
            Header = header;
            CheckList = checkList;

            // Remove any plan checks that were not run
            foreach (var check in CheckList.Where(x => x.MachineExempt).ToList())
                CheckList.Remove(check);

            foreach (var check in CheckList)
            {
                if (TabColor == DisplayColors.ColorLookup[ResultColorChoices.Fail]) { }
                else if (TabColor == DisplayColors.ColorLookup[ResultColorChoices.Warn])
                {
                    if (check.ResultColor == DisplayColors.ColorLookup[ResultColorChoices.Fail])
                        TabColor = check.ResultColor;
                }
                else
                {
                    if (check.ResultColor == DisplayColors.ColorLookup[ResultColorChoices.Warn])
                        TabColor = check.ResultColor;
                    if (check.ResultColor == DisplayColors.ColorLookup[ResultColorChoices.Fail])
                        TabColor = check.ResultColor;
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
