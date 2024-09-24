using Movies.Driver.SqlServer.Modals;

namespace Movies.Driver.SqlServer.Repositories
{
    public class ActorRepository : IActorRepository
    {
        private readonly MovieDbContext _movieDbContext;

        public ActorRepository(MovieDbContext movieDbContext)
        {
            _movieDbContext = movieDbContext;
        }

        public void Delete(string id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Actor> Get()
        {
            throw new NotImplementedException();
        }

        public Actor Get(string id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Actor> Get(List<string> ids)
        {
            throw new NotImplementedException();
        }

        public Actor Get(string firstName, string lastName)
        {
            throw new NotImplementedException();
        }

        public void LinkMovies(string actorId, List<string> movieIds)
        {
            throw new NotImplementedException();
        }

        public void Post(Actor actor)
        {
            throw new NotImplementedException();
        }

        public void Put(Actor actor)
        {
            throw new NotImplementedException();
        }
    }
}
