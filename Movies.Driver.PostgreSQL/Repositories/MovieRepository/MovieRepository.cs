using Movies.Driver.PostgreSQL.Modals;

namespace Movies.Driver.PostgreSQL.Repositories
{
    public class MovieRepository : IMovieRepository
    {
        private readonly MovieDbContext _movieDbContext;

        public MovieRepository(MovieDbContext movieDbContext)
        {
            _movieDbContext = movieDbContext;
        }

        public void Delete(string id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Movie> Get()
        {
            throw new NotImplementedException();
        }

        public Movie Get(string id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Movie> Get(List<string> ids)
        {
            throw new NotImplementedException();
        }

        public Movie GetByName(string title)
        {
            throw new NotImplementedException();
        }

        public void LinkActors(string movieId, List<string> actorIds)
        {
            throw new NotImplementedException();
        }

        public void Post(Movie movie)
        {
            throw new NotImplementedException();
        }

        public void Put(Movie movie)
        {
            throw new NotImplementedException();
        }
    }
}
