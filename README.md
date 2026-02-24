# GKPanStats

<p align="center">
  <img src="GKPanStatS.png" width="200" alt="GKPanStats Icon">
</p>

<p align="center">
  <b>Windows System Monitor - System Tray App</b>
</p>

<p align="center">
  <img src="gkpan-stats-system-tray.png" width="300" alt="System Tray">
  &nbsp;&nbsp;
  <img src="gkpan-stats-about.png" width="260" alt="About">
</p>

## Features
- CPU usage monitoring
- RAM usage monitoring
- Disk usage monitoring
- Network speed (download/upload)
- Battery status

## Supported Languages
| Language | Translator |
|----------|-----------|
| English | [PanagiotarakoS](https://pngs.gr) |
| Greek (Ελληνικά) | [PanagiotarakoS](https://pngs.gr) |
| French (Français) | [PanagiotarakoS](https://pngs.gr) |
| Italian (Italiano) | [PanagiotarakoS](https://pngs.gr) |
| German (Deutsch) | [PanagiotarakoS](https://pngs.gr) |

## Install
1. Download ZIP from GitHub
2. Open `GKPanStats.sln` in Visual Studio
3. Build & Run (F5)

## Build standalone exe
Open Command Prompt (cmd) and run:
```
cd /d <path-to-project-folder>
dotnet publish GKPanStats\GKPanStats.csproj -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -o publish
```
The standalone `GKPanStats.exe` will be in the `publish` folder.

## Author
[PanagiotarakoS](https://panagiotarakos.com) - [pngs.gr](https://pngs.gr) | [rdev.gr](https://rdev.gr)

## License
[MIT License](https://github.com/Panagiotarakos/GKPan-StatS-winOS/blob/main/LICENSE)
