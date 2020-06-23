// <copyright file="ServiceCollectionExtensions.cs" company="Fvect">
// Copyright (c)
//  Aaron Slots, Arthur Heidt, Jeroen de Klerk, Luc van Dijk,
//  Ryan van Gemert, Sjanne Flinterman, Thomas Maassen and Tom Barten.
// All rights reserved.
// </copyright>

// ReSharper disable once CheckNamespace
namespace Fvect.Backend.Business
{
    using System;
    using Fvect.Backend.Business.Geo.Map;
    using Fvect.Backend.Business.Geo.Map.Abstraction;
    using Fvect.Backend.Business.Geo.Map.Implementation;
    using Fvect.Backend.Business.Manager.Abstraction;
    using Fvect.Backend.Business.Manager.Implementation;
    using Fvect.Backend.Business.Task.Abstraction;
    using Fvect.Backend.Business.Task.Implementation;
    using Fvect.Backend.Data.Database.Model;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// Contains functionality that can register the business layer
    /// to a service container.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Registers the FVect Business Layer to the given service collection.
        /// </summary>
        /// <param name="serviceCollection">The service collection.</param>
        /// <returns>The value of <paramref name="serviceCollection"/> after the operation completed sucessfully, for chaining.</returns>
        public static IServiceCollection AddFVectBusinessLayer(this IServiceCollection serviceCollection)
        {
            serviceCollection = serviceCollection ?? throw new ArgumentNullException(nameof(serviceCollection));

            // Managers
            {
                serviceCollection.AddTransient<IMathManager, MathManager>();
                serviceCollection.AddTransient<IGeoManager, GeoManager>();
                serviceCollection.AddTransient<IQuestionManager, QuestionManager>();
                serviceCollection.AddTransient<IAnswerManager, AnswerManager>();
                serviceCollection.AddTransient<ILevelManager, LevelManager>();
                serviceCollection.AddTransient<IUserProfileManager, UserProfileManager>();
                serviceCollection.AddTransient<IStudentGroupManager, StudentGroupManager>();
                serviceCollection.AddTransient<ITeacherManager, TeacherManager>();
            }

            // Tasks
            {
                serviceCollection.AddTransient<IGeoMapImageCacheCleaningTask, GeoMapImageCacheCleaningTask>();
            }

            // Geo
            {
                serviceCollection.AddTransient(MapImageRetrieverFactory.CreateDefault);
                serviceCollection.AddTransient<IImageResizingStrategy, ImageSharpImageResizingStrategy>();
            }

            return serviceCollection;
        }
    }
}
