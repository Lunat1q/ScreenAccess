$cloned = $False
if (Test-Path ".git")
{
	$cloned = $True
	Write-Host "Repo is already cloned"
}

Set-ExecutionPolicy Bypass -Scope Process -Force; iex ((New-Object System.Net.WebClient).DownloadString('https://chocolatey.org/install.ps1'))

choco install git.install -y
choco install visualstudio2019buildtools --package-parameters "--passive --locale en-US --includeRecomended --quiet" -y 
choco install visualstudio2019-workload-manageddesktopbuildtools --package-parameters "--passive --locale en-US --includeRecomended --quiet" -y
choco install visualstudio2019-workload-netcorebuildtools --package-parameters "--passive --locale en-US --includeRecomended --quiet" -y
choco install nuget.commandline -y
choco install dotnetcore-sdk -y
choco install netfx-4.5.2-devpack -y
choco install netfx-4.6.2-devpack -y

if (!($cloned))
{
	if (Test-Path "ScreenAccess")
	{
		cd "ScreenAccess"
	}
	else
	{
		git clone "https://github.com/Lunat1q/ScreenAccess" -b "weapons_test"
		cd "ScreenAccess"
	}
}

git pull

git submodule init
git submodule update
& ".\buildMe_Adm.cmd"

