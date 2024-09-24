using Microsoft.Extensions.Caching.Memory;
using Movies.Data.Configurations;
using Movies.Data.Modals;

namespace Movies.Data.Repositories.MovieRepository
{
    public class MovieRepository : IMovieRepository
    {
        private readonly IMemoryCache _memoryCache;

        public MovieRepository(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;

            init();
        }

        private void init()
        {
            var movies = _memoryCache.Get<List<Movie>>(MemoryKeys.MovieMemoryKey) ?? new List<Movie>();

            if (movies.Count == 0)
            {
                movies = SeedData.SeedMovies();

                _memoryCache.Set(MemoryKeys.MovieMemoryKey, movies);
            }
        }

        public void Delete(string id)
        {
            var movies = _memoryCache.Get<List<Movie>>(MemoryKeys.MovieMemoryKey) ?? new List<Movie>();

            var moviesToPreserve = movies.Where(a => a.Id?.ToLower().Trim() != id.ToLower().Trim()).ToList();

            _memoryCache.Set(MemoryKeys.MovieMemoryKey, moviesToPreserve);
        }

        public IEnumerable<Movie> Get()
        {
            var movies = _memoryCache.Get<List<Movie>>(MemoryKeys.MovieMemoryKey) ?? new List<Movie>();

            return movies;
        }

        public Movie Get(string id)
        {
            var movies = _memoryCache.Get<List<Movie>>(MemoryKeys.MovieMemoryKey) ?? new List<Movie>();

            var movie = movies.Where(x => x.Id?.ToLower().Trim() == id.ToLower().Trim()).FirstOrDefault();

            return movie!;
        }

        public IEnumerable<Movie> Get(List<string> ids)
        {
            var movies = _memoryCache.Get<List<Movie>>(MemoryKeys.MovieMemoryKey) ?? new List<Movie>();

            return movies.Where(x => ids.Contains(x.Id!));
        }

        public Movie GetByName(string title)
        {
            var movies = _memoryCache.Get<List<Movie>>(MemoryKeys.MovieMemoryKey) ?? new List<Movie>();

            var movie = movies.Where(x => x.Title?.ToLower().Trim() == title.ToLower().Trim()).FirstOrDefault();

            return movie!;
        }

        public void Post(Movie movie)
        {
            var movies = _memoryCache.Get<List<Movie>>(MemoryKeys.MovieMemoryKey) ?? new List<Movie>();

            movies.Add(movie);

            _memoryCache.Set(MemoryKeys.MovieMemoryKey, movies);
        }

        public void Put(Movie movie)
        {
            var movies = _memoryCache.Get<List<Movie>>(MemoryKeys.MovieMemoryKey) ?? new List<Movie>();

            foreach (var item in movies.Where(x => x.Id?.ToLower().Trim() == movie.Id?.ToLower().Trim()))
            {
                item.Year = movie.Year;
                item.Title = movie.Title;
                item.StarringActor = movie.StarringActor;
                item.Genre = movie.Genre;
            }

            _memoryCache.Set(MemoryKeys.MovieMemoryKey, movies);
        }

        public void LinkActors(string movieId, List<string> actorIds)
        {
            var movies = _memoryCache.Get<List<Movie>>(MemoryKeys.MovieMemoryKey) ?? new List<Movie>();

            foreach (var item in movies.Where(x => x.Id?.ToLower().Trim() == movieId.ToLower().Trim()))
            {
                item.StarringActor?.AddRange(actorIds);
            }

            _memoryCache.Set(MemoryKeys.MovieMemoryKey, movies);
        }
    }
}
