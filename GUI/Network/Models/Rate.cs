using System;
using System.Collections.Generic;

namespace GUI.Network.Models
{
    internal class Rate
    {
        public string BaseCurrency { get; set; }
        public DateTime Date { get; set; }
        public IList<SubRate> Rates { get; set; }
    }
}
