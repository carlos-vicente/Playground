namespace Playground.TicketOffice.Api.Theater.Contracts
{
    public static class Routes
    {
        private const string RoutePrefix = "theaters";

        public const string GetAllMovieTheaters = RoutePrefix + "/";
        public const string CreateNewMovieTheater = RoutePrefix + "/";
        public const string GetMovieTheaterById = RoutePrefix + "/{theaterId:guid}";
        public const string GetMovieTheaterMovies = RoutePrefix + "/{theaterId:guid}/movies";
        public const string GetMovieTheaterRooms = RoutePrefix + "/{theaterId:guid}/rooms";
    }
}