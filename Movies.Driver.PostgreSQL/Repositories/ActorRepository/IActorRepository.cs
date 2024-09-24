﻿using Movies.Driver.PostgreSQL.Modals;

namespace Movies.Driver.PostgreSQL.Repositories
{
    public interface IActorRepository
    {
        IEnumerable<Actor> Get();
        Actor Get(string id);
        IEnumerable<Actor> Get(List<string> ids);
        Actor Get(string firstName, string lastName);
        void Post(Actor actor);
        void Put(Actor actor);
        void LinkMovies(string actorId, List<string> movieIds);
        void Delete(string id);
    }
}
