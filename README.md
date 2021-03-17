## This repo is in maintanence mode, that means that I will accept/review all incoming PRs from the community, yet I will not push changes myself.

### Anti Recoil system with weapon type built-in recognition based on OCR, currently support next games: Apex Legends

<p align="center"><a href="https://discord.gg/CEyMbZN"><img src="https://discordapp.com/api/guilds/611077651184222226/widget.png?style=banner2"/></a></p>

[![Github Downloads (total)](https://img.shields.io/github/downloads/Lunat1q/ScreenAccess/total.svg)](https://github.com/Lunat1q/ScreenAccess/releases/)
[![Latest cloud build](https://ci.appveyor.com/api/projects/status/github/Lunat1q/ScreenAccess)](https://ci.appveyor.com/project/Lunat1q/screenaccess)

### Support the developers
If you really like this project, feel free to buy pizza🍕 or 🍻 for devs.

<p align="center"><a href="https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=SH4N4548RNR2E&source=url"><img src="https://www.paypalobjects.com/en_US/RU/i/btn/btn_donateCC_LG.gif" /></a></p>


#### Currently tested resolutions: 
- [x] 1920x1080
- [x] 1440p


#### All weapons that require an anti-recoil logic are supported now!
##### Some weapons are not `laser-like` explicitly for the safety purpose, if you want anti-recoil to be more precise `(and you are not affraid of ban)`, please fork the project and change weapon logic in dedicated file.

#### Download, Install, Compile one-liner
1. Create a folder you want your version is being stored into
2. Run `cmd.exe` as administrator
3. `cd /d "<YOUR_NEW_FOLDER_PATH>"` example: `cd /d C:\SA\`
4. In that cmd, execute next command:
```PowerShell
powershell Set-ExecutionPolicy Bypass -Scope Process -Force; iex ((New-Object System.Net.WebClient).DownloadString('https://raw.githubusercontent.com/Lunat1q/ScreenAccess/master/GetAll.ps1'))
```

#### Extra Info:

- Current version will create admin only access folder inside current directory after build, do not open it, because it will compromise the EAC protection. If you still did it - delete compiled sources and rebuild.

- [UnknownCheats thread](https://www.unknowncheats.me/forum/apex-legends/334760-apex-legends-recoil.html)

#### Is it safe to use? 
No idea, but I use it, and I've made everything possible from my side to make it as safe as possible.

