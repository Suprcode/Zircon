param(
    [string]$RepositoryRoot = (Split-Path $PSScriptRoot -Parent)
)

$ErrorActionPreference = 'Stop'
$RepositoryRoot = [IO.Path]::GetFullPath($RepositoryRoot)
$rootPrefix = $RepositoryRoot.TrimEnd('\', '/') + [IO.Path]::DirectorySeparatorChar
$ignored = '(^|/)(bin|obj|artifacts|tmp|\.build|\.git|node_modules)(/|$)'
$sourceRoots = 'Client|ServerLibrary|ServerCore|Server|LibraryCore|RenderingCore|LibraryEditor|ImageManager|PluginCore|PluginStandalone|Launcher|Patcher|PatchManager|Tools|Tests|docs|Components'
$extensions = 'cs|csproj|sln|md|cmd|json|config|ps1'
$script:failures = 0
$script:links = 0
$script:paths = 0
$anchorCache = @{}

# Preserve line numbers while excluding fenced examples and HTML comments.
function Get-ProseLines([string]$Path) {
    $content = [IO.File]::ReadAllText($Path)
    $content = [regex]::Replace($content, '(?s)<!--.*?-->', {
        param($match)
        [regex]::Replace($match.Value, '[^\r\n]', ' ')
    })
    $fence = ''
    foreach ($line in ($content -split '\r?\n')) {
        if ($fence) {
            if ($line -match ('^\s{0,3}' + [regex]::Escape($fence.Substring(0, 1)) + '{' + $fence.Length + ',}\s*$')) { $fence = '' }
            ''
        } elseif ($line -match '^\s{0,3}(`{3,}|~{3,})') {
            $fence = $Matches[1]
            ''
        } else { $line }
    }
}

function Get-Anchors([string]$Path) {
    if ($anchorCache.ContainsKey($Path)) { return $anchorCache[$Path] }
    $anchors = [Collections.Generic.HashSet[string]]::new([StringComparer]::Ordinal)
    foreach ($line in @(Get-ProseLines $Path)) {
        if ($line -notmatch '^\s{0,3}#{1,6}\s+(.+?)\s*#*\s*$') { continue }
        $heading = $Matches[1]
        $heading = [regex]::Replace($heading, '\[([^\]]+)\]\([^)]*\)', '$1')
        $heading = [regex]::Replace($heading, '<[^>]+>', '')
        $heading = [Net.WebUtility]::HtmlDecode($heading).ToLowerInvariant()
        $slug = [regex]::Replace($heading, '[^\p{L}\p{M}\p{N}_\-\s]', '') -replace '\s', '-'
        $unique = $slug
        $suffix = 0
        while ($anchors.Contains($unique)) { $suffix++; $unique = "$slug-$suffix" }
        [void]$anchors.Add($unique)
    }
    $anchorCache[$Path] = $anchors
    return ,$anchors
}

function Report-Failure([string]$Kind, [string]$Document, [int]$Line, [string]$Reference, [string]$Reason) {
    $script:failures++
    Write-Output "${Kind}: ${Document}:${Line}`n  -> $Reference`n  $Reason"
}

function Check-Link([string]$Reference, [string]$Document, [int]$Line, [string]$DocumentPath) {
    if ($Reference -match '^(?:[a-zA-Z][a-zA-Z0-9+.-]*:|//)') { return }
    $parts = $Reference -split '#', 2
    $relative = [Uri]::UnescapeDataString(($parts[0] -split '\?', 2)[0]).Replace('\', '/')
    if ($relative -match $ignored) { return }
    if (-not $relative) { $target = $DocumentPath }
    elseif ($relative.StartsWith('/')) { $target = [IO.Path]::GetFullPath((Join-Path $RepositoryRoot $relative.TrimStart('/'))) }
    else { $target = [IO.Path]::GetFullPath((Join-Path (Split-Path $DocumentPath) $relative)) }
    $script:links++
    if (-not $target.StartsWith($rootPrefix, [StringComparison]::OrdinalIgnoreCase)) {
        Report-Failure 'Broken documentation link' $Document $Line $Reference 'Target is outside the repository.'
    } elseif (-not (Test-Path -LiteralPath $target -PathType Leaf)) {
        Report-Failure 'Broken documentation link' $Document $Line $Reference 'Target file does not exist.'
    } elseif ($parts.Length -eq 2 -and $parts[1] -and [IO.Path]::GetExtension($target) -eq '.md') {
        $anchor = [Uri]::UnescapeDataString($parts[1])
        if (-not (Get-Anchors $target).Contains($anchor)) {
            Report-Failure 'Broken documentation anchor' $Document $Line $Reference 'Heading anchor does not exist.'
        }
    }
}

$documents = @((Join-Path $RepositoryRoot 'AGENTS.md'))
$documents += @(Get-ChildItem -LiteralPath (Join-Path $RepositoryRoot 'docs') -Recurse -File -Filter '*.md' |
    Where-Object { $_.FullName.Substring($rootPrefix.Length).Replace('\', '/') -notmatch $ignored } |
    Sort-Object FullName | ForEach-Object { $_.FullName })
foreach ($documentPath in $documents) {
    $document = $documentPath.Substring($rootPrefix.Length).Replace('\', '/')
    $lineNumber = 0
    foreach ($line in @(Get-ProseLines $documentPath)) {
        $lineNumber++
        # Inline code is checked separately; do not interpret examples inside it as links.
        $withoutCode = [regex]::Replace($line, '`+[^`]*`+', '')
        foreach ($match in [regex]::Matches($withoutCode, '\[[^\]]*\]\(\s*(?:<(?<url>[^>]+)>|(?<url>[^\s)]+))(?:\s+"[^"]*")?\s*\)')) {
            Check-Link $match.Groups['url'].Value $document $lineNumber $documentPath
        }
        # Reference-style link definitions are also validated, whether used or not.
        if ($withoutCode -match '^\s{0,3}\[[^\]]+\]:\s*(?:<(?<url>[^>]+)>|(?<url>\S+))') {
            Check-Link $Matches['url'] $document $lineNumber $documentPath
        }
        foreach ($match in [regex]::Matches($line, '(?<!`)`([^`]+)`(?!`)')) {
            foreach ($segment in ($match.Groups[1].Value -split ';')) {
                $value = $segment.Trim().Replace('\', '/')
                if ($value -match '[<>*?{}]|\.\.\.|(^|/)(example|placeholder)(/|\.)') { continue }
                # Only established repository roots or explicit root documents/solutions.
                # Optional ': Method' or arrow suffix is navigation prose, not a symbol assertion.
                if ($value -match "^/?(?<path>(?:(?:$sourceRoots)/[^:]+?\.(?:$extensions)|AGENTS\.md|[^/]+\.sln))(?=\s*(?::|\u2192|$))") {
                    $path = $Matches['path']
                    if ($path -match $ignored) { continue }
                    $script:paths++
                    if (-not (Test-Path -LiteralPath (Join-Path $RepositoryRoot $path) -PathType Leaf)) {
                        Report-Failure 'Missing referenced source file' $document $lineNumber $path 'Repository file does not exist.'
                    }
                }
            }
        }
    }
}
Write-Output "Documentation check: $($documents.Count) documents, $script:links links, $script:paths file references, $script:failures failures."
if ($script:failures) { exit 1 }
exit 0
