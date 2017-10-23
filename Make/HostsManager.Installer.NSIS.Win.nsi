; HostsManager.Installer.NSIS.Win.nsi
; Version: 2017.10.23a

;--------------------------------

; Header

!include "MUI2.nsh"
!include "FileFunc.nsh"

;!define Version "2017.10.20a"
!define Company "LV-Crew"
!define Product "HostsManager"
!define FOLDERNAME "${Product}"
!define FILENAME "${Company}.${Product}"

!define MUI_ICON "..\Branding\Logo\${FILENAME}.Logo.ico"
;!define MUI_UNICON "..\Branding\${FILENAME}.Icon.ico"
!define MUI_HEADERIMAGE_BITMAP "..\Branding\Banner\${FILENAME}.Banner.bmp"
!define MUI_HEADERIMAGE_BITMAP_STRETCH AspectFitHeight
!define MUI_HEADERIMAGE_UNBITMAP "..\Branding\Banner\${FILENAME}.Banner.bmp"
!define MUI_HEADERIMAGE_UNBITMAP_STRETCH AspectFitHeight
!define MUI_BGCOLOR FFFFFF
!define ARP "Software\Microsoft\Windows\CurrentVersion\Uninstall\${Company} ${Product}"

CRCCheck On

; The name of the installer
Name "${Company} ${Product}"

; The file to write
OutFile "Win.NSIS.exe"

; The default installation directory
InstallDir "$PROGRAMFILES32\${Company}\${FOLDERNAME}"

; Request application privileges for Windows Vista
RequestExecutionLevel admin

;--------------------------------

; Pages Normal

;LicenseData "..\Packages\Win.Setup\License.rtf"

;Page license
;Page directory
;Page instfiles

;--------------------------------

; Pages MUI

;!insertmacro MUI_PAGE_WELCOME
!insertmacro MUI_PAGE_LICENSE "..\Packages\Win.Setup\License.rtf"
;!insertmacro MUI_PAGE_COMPONENTS
!insertmacro MUI_PAGE_DIRECTORY
;!insertmacro MUI_PAGE_STARTMENU pageid variable
!insertmacro MUI_PAGE_INSTFILES
;!insertmacro MUI_PAGE_FINISH

;!insertmacro MUI_UNPAGE_WELCOME
!insertmacro MUI_UNPAGE_CONFIRM
;!insertmacro MUI_UNPAGE_LICENSE "..\Packages\Win.Setup\License.rtf"
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
	File /r "..\Packages\Win.Setup\*.*"
	
	; Delete old files
	Delete "$INSTDIR\Uninstall.exe"
	Delete "$INSTDIR\uninstall.dat"
	Delete "$INSTDIR\uninstall_l.ifl"
	
	CreateShortCut "$SMPROGRAMS\${Company}\${FOLDERNAME}\${Company} ${Product}.lnk" "$INSTDIR\${FILENAME}.exe" "" "$INSTDIR\${FILENAME}.Icon.ico"
	
	WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${Company} ${Product}" \
		"DisplayName" "${Company} ${Product}"
	WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${Company} ${Product}" \
		"UninstallString" "$\"$INSTDIR\${FILENAME}.Uninstaller.exe$\""
	WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${Company} ${Product}" \
		"QuietUninstallString" "$\"$INSTDIR\${FILENAME}.Uninstaller.exe$\" /S"
	
	${GetSize} "$INSTDIR" "/S=0K" $0 $1 $2
	IntFmt $0 "0x%08X" $0
	WriteRegDWORD HKLM "${ARP}" "EstimatedSize" "$0"
	
SectionEnd ; end the section

;--------------------------------

; The stuff to uninstall

Section "uninstall"
	
	# Remove Start Menu launcher
	Delete "$SMPROGRAMS\${Company}\${FOLDERNAME}\${Company} ${Product}.lnk"
	
	# Try to remove the Start Menu folder - this will only happen if it is empty
	RMDir "$SMPROGRAMS\${Company}"
	
	;Remove Files
	RMDir /r "$INSTDIR"
	
	# Try to remove the Company folder - this will only happen if it is empty
	RMDir "$PROGRAMFILES32\${Company}"
	
	# Remove uninstaller information from the registry
	DeleteRegKey HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${Company} ${Product}"
	
SectionEnd