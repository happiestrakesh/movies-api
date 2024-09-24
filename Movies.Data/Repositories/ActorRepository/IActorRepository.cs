using Movies.Data.Modals;
using System;
using System.Collections.Generic;
using System.Text;

namespace Movies.Data.Repositories.ActorRepository
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
