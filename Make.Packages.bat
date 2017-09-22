@echo off
cls 
echo Make.Packages.bat
echo Version 2017.09.22b
echo.
echo Compile project in x86...
echo.
"c:\Program Files (x86)\Microsoft Visual Studio\2017\Community\MSBuild\15.0\Bin\MSBuild.exe"
echo.
pause
echo.
rem echo Compile project in x64...
rem echo.
rem "c:\Program Files (x86)\Microsoft Visual Studio\2017\Community\MSBuild\15.0\Bin\MSBuild.exe"
rem echo.
rem pause
rem echo.
echo Copy files together...
echo.
echo copy .\bin\Debug\HostsManager.exe .\Packages\Win-x64.Archive\LV-Crew.HostsManager.exe
copy .\bin\Debug\HostsManager.exe .\Packages\Win-x64.Archive\LV-Crew.HostsManager.exe
echo copy .\bin\Debug\HostsManager.exe .\Packages\Win-x64.Setup\LV-Crew.HostsManager.exe
copy .\bin\Debug\HostsManager.exe .\Packages\Win-x64.Setup\LV-Crew.HostsManager.exe
echo copy .\bin\Debug\HostsManager.exe .\Packages\Win-x86.Archive\LV-Crew.HostsManager.exe
copy .\bin\Debug\HostsManager.exe .\Packages\Win-x86.Archive\LV-Crew.HostsManager.exe
echo copy .\bin\Debug\HostsManager.exe .\Packages\Win-x86.Setup\LV-Crew.HostsManager.exe
copy .\bin\Debug\HostsManager.exe .\Packages\Win-x86.Setup\LV-Crew.HostsManager.exe
echo.
echo copy .\Branding\*.* .\bin\Debug\
copy .\Branding\*.* .\bin\Debug\
echo.
echo copy .\Branding\*.* .\bin\Release\
copy .\Branding\*.* .\bin\Release\
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
echo Make Packages...
echo.
echo Make Win-x64.zip...
del .\Packages\Win-x64.zip
"C:\Program Files\WinZip\WZZIP.exe" -p -r .\Packages\Win-x64.zip .\Packages\Win-x64.Archive\*.*
echo.
echo Make Win-x64.7z...
del .\Packages\Win-x64.7z
.\Make\7za a .\Packages\Win-x64.7z .\Packages\Win-x64.Archive\*.*
.\Make\7za a .\Packages\Win-x64.7z .\Packages\Win-x64.Archive\certutil
echo.
echo Make Win-x64.exe...
del .\Packages\Win-x64.exe
rem "C:\Program Files (x86)\NSIS\makensis.exe" .\Make\Win-x64.nsi
echo.
echo Move EXE to .\Packages\...
move .\Make\Win-x64.exe .\Packages\
echo.
echo Make Win-x86.zip...
del .\Packages\Win-x86.zip
"C:\Program Files\WinZip\WZZIP.exe" -p -r .\Packages\Win-x86.zip .\Packages\Win-x86.Archive\*.*
echo.
echo Make Win-x86.7z...
del .\Packages\Win-x86.7z
.\Make\7za a .\Packages\Win-x86.7z .\Packages\Win-x86.Archive\*.*
.\Make\7za a .\Packages\Win-x86.7z .\Packages\Win-x86.Archive\certutil
echo.
echo Make Win-x86.exe...
del .\Packages\Win-x86.exe
"C:\Program Files (x86)\NSIS\makensis.exe" .\Make\Win-x86.nsi
echo.
echo Move EXE to .\Packages\...
move .\Make\Win-x86.exe .\Packages\
echo.
echo All done!
echo.
pause
