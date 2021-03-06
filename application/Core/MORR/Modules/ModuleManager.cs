﻿using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using MORR.Shared.Configuration;
using MORR.Shared.Modules;

namespace MORR.Core.Modules
{
    /// <summary>
    ///     Initializes and manages all modules.
    /// </summary>
    public class ModuleManager : IModuleManager
    {
        private IEnumerable<IModule> enabledModules = new IModule[0];

#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
        /// <summary>
        ///     All <see cref="IModule" /> instances available through MEF.
        /// </summary>
        [ImportMany]
        private IEnumerable<IModule> Modules { get; set; }

        /// <summary>
        ///     The <see cref="IConfiguration" /> instance specifying configuration options regarding all modules.
        /// </summary>
        [Import]
        private GlobalModuleConfiguration ModuleConfiguration { get; set; }
#pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.

        public void InitializeModules()
        {
            enabledModules = Modules.Where(x => ModuleConfiguration.EnabledModules.Contains(x.GetType()));

            foreach (var module in Modules)
            {
                module.Initialize(enabledModules.Contains(module));
            }
        }

        public void NotifyModulesOnSessionStart()
        {
            foreach (var module in enabledModules)
            {
                module.IsActive = true;
            }
        }

        public void NotifyModulesOnSessionStop()
        {
            foreach (var module in enabledModules)
            {
                module.IsActive = false;
            }
        }
    }
}