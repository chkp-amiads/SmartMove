using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckPointObjects
{
    public interface CLIScriptBuilder
    {
        string GenerateScriptHeader(string toolVersion, bool isObjectsScript);

        string GenerateScriptFooter(string errorsReportFileName);

        string GenerateRunCommandScript(string errorsReportFileName);

        string GenerateLoginScript(string domainName, string errorsReportFileName);

        string GeneratePublishScript();

        string GenerateObjectScript(CheckPointObject cpObject);

        string GenerateGeneralCommandScript(string command);

        string GenerateDiagnosticsCommandScript(string command, string vendorName);

        string GenerateRuleInstructionScript(string instruction);

        string GenerateInstructionScript(string instruction);

        string GetObjectsScriptFilePostfix();

        string GetPolicyScriptFilePostfix();

        string GetPolicyOptimizedScriptFilePostfix();


    }
}
