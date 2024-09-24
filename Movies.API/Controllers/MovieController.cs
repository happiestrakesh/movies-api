using Microsoft.AspNetCore.Mvc;
using Movies.API.Handler;
using Movies.Data.Modals;
using Movies.Data.Repositories.ActorRepository;
using Movies.Data.Repositories.MovieRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Movies.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MovieController : ControllerBase
    {
        private readonly ResilientOperationHandler _resilientOperationHandler;
        private readonly IMovieRepository _movieRepository;
        private readonly IActorRepository _actorRepository;

        public MovieController(ResilientOperationHandler resilientOperationHandler,
            IMovieRepository movieRepository, IActorRepository actorRepository)
        {
            _resilientOperationHandler = resilientOperationHandler;
            _movieRepository = movieRepository;
            _actorRepository = actorRepository;
        }

        // GET: api/<MovieController>
        [HttpGet("GetAll")]
        public IActionResult Get()
        {
            var movies = _resilientOperationHandler.ExecuteAsync(() =>
             {
                 var moviesTask = _movieRepository.Get();

                 return Task.FromResult(moviesTask);
             });

            return Ok(movies);
        }

        // GET api/<MovieController>/5
        [HttpGet("GetById/{id}")]
        public IActionResult Get(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return BadRequest(id);

            var movie = _movieRepository.Get(id.Trim());

            if (movie == null)
                return NotFound();

            return Ok(movie);
        }

        [HttpGet("GetByIds")]
        public IActionResult Get([FromQuery] List<string> ids)
        {
            ids = ids.Where(x => x != "string").ToList();

            if (ids is null || ids.Count() < 1)
                return BadRequest(ids);

            var movies = _movieRepository.Get(ids);

            if (movies == null)
                return NotFound();

            return Ok(movies);
        }

        // POST api/<MovieController>
        [HttpPost("Create")]
        public IActionResult Post([FromBody] Movie movie)
        {
            if (!ModelState.IsValid)
                return BadRequest(movie);

            movie.Title = (movie.Title is null || movie.Title == "string") ? string.Empty : movie.Title.Trim();

            var validationStatus = TryValidateModel(movie);

            if (!validationStatus)
                return BadRequest(ModelState);

            var doesMovieExist = _movieRepository.GetByName(movie.Title);

            if (doesMovieExist != null)
                return BadRequest(movie);

            movie.Id = (movie.Id is null || movie.Id == "string") ? Guid.NewGuid().ToString() : movie.Id.Trim();

            movie.StarringActor = movie.StarringActor.Where(x => x != "string").ToList();

            _movieRepository.Post(movie);

            return Created($"api/movie/{movie.Id}", movie);
        }

        // PUT api/<MovieController>/5
        [HttpPut("Update/{id}")]
        public IActionResult Put(string id, [FromBody] Movie movie)
        {
            if (string.IsNullOrWhiteSpace(id) || !ModelState.IsValid || id.ToLower().Trim() != movie.Id.ToLower().Trim())
            {
                return BadRequest(ModelState);
            }

            movie.Title = (movie.Title is null || movie.Title == "string") ? string.Empty : movie.Title.Trim();

            var validationStatus = TryValidateModel(movie);

            if (!validationStatus)
                return BadRequest(ModelState);

            var doesMovieExist = _movieRepository.Get(id.Trim());

            if (doesMovieExist == null)
                return BadRequest(id);

            if (movie.Id is null || movie.Id == "string")
                movie.Id = doesMovieExist.Id;

            if (movie.Genre is null || movie.Genre == "string")
                movie.Genre = doesMovieExist.Genre;

            movie.StarringActor = movie.StarringActor.Where(x => x != "string").ToList();

            if (movie.StarringActor is null || movie.StarringActor.Count == 0)
                movie.StarringActor = doesMovieExist.StarringActor;

            _movieRepository.Put(movie);

            return Ok(movie);
        }

        // HttpPatch api/<MovieController>/5
        [HttpPatch("LinkActors/{id}")]
        public IActionResult LinkActors(string id, [FromBody] List<string> actorIds)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return BadRequest(id);
            }

            if (actorIds.Count() < 1)
            {
                return BadRequest(actorIds);
            }

            var doesMovieExist = _movieRepository.Get(id.Trim());

            if (doesMovieExist == null)
                return BadRequest(id);

            var validActorIds = _actorRepository.Get(actorIds);

            if (validActorIds == null)
                return BadRequest(validActorIds);

            _movieRepository.LinkActors(id.Trim(), validActorIds.Select(x => x.Id).ToList());

            return Ok(validActorIds);
        }


        // DELETE api/<MovieController>/5
        [HttpDelete("Delete/{id}")]
        public IActionResult Delete(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return BadRequest(id);
            }

            var movie = _movieRepository.Get(id.Trim());

            if (movie == null)
                return NotFound(id);

            _movieRepository.Delete(id.Trim());

            return Ok();
        }

        /// <summary>
        /// Test error with global exception
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        [HttpGet("error")]
        public IActionResult GenerateError()
        {
            throw new Exception("Test exception");
        }
    }
}
