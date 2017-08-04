@echo off

rem LVC.HM.Packages.bat

cd LVC.HM.Packages

rem Make LVC.HM.Win-x86.zip
del LVC.HM.Win-x86.zip
7za a LVC.HM.Win-x86.zip .\LVC.HM.Win-x86.Archive\*.*
7za a LVC.HM.Win-x86.zip .\LVC.HM.Win-x86.Archive\certutil

rem Make LVC.HM.Win-x86.exe
"C:\Program Files (x86)\NSIS\makensis.exe" .\LVC.HM.Win-x86.nsi

cd ..