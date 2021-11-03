using GUI.Network.Models;
using System.Threading.Tasks;

namespace GUI.Network.Services
{
    internal interface IRateService
    {
        Task<Rate> GetTodayRate(string countryCode);
    }
}
