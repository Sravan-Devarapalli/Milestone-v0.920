@echo off

set logPath="%~3\upgrade_log.txt"
set vsdbcmdPath="%~3\Deployment\vsdbcmd.exe"
set modelFilePath="%~3\PracticeManagementDatabase.dbschema"
set upgradeScriptPath="%~3\UpgradeScript.sql"

@echo on

echo --- Logging started at %date% %time% > %logPath%

%vsdbcmdPath% /a:Deploy /dsp:Sql /model:%modelFilePath% /dd+ /cs:"Server=%~1; Integrated Security=SSPI;" /p:TargetDatabase="%~2" /script:%upgradeScriptPath% >> %logPath%

echo --- Logging completed at %date% %time% >> %logPath%

echo Log file: %logPath% >> %logPath%
echo Upgrade script: %upgradeScriptPath% >> %logPath%
