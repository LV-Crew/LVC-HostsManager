; HostsManager.Installer.NSIS.Win-x86.nsi
; Version: 2017.09.24b

;--------------------------------

; Header

!include "MUI2.nsh"
!include "FileFunc.nsh"

!define COMPANYNAME "LV-Crew"
!define APPNAME "LV-Crew HostsManager"
!define FOLDERNAME "HostsManager"
!define FILENAME "LV-Crew.HostsManager"

!define MUI_ICON "..\Branding\${FILENAME}.Icon.ico"
;!define MUI_UNICON "..\Branding\${FILENAME}.Icon.ico"
!define MUI_HEADERIMAGE_BITMAP "..\Branding\Banner\${FILENAME}.Banner.bmp"
!define MUI_HEADERIMAGE_BITMAP_STRETCH AspectFitHeight
!define MUI_HEADERIMAGE_UNBITMAP "..\Branding\Banner\${FILENAME}.Banner.bmp"
!define MUI_HEADERIMAGE_UNBITMAP_STRETCH AspectFitHeight
!define MUI_BGCOLOR FFFFFF
!define ARP "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APPNAME}"

CRCCheck On

; The name of the installer
Name "${APPNAME}"

; The file to write
OutFile "Win-x86.NSIS.exe"

; The default installation directory
InstallDir "$PROGRAMFILES32\${COMPANYNAME}\${FOLDERNAME}"

; Request application privileges for Windows Vista
RequestExecutionLevel admin

;--------------------------------

; Pages Normal

;LicenseData "..\Packages\Win-x86.Setup\License.rtf"

;Page license
;Page directory
;Page instfiles

;--------------------------------

; Pages MUI

;!insertmacro MUI_PAGE_WELCOME
!insertmacro MUI_PAGE_LICENSE "..\Packages\Win-x86.Setup\License.rtf"
;!insertmacro MUI_PAGE_COMPONENTS
!insertmacro MUI_PAGE_DIRECTORY
;!insertmacro MUI_PAGE_STARTMENU pageid variable
!insertmacro MUI_PAGE_INSTFILES
;!insertmacro MUI_PAGE_FINISH

;!insertmacro MUI_UNPAGE_WELCOME
!insertmacro MUI_UNPAGE_CONFIRM
;!insertmacro MUI_UNPAGE_LICENSE "..\Packages\Win-x86.Setup\License.rtf"
;!insertmacro MUI_UNPAGE_COMPONENTS
!insertmacro MUI_UNPAGE_DIRECTORY
!insertmacro MUI_UNPAGE_INSTFILES
;!insertmacro MUI_UNPAGE_FINISH

;--------------------------------

; The stuff to install

Section "install" ;No components page, name is not important
	
	; Set output path to the installation directory.
	SetOutPath $INSTDIR
	
	WriteUninstaller "$INSTDIR\${FILENAME}.Uninstaller.exe"
	
	; Put file there
	File /r "..\Packages\Win-x86.Setup\*.*"
	
	; Delete old files
	Delete "$INSTDIR\Uninstall.exe"
	Delete "$INSTDIR\uninstall.dat"
	Delete "$INSTDIR\uninstall_l.ifl"
	
	CreateShortCut "$SMPROGRAMS\${COMPANYNAME}\${FOLDERNAME}\${APPNAME}.lnk" "$INSTDIR\${FILENAME}.exe" "" "$INSTDIR\${FILENAME}.Icon.ico"
	
	WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APPNAME}" \
		"DisplayName" "${APPNAME}"
	WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APPNAME}" \
		"UninstallString" "$\"$INSTDIR\${FILENAME}.Uninstaller.exe$\""
	WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APPNAME}" \
		"QuietUninstallString" "$\"$INSTDIR\${FILENAME}.Uninstaller.exe$\" /S"
	
	${GetSize} "$INSTDIR" "/S=0K" $0 $1 $2
	IntFmt $0 "0x%08X" $0
	WriteRegDWORD HKLM "${ARP}" "EstimatedSize" "$0"
	
SectionEnd ; end the section

;--------------------------------

; The stuff to uninstall

Section "uninstall"
	
	# Remove Start Menu launcher
	Delete "$SMPROGRAMS\${COMPANYNAME}\${FOLDERNAME}\${APPNAME}.lnk"
	
	# Try to remove the Start Menu folder - this will only happen if it is empty
	RMDir "$SMPROGRAMS\${COMPANYNAME}"
	
	;Remove Files
	RMDir /r "$INSTDIR"
	
	# Try to remove the Company folder - this will only happen if it is empty
	RMDir "$PROGRAMFILES32\${COMPANYNAME}"
	
	# Remove uninstaller information from the registry
	DeleteRegKey HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APPNAME}"
	
SectionEnd