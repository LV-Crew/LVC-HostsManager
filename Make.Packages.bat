@echo off
cls 
echo Make.Packages.bat
echo.
echo Version: 2017.10.01a
echo.
echo.
echo Compile project in x86...
echo.
rem "%SystemRoot%\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe" /maxcpucount /property:Platform=x86
rem echo.
rem pause
rem echo.
rem "%SystemRoot%\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe" /maxcpucount /property:Configuration=Release /property:Platform=x86
rem echo.
rem pause
rem echo.
"%PROGRAMFILES(X86)%\Microsoft Visual Studio\2017\Community\MSBuild\15.0\Bin\MSBuild.exe" /maxcpucount /property:Platform=x86
echo.
pause
echo.
"%PROGRAMFILES(X86)%\Microsoft Visual Studio\2017\Community\MSBuild\15.0\Bin\MSBuild.exe" /maxcpucount /property:Configuration=Release /property:Platform=x86
echo.
pause
echo.
echo.
echo Compile project in x64...
echo.
rem "%SystemRoot%\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe" /maxcpucount /property:Platform=x64
rem echo.
rem pause
rem echo.
rem "%SystemRoot%\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe" /maxcpucount /property:Configuration=Release /property:Platform=x64
rem echo.
rem pause
rem echo.
"%PROGRAMFILES(X86)%\Microsoft Visual Studio\2017\Community\MSBuild\15.0\Bin\MSBuild.exe" /maxcpucount /property:Platform=x64
echo.
pause
echo.
"%PROGRAMFILES(X86)%\Microsoft Visual Studio\2017\Community\MSBuild\15.0\Bin\MSBuild.exe" /maxcpucount /property:Configuration=Release /property:Platform=x64
echo.
pause
echo.
echo.
echo Copy files together...
echo.
echo copy .\bin\x86\Release\HostsManager.exe .\Packages\Win-x86.Archive\LV-Crew.HostsManager.exe
copy .\bin\x86\Release\HostsManager.exe .\Packages\Win-x86.Archive\LV-Crew.HostsManager.exe
echo copy .\bin\x86\Release\HostsManager.exe .\Packages\Win-x86.Setup\LV-Crew.HostsManager.exe
copy .\bin\x86\Release\HostsManager.exe .\Packages\Win-x86.Setup\LV-Crew.HostsManager.exe
echo.
echo copy .\bin\x64\Release\HostsManager.exe .\Packages\Win-x64.Archive\LV-Crew.HostsManager.exe
copy .\bin\x64\Release\HostsManager.exe .\Packages\Win-x64.Archive\LV-Crew.HostsManager.exe
echo copy .\bin\x64\Release\HostsManager.exe .\Packages\Win-x64.Setup\LV-Crew.HostsManager.exe
copy .\bin\x64\Release\HostsManager.exe .\Packages\Win-x64.Setup\LV-Crew.HostsManager.exe
echo.
echo copy .\Branding\*.* .\bin\x86\Debug\
copy .\Branding\*.* .\bin\x86\Debug\
echo.
echo copy .\Branding\*.* .\bin\x64\Debug\
copy .\Branding\*.* .\bin\x64\Debug\
echo.
echo copy .\Branding\*.* .\bin\x86\Release\
copy .\Branding\*.* .\bin\x86\Release\
echo.
echo copy .\Branding\*.* .\bin\x64\Release\
copy .\Branding\*.* .\bin\x64\Release\
echo.
echo copy .\Branding\*.* .\Packages\Win-x64.Archive\
copy .\Branding\*.* .\Packages\Win-x64.Archive\
echo.
echo copy .\Branding\*.* .\Packages\Win-x64.Setup\
copy .\Branding\*.* .\Packages\Win-x64.Setup\
echo.
echo copy .\Branding\*.* .\Packages\Win-x86.Archive\
copy .\Branding\*.* .\Packages\Win-x86.Archive\
echo.
echo copy .\Branding\*.* .\Packages\Win-x86.Setup\
copy .\Branding\*.* .\Packages\Win-x86.Setup\
rem echo.
rem echo copy .\Settings.xml .\Packages\Win-x64.Archive\
rem copy .\Settings.xml .\Packages\Win-x64.Archive\
rem echo copy .\Settings.xml .\Packages\Win-x64.Setup\
rem copy .\Settings.xml .\Packages\Win-x64.Setup\
rem echo copy .\Settings.xml .\Packages\Win-x86.Archive\
rem copy .\Settings.xml .\Packages\Win-x86.Archive\
rem echo copy .\Settings.xml .\Packages\Win-x86.Setup\
rem copy .\Settings.xml .\Packages\Win-x86.Setup\
echo.
echo copy .\Readme.txt .\Packages\Win-x64.Archive\
copy .\Readme.txt .\Packages\Win-x64.Archive\
echo copy .\Readme.txt .\Packages\Win-x64.Setup\
copy .\Readme.txt .\Packages\Win-x64.Setup\
echo copy .\Readme.txt .\Packages\Win-x86.Archive\
copy .\Readme.txt .\Packages\Win-x86.Archive\
echo copy .\Readme.txt .\Packages\Win-x86.Setup\
copy .\Readme.txt .\Packages\Win-x86.Setup\
echo.
echo copy .\License.rtf .\Packages\Win-x64.Archive\
copy .\License.rtf .\Packages\Win-x64.Archive\
echo copy .\License.rtf .\Packages\Win-x64.Setup\
copy .\License.rtf .\Packages\Win-x64.Setup\
echo copy .\License.rtf .\Packages\Win-x86.Archive\
copy .\License.rtf .\Packages\Win-x86.Archive\
echo copy .\License.rtf .\Packages\Win-x86.Setup\
copy .\License.rtf .\Packages\Win-x86.Setup\
echo.
pause.
echo.
echo.
echo Make Packages...
echo.
echo Make Win-x64.zip...
del .\Packages\Win-x64.zip
"%PROGRAMFILES%\WinZip\WZZIP.exe" -p -r .\Packages\Win-x64.zip .\Packages\Win-x64.Archive\*.*
echo.
echo Make Win-x64.7z...
del .\Packages\Win-x64.7z
.\Make\7za a .\Packages\Win-x64.7z .\Packages\Win-x64.Archive\*.*
.\Make\7za a .\Packages\Win-x64.7z .\Packages\Win-x64.Archive\certutil
echo.
rem echo Make Win-x64.exe...
rem del .\Packages\Win-x64.exe
rem "%PROGRAMFILES(X86)%\NSIS\makensis.exe" .\Make\HostsManager.Installer.NSIS.Win-x64.nsi
rem echo.
rem echo Move EXE to .\Packages\...
rem move .\Make\Win-x64.NSIS.exe .\Packages\
rem echo.
echo Make Win-x86.zip...
del .\Packages\Win-x86.zip
"%PROGRAMFILES%\WinZip\WZZIP.exe" -p -r .\Packages\Win-x86.zip .\Packages\Win-x86.Archive\*.*
echo.
echo Make Win-x86.7z...
del .\Packages\Win-x86.7z
.\Make\7za a .\Packages\Win-x86.7z .\Packages\Win-x86.Archive\*.*
.\Make\7za a .\Packages\Win-x86.7z .\Packages\Win-x86.Archive\certutil
echo.
pause
echo.
echo Make Win.NSIS.exe...
del .\Packages\Win.NSIS.exe
"%PROGRAMFILES(X86)%\NSIS\makensis.exe" .\Make\HostsManager.Installer.NSIS.Win.nsi
echo.
echo Move EXE to .\Packages\...
move .\Make\Win.NSIS.exe .\Packages\
echo.
echo.
echo ---> All done! <---
echo.
echo.
pause
