@echo off
cls 
echo Make.Packages.bat
echo Version 2017.08.27a
echo.
echo Copy files together...
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
echo Make Win-x64.zip
del .\Packages\Win-x64.zip
.\Make\7za a .\Packages\Win-x64.zip .\Packages\Win-x64.Archive\*.*
.\Make\7za a .\Packages\Win-x64.zip .\Packages\Win-x64.Archive\certutil
echo.
echo Make Win-x64.exe
del .\Packages\Win-x64.exe
rem "C:\Program Files (x86)\NSIS\makensis.exe" .\Make\Win-x64.nsi
echo.
echo Move EXE to .\Packages\...
move .\Make\Win-x64.exe .\Packages\
echo.
echo Make Win-x86.zip
del .\Packages\Win-x86.zip
.\Make\7za a .\Packages\Win-x86.zip .\Packages\Win-x86.Archive\*.*
.\Make\7za a .\Packages\Win-x86.zip .\Packages\Win-x86.Archive\certutil
echo.
echo Make Win-x86.exe
del .\Packages\Win-x86.exe
"C:\Program Files (x86)\NSIS\makensis.exe" .\Make\Win-x86.nsi
echo.
echo Move EXE to .\Packages\...
move .\Make\Win-x86.exe .\Packages\
