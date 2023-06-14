## Script to install ICP mock as a windows service.
param([string][Parameter(Mandatory=$true)] $envName)

Write-Output("Installing service 'Dcis.Am.Icp.Mock.$envName'");
$service = Get-WmiObject win32_service -filter "displayName='Dcis.Am.Icp.Mock.$envName'";
if ($null -ne $service) {
    Write-Host("'Dcis.Am.Icp.Mock.$envName' is still installed. Please remove service, or wait for it to be deleted.") -ForegroundColor Red -BackgroundColor Black;
    exit;
}

New-Service -Name "Dcis.Am.Icp.Mock.$envName" -BinaryPathName "C:\Dcis.Am.Icp.Mock\$envName\publish\Dcis.Am.Mock.Icp.exe $envName" -StartupType Automatic;

Write-Output("Starting service 'Dcis.Am.Icp.Mock.$envName'");
Start-Service -Name "Dcis.Am.Icp.Mock.$envName";