using System;
using System.Collections.Generic;
using System.Text;
using Movies.Data.Modals;

namespace Movies.Data.Repositories.MovieRepository
{
    public interface IMovieRepository
    {
        IEnumerable<Movie> Get();
        Movie Get(string id);
        IEnumerable<Movie> Get(List<string> ids);
        Movie GetByName(string title);
        void Post(Movie movie);
        void Put(Movie movie);
        void Delete(string id);
        void LinkActors(string movieId, List<string> actorIds);
    }
}
