using Microsoft.Extensions.Caching.Memory;
using Movies.Data.Configurations;
using Movies.Data.Modals;

namespace Movies.Data.Repositories.ActorRepository
{
    public class ActorRepository : IActorRepository
    {
        private readonly IMemoryCache _memoryCache;

        public ActorRepository(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;

            init();
        }

        private void init()
        {
            var actors = _memoryCache.Get<List<Actor>>(MemoryKeys.ActorMemoryKey) ?? new List<Actor>();

            if (actors.Count == 0)
            {
                actors = SeedData.SeedActors();

                _memoryCache.Set(MemoryKeys.ActorMemoryKey, actors);
            }
        }

        public void Delete(string id)
        {
            var actors = _memoryCache.Get<List<Actor>>(MemoryKeys.ActorMemoryKey) ?? new List<Actor>();

            var actorsToPreserve = actors?.Where(a => a.Id!.ToLower().Trim() != id.ToLower().Trim()).ToList();

            _memoryCache.Set(MemoryKeys.ActorMemoryKey, actorsToPreserve);
        }

        public IEnumerable<Actor> Get()
        {
            var actors = _memoryCache.Get<List<Actor>>(MemoryKeys.ActorMemoryKey) ?? new List<Actor>();

            return actors!;
        }

        public Actor Get(string id)
        {
            var actors = _memoryCache.Get<List<Actor>>(MemoryKeys.ActorMemoryKey) ?? new List<Actor>();

            var actor = actors?.Where(x => x.Id?.ToLower().Trim() == id.ToLower().Trim()).FirstOrDefault();

            return actor!;
        }

        public IEnumerable<Actor> Get(List<string> ids)
        {
            var actors = _memoryCache.Get<List<Actor>>(MemoryKeys.ActorMemoryKey) ?? new List<Actor>();

            var existingActors = actors?.Where(x => ids.Contains(x.Id!));

            return existingActors!;
        }

        public Actor Get(string firstName, string lastName)
        {
            var actors = _memoryCache.Get<List<Actor>>(MemoryKeys.ActorMemoryKey) ?? new List<Actor>();

            var actor = actors?.Where(x => x.FirstName?.ToLower().Trim() == firstName.ToLower().Trim() &&
            x.LastName?.ToLower().Trim() == lastName.ToLower().Trim()).FirstOrDefault();

            return actor!;
        }

        public void Post(Actor actor)
        {
            var actors = _memoryCache.Get<List<Actor>>(MemoryKeys.ActorMemoryKey) ?? new List<Actor>();

            actors.Add(actor);

            _memoryCache.Set(MemoryKeys.ActorMemoryKey, actors);
        }

        public void Put(Actor actor)
        {
            var actors = _memoryCache.Get<List<Actor>>(MemoryKeys.ActorMemoryKey) ?? new List<Actor>();

            if (actors.Count == 0)
                throw new ArgumentNullException(nameof(actors));

            foreach (var item in actors.Where(x => x.Id?.ToLower().Trim() == actor.Id?.ToLower().Trim()))
            {
                item.FirstName = actor.FirstName;
                item.LastName = actor.LastName;
                item.BirthDay = actor.BirthDay;
                item.Filmography = actor.Filmography;
            }

            _memoryCache.Set(MemoryKeys.ActorMemoryKey, actors);
        }

        public void LinkMovies(string actorId, List<string> movieIds)
        {
            var actors = _memoryCache.Get<List<Actor>>(MemoryKeys.ActorMemoryKey) ?? new List<Actor>();

            foreach (var item in actors.Where(x => x.Id?.ToLower().Trim() == actorId.ToLower().Trim()))
            {
                item.Filmography?.AddRange(movieIds);
            }

            _memoryCache.Set(MemoryKeys.ActorMemoryKey, actors);
        }
    }
}
