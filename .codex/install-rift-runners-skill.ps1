[CmdletBinding()]
param(
	[switch]$Force
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

$SkillName = "rift-runners-unity"
$CodexFolderName = ".codex"
$SkillsFolderName = "skills"
$SkillSourceRoot = Join-Path $PSScriptRoot "skill-src"
$SkillSourcePath = Join-Path $SkillSourceRoot $SkillName
$CodexHomeRoot = if ($env:CODEX_HOME) { $env:CODEX_HOME } else { Join-Path $env:USERPROFILE $CodexFolderName }
$DestinationSkillsRoot = Join-Path $CodexHomeRoot $SkillsFolderName
$DestinationSkillPath = Join-Path $DestinationSkillsRoot $SkillName
$RequiredRelativePaths = @(
	"SKILL.md",
	"agents/openai.yaml"
)

function Assert-DirectoryExists {
	param(
		[string]$Path,
		[string]$Description
	)

	if (-not (Test-Path -LiteralPath $Path -PathType Container)) {
		throw "$Description was not found: $Path"
	}
}

function Assert-RequiredFilesExist {
	param(
		[string]$BasePath,
		[string[]]$RequiredPaths
	)

	foreach ($relativePath in $RequiredPaths) {
		$fullPath = Join-Path $BasePath $relativePath
		if (-not (Test-Path -LiteralPath $fullPath -PathType Leaf)) {
			throw "Required skill file was not found: $fullPath"
		}
	}
}

function Get-RelativePathString {
	param(
		[string]$BasePath,
		[string]$TargetPath
	)

	$normalizedBasePath = (Resolve-Path -LiteralPath $BasePath).Path.TrimEnd('\') + '\'
	$normalizedTargetPath = (Resolve-Path -LiteralPath $TargetPath).Path
	$baseUri = [System.Uri]$normalizedBasePath
	$targetUri = [System.Uri]$normalizedTargetPath
	$relativeUri = $baseUri.MakeRelativeUri($targetUri)
	return [System.Uri]::UnescapeDataString($relativeUri.ToString()).Replace('/', '\')
}

function Remove-StaleEntries {
	param(
		[string]$SourceRoot,
		[string]$DestinationRoot
	)

	if (-not (Test-Path -LiteralPath $DestinationRoot -PathType Container)) {
		return
	}

	$sourceEntries = Get-ChildItem -LiteralPath $SourceRoot -Recurse -Force
	$sourceRelativePaths = New-Object "System.Collections.Generic.HashSet[string]" ([System.StringComparer]::OrdinalIgnoreCase)

	foreach ($entry in $sourceEntries) {
		$relativePath = Get-RelativePathString -BasePath $SourceRoot -TargetPath $entry.FullName
		[void]$sourceRelativePaths.Add($relativePath)
	}

	$destinationEntries = Get-ChildItem -LiteralPath $DestinationRoot -Recurse -Force | Sort-Object FullName -Descending
	foreach ($entry in $destinationEntries) {
		$relativePath = Get-RelativePathString -BasePath $DestinationRoot -TargetPath $entry.FullName
		if (-not $sourceRelativePaths.Contains($relativePath)) {
			Remove-Item -LiteralPath $entry.FullName -Recurse -Force
		}
	}
}

Assert-DirectoryExists -Path $SkillSourceRoot -Description "Skill source root"
Assert-DirectoryExists -Path $SkillSourcePath -Description "Skill source directory"
Assert-RequiredFilesExist -BasePath $SkillSourcePath -RequiredPaths $RequiredRelativePaths

if (Test-Path -LiteralPath $DestinationSkillsRoot -PathType Leaf) {
	throw "Destination skills root points to a file instead of a directory: $DestinationSkillsRoot"
}

if (-not (Test-Path -LiteralPath $DestinationSkillsRoot -PathType Container)) {
	New-Item -ItemType Directory -Path $DestinationSkillsRoot -Force | Out-Null
}

if (Test-Path -LiteralPath $DestinationSkillPath -PathType Leaf) {
	throw "Destination skill path points to a file instead of a directory: $DestinationSkillPath"
}

if ($Force -and (Test-Path -LiteralPath $DestinationSkillPath -PathType Container)) {
	Get-ChildItem -LiteralPath $DestinationSkillPath -Force | Remove-Item -Recurse -Force
}

if (-not (Test-Path -LiteralPath $DestinationSkillPath -PathType Container)) {
	New-Item -ItemType Directory -Path $DestinationSkillPath -Force | Out-Null
}

$SourceContentsPath = Join-Path $SkillSourcePath "*"
Copy-Item -Path $SourceContentsPath -Destination $DestinationSkillPath -Recurse -Force
Remove-StaleEntries -SourceRoot $SkillSourcePath -DestinationRoot $DestinationSkillPath
Assert-RequiredFilesExist -BasePath $DestinationSkillPath -RequiredPaths $RequiredRelativePaths

Write-Host "Synced skill '$SkillName' to '$DestinationSkillPath'."
