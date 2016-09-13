using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Playground.Data.Contracts;
using Playground.TicketOffice.Theater.Data.Contracts;
using Playground.TicketOffice.Theater.Domain;

namespace Playground.TicketOffice.Theater.Data
{
    public class MovieTheaterRepository : IMovieTheaterRepository
    {
        private readonly IConnectionFactory _connectionFactory;

        public MovieTheaterRepository(IConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task Create(MovieTheater movieTheater)
        {
            if (movieTheater == null)
                throw new ArgumentNullException(nameof(movieTheater));

            using (var connection = _connectionFactory.CreateConnection())
            {
                await connection
                    .ExecuteCommand(
                        "INSERT INTO [MovieTheater].[Theater](Id, Name) VALUES (@Id, @Name)",
                        movieTheater)
                    .ConfigureAwait(false);
            }
        }

        public async Task<IEnumerable<MovieTheater>> GetAll()
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var theaters = await connection
                    .ExecuteQueryMultiple<dynamic>(
                        "SELECT Id, Name FROM [MovieTheater].[Theater]",
                        new {})
                    .ConfigureAwait(false);

                return theaters
                    ?.Select(mt => new MovieTheater(mt.Id, mt.Name))
                       ?? new List<MovieTheater>();
            }
        }

        public async Task<MovieTheater> GetById(Guid id)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var mt = await connection
                    .ExecuteQuerySingle<dynamic>(
                        "SELECT Id, Name FROM [MovieTheater].[Theater] WHERE Id = @Id",
                        new { Id = id })
                    .ConfigureAwait(false);

                return mt == null 
                    ? null 
                    : new MovieTheater(mt.Id, mt.Name);
            }
        }
    }
}