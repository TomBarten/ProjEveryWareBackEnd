// <copyright file="DevController.cs" company="Fvect">
// Copyright (c)
//  Aaron Slots, Arthur Heidt, Jeroen de Klerk, Luc van Dijk,
//  Ryan van Gemert, Sjanne Flinterman, Thomas Maassen and Tom Barten.
// All rights reserved.
// </copyright>

#if DEBUG // DO NOT REMOVE THIS COMPILER INSTRUCTION!
// TO THE TEACHER: AS STATED IN THE SUMMARY,
// THIS IS A DEVELOPMENT TOOL. IT SHOULD NOT BE CONSIDERED IN THE
// GRADING OF OUR PROJECT.
namespace Fvect.Backend.API.V1.Controllers
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading.Tasks;
    using Fvect.Backend.Data.Database;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Hosting;

    /// <summary>
    /// Represents a controller that contains development operations.
    /// This API should not be considered stable. It is only included included
    /// in development builds and may be modified at any point in time.
    /// The controller does not have to comply to the architectural principles
    /// of the application, does not have to be tested and does not have to
    /// be secure.
    /// </summary>
    [AllowAnonymous]
    [ApiController]
    [ExcludeFromCodeCoverage]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class DevController : ControllerBase
    {
        private readonly FvectContext dbContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="DevController"/> class.
        /// </summary>
        /// <param name="hostEnvironment">The current host environment.</param>
        /// <param name="dbContext">The db context.</param>
        /// <exception cref="InvalidOperationException">
        /// When the current environment is not the development environment.
        /// </exception>
        public DevController(
            IHostEnvironment hostEnvironment,
            FvectContext dbContext)
        {
            if (!(hostEnvironment ?? throw new ArgumentNullException(nameof(hostEnvironment))).IsDevelopment())
            {
                throw new InvalidOperationException(
                    $"{nameof(DevController)} operations may only be invoked in a development environment.");
            }

            this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        /// <summary>
        /// Sets up the default Auth Client IDs as specced in the README.md.
        /// </summary>
        /// <returns>A task.</returns>
        [HttpPost("SetupDefaultClientIds")]
        public Task SetupDefaultClientIds() =>
            this.dbContext.Database.ExecuteSqlRawAsync(@"
INSERT INTO [ClientApplications] (Id, Name) VALUES
	('3fa85f64-5717-4562-b3fc-2c963f66afa6', 'Swagger UI'),
	('2a7d62cb-755c-42ae-9063-55c6b5fe793f', 'DevTools'),
	('8f7fbd3b-3a1f-4884-ba77-77a5a8b58175', 'Backoffice'),
	('eb59e004-3921-45ec-95ef-07553d4a9b85', 'App')
");
    }
}
#endif // DO NOT REMOVE THIS COMPILER INSTRUCTION!
