# PowerShell script to verify and fix final newline and UTF-8 BOM constraints
param(
    [switch]$Fix
)

$failed = $false

# Helper to load file as raw bytes, trim BOM, trim ending newlines, and rewrite
function Fix-File {
    param(
        [string]$Path,
        [int]$NewlineCount # 0 for no newlines, 1 for single LF newline
    )
    $bytes = [System.IO.File]::ReadAllBytes($Path)
    if ($bytes.Length -eq 0) { return }

    # Detect and skip UTF-8 BOM (0xEF, 0xBB, 0xBF)
    $startIdx = 0
    if ($bytes.Length -ge 3 -and $bytes[0] -eq 239 -and $bytes[1] -eq 187 -and $bytes[2] -eq 191) {
        $startIdx = 3
    }

    # Find the end of data (non-newline characters)
    $endIdx = $bytes.Length - 1
    while ($endIdx -ge $startIdx -and ($bytes[$endIdx] -eq 10 -or $bytes[$endIdx] -eq 13)) {
        $endIdx--
    }

    # Slice the bytes array
    if ($endIdx -lt $startIdx) {
        $newBytes = [byte[]]@()
    } else {
        $newBytes = $bytes[$startIdx..$endIdx]
    }

    # Append appropriate number of LF newlines (10)
    if ($NewlineCount -eq 1) {
        $newBytes = $newBytes + 10
    }

    [System.IO.File]::WriteAllBytes($Path, $newBytes)
}

# 1. C# files must end with exactly one newline (LF = 10) and have no BOM
Get-ChildItem -Recurse -Filter "*.cs" | Where-Object { $_.FullName -notmatch '\\(obj|bin)\\' } | ForEach-Object {
    $bytes = [System.IO.File]::ReadAllBytes($_.FullName)
    if ($bytes.Length -eq 0) { return }
    
    # Detect BOM
    $hasBom = $false
    if ($bytes.Length -ge 3 -and $bytes[0] -eq 239 -and $bytes[1] -eq 187 -and $bytes[2] -eq 191) {
        $hasBom = $true
    }

    $last = $bytes[$bytes.Length - 1]
    $prev = if ($bytes.Length -gt 1) { $bytes[$bytes.Length - 2] } else { 0 }
    
    $isViolated = $false
    if ($last -ne 10 -or $hasBom) {
        $isViolated = $true
    } elseif ($prev -eq 10 -or $prev -eq 13) {
        $isViolated = $true
    }

    if ($isViolated) {
        if ($Fix) {
            Fix-File $_.FullName 1
            Write-Host "Fixed formatting (stripped BOM/newlines): $($_.FullName)"
        } else {
            if ($hasBom) {
                Write-Warning "Format violation: $($_.FullName) contains a UTF-8 BOM signature."
            } else {
                Write-Warning "Format violation: $($_.FullName) does not end with exactly one newline."
            }
            $failed = $true
        }
    }
}

# 2. .csproj and .slnx files must end with zero newlines and have no BOM
Get-ChildItem -Recurse -Include "*.csproj", "*.slnx" | Where-Object { $_.FullName -notmatch '\\(obj|bin)\\' } | ForEach-Object {
    $bytes = [System.IO.File]::ReadAllBytes($_.FullName)
    if ($bytes.Length -eq 0) { return }
    
    # Detect BOM
    $hasBom = $false
    if ($bytes.Length -ge 3 -and $bytes[0] -eq 239 -and $bytes[1] -eq 187 -and $bytes[2] -eq 191) {
        $hasBom = $true
    }

    $last = $bytes[$bytes.Length - 1]
    
    $isViolated = $false
    if ($last -eq 10 -or $last -eq 13 -or $hasBom) {
        $isViolated = $true
    }

    if ($isViolated) {
        if ($Fix) {
            Fix-File $_.FullName 0
            Write-Host "Fixed formatting (stripped BOM/newlines): $($_.FullName)"
        } else {
            if ($hasBom) {
                Write-Warning "Format violation: $($_.FullName) contains a UTF-8 BOM signature."
            } else {
                Write-Warning "Format violation: $($_.FullName) must end with zero newlines, but a trailing newline was found."
            }
            $failed = $true
        }
    }
}

if ($failed) {
    Write-Error "Formatting validation failed. Run the script with -Fix to resolve."
    exit 1
} else {
    Write-Host "Formatting validation passed successfully."
    exit 0
}
