// <copyright file="Program.cs" company="Fvect">
// Copyright (c)
//  Aaron Slots, Arthur Heidt, Jeroen de Klerk, Luc van Dijk,
//  Ryan van Gemert, Sjanne Flinterman, Thomas Maassen and Tom Barten.
// All rights reserved.
// </copyright>

namespace Fvect.Backend.API
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Fvect.Backend.Data.Database;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Serilog;
    using Serilog.Events;

    /// <summary>
    /// Provides functionality to run the application.
    /// </summary>
    public static class Program
    {
        private static readonly string EnvironmentNameEnvironmentVariableName = "ASPNETCORE_ENVIRONMENT";
        private static readonly string AzureKeyVaultUriEnvironmentVariableName = "AZ_KEYVAULT_URI";
        private static readonly string AzureAppServiceEnvironmentName = "AZAPPSVC";

        /// <summary>
        /// Runs the application.
        /// </summary>
        /// <param name="args">The command line arguments passed to the application.</param>
        /// <returns>The exit code of the application.</returns>
        public static int Main(string[] args) => Run(args);

        /// <summary>
        /// Creates the application host builder.
        /// </summary>
        /// <param name="args">The command line arguments passed to the application.</param>
        /// <returns>The builder for the host of the application.</returns>
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog()
                .ConfigureHostConfiguration(configBuilder =>
                {
                    configBuilder
                        .AddJsonFile(path: "appsettings.Local.json", optional: true, reloadOnChange: true);

                    if (Environment.GetEnvironmentVariable(EnvironmentNameEnvironmentVariableName) == AzureAppServiceEnvironmentName)
                    {
                        configBuilder
                            .AddAzureKeyVault(
                                Environment.GetEnvironmentVariable(
                                    AzureKeyVaultUriEnvironmentVariableName));
                    }
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });

        /// <summary>
        /// Runs the application.
        /// </summary>
        /// <param name="args">The command line arguments passed to the application.</param>
        /// <param name="dryRun">A value indicating whether the application should not actually run (and not migrate the database).</param>
        /// <returns>The exit code of the application.</returns>
        [SuppressMessage(
            "Design",
            "CA1031:Do not catch general exception types",
            Justification = "Exception is logged.")]
        public static int Run(string[] args, bool dryRun = false)
        {
            try
            {
                Log.Logger = new LoggerConfiguration()
                    .MinimumLevel.Verbose()
                    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                    .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Debug)
                    .Enrich.FromLogContext()
                    .WriteTo.Console()
                    .CreateLogger();
            }
            catch
            {
                Console.WriteLine("Error setting up logging. Exception will be rethrown and the application will exit with a non-zero exit code.");
                throw;
            }

            try
            {
                var errorExitCode = 1;

                IHost host;
                try
                {
                    Log.Information("Going to build application host.");
                    host = CreateHostBuilder(args).Build();
                    Log.Information("Built application host.");
                }
                catch (Exception ex)
                {
                    Log.Fatal(ex, "Error building application host. See the exception for more details.");
                    Log.Information("Going to exit with code {exitCode}.", errorExitCode);
                    return errorExitCode;
                }

                errorExitCode++;

                try
                {
                    Log.Information("Going to migrate the application's database.");

                    if (dryRun)
                    {
                        Log.Warning("DRYRUN");
                    }
                    else
                    {
                        using var scope = host.Services.CreateScope();
                        var dbContext = scope.ServiceProvider.GetRequiredService<FvectContext>();
                        dbContext.Database.Migrate();
                    }

                    Log.Information("Migrated the application's database.");
                }
                catch (Exception ex)
                {
                    Log.Fatal(ex, "Error migrating database. See the exception for more details.");
                    Log.Information("Going to exit with code {exitCode}.", errorExitCode);
                    return errorExitCode;
                }

                errorExitCode++;

                try
                {
                    Log.Information("Going to run the application.");

                    if (dryRun)
                    {
                        Log.Warning("DRYRUN");
                    }
                    else
                    {
                        try
                        {
                            host.Run();
                        }
                        catch (OperationCanceledException)
                        {
                            Log.Debug("Caught an operation cancelled exception after running the host.");

                            // Ignore.
                        }
                    }

                    Log.Information("Ran the application.");
                    Log.Information("Going to exit with code {exitCode}.", 0);
                    return 0;
                }
                catch (Exception ex)
                {
                    Log.Fatal(ex, "Error running the application. See the exception for more details.");
                    Log.Information("Going to exit with code {exitCode}.", errorExitCode);
                    return errorExitCode;
                }
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}
