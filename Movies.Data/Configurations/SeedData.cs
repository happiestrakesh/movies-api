using Movies.Data.Modals;
using System.Collections.Generic;

namespace Movies.Data.Configurations
{
    internal static class MemoryKeys
    {
        public static string ActorMemoryKey = "actors";
        public static string MovieMemoryKey = "movies";
    }

    internal static class SeedData
    {
        internal static List<Movie> SeedMovies()
        {
            var movies = new List<Movie>();

            movies.Add(
                new Movie
                {
                    Id = "f8fb31ce-467a-4174-a799-ad2ec3b82384",
                    Genre = "Drama",
                    Title = "The Shawshank Redemption",
                    Year = "1994",
                    StarringActor = new List<string>
                    {
                        "4c3bb8c9-5c4a-4c96-979a-6c47d9a620cd"
                    }
                });

            return movies;
        }

        internal static List<Actor> SeedActors()
        {
            var actors = new List<Actor>();

            actors.Add(
                new Actor
                {
                    Id = "4c3bb8c9-5c4a-4c96-979a-6c47d9a620cd",
                    FirstName = "Tim",
                    LastName = "Robbins",
                    BirthDay = "10-16-1958",
                    Filmography = new List<string> { "f8fb31ce-467a-4174-a799-ad2ec3b82384" }
                });

            return actors;
        }
    }
}
