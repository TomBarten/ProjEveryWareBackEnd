// <copyright file="ConfigureSwagerGenOptionsTest.cs" company="Fvect">
// Copyright (c)
//  Aaron Slots, Arthur Heidt, Jeroen de Klerk, Luc van Dijk,
//  Ryan van Gemert, Sjanne Flinterman, Thomas Maassen and Tom Barten.
// All rights reserved.
// </copyright>

namespace Fvect.Backend.Test.API.Configuration
{
    using Fvect.Backend.API.Configuration;
    using Swashbuckle.AspNetCore.SwaggerGen;
    using Xunit;

    /// <summary>
    /// Contains tests for functions of the static
    /// <see cref="ConfigureSwaggerGenOptions"/> class.
    /// </summary>
    public class ConfigureSwagerGenOptionsTest
    {
        /// <summary>
        /// Verifies that the <see cref="ConfigureSwaggerGenOptions.Configure(SwaggerGenOptions)"/>
        /// can be executed without throwing an exception.
        /// </summary>
        [Fact]
        public void CanRunWithoutThrowingAnException()
        {
            var mock = IApiVersionDescriptionProviderMockFactory.CreateMock();
            var configurator = new ConfigureSwaggerGenOptions(mock.Object);
            configurator.Configure(new SwaggerGenOptions());
        }
    }
}
