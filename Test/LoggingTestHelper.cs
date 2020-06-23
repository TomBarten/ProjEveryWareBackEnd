// <copyright file="LoggingTestHelper.cs" company="Fvect">
// Copyright (c)
//  Aaron Slots, Arthur Heidt, Jeroen de Klerk, Luc van Dijk,
//  Ryan van Gemert, Sjanne Flinterman, Thomas Maassen and Tom Barten.
// All rights reserved.
// </copyright>

namespace Fvect.Backend.Test
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Xunit.Abstractions;

    /// <summary>
    /// Contains helper functions for logging during tests.
    /// </summary>
    public static class LoggingTestHelper
    {
        /// <summary>
        /// Creates an <see cref="ILoggerFactory"/> whose output will be sent to an
        /// XUnit <see cref="ITestOutputHelper"/>.
        /// </summary>
        /// <param name="testOutputHelper">
        /// The test output helper to which logs should be sent.
        /// Note that an object implementing this interface can be requested in a test constructor.
        /// </param>
        /// <returns>
        /// The created <see cref="ILoggerFactory"/>.
        /// </returns>
        public static ILoggerFactory CreateLoggerFactory(
            ITestOutputHelper testOutputHelper) =>
            new ServiceCollection()
                .AddLogging(logging => logging.AddXUnit(testOutputHelper ?? throw new ArgumentNullException(nameof(testOutputHelper))))
                .BuildServiceProvider()
                .GetService<ILoggerFactory>();

        /// <summary>
        /// Creates an <see cref="ILogger{TCategoryName}"/> whose output will be sent to an
        /// XUnit <see cref="ITestOutputHelper"/>.
        /// </summary>
        /// <typeparam name="TCategoryName">
        /// The logging category name.
        /// </typeparam>
        /// <param name="testOutputHelper">
        /// The test output helper to which logs should be sent.
        /// Note that an object implementing this interface can be requested in a test constructor.
        /// </param>
        /// <returns>
        /// The created <see cref="ILogger{TCategoryName}"/>.
        /// </returns>
        [SuppressMessage(
            "Reliability",
            "CA2000:Dispose objects before losing scope",
            Justification = "Underlying logging provider does not have to be disposed.")]
        public static ILogger<TCategoryName> CreateLogger<TCategoryName>(
            ITestOutputHelper testOutputHelper) =>
            CreateLoggerFactory(testOutputHelper).CreateLogger<TCategoryName>();

        /// <summary>
        /// Creates an <see cref="ILogger"/> whose output will be sent to an
        /// XUnit <see cref="ITestOutputHelper"/>.
        /// </summary>
        /// <param name="testOutputHelper">
        /// The test output helper to which logs should be sent.
        /// Note that an object implementing this interface can be requested in a test constructor.
        /// </param>
        /// <param name="categoryName">
        /// The logging category name.
        /// </param>
        /// <returns>
        /// The created <see cref="ILogger"/>.
        /// </returns>
        [SuppressMessage(
            "Reliability",
            "CA2000:Dispose objects before losing scope",
            Justification = "Underlying logging provider does not have to be disposed.")]
        public static ILogger CreateLogger(
            ITestOutputHelper testOutputHelper,
            string categoryName)
            => CreateLoggerFactory(testOutputHelper).CreateLogger(categoryName);
    }
}
