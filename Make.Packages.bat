// @echo off


// Make.Packages.bat
// Version 2017.08.05a


// Copy files together
copy .\Branding\Branding.ini .\Packages\Win-x64.Archive
copy .\Branding\Branding.ini .\Packages\Win-x64.Setup
copy .\Branding\Branding.ini .\Packages\Win-x86.Archive
copy .\Branding\Branding.ini .\Packages\Win-x86.Setup

copy .\Branding\LV-Crew.HostsManager.Logo.ico .\Packages\Win-x64.Archive
copy .\Branding\LV-Crew.HostsManager.Logo.ico .\Packages\Win-x64.Setup
copy .\Branding\LV-Crew.HostsManager.Logo.ico .\Packages\Win-x86.Archive
copy .\Branding\LV-Crew.HostsManager.Logo.ico .\Packages\Win-x86.Setup

copy .\Branding\LV-Crew.HostsManager.Logo.png .\Packages\Win-x64.Archive\LV-Crew.HostsManager.Logo.png
copy .\Branding\LV-Crew.HostsManager.Logo.png .\Packages\Win-x64.Setup\LV-Crew.HostsManager.Logo.png
copy .\Branding\LV-Crew.HostsManager.Logo.png .\Packages\Win-x86.Archive\LV-Crew.HostsManager.Logo.png
copy .\Branding\LV-Crew.HostsManager.Logo.png .\Packages\Win-x86.Setup\LV-Crew.HostsManager.Logo.png

copy .\Readme.txt .\Packages\Win-x64.Archive\Readme.txt
copy .\Readme.txt .\Packages\Win-x64.Setup\Readme.txt
copy .\Readme.txt .\Packages\Win-x86.Archive\Readme.txt
copy .\Readme.txt .\Packages\Win-x86.Setup\Readme.txt

copy .\License.rtf .\Packages\Win-x64.Archive\License.rtf
copy .\License.rtf .\Packages\Win-x64.Setup\License.rtf
copy .\License.rtf .\Packages\Win-x86.Archive\License.rtf
copy .\License.rtf .\Packages\Win-x86.Setup\License.rtf


// Make Packages
cd Packages

// Make Win-x64.zip
del Win-x64.zip
7za a Win-x64.zip .\Win-x64.Archive\*.*
7za a Win-x64.zip .\Win-x64.Archive\certutil

// Make Win-x64.exe
// "C:\Program Files (x86)\NSIS\makensis.exe" .\Win-x64.nsi

// Make Win-x86.zip
del Win-x86.zip
7za a Win-x86.zip .\Win-x86.Archive\*.*
7za a Win-x86.zip .\Win-x86.Archive\certutil

// Make Win-x86.exe
"C:\Program Files (x86)\NSIS\makensis.exe" .\Win-x86.nsi

cd ..
