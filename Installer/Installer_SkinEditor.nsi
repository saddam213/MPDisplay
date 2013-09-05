# Installer for MPDisplay++ Skins
# Created by Sa_ddam213

##------------------------------------------------------------------------------##
##-Installer\Skin Settings------------------------------------------------------##

;Skin Version
!define VERSION "0.7.8.3"
;Skin Name
!define APP_NAME "MPDSkinEditor"

;Skin Setup Build Folder Location
!define BUILD_FOLDER "C:\Users\Dev64\Desktop\MPDisplayRelease"
;Installer EXE Name
!define INSTALLER_EXE_NAME "Setup_${APP_NAME}.exe"
;Installer EXE Icon
!define MUI_ICON "${BUILD_FOLDER}\MPDicon.ico"
;Setup Image (Top Right)
!define MUI_HEADERIMAGE_BITMAP "${BUILD_FOLDER}\header.bmp"
;Setup Image (Side)
!define MUI_WELCOMEFINISHPAGE_BITMAP "${BUILD_FOLDER}\headerSide.bmp"

##------------------------------------------------------------------------------##
##------------------------------------------------------------------------------##




# Installer Stuff
!include "MUI2.nsh"
!include "FileFunc.nsh"
SetCompress force
ShowInstDetails show
Name "${APP_NAME}"
XPStyle on
RequestExecutionLevel user
InstallDir "$PROGRAMFILES\MPDisplay++\SkinEditor"
# Set Installer Output Location #
!system "md ${BUILD_FOLDER}\Packages\${APP_NAME}\${VERSION}"
OutFile "${BUILD_FOLDER}\Packages\${APP_NAME}\${VERSION}\${INSTALLER_EXE_NAME}"
!define REG_HKLM "HKLM"
!define REG_APP_PATH "Software\MPDisplay"
!define MUI_ABORTWARNING
!define MUI_HEADERIMAGE
!define MUI_HEADERIMAGE_RIGHT

!insertmacro MUI_PAGE_WELCOME
!insertmacro MUI_PAGE_DIRECTORY
!insertmacro MUI_PAGE_INSTFILES
!insertmacro MUI_PAGE_FINISH
# Languages
!insertmacro MUI_LANGUAGE "English"
!insertmacro MUI_LANGUAGE "German"
!insertmacro MUI_LANGUAGE "Dutch"

Function .onInit
    SetShellVarContext all	
	# Display the language selector #
    !insertmacro MUI_LANGDLL_DISPLAY
FunctionEnd

# Install Skin Files
Section -MainProgram
	SetOverwrite on
	SetShellVarContext all	
	# Copy Files
	SetOutPath "$INSTDIR"
	CreateDirectory "$INSTDIR"
	File /r "${BUILD_FOLDER}\SkinEditor\*.*"
SectionEnd









