; example1.nsi
;
; This script is perhaps one of the simplest NSIs you can make. All of the
; optional settings are left to their default settings. The installer simply 
; prompts the user asking them where to install, and drops a copy of example1.nsi
; there. 

;--------------------------------
!define APPNAME "LV-Crew HostsManager"
!define COMPANYNAME "LV-Crew"
; The name of the installer
Name ${APPNAME}"
; The file to write
OutFile "LVC.HM.Win-x86.exe"

; The default installation directory
InstallDir "$PROGRAMFILES\LV-Crew\HostsManager"

; Request application privileges for Windows Vista
RequestExecutionLevel admin
LicenseData "LVC.HM.Win-x86.Setup\license.rtf"

page license
page directory
Page instfiles
;--------------------------------

; The stuff to install
Section "install" ;No components page, name is not important

  ; Set output path to the installation directory.
  SetOutPath $INSTDIR
  writeUninstaller "$INSTDIR\uninstall.exe"
  createShortCut "$SMPROGRAMS\LV-Crew\HostsManager\LV-Crew HostsManager.lnk" "$INSTDIR\LV-Crew.HostsManager.exe" "" "$INSTDIR\logo.ico"
  ; Put file there
  File "LVC.HM.Win-x86.Setup\LV-Crew.HostsManager.exe"
  File "LVC.HM.Win-x86.Setup\cert.pem"
  File "LVC.HM.Win-x86.Setup\logo.ico"
  SetOutPath "$INSTDIR\certutil"
  File "LVC.HM.Win-x86.Setup\certutil\*.*"
  
SectionEnd ; end the section

section "uninstall"
 
	# Remove Start Menu launcher
	delete "$PROGRAMFILES\LV-Crew\HostsManager\LV-Crew HostsManager.lnk"
	# Try to remove the Start Menu folder - this will only happen if it is empty
	rmDir "$SMPROGRAMS\${COMPANYNAME}"
 
	# Remove files
	delete $INSTDIR\app.exe
	delete $INSTDIR\logo.ico
 
	# Always delete uninstaller as the last action
	delete $INSTDIR\uninstall.exe
 
	# Try to remove the install directory - this will only happen if it is empty
	rmDir $INSTDIR
 
	# Remove uninstaller information from the registry
	DeleteRegKey HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${COMPANYNAME} ${APPNAME}"
sectionEnd