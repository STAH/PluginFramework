﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Weikio.PluginFramework.Context
{
    /// <summary>
    /// Options for PluginLoadContext
    /// </summary>
    public class PluginLoadContextOptions
    {
        /// <summary>
        /// Gets or sets if the plugin should by default to use the assemblies referenced by the plugin or by the host application. Useful in situations where it is important that the host application
        /// and the plugin use the same version of the assembly, even if they reference different versions.
        /// </summary>
        public UseHostApplicationAssembliesEnum UseHostApplicationAssemblies { get; set; } = Defaults.UseHostApplicationAssemblies;

        /// <summary>
        /// Gets or sets the assemblies which the plugin should use if UseHostApplicationAssemblies is set to Selected. These assemblies are used
        /// even if the plugin itself references an another version of the same assembly.
        /// </summary>
        public List<AssemblyName> HostApplicationAssemblies { get; set; } = Defaults.HostApplicationAssemblies;

        /// <summary>
        /// Gets or sets the function which is used to create the logger for PluginLoadContextOptions
        /// </summary>
        public Func<ILogger<PluginAssemblyLoadContext>> LoggerFactory { get; set; } = Defaults.LoggerFactory;

        /// <summary>
        /// Gets or sets the additional runtime paths which are used when locating plugin assemblies  
        /// </summary>
        public List<string> AdditionalRuntimePaths { get; set; } = Defaults.AdditionalRuntimePaths;

        /// <summary>
        /// Gets or sets the func for retuning current runtime identifier  
        /// </summary>
        public static Func<string> GetRuntimeIdentifier { get; set; } = Defaults.GetRuntimeIdentifier;

        public static class Defaults
        {
            /// <summary>
            /// Gets or sets if the plugin should by default to use the assemblies referenced by the plugin or by the host application. Default = Always. Useful in situations where it is important that the host application
            /// and the plugin use the same version of the assembly, even if they reference different versions. 
            /// </summary>
            public static UseHostApplicationAssembliesEnum UseHostApplicationAssemblies { get; set; } = UseHostApplicationAssembliesEnum.Always;

            /// <summary>
            /// Gets or sets the assemblies which the plugin should use if UseHostApplicationAssemblies is set to Selected. These assemblies are used
            /// even if the plugin itself references an another version of the same assembly.
            /// </summary>
            public static List<AssemblyName> HostApplicationAssemblies { get; set; } = new List<AssemblyName>();

            /// <summary>
            /// Gets or sets the function which is used to create the logger for PluginLoadContextOptions
            /// </summary>
            public static Func<ILogger<PluginAssemblyLoadContext>> LoggerFactory { get; set; } = () => NullLogger<PluginAssemblyLoadContext>.Instance;
            
            /// <summary>
            /// Gets or sets the additional runtime paths which are used when locating plugin assemblies  
            /// </summary>
            public static List<string> AdditionalRuntimePaths { get; set; } = new List<string>();

            /// <summary>
            /// Gets or sets the func for retuning current runtime identifier  
            /// </summary>
            public static Func<string> GetRuntimeIdentifier { get; set; } = Microsoft.DotNet.PlatformAbstractions.RuntimeEnvironment.GetRuntimeIdentifier;

            /// <summary>
            /// Gets or sets the func for retuning supported runtime identifiers  
            /// </summary>
            public static Func<List<string>> GetSupportedRuntimeIdentifiers { get; set; } = () =>
            {
                var result = new List<string>();
                var defaultDependencyContext = DependencyContext.Default;

                if (defaultDependencyContext == null)
                {
                    return result;
                }

                var runtimeIdentifier = GetRuntimeIdentifier();

                if (string.IsNullOrWhiteSpace(runtimeIdentifier))
                {
                    return result;
                }

                result.Add(runtimeIdentifier);

                var fallbacks = defaultDependencyContext.RuntimeGraph
                    .FirstOrDefault(x => string.Equals(x.Runtime, runtimeIdentifier, StringComparison.InvariantCulture))?.Fallbacks;

                if (fallbacks?.Any() == true)
                {
                    result.AddRange(fallbacks);
                }
                
                return result;
            };
        }
    }
}
