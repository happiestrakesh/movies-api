using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using Movies.API.Controllers;
using Movies.API.Handler;
using Movies.Data.Attributes;
using Movies.Data.Modals;
using Movies.Data.Repositories.ActorRepository;
using Movies.Data.Repositories.MovieRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Xunit;

namespace Movies.API.Test
{
    public class APITest
    {
        private readonly ResilientOperationHandler _resilientOperationHandler;
        private readonly IMemoryCache _memoryCache;
        private readonly IActorRepository _actorService;
        private readonly IMovieRepository _movieService;
        private readonly ActorController _actorController;
        private readonly MovieController _movieController;

        public APITest()
        {
            _resilientOperationHandler = new ResilientOperationHandler();

            _memoryCache = new MemoryCache(new MemoryCacheOptions());

            _actorService = new ActorRepository(_memoryCache);
            _movieService = new MovieRepository(_memoryCache);

            _actorController = new ActorController(_resilientOperationHandler, _actorService, _movieService);
            _movieController = new MovieController(_resilientOperationHandler, _movieService, _actorService);
        }

        private Actor GetSampleActor()
        {
            return new Actor
            {
                Id = Guid.NewGuid().ToString(),
                FirstName = "Tim" + Guid.NewGuid().ToString(),
                LastName = "Robbins" + Guid.NewGuid().ToString(),
                BirthDay = "10-16-1958",
                Filmography = new List<string> { }
            };
        }

        private Movie GetSampleMovie()
        {
            return new Movie
            {
                Id = Guid.NewGuid().ToString(),
                Genre = "Drama",
                Title = Guid.NewGuid().ToString(),
                Year = "1994",
                StarringActor = new List<string> { }
            };
        }

        #region Actor

        [Fact]
        public void GetAllActors()
        {
            // Arrange

            // Act            
            IActionResult actionResult = _actorController.Get();

            // Assert
            var okObjectResult = actionResult as OkObjectResult;
            var resultModels = (List<Actor>)okObjectResult.Value;

            Assert.NotNull(okObjectResult);
            Assert.NotNull(resultModels);
            Assert.Equal(200, okObjectResult.StatusCode.Value);
            Assert.Single(resultModels);
            Assert.NotNull(resultModels.Single().Id);
            Assert.NotNull(resultModels.Single().FirstName);
            Assert.NotNull(resultModels.Single().LastName);
        }

        [Fact]
        public void GetActorById()
        {
            // Arrange
            IActionResult actionExistingResult = _actorController.Get();
            var okObjectExistingResult = actionExistingResult as OkObjectResult;
            var resultExistingModels = (List<Actor>)okObjectExistingResult.Value;
            var existingModel = resultExistingModels.Single();

            // Act            
            IActionResult actionResult = _actorController.Get(existingModel.Id);

            // Assert
            var okObjectResult = actionResult as OkObjectResult;
            var resultModels = (Actor)okObjectResult.Value;

            Assert.NotNull(okObjectResult);
            Assert.Equal(resultModels, existingModel);
            Assert.Equal(200, okObjectResult.StatusCode.Value);

            Assert.NotNull(resultModels.Id);
            Assert.NotNull(resultModels.FirstName);
            Assert.NotNull(resultModels.LastName);
        }

        [Fact]
        public void GetActorByIds()
        {
            // Arrange
            IActionResult actionExistingResult = _actorController.Get();
            var okObjectExistingResult = actionExistingResult as OkObjectResult;
            var resultExistingModels = (List<Actor>)okObjectExistingResult.Value;
            var actorIds = resultExistingModels.Select(x => x.Id).ToList();

            // Act            
            IActionResult actionResult = _actorController.Get(actorIds);

            // Assert
            var okObjectResult = actionResult as OkObjectResult;
            var resultModels = (IEnumerable<Actor>)okObjectResult.Value;

            Assert.NotNull(okObjectResult);
            Assert.True(okObjectResult is OkObjectResult);
            Assert.Equal(200, okObjectResult.StatusCode.Value);
            Assert.Equal(resultExistingModels.Count, resultModels.Count());
        }

