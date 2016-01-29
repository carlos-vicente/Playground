using System.Collections.Generic;
using System.Threading.Tasks;
using Playground.Web.Services.Contracts.Entities;

namespace Playground.Web.Services.Contracts
{
    public interface IUserService
    {
        Task<Profile> GetProfile(string userId);

        Task<IEnumerable<Measurement>> GetMeasurements(string userId, MeasurementTypes type);
    }
}