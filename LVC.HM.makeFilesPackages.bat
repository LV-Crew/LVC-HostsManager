@echo off

rem LVC.HM.makeFilesPackages.bat

copy .\Branding\LV-Crew.HostsManager.Logo.ico .\LVC.HM.Packages\LVC.HM.Win-x64.Archive
copy .\Branding\LV-Crew.HostsManager.Logo.ico .\LVC.HM.Packages\LVC.HM.Win-x64.Setup
copy .\Branding\LV-Crew.HostsManager.Logo.ico .\LVC.HM.Packages\LVC.HM.Win-x86.Archive
copy .\Branding\LV-Crew.HostsManager.Logo.ico .\LVC.HM.Packages\LVC.HM.Win-x86.Setup

copy .\Branding\LV-Crew.HostsManager.Logo.png .\LVC.HM.Packages\LVC.HM.Win-x64.Archive\LV-Crew.HostsManager.Logo1.png
copy .\Branding\LV-Crew.HostsManager.Logo.png .\LVC.HM.Packages\LVC.HM.Win-x64.Setup\LV-Crew.HostsManager.Logo1.png
copy .\Branding\LV-Crew.HostsManager.Logo.png .\LVC.HM.Packages\LVC.HM.Win-x86.Archive\LV-Crew.HostsManager.Logo1.png
copy .\Branding\LV-Crew.HostsManager.Logo.png .\LVC.HM.Packages\LVC.HM.Win-x86.Setup\LV-Crew.HostsManager.Logo1.png

copy .\LVC.HM.Readme.txt .\LVC.HM.Packages\LVC.HM.Win-x64.Archive\Readme.txt
copy .\LVC.HM.Readme.txt .\LVC.HM.Packages\LVC.HM.Win-x64.Setup\Readme.txt
copy .\LVC.HM.Readme.txt .\LVC.HM.Packages\LVC.HM.Win-x86.Archive\Readme.txt
copy .\LVC.HM.Readme.txt .\LVC.HM.Packages\LVC.HM.Win-x86.Setup\Readme.txt

copy .\LVC.HM.License.rtf .\LVC.HM.Packages\LVC.HM.Win-x64.Archive\License.rtf
copy .\LVC.HM.License.rtf .\LVC.HM.Packages\LVC.HM.Win-x64.Setup\License.rtf
copy .\LVC.HM.License.rtf .\LVC.HM.Packages\LVC.HM.Win-x86.Archive\License.rtf
copy .\LVC.HM.License.rtf .\LVC.HM.Packages\LVC.HM.Win-x86.Setup\License.rtf
