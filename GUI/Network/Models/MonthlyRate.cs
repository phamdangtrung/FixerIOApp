using System.Collections.Generic;

namespace GUI.Network.Models
{
    internal class MonthlyRate
    {
        public string BaseCurrency;
        public string TargetCurrency;
        public IList<SubMonthlyRate> SubRates;
    }
}
