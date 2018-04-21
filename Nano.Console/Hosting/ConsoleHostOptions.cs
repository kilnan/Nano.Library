﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.Extensions.Configuration;

namespace Nano.Console.Hosting
{
    /// <summary>
    /// Console Host Options.
    /// </summary>
    public class ConsoleHostOptions
    {
        /// <summary>
        /// Environment.
        /// </summary>
        public virtual string Environment { get; set; }

        /// <summary>
        /// Application Name.
        /// </summary>
        public virtual string ApplicationName { get; set; }

        /// <summary>
        /// Startup Assembly.
        /// </summary>
        public virtual string StartupAssembly { get; set; }

        /// <summary>
        /// Console Root.
        /// </summary>
        public virtual string ConsoleRoot { get; set; }

        /// <summary>
        /// Content Root Path.
        /// </summary>
        public virtual string ContentRootPath { get; set; }

        /// <summary>
        /// Capture Startup Errors.
        /// </summary>
        public virtual bool CaptureStartupErrors { get; set; }

        /// <summary>
        /// Shutdown Timeout.
        /// </summary>
        public virtual TimeSpan ShutdownTimeout { get; set; } = TimeSpan.FromSeconds(5);

        /// <summary>
        /// Hosting Startup Assemblies.
        /// </summary>
        public virtual IEnumerable<string> HostingStartupAssemblies { get; set; }

        /// <summary>
        /// Hosting Startup Exclude Assemblies.
        /// </summary>
        public virtual IEnumerable<string> HostingStartupExcludeAssemblies { get; set; }

        /// <summary>
        /// Hosting Startup Assemblies Final.
        /// </summary>
        public virtual IEnumerable<string> HostingStartupAssembliesFinal
            => HostingStartupAssemblies.Except(HostingStartupExcludeAssemblies, StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Constructor (Default).
        /// </summary>
        public ConsoleHostOptions()
        {

        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="configuration">The <see cref="IConfiguration"/>.</param>
        public ConsoleHostOptions(IConfiguration configuration)
            : this(configuration, string.Empty)
        {

        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="configuration">The <see cref="IConfiguration"/>.</param>
        /// <param name="applicationNameFallback">The name of the application (fallback)</param>
        public ConsoleHostOptions(IConfiguration configuration, string applicationNameFallback)
        {
            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));

            this.Environment = configuration[ConsoleHostDefaults.EnvironmentKey];
            this.ApplicationName = configuration[ConsoleHostDefaults.ApplicationKey] ?? applicationNameFallback;
            this.StartupAssembly = configuration[ConsoleHostDefaults.StartupAssemblyKey];
            this.ConsoleRoot = configuration[ConsoleHostDefaults.ConsoleRootKey];
            this.ContentRootPath = configuration[ConsoleHostDefaults.ContentRootKey];
            this.CaptureStartupErrors = this.ParseBool(configuration, ConsoleHostDefaults.CaptureStartupErrorsKey);
            this.HostingStartupAssemblies = Split($"{ApplicationName};{configuration[ConsoleHostDefaults.HostingStartupAssembliesKey]}");
            this.HostingStartupExcludeAssemblies = Split(configuration[ConsoleHostDefaults.HostingStartupExcludeAssembliesKey]);

            var timeout = configuration[ConsoleHostDefaults.ShutdownTimeoutKey];
            if (!string.IsNullOrEmpty(timeout) && int.TryParse(timeout, NumberStyles.None, CultureInfo.InvariantCulture, out var seconds))
            {
                ShutdownTimeout = TimeSpan.FromSeconds(seconds);
            }
        }

        private IEnumerable<string> Split(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return Array.Empty<string>();

            return value
                .Split(new[] {';'}, StringSplitOptions.RemoveEmptyEntries)
                .Where(x => !string.IsNullOrEmpty(x));
        }
        private bool ParseBool(IConfiguration configuration, string value)
        {
            return
                string.Equals("1", configuration[value], StringComparison.OrdinalIgnoreCase) ||
                string.Equals("true", configuration[value], StringComparison.OrdinalIgnoreCase);
        }
    }
}