## Script to uninstall ICP mock as a windows service.
param([string][Parameter(Mandatory=$true)] $envName)

Write-Output("Stopping services...");
Stop-Service -Name "Dcis.Am.Icp.Mock.$envName";

Write-Output("Removing services...");
sc.exe delete "Dcis.Am.Icp.Mock.$envName";
