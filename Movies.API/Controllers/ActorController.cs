using Microsoft.AspNetCore.Mvc;
using Movies.API.Handler;
using Movies.Data.Modals;
using Movies.Data.Repositories.ActorRepository;
using Movies.Data.Repositories.MovieRepository;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Movies.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ActorController : ControllerBase
    {
        private readonly ResilientOperationHandler _resilientOperationHandler;
        private readonly IActorRepository _actorRepository;
        private readonly IMovieRepository _movieRepository;

        public ActorController(ResilientOperationHandler resilientOperationHandler,
            IActorRepository actorRepository,
            IMovieRepository movieRepository)
        {
            _resilientOperationHandler = resilientOperationHandler;
            _actorRepository = actorRepository;
            _movieRepository = movieRepository;
        }

        // GET: api/<ActorController>
        [HttpGet("GetAll")]
        public IActionResult Get()
        {
            var actors = _actorRepository.Get();

            return Ok(actors);
        }

        // GET api/<ActorController>/5
        /// <summary>
        /// Retrieves a specific product by unique id
        /// </summary>
        /// <remarks>Awesomeness!</remarks>
        /// <response code="200">Product created</response>
        /// <response code="404">Product has missing/invalid values</response>
        /// <response code="500">Oops! Can't create your product right now</response>
        [ProducesResponseType(typeof(Actor), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        [HttpGet("GetById/{id}")]
        public IActionResult Get(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return BadRequest(id);

            var actor = _actorRepository.Get(id.Trim());

            if (actor == null)
                return NotFound();

            return Ok(actor);
        }

        // GET api/<ActorController>/5
        [HttpGet("GetByIds")]
        public IActionResult Get([FromQuery] List<string> ids)
        {
            ids = ids.Where(x => x != "string").ToList();

            if (ids is null || ids.Count() < 1)
                return BadRequest(ids);

            var actors = _actorRepository.Get(ids);

            if (actors == null)
                return NotFound();

            return Ok(actors);
        }

        // POST api/<ActorController>
        [HttpPost("Create")]
        public IActionResult Post([FromBody] Actor actor)
        {
            if (!ModelState.IsValid)
                return BadRequest(actor);

            actor.FirstName = (actor.FirstName is null || actor.FirstName == "string") ? string.Empty : actor.FirstName.Trim();

            actor.LastName = (actor.LastName is null || actor.LastName == "string") ? string.Empty : actor.LastName.Trim();

            var validationStatus = TryValidateModel(actor);

            if (!validationStatus)
                return BadRequest(ModelState);

            var doesActorExist = _actorRepository.Get(actor.FirstName.Trim(), actor.LastName.Trim());

            if (doesActorExist != null)
                return BadRequest(actor);

            actor.Id = (actor.Id is null || actor.Id == "string") ? Guid.NewGuid().ToString() : actor.Id.Trim();

            actor.Filmography = actor.Filmography.Where(x => x != "string").ToList();

            _actorRepository.Post(actor);

            return Created($"api/actor/{actor.Id}", actor);
        }

        // PUT api/<ActorController>/5
        [HttpPut("Update/{id}")]
        public IActionResult Put(string id, [FromBody] Actor actor)
        {
            if (string.IsNullOrWhiteSpace(id) || !ModelState.IsValid || id.ToLower().Trim() != actor.Id.ToLower().Trim())
            {
                return BadRequest(ModelState);
            }

            actor.FirstName = (actor.FirstName is null || actor.FirstName == "string") ? string.Empty : actor.FirstName.Trim();

            actor.LastName = (actor.LastName is null || actor.LastName == "string") ? string.Empty : actor.LastName.Trim();

            var validationStatus = TryValidateModel(actor);

            if (!validationStatus)
                return BadRequest(ModelState);

            var doesActorExist = _actorRepository.Get(id.Trim());

            if (doesActorExist == null)
                return BadRequest(id);

            if (actor.Id is null || actor.Id == "string")
                actor.Id = doesActorExist.Id;

            if (actor.BirthDay is null || actor.BirthDay == "string")
                actor.BirthDay = doesActorExist.BirthDay;

            actor.Filmography = actor.Filmography.Where(x => x != "string").ToList();

            if (actor.Filmography is null || actor.Filmography.Count == 0)
                actor.Filmography = doesActorExist.Filmography;

            _actorRepository.Put(actor);

            return Ok(actor);
        }

        // HttpPatch api/<ActorController>/5
        [HttpPatch("LinkMovies/{id}")]
        public IActionResult LinkMovies(string id, [FromBody] List<string> movieIds)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return BadRequest(id);
            }

            if (movieIds.Count() < 1)
            {
                return BadRequest(movieIds);
            }

            var doesActorExist = _actorRepository.Get(id.Trim());

            if (doesActorExist == null)
                return BadRequest(id);

            var validMovieIds = _movieRepository.Get(movieIds);

            if (validMovieIds == null)
                return BadRequest(movieIds);

            _actorRepository.LinkMovies(id.Trim(), validMovieIds.Select(x => x.Id).ToList());

            return Ok(validMovieIds);
        }

        // DELETE api/<ActorController>/5
        [HttpDelete("Delete/{id}")]
        public IActionResult Delete(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return BadRequest(id);
            }

            var actor = _actorRepository.Get(id.Trim());

            if (actor == null)
                return NotFound(id);

            _actorRepository.Delete(id.Trim());

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
