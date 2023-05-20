param (
    [Parameter(Mandatory = $true)]
    [ValidateNotNullOrEmpty()]
    [string]$FilePath,
    
    [Parameter(Mandatory = $true)]
    [ValidateNotNullOrEmpty()]
    [string]$Content
)

# Function to check if the script is running with administrative privileges
function Test-Admin {
    $currentUser = New-Object Security.Principal.WindowsPrincipal([Security.Principal.WindowsIdentity]::GetCurrent())
    $isAdmin = $currentUser.IsInRole([Security.Principal.WindowsBuiltinRole]::Administrator)
    return $isAdmin
}

# Check if the script is running with administrative privileges
$isAdmin = Test-Admin

if (-not $isAdmin) {
    # Relaunch the script with administrative rights
    $arguments = "-NoProfile -ExecutionPolicy Bypass -File `"$PSCommandPath`" -FilePath `"$FilePath`" -Content `"$Content`""
    Start-Process powershell.exe -Verb RunAs -ArgumentList $arguments
    Exit
}

# Rest of your script code goes here

try {
    # Write the content to the file
    Set-Content -Path $FilePath -Value $Content -Force
    Write-Host "Content written to file successfully."
} catch {
    Write-Host "An error occurred while writing to the file: $_"
}
