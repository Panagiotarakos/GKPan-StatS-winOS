@echo off
echo Building GKPanStats single-file...
dotnet publish GKPanStats\GKPanStats.csproj -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -o publish
echo.
echo Done! GKPanStats.exe is in the "publish" folder.
pause