        [Fact]
        public void PostActor()
        {
            // Arrange
            var actorModel = GetSampleActor();

            // Act
            var objectValidator = new Mock<IObjectModelValidator>();
            objectValidator.Setup(o => o.Validate(It.IsAny<ActionContext>(),
                                                         It.IsAny<ValidationStateDictionary>(),
                                                         It.IsAny<string>(),
                                                         It.IsAny<object>()));

            _actorController.ObjectValidator = objectValidator.Object;

            IActionResult actionResult = _actorController.Post(actorModel);

            // Assert
            var createdResult = actionResult as CreatedResult;

            Assert.NotNull(createdResult);
            Assert.Equal(201, createdResult.StatusCode.Value);
            Assert.NotEmpty(createdResult.Location);
            Assert.Equal($"api/actor/{actorModel.Id}", createdResult.Location);
        }

        [Fact]
        public void PutActor()
        {
            // Arrange
            IActionResult actionExistingResult = _actorController.Get();
            var okObjectExistingResult = actionExistingResult as OkObjectResult;
            var resultExistingModels = (List<Actor>)okObjectExistingResult.Value;
            var actorModel = resultExistingModels.FirstOrDefault();

            // Act
            var objectValidator = new Mock<IObjectModelValidator>();
            objectValidator.Setup(o => o.Validate(It.IsAny<ActionContext>(),
                                                         It.IsAny<ValidationStateDictionary>(),
                                                         It.IsAny<string>(),
                                                         It.IsAny<object>()));

            _actorController.ObjectValidator = objectValidator.Object;

            actorModel.FirstName = Guid.NewGuid().ToString();

            IActionResult actionResult = _actorController.Put(actorModel.Id, actorModel);

            // Assert
            var okObjectResult = actionResult as OkObjectResult;
            var resultModel = (Actor)okObjectResult.Value;

            Assert.NotNull(okObjectResult);
            Assert.Equal(200, okObjectResult.StatusCode.Value);

            Assert.Equal(actorModel.Id, resultModel.Id);
            Assert.Equal(actorModel.FirstName, resultModel.FirstName);
            Assert.Equal(actorModel.LastName, resultModel.LastName);
            Assert.Equal(actorModel.BirthDay, resultModel.BirthDay);
            Assert.Equal(actorModel.Filmography, resultModel.Filmography);
        }

        [Fact]
        public void DeleteActor()
        {
            // Arrange
            IActionResult actionResultForExisting = _actorController.Get();
            var okObjectResultForExisting = actionResultForExisting as OkObjectResult;
            var resultModelsExisting = (List<Actor>)okObjectResultForExisting.Value;
            var existingModel = resultModelsExisting.Single();

            // Act            
            IActionResult actionResult = _actorController.Delete(existingModel.Id);

            // Assert
            var okResult = actionResult as OkResult;

            Assert.NotNull(okResult);
            Assert.Equal(200, okResult.StatusCode);

            IActionResult actionResultCheckIfExist = _actorController.Get(existingModel.Id);
            var notFoundResult = actionResultCheckIfExist as NotFoundResult;

            Assert.NotNull(notFoundResult);
            Assert.Equal(404, notFoundResult.StatusCode);
        }

        #endregion

        #region Movie

        [Fact]
        public void GetAllMovies()
        {
            // Arrange

            // Act            
            IActionResult actionResult = _movieController.Get();

            // Assert
            var okObjectResult = actionResult as OkObjectResult;
            var resultModels = (List<Movie>)okObjectResult.Value;

            Assert.NotNull(okObjectResult);
            Assert.NotNull(resultModels);
            Assert.Equal(200, okObjectResult.StatusCode.Value);
            Assert.Single(resultModels);
            Assert.NotNull(resultModels.Single().Id);
            Assert.NotNull(resultModels.Single().Title);
            Assert.NotNull(resultModels.Single().Year);
        }

        [Fact]
        public void GetMovieById()
        {
            // Arrange
            IActionResult actionExistingResult = _movieController.Get();
            var okObjectExistingResult = actionExistingResult as OkObjectResult;
            var resultExistingModels = (List<Movie>)okObjectExistingResult.Value;
            var existingModel = resultExistingModels.Single();

            // Act            
            IActionResult actionResult = _movieController.Get(existingModel.Id);

            // Assert
            var okObjectResult = actionResult as OkObjectResult;
            var resultModels = (Movie)okObjectResult.Value;

            Assert.NotNull(okObjectResult);
            Assert.Equal(resultModels, existingModel);
            Assert.Equal(200, okObjectResult.StatusCode.Value);

            Assert.NotNull(resultModels.Id);
            Assert.NotNull(resultModels.Title);
            Assert.NotNull(resultModels.Year);
        }

