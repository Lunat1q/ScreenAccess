$cloned = $False
if (Test-Path ".git")
{
	$cloned = $True
	Write-Host "Repo is already cloned"
}

Set-ExecutionPolicy Bypass -Scope Process -Force; iex ((New-Object System.Net.WebClient).DownloadString('https://chocolatey.org/install.ps1'))

choco install git.install -y
choco install visualstudio2019buildtools -y

if (!($cloned))
{
	git clone "https://github.com/Lunat1q/ScreenAccess"
	cd "ScreenAccess"
}

git submodule init
git submodule update
& ".\buildMe_Adm.cmd"

