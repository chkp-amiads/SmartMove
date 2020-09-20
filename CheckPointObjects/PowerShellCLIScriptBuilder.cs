﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckPointObjects
{
    public class PowerShellCLIScriptBuilder : CLIScriptBuilder
    {

        private PowerShellCLIScriptBuilder()
        {

        }

        public static PowerShellCLIScriptBuilder psCLIScriptBuilder;

        public static PowerShellCLIScriptBuilder getInstance()
        {
            if (psCLIScriptBuilder == null)
                psCLIScriptBuilder = new PowerShellCLIScriptBuilder();
            return psCLIScriptBuilder;
        }

        public string GenerateScriptHeader(string toolVersion, bool isObjectsScript)
        {
            string version = string.Format("# This script was generated by Check Point SmartMove tool v{0}.", toolVersion);
            string scriptType = string.Format("# Run this script on your Windows machine where you have mgmt_cli.exe tool to create the {0}",
                isObjectsScript ? "objects in your policy package." : "policy package.");

            var sb = new StringBuilder();
            sb.Append(version)
              .Append(Environment.NewLine)
              .Append(scriptType)
              .Append(Environment.NewLine)
              .Append("# Note: Make sure this script reside in same folder as mgmt_cli.exe (mgmt_cli tool is part of SmartConsole installation) .")
              .Append(Environment.NewLine);

            return sb.ToString();
        }

        public string GenerateScriptFooter(string errorsReportFileName)
        {
            var sb = new StringBuilder();
            sb.Append(Environment.NewLine)
              .Append("$cmd = '.\\mgmt_cli.exe logout --session-id ' + $sid")
              .Append(Environment.NewLine)
              .Append("run_command $cmd")
              .Append(Environment.NewLine)
              .Append(Environment.NewLine)
              .Append("if ([System.IO.File]::Exists('").Append(errorsReportFileName).Append("')){")
              .Append(Environment.NewLine)
              .Append("Write-Host ''")
              .Append(Environment.NewLine)
              .Append("  Write-Host 'Some objects were not created successfully.'")
              .Append(Environment.NewLine)
              .Append("  Write-Host 'Check file ").Append(errorsReportFileName).Append(" for details.'")
              .Append(Environment.NewLine)
              .Append("} else{")
              .Append(Environment.NewLine)
              .Append("  Write-Host ''")
              .Append(Environment.NewLine)
              .Append("  Write-Host 'Done. All objects were created successfully.'")
              .Append(Environment.NewLine)
              .Append("}");

            return sb.ToString();
        }

        public string GenerateRunCommandScript(string errorsReportFileName) // TODO handle stderror like the bash script
        {
            var sb = new StringBuilder();
            sb.Append("function run_command($cmd) {")
              .Append(Environment.NewLine)
              .Append("try{")
              .Append(Environment.NewLine)
              .Append(" $command = $cmd + ' -m ' + $fullNameServiceId + ' --context ' + $sharedSecret + '/web_api'")
              .Append(Environment.NewLine)
              .Append("Invoke-Expression $command #> last_output.txt 2>&1")
              .Append(Environment.NewLine)
              .Append("  } catch{")
              .Append(Environment.NewLine)
              .Append("Write-Host $cmd >> ").Append(errorsReportFileName)
              .Append(Environment.NewLine)
              .Append("Get-Content last_output.txt >> ").Append(errorsReportFileName)
              .Append(Environment.NewLine)
              .Append("Write-Host ''")
              .Append(Environment.NewLine)
              .Append("Write-Host $cmd")
              .Append(Environment.NewLine)
              .Append("Get-Content last_output.txt")
              .Append(Environment.NewLine)
              .Append("}")
              .Append(Environment.NewLine)
              .Append("}")
              .Append(Environment.NewLine);

            return sb.ToString();
        }

        public string GenerateLoginScript(string domainName, string errorsReportFileName)
        {

            var sb = new StringBuilder();

            sb.Append("$connectionToken = Read-Host 'Enter Connection Token for Smart-1 Cloud'")
              .Append(Environment.NewLine)
              .Append("$serviceId,$sharedSecret = $connectionToken.split('/')")
              .Append(Environment.NewLine)
              .Append("$fullNameServiceId = $serviceId+'.maas.checkpoint.com'")
              .Append(Environment.NewLine)
              .Append("$name = Read-Host 'Enter username'")
              .Append(Environment.NewLine)
              .Append("$pass = Read-Host 'Enter password'")
              .Append(Environment.NewLine);

            sb.Append("Write-Host 'Logging in...'")
              .Append(Environment.NewLine)
              .Append("if ([System.IO.File]::Exists('").Append(errorsReportFileName).Append("'))")
              .Append(Environment.NewLine)
              .Append("{")
              .Append("Remove-Item ").Append(errorsReportFileName)
              .Append(Environment.NewLine)
              .Append("}")
              .Append(Environment.NewLine)
              .Append("Write-Host ''")
              .Append(Environment.NewLine);

            sb.Append("try{")
              .Append(Environment.NewLine)
              .Append("$cmd = '.\\mgmt_cli.exe login user ' + $name + ' password ' + $pass + ' -m ' + $fullNameServiceId + ' --context ' + $sharedSecret + '/web_api -v 1.1 -f json'")
              .Append(Environment.NewLine)
              .Append("Write-Host 'loing command: ' + $cmd")
              .Append(Environment.NewLine)
              .Append("$response = Invoke-Expression $cmd | ConvertFrom-Json")
              .Append(Environment.NewLine)
              .Append("Write-Host 'respons: ' $response")
              .Append(Environment.NewLine)
              .Append("$sid = $response.sid")
              .Append(Environment.NewLine)
              .Append("Write-Host 'sid: ' $sid")
              .Append(Environment.NewLine)
              .Append("} catch{")
              .Append("Write-Host 'Login failed'")
              .Append(Environment.NewLine)
              .Append("exit 1")
              .Append(Environment.NewLine)
              .Append("}");

            return sb.ToString();
        }

        public string GeneratePublishScript()
        {
            var sb = new StringBuilder();

            sb.Append("$cmd = '.\\mgmt_cli.exe publish --session-id ' + $sid")
                .Append(Environment.NewLine)
                .Append("run_command $cmd")
                .Append(Environment.NewLine);
            return sb.ToString();
        }

        public string GenerateObjectScript(CheckPointObject cpObject)
        {
            string scriptInstruction = cpObject.ToCLIScriptInstruction();

            var sb = new StringBuilder();

            if (!string.IsNullOrEmpty(scriptInstruction))
            {
                sb.Append(GenerateInstructionScript(scriptInstruction)).Append(Environment.NewLine);
            }

            sb.Append("$cmd='.\\mgmt_cli.exe ").Append(cpObject.ToCLIScript()).Append(" ignore-warnings true --session-id ' + $sid + ' --user-agent mgmt_cli_smartmove'")
              .Append(Environment.NewLine)
              .Append("run_command $cmd");

            return sb.ToString();
        }

        public string GenerateGeneralCommandScript(string command)
        {
            if (string.IsNullOrEmpty(command))
            {
                return "";
            }

            var sb = new StringBuilder();

            sb.Append("$cmd='.\\mgmt_cli.exe ").Append(command).Append(" --session-id ' + $sid + ' --user-agent mgmt_cli_smartmove'")
              .Append(Environment.NewLine)
              .Append("run_command $cmd");

            return sb.ToString();
        }

        public string GenerateDiagnosticsCommandScript(string command, string vendorName)
        {
            if (string.IsNullOrEmpty(command))
            {
                return "";
            }

            var sb = new StringBuilder();

            sb.Append("$cmd='.\\mgmt_cli.exe ").Append(command + "_" + vendorName).Append(" --session-id ' + $sid")  
            .Append(Environment.NewLine)
              .Append("run_command $cmd");


            return sb.ToString();
        }

        public string GenerateRuleInstructionScript(string instruction)
        {
            var sb = new StringBuilder();
            sb.Append("Write-Host -n $'\\r").Append(instruction).Append(" '"); // TODO remove bash signs here

            return sb.ToString();
        }

        public string GenerateInstructionScript(string instruction)
        {
            var sb = new StringBuilder();
            sb.Append("Write-Host '").Append(instruction).Append("'");

            return sb.ToString();
        }

        public string GetObjectsScriptFilePostfix()
        {
            return "_objects.ps1";
        }

        public string GetPolicyScriptFilePostfix()
        {
            return ".ps1";
        }

        public string GetPolicyOptimizedScriptFilePostfix()
        {
            return ".ps1";
        }
    }
}
