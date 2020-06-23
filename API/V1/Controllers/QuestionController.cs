// <copyright file="QuestionController.cs" company="Fvect">
// Copyright (c)
//  Aaron Slots, Arthur Heidt, Jeroen de Klerk, Luc van Dijk,
//  Ryan van Gemert, Sjanne Flinterman, Thomas Maassen and Tom Barten.
// All rights reserved.
// </copyright>

namespace Fvect.Backend.API.V1.Controllers
{
    using Fvect.Backend.API.Abstraction;
    using Fvect.Backend.Business.Manager.Abstraction;
    using Fvect.Backend.Data.Database.Model;
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    /// Represents a controller for questions.
    /// </summary>
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class QuestionController : ResourceControllerBase<Question, IQuestionManager>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="QuestionController"/> class.
        /// </summary>
        /// <param name="manager">The manager to use.</param>
        public QuestionController(IQuestionManager manager)
            : base(manager)
        {
        }
    }
}