        [Fact]
        public void GetMovieByIds()
        {
            // Arrange
            IActionResult actionExistingResult = _movieController.Get();
            var okObjectExistingResult = actionExistingResult as OkObjectResult;
            var resultExistingModels = (List<Movie>)okObjectExistingResult.Value;
            var movieIds = resultExistingModels.Select(x => x.Id).ToList();

            // Act            
            IActionResult actionResult = _movieController.Get(movieIds);

            // Assert
            var okObjectResult = actionResult as OkObjectResult;
            var resultModels = (IEnumerable<Movie>)okObjectResult.Value;

            Assert.NotNull(okObjectResult);
            Assert.True(okObjectResult is OkObjectResult);
            Assert.Equal(200, okObjectResult.StatusCode.Value);
            Assert.Equal(resultExistingModels.Count, resultModels.Count());
        }

        [Fact]
        public void PostMovie()
        {
            // Arrange
            var movieModel = GetSampleMovie();

            // Act
            var objectValidator = new Mock<IObjectModelValidator>();
            objectValidator.Setup(o => o.Validate(It.IsAny<ActionContext>(),
                                                         It.IsAny<ValidationStateDictionary>(),
                                                         It.IsAny<string>(),
                                                         It.IsAny<object>()));

            _movieController.ObjectValidator = objectValidator.Object;

            IActionResult actionResult = _movieController.Post(movieModel);

            // Assert
            var createdResult = actionResult as CreatedResult;

            Assert.NotNull(createdResult);
            Assert.Equal(201, createdResult.StatusCode.Value);
            Assert.NotEmpty(createdResult.Location);
            Assert.Equal($"api/movie/{movieModel.Id}", createdResult.Location);
        }

        [Fact]
        public void PutMovie()
        {
            // Arrange
            IActionResult actionExistingResult = _movieController.Get();
            var okObjectExistingResult = actionExistingResult as OkObjectResult;
            var resultExistingModels = (List<Movie>)okObjectExistingResult.Value;
            var movieModel = resultExistingModels.FirstOrDefault();

            // Act
            var objectValidator = new Mock<IObjectModelValidator>();
            objectValidator.Setup(o => o.Validate(It.IsAny<ActionContext>(),
                                                         It.IsAny<ValidationStateDictionary>(),
                                                         It.IsAny<string>(),
                                                         It.IsAny<object>()));

            _movieController.ObjectValidator = objectValidator.Object;

            movieModel.Title = Guid.NewGuid().ToString();

            IActionResult actionResult = _movieController.Put(movieModel.Id, movieModel);

            // Assert
            var okObjectResult = actionResult as OkObjectResult;
            var resultModel = (Movie)okObjectResult.Value;

            Assert.NotNull(okObjectResult);
            Assert.Equal(200, okObjectResult.StatusCode.Value);

            Assert.Equal(movieModel.Id, resultModel.Id);
            Assert.Equal(movieModel.Title, resultModel.Title);
            Assert.Equal(movieModel.Genre, resultModel.Genre);
            Assert.Equal(movieModel.StarringActor, resultModel.StarringActor);
            Assert.Equal(movieModel.Year, resultModel.Year);
        }

        [Fact]
        public void DeleteMovie()
        {
            // Arrange
            IActionResult actionResultForExisting = _movieController.Get();
            var okObjectResultForExisting = actionResultForExisting as OkObjectResult;
            var resultModelsExisting = (List<Movie>)okObjectResultForExisting.Value;
            var existingModel = resultModelsExisting.Single();

            // Act            
            IActionResult actionResult = _movieController.Delete(existingModel.Id);

            // Assert
            var okResult = actionResult as OkResult;

            Assert.NotNull(okResult);
            Assert.Equal(200, okResult.StatusCode);

            IActionResult actionResultCheckIfExist = _movieController.Get(existingModel.Id);
            var notFoundResult = actionResultCheckIfExist as NotFoundResult;

            Assert.NotNull(notFoundResult);
            Assert.Equal(404, notFoundResult.StatusCode);
        }

        #endregion
    }
}
