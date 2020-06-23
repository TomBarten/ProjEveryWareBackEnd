// <copyright file="AnswerController.cs" company="Fvect">
// Copyright (c)
//  Aaron Slots, Arthur Heidt, Jeroen de Klerk, Luc van Dijk,
//  Ryan van Gemert, Sjanne Flinterman, Thomas Maassen and Tom Barten.
// All rights reserved.
// </copyright>

namespace Fvect.Backend.API.V1.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Threading.Tasks;
    using Fvect.Backend.API.Abstraction;
    using Fvect.Backend.Business.Manager.Abstraction;
    using Fvect.Backend.Business.Manager.Implementation;
    using Fvect.Backend.Data.Database;
    using Fvect.Backend.Data.Database.Model;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    /// Represents a controller for answers.
    /// </summary>
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class AnswerController : ResourceControllerBase<Answer, IAnswerManager>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AnswerController"/> class.
        /// </summary>
        /// <param name="manager">the manager to use.</param>
        public AnswerController(IAnswerManager manager)
            : base(manager)
        {
        }
    }
}
