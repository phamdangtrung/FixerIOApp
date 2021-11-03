using System.Collections.Generic;

namespace GUI.Network.API.Models
{
    internal class APIRate
    {
        public string Timestamp { get; set; }
        public string Base { get; set; }
        public string Date { get; set; }
        public Dictionary<string, decimal> Rates { get; set; }
    }
}
