@echo off
cls 
echo Make.Packages.bat
echo.
echo Version: 2017.10.01b
echo.
echo.
echo Compile project in x86...
rem echo.
rem echo Compile Debug x86...
rem echo.
rem "%SystemRoot%\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe" /maxcpucount /property:Platform=x86
rem echo.
rem pause
rem echo.
rem echo Compile Release x86...
rem echo.
rem "%SystemRoot%\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe" /maxcpucount /property:Configuration=Release /property:Platform=x86
rem echo.
rem pause
echo.
echo Compile Debug x86...
echo.
"%PROGRAMFILES(X86)%\Microsoft Visual Studio\2017\Community\MSBuild\15.0\Bin\MSBuild.exe" /maxcpucount /property:Platform=x86
echo.
pause
echo.
echo Compile Release x86...
echo.
"%PROGRAMFILES(X86)%\Microsoft Visual Studio\2017\Community\MSBuild\15.0\Bin\MSBuild.exe" /maxcpucount /property:Configuration=Release /property:Platform=x86
echo.
pause
echo.
echo.
echo Compile project in x64...
rem echo.
rem echo Compile Debug x64...
rem echo.
rem "%SystemRoot%\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe" /maxcpucount /property:Platform=x64
rem echo.
rem pause
rem echo.
rem echo Compile Release x64...
rem echo.
rem "%SystemRoot%\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe" /maxcpucount /property:Configuration=Release /property:Platform=x64
rem echo.
rem pause
echo.
echo Compile Debug x64...
echo.
"%PROGRAMFILES(X86)%\Microsoft Visual Studio\2017\Community\MSBuild\15.0\Bin\MSBuild.exe" /maxcpucount /property:Platform=x64
echo.
pause
echo.
echo Compile Release x64...
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
echo.
echo copy .\bin\x86\Release\HostsManager.exe .\Packages\Win.Setup\LV-Crew.HostsManager32.exe
copy .\bin\x86\Release\HostsManager.exe .\Packages\Win.Setup\LV-Crew.HostsManager32.exe
echo.
echo copy .\bin\x64\Release\HostsManager.exe .\Packages\Win-x64.Archive\LV-Crew.HostsManager.exe
copy .\bin\x64\Release\HostsManager.exe .\Packages\Win-x64.Archive\LV-Crew.HostsManager.exe
echo.
echo copy .\bin\x64\Release\HostsManager.exe .\Packages\Win.Setup\LV-Crew.HostsManager64.exe
copy .\bin\x64\Release\HostsManager.exe .\Packages\Win.Setup\LV-Crew.HostsManager64.exe
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
echo copy .\Branding\*.* .\Packages\Win-x86.Archive\
copy .\Branding\*.* .\Packages\Win-x86.Archive\
echo.
echo copy .\Branding\*.* .\Packages\Win.Setup\
copy .\Branding\*.* .\Packages\Win.Setup\
echo.
echo copy .\README.md .\Packages\Win-x64.Archive\
copy .\README.md .\Packages\Win-x64.Archive\
echo.
echo copy .\README.md .\Packages\Win-x86.Archive\
copy .\README.md .\Packages\Win-x86.Archive\
echo.
echo copy .\README.md .\Packages\Win.Setup\
copy .\README.md .\Packages\Win.Setup\
echo.
echo copy .\docs\readme.html .\bin\x86\Debug\Readme.html
copy .\docs\readme.html .\bin\x86\Debug\Readme.html
echo.
echo copy .\docs\readme.html .\bin\x64\Debug\Readme.html
copy .\docs\readme.html .\bin\x64\Debug\Readme.html
echo.
echo copy .\docs\readme.html .\bin\x86\Release\Readme.html
copy .\docs\readme.html .\bin\x86\Release\Readme.html
echo.
echo copy .\docs\readme.html .\bin\x64\Release\Readme.html
copy .\docs\readme.html .\bin\x64\Release\Readme.html
echo.
echo copy .\docs\readme.html .\Packages\Win-x64.Archive\Readme.html
copy .\docs\readme.html .\Packages\Win-x64.Archive\Readme.html
echo.
echo copy .\docs\readme.html .\Packages\Win-x86.Archive\Readme.html
copy .\docs\readme.html .\Packages\Win-x86.Archive\Readme.html
echo.
echo copy .\docs\readme.html .\Packages\Win.Setup\Readme.html
copy .\docs\readme.html .\Packages\Win.Setup\Readme.html
echo.
echo copy .\License.rtf .\Packages\Win-x64.Archive\
copy .\License.rtf .\Packages\Win-x64.Archive\
echo.
echo copy .\License.rtf .\Packages\Win-x86.Archive\
copy .\License.rtf .\Packages\Win-x86.Archive\
echo.
echo copy .\License.rtf .\Packages\Win.Setup\
copy .\License.rtf .\Packages\Win.Setup\
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
echo --- All done! ---
echo.
echo.
pause
