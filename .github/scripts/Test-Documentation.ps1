<#
.SYNOPSIS
    Checks that the documentation follows Docs/Documentation.md.

.DESCRIPTION
    Checks the documentation structure of the repository and exits with 1 if a rule is violated:
    - Every documentation content folder has an Overview.md.
    - Every solution (folder with a *.slnx) has a README.md and a Docs folder with the required files and sections.
    - Every .NET project (*.csproj) has a README.md with the required sections.
    - Projects have no Docs folder of their own and drawio diagrams are stored in a Docs/Assets folder.
    - All relative links and anchors in the Markdown files resolve.

    Runs with Windows PowerShell 5.1 and PowerShell 7 (also on Linux).

.PARAMETER Root
    Repository root. Defaults to the folder two levels above this script.

.EXAMPLE
    powershell -NoProfile -File .github/scripts/Test-Documentation.ps1
#>
[CmdletBinding()]
param(
    [string]$Root = ''
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

# $PSScriptRoot is not available in parameter defaults of Windows PowerShell 5.1.
if ($Root -eq '') {
    $Root = (Resolve-Path (Join-Path $PSScriptRoot '../..')).Path
}

$Root = (Resolve-Path $Root).Path.TrimEnd('\', '/')

# Solutions without implementation, only README.md and Docs/Overview.md are required.
$placeholderSolutions = @('paskalON.PlantControls')

# Folders that are never checked (build output, dependencies, tool folders).
$excludedFolderPattern = '/(bin|obj|node_modules|dist|TestResults|\.vs|\.git)(/|$)'

# Repository documentation folders without Overview.md (assets, versioned manual and marketing).
$docsFoldersWithoutOverview = '^Docs/(Assets|Manual|Marketing)(/|$)'

$repositoryDocumentationLinks = @(
    '/README.md',
    '/Docs/02-Architect/Overview.md',
    '/Docs/03-Design/Overview.md',
    '/Docs/04-Develop/Overview.md',
    '/Docs/05-Test/Overview.md',
    '/Docs/06-Release/Overview.md'
)

$problems = New-Object System.Collections.Generic.List[string]


function Get-RelativePath([string]$Path) {
    $relative = $Path.Substring($Root.Length).TrimStart('\', '/')

    return $relative.Replace('\', '/')
}


function Add-Problem([string]$Path, [string]$Message) {
    $problems.Add("$(Get-RelativePath $Path): $Message")
}


function Get-RepositoryFiles([string]$Filter) {
    return Get-ChildItem -Path $Root -Filter $Filter -Recurse -File -Force |
        Where-Object { ('/' + (Get-RelativePath $_.FullName)) -notmatch $excludedFolderPattern }
}


function Get-FileText([string]$Path) {
    return [System.IO.File]::ReadAllText($Path, [System.Text.Encoding]::UTF8)
}


function Test-Contains([string]$Path, [string]$Text, [string]$Expected, [string]$Message) {
    if ($Text.Contains($Expected) -eq $false) {
        Add-Problem $Path $Message
    }
}


function Test-RepositoryDocumentation([string]$Path, [string]$Text) {
    Test-Contains $Path $Text '## Repository Documentation' "missing section 'Repository Documentation'"

    foreach ($link in $repositoryDocumentationLinks) {
        Test-Contains $Path $Text "]($link)" "missing repository documentation link '$link'"
    }
}


function Get-HeadingAnchors([string]$Path) {
    $text = [regex]::Replace((Get-FileText $Path), '(?s)```.*?```', '')
    $anchors = New-Object System.Collections.Generic.HashSet[string]

    foreach ($match in [regex]::Matches($text, '(?m)^#+\s+(.*?)\s*$')) {
        $anchor = [regex]::Replace($match.Groups[1].Value.ToLowerInvariant(), '[^\w\- ]', '').Replace(' ', '-')
        [void]$anchors.Add($anchor)
    }

    return ,$anchors
}


#------------------------------------------------------------------------------
# Repository documentation folders
#------------------------------------------------------------------------------
$docsRoot = Join-Path $Root 'Docs'

foreach ($folder in Get-ChildItem -Path $docsRoot -Directory -Recurse) {
    $relative = Get-RelativePath $folder.FullName

    if ($relative -match $docsFoldersWithoutOverview) {
        continue
    }

    if ((Test-Path (Join-Path $folder.FullName 'Overview.md')) -eq $false) {
        Add-Problem $folder.FullName 'documentation content folder without Overview.md'
    }
}


#------------------------------------------------------------------------------
# Solutions
#------------------------------------------------------------------------------
$solutionFolders = Get-ChildItem -Path $Root -Directory |
    Where-Object { @(Get-ChildItem -Path $_.FullName -Filter '*.slnx' -File).Count -gt 0 }

foreach ($solution in $solutionFolders) {
    $readme = Join-Path $solution.FullName 'README.md'
    $overview = Join-Path $solution.FullName 'Docs/Overview.md'

    if (Test-Path $readme) {
        $text = Get-FileText $readme
        Test-Contains $readme $text '](./Docs/Overview.md)' 'missing link to ./Docs/Overview.md'
        Test-RepositoryDocumentation $readme $text

        if ($placeholderSolutions -notcontains $solution.Name) {
            Test-Contains $readme $text '## Structure' "missing section 'Structure'"

            if (Test-Path (Join-Path $solution.FullName 'docker-compose.yml')) {
                Test-Contains $readme $text '## Docker Compose' "missing section 'Docker Compose'"
            }

            $hasWebApi = @(Get-ChildItem -Path $solution.FullName -Filter '*.csproj' -Recurse -File |
                Where-Object { (Get-FileText $_.FullName) -match 'Sdk="Microsoft\.NET\.Sdk\.Web"' }).Count -gt 0

            if ($hasWebApi) {
                Test-Contains $readme $text '## Web API' "missing section 'Web API'"
            }
        }
    }
    else {
        Add-Problem $solution.FullName 'solution without README.md'
    }

    if (Test-Path $overview) {
        $text = Get-FileText $overview
        Test-Contains $overview $text '](../README.md)' 'missing link to ../README.md'
        Test-RepositoryDocumentation $overview $text
    }
    else {
        Add-Problem $solution.FullName 'solution without Docs/Overview.md'
    }

    if ($placeholderSolutions -notcontains $solution.Name) {
        foreach ($file in @('Dependencies.md', 'Interfaces.md', 'DomainModel.md', 'Configuration.md', 'Design/Overview.md')) {
            if ((Test-Path (Join-Path $solution.FullName "Docs/$file")) -eq $false) {
                Add-Problem $solution.FullName "solution without Docs/$file"
            }
        }
    }

    $solutionDocs = Join-Path $solution.FullName 'Docs'

    if (Test-Path $solutionDocs) {
        foreach ($folder in Get-ChildItem -Path $solutionDocs -Directory -Recurse) {
            if ((Get-RelativePath $folder.FullName) -match '/Docs/Assets(/|$)') {
                continue
            }

            if ((Test-Path (Join-Path $folder.FullName 'Overview.md')) -eq $false) {
                Add-Problem $folder.FullName 'documentation content folder without Overview.md'
            }
        }
    }
}


#------------------------------------------------------------------------------
# Projects
#------------------------------------------------------------------------------
foreach ($project in Get-RepositoryFiles '*.csproj') {
    $folder = $project.Directory.FullName
    $readme = Join-Path $folder 'README.md'

    if (Test-Path (Join-Path $folder 'Docs')) {
        Add-Problem $folder 'project with its own Docs folder, move the files to the Docs/Assets folder of the solution'
    }

    if ((Test-Path $readme) -eq $false) {
        Add-Problem $project.FullName 'project without README.md'
        continue
    }

    $text = Get-FileText $readme
    Test-Contains $readme $text '](../README.md)' 'missing link to the solution README.md'
    Test-RepositoryDocumentation $readme $text

    if ($project.BaseName.EndsWith('.UnitTest')) {
        Test-Contains $readme $text '## Unit Test' "missing section 'Unit Test'"
    }

    if ($project.BaseName.EndsWith('.IntegrationTest')) {
        Test-Contains $readme $text '## Integration Test' "missing section 'Integration Test'"
    }
}


#------------------------------------------------------------------------------
# Diagrams
#------------------------------------------------------------------------------
foreach ($diagram in Get-RepositoryFiles '*.drawio*') {
    # Skip the backup files of the draw.io editor (e.g. ".$Name.drawio.svg.bkp").
    if ($diagram.Name.StartsWith('.$') -or $diagram.Name.EndsWith('.bkp')) {
        continue
    }

    if ((Get-RelativePath $diagram.FullName) -notmatch '(^|/)Docs/Assets/') {
        Add-Problem $diagram.FullName 'diagram outside of a Docs/Assets folder'
    }
}


#------------------------------------------------------------------------------
# Links and anchors
#------------------------------------------------------------------------------
$anchorCache = @{}
$markdownFiles = @(Get-RepositoryFiles '*.md')

foreach ($file in $markdownFiles) {
    # Code blocks and inline code contain examples, not links.
    $text = [regex]::Replace((Get-FileText $file.FullName), '(?s)```.*?```', '')
    $text = [regex]::Replace($text, '`[^`\r\n]*`', '')

    foreach ($match in [regex]::Matches($text, '\]\(([^)\s]+)\)')) {
        $link = $match.Groups[1].Value

        if ($link -match '^(https?|mailto):') {
            continue
        }

        $parts = $link.Split('#', 2)
        $linkPath = [System.Uri]::UnescapeDataString($parts[0])
        $anchor = $null

        if ($parts.Count -gt 1) {
            $anchor = $parts[1]
        }

        if ($linkPath -eq '') {
            $target = $file.FullName
        }
        elseif ($linkPath.StartsWith('/')) {
            $target = Join-Path $Root $linkPath.TrimStart('/')
        }
        else {
            $target = Join-Path $file.Directory.FullName $linkPath
        }

        if ((Test-Path -LiteralPath $target) -eq $false) {
            Add-Problem $file.FullName "broken link '$link'"
            continue
        }

        if ($null -ne $anchor -and $target.EndsWith('.md')) {
            $key = (Resolve-Path -LiteralPath $target).Path

            if ($anchorCache.ContainsKey($key) -eq $false) {
                $anchorCache[$key] = Get-HeadingAnchors $key
            }

            if ($anchorCache[$key].Contains($anchor) -eq $false) {
                Add-Problem $file.FullName "broken anchor '$link'"
            }
        }
    }
}


#------------------------------------------------------------------------------
# Result
#------------------------------------------------------------------------------
Write-Host "Checked $($solutionFolders.Count) solutions and $($markdownFiles.Count) Markdown files."

if ($problems.Count -gt 0) {
    foreach ($problem in $problems) {
        Write-Host "  $problem" -ForegroundColor Red
    }

    Write-Host "Documentation check failed with $($problems.Count) problem(s). See Docs/Documentation.md for the rules." -ForegroundColor Red
    exit 1
}

Write-Host 'Documentation check passed.' -ForegroundColor Green
exit 0
