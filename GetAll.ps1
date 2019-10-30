$cloned = $False
if (Test-Path ".git")
{
	$cloned = $True
	Write-Host "Repo is already cloned"
}

Set-ExecutionPolicy Bypass -Scope Process -Force; iex ((New-Object System.Net.WebClient).DownloadString('https://chocolatey.org/install.ps1'))

choco install git.install -y
choco install visualstudio2019buildtools -y --package-parameters "--includeRecommended --passive --locale en-US"
choco install nuget.commandline -y
choco install dotnetcore-sdk -y
choco install netfx-4.5-devpack -y
choco install netfx-4.5.2-devpack -y
choco install netfx-4.6.2-devpack -y

if (!($cloned))
{
	git clone "https://github.com/Lunat1q/ScreenAccess" -b "weapons_test"
	cd "ScreenAccess"
}

git submodule init
git submodule update
& ".\buildMe_Adm.cmd"

