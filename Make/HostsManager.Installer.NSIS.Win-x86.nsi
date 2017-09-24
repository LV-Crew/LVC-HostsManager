; HostsManager.Installer.NSIS.Win-x86.nsi
; Version: 2017.09.24a

;--------------------------------

!define COMPANYNAME "LV-Crew"
!define APPNAME "LV-Crew HostsManager"

!define ARP "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APPNAME}"
!include "FileFunc.nsh"

; The name of the installer
Name "${APPNAME}"

; The file to write
OutFile "Win-x86.NSIS.exe"

; The default installation directory
InstallDir "$PROGRAMFILES32\${COMPANYNAME}\HostsManager"

; Request application privileges for Windows Vista
RequestExecutionLevel admin

;--------------------------------

; Pages

LicenseData "..\Packages\Win-x86.Setup\License.rtf"

Page license
Page directory
Page instfiles

;--------------------------------

; The stuff to install

Section "install" ;No components page, name is not important
	
	; Set output path to the installation directory.
	SetOutPath $INSTDIR
	
	WriteUninstaller "$INSTDIR\LV-Crew.Hostsmanager.Uninstaller.exe"
	
	; Put file there
	File /r "..\Packages\Win-x86.Setup\*.*"
	
	; Delete old files
	Delete "$INSTDIR\Uninstall.exe"
	Delete "$INSTDIR\uninstall.dat"
	Delete "$INSTDIR\uninstall_l.ifl"
	
	CreateShortCut "$SMPROGRAMS\${COMPANYNAME}\HostsManager\LV-Crew HostsManager.lnk" "$INSTDIR\LV-Crew.HostsManager.exe" "" "$INSTDIR\LV-Crew.HostsManager.Icon.ico"
	
	WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APPNAME}" \
		"DisplayName" "LV-Crew HostsManager"
	WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APPNAME}" \
		"UninstallString" "$\"$INSTDIR\LV-Crew.Hostsmanager.Uninstaller.exe$\""
	WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APPNAME}" \
		"QuietUninstallString" "$\"$INSTDIR\LV-Crew.Hostsmanager.Uninstaller.exe$\" /S"
	
	${GetSize} "$INSTDIR" "/S=0K" $0 $1 $2
	IntFmt $0 "0x%08X" $0
	WriteRegDWORD HKLM "${ARP}" "EstimatedSize" "$0"
	
SectionEnd ; end the section

;--------------------------------

; The stuff to uninstall

Section "uninstall"
	
	# Remove Start Menu launcher
	Delete "$SMPROGRAMS\${COMPANYNAME}\HostsManager\LV-Crew HostsManager.lnk"
	
	# Try to remove the Start Menu folder - this will only happen if it is empty
	RMDir "$SMPROGRAMS\${COMPANYNAME}"
	
	;Remove Files
	RMDir /r "$INSTDIR"
	
	# Try to remove the Company folder - this will only happen if it is empty
	RMDir "$PROGRAMFILES32\${COMPANYNAME}"
	
	# Remove uninstaller information from the registry
	DeleteRegKey HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APPNAME}"
	
SectionEnd