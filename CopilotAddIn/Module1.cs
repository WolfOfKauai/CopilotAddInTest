using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CopilotAddIn
{
    /// <summary>
    /// Main module class for the CopilotAddIn
    /// This class is loaded when ArcGIS Pro starts
    /// </summary>
    internal class Module1
    {
        private static Module1 _this = null;

        /// <summary>
        /// Retrieve the singleton instance to this module
        /// </summary>
        public static Module1 Current
        {
            get
            {
                return _this ?? (_this = new Module1());
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        private Module1()
        {
        }

        /// <summary>
        /// Called by Framework when ArcGIS Pro is closing
        /// </summary>
        /// <returns>False to prevent Pro from closing, otherwise True</returns>
        protected bool CanUnload()
        {
            // TODO: add custom logic
            return true;
        }

        #region Overrides
        /// <summary>
        /// Called by Framework when the module is initialized
        /// </summary>
        protected void Initialize()
        {
            // TODO: Add initialization logic
        }

        /// <summary>
        /// Called by Framework when the module is unloaded
        /// </summary>
        protected void Uninitialize()
        {
            // TODO: Add cleanup logic
        }
        #endregion Overrides
    }
}
