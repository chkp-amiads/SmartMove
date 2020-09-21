using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel;

namespace MigrationBase
{
    public class SupportedOutputScripts
    {
        #region Constants

        

        #endregion

        #region Private Members

        private readonly List<OutputScript> _outputScripts = new List<OutputScript> { OutputScript.BashCLI, OutputScript.PowerShellCLI };

        #endregion

        #region Properties

        public List<OutputScript> OutputScripts
        {
            get { return _outputScripts; }
        }

        public OutputScript SelectedOutputScript { get; set; }

        #endregion
    }

    [TypeConverter(typeof(OutputScriptDescriptionConverter))]
    public enum OutputScript
    {
        [Description("Bash CLI (On-Prem Management Server)")]
        BashCLI,
        [Description("PowerShell CLI (Smart-1 Cloud)")]
        PowerShellCLI
    }

}
