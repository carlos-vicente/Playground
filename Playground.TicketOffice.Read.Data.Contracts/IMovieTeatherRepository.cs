using System.Threading.Tasks;
using Playground.TicketOffice.Read.Data.Contracts.Model;

namespace Playground.TicketOffice.Read.Data.Contracts
{
    public interface IMovieTeatherRepository
    {
        Task CreateMovieThreater(MovieTheater movieTheater);
    }
}