using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Playground.TicketOffice.Theater.Domain;

namespace Playground.TicketOffice.Theater.Data.Contracts
{
    public interface IMovieTheaterRepository
    {
        Task Create(MovieTheater movieTheater);

        Task<IEnumerable<MovieTheater>> GetAll();

        Task<MovieTheater> GetById(Guid id);
    }
}