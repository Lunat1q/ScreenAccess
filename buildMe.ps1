function Test-RegistryValue($path, $name)
{
    $key = Get-Item -LiteralPath $path -ErrorAction SilentlyContinue
    $key -and $null -ne $key.GetValue($name, $null)
}

git pull

$vsPath = "C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\MSBuild\Current\Bin\MSBuild.exe"
if (!(Test-Path $vsPath))
{
	Write-Host "Visual Studio 2019 not found, looking for VS2017"
	$vsRegPath = "Registry::HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\Microsoft\VisualStudio\SxS\VS7"
	$vs17Path = $null
	if ((Test-RegistryValue $vsRegPath "15.0"))
	{
		$vs17Path = Get-ItemPropertyValue -Path $vsRegPath -Name "15.0"
		$vs17Path = [System.IO.Path]::Combine($vs17Path, "MSBuild\15.0\Bin\amd64\MSBuild.exe");
	}
	
	if ($vs17Path -and (Test-Path $vs17Path))
	{
		$vsPath = $vs17Path
	}
	else
	{
		$vsToolsPath = "C:\Program Files (x86)\Microsoft Visual Studio\2019\BuildTools\MSBuild\Current\Bin\MSBuild.exe"
		$vsToolsPath32 = "C:\Program Files\Microsoft Visual Studio\2019\BuildTools\MSBuild\Current\Bin\MSBuild.exe"
		if (Test-Path $vsToolsPath)
		{
			$vsPath = $vsToolsPath
		}
		elseif (Test-Path $vsToolsPath32)
		{
			$vsPath = $vsToolsPath32
		}
		else
		{
			Write-Host "Visual Studio / Tools not found, please install 2017 or 2019 from https://visualstudio.microsoft.com/ru/downloads/"
			exit -1
		}
	}
}

nuget restore

$restoreProc = Start-Process -FilePath $vsPath -ArgumentList "/Restore /p:Configuration=Release build.proj" -PassThru

$restoreProc | Wait-Process -Timeout 60 -ErrorAction SilentlyContinue
$buildProc = Start-Process -FilePath $vsPath -ArgumentList "/t:Build /p:Configuration=Release build.proj" -PassThru
$buildProc | Wait-Process -Timeout 120 -ErrorAction SilentlyContinue
if (Test-Path "TiqLauncher.ScreenAssistant\bin\Release")
{
	explorer .\TiqLauncher.ScreenAssistant\bin\Release
}