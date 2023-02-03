using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanCheck
{
    public enum ResultColorChoices
    {
        Pass,
        Warn,
        Fail,
        TestNotRun
    }

    public static class DisplayColors
    {
        public static Dictionary<ResultColorChoices, string> ColorLookup = new Dictionary<ResultColorChoices, string>()
        {
            { ResultColorChoices.Pass, "LightGreen" },
            { ResultColorChoices.Warn, "Khaki" },
            { ResultColorChoices.Fail, "Salmon" },
            { ResultColorChoices.TestNotRun, "LightGray" }
        };
    }
}
