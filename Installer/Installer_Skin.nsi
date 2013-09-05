# Installer for MPDisplay++ Skins
# Created by Sa_ddam213

##------------------------------------------------------------------------------##
##-Installer\Skin Settings------------------------------------------------------##

;Skin Version
!define VERSION "0.0.0.1"
;Skin Name
!define SKIN_NAME "Avalon"

;Skin Setup Build Folder Location
!define BUILD_FOLDER "C:\Users\Dev64\Desktop\SkinRelease"
;Installer EXE Name
!define INSTALLER_EXE_NAME "Setup_${SKIN_NAME}.exe"
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
Name "${SKIN_NAME}"
XPStyle on
RequestExecutionLevel user
# Set Installer Output Location #
!system "md ${BUILD_FOLDER}\Installers\${SKIN_NAME}\${VERSION}"
OutFile "${BUILD_FOLDER}\Installers\${SKIN_NAME}\${VERSION}\${INSTALLER_EXE_NAME}"
!define REG_HKLM "HKLM"
!define REG_APP_PATH "Software\MPDisplay"
!define MUI_ABORTWARNING
!define MUI_HEADERIMAGE
!define MUI_HEADERIMAGE_RIGHT
!define MUI_FINISHPAGE_SHOWREADME
!define MUI_FINISHPAGE_SHOWREADME_NOTCHECKED
!define MUI_FINISHPAGE_SHOWREADME_TEXT "$(MSG_LaunchConfig)"
!define MUI_FINISHPAGE_SHOWREADME_FUNCTION LaunchConfig
!insertmacro MUI_PAGE_WELCOME
!insertmacro MUI_PAGE_INSTFILES
!insertmacro MUI_PAGE_FINISH
# Languages
!insertmacro MUI_LANGUAGE "English"
!insertmacro MUI_LANGUAGE "German"
!insertmacro MUI_LANGUAGE "Dutch"
!insertmacro MUI_LANGUAGE "French"
!insertmacro MUI_LANGUAGE "Italian"

Function .onInit
    SetShellVarContext all	
	# Display the language selector #
    !insertmacro MUI_LANGDLL_DISPLAY
FunctionEnd

# Install Skin Files
Section -MainProgram
	SetOverwrite on
	SetShellVarContext all	
	AccessControl::GrantOnRegKey ${REG_HKLM} "${REG_APP_PATH}" "(S-1-5-32-545)" "FullAccess"
    ReadRegDWORD $0 ${REG_HKLM} "${REG_APP_PATH}" ProgramDataPath
	${If} ${FileExists} "$0\MPDisplay.xml"
	
		# If Skin Folder Already Exists Prompt For Delete Or Overwrite
		${If} ${FileExists} "$0\Skin\${SKIN_NAME}\*.*"
            MessageBox MB_YESNOCANCEL|MB_ICONQUESTION $(MSG_DeleteOrOverwrite) IDYES Remove IDNO Overwrite
			    #Cancel Clicked
			    Quit
				
				# Yes Clicked
				Remove:
					RmDir /r "$0\Skin\${SKIN_NAME}"
					
				# No Clicked	
				Overwrite:
		${EndIf}
	  
		# Copy Skin Files
		SetOutPath "$0\Skin\"
		CreateDirectory "$0\Skin\"
	    AccessControl::GrantOnFile "$0\Skin\" "(S-1-5-32-545)" "FullAccess"
		File /r "${BUILD_FOLDER}\SkinData\*.*"
	${Else}
		MessageBox MB_OK|MB_ICONINFORMATION $(MSG_InsatllPathNotFound)
		Quit
	${EndIf}
SectionEnd

# If FinishPage Checkbox is checked launch config app
Function LaunchConfig
    ReadRegDWORD $0 ${REG_HKLM} "${REG_APP_PATH}" MPDisplayConfigExePath
	Exec "$0"	
FunctionEnd




##------------------------------------------------------------------------------##
##-Language Strings-------------------------------------------------------------##

#English
LangString MSG_LaunchConfig ${LANG_ENGLISH} "Launch MPDisplay Config"
LangString MSG_InsatllPathNotFound ${LANG_ENGLISH} "Setup could not locate MPDisplay++ userdata path, Aborting!"
LangString MSG_DeleteOrOverwrite ${LANG_ENGLISH} "Skin ${SKIN_NAME} already exists, would you like to remove now?$\n$\n Click 'Yes' to delete,  'No' to overwrite"
#German
LangString MSG_LaunchConfig ${LANG_GERMAN} "Starte MPDisplay Konfiguration"
LangString MSG_InsatllPathNotFound ${LANG_GERMAN} "Setup kann den MPDisplay++ Pfad finden, Abbruch!"
LangString MSG_DeleteOrOverwrite ${LANG_GERMAN} "Skin ${SKIN_NAME} existiert bereits, soll er entfernt werden?$\n$\n Klicke 'Ja' für's löschen,  'Nein' zum überschreiben."
#Dutch
LangString MSG_LaunchConfig ${LANG_DUTCH} "Launch MPDisplay Config"
LangString MSG_InsatllPathNotFound ${LANG_DUTCH} "Setup could not locate MPDisplay++ userdata path, Aborting!"
LangString MSG_DeleteOrOverwrite ${LANG_DUTCH} "Skin ${SKIN_NAME} already exists, would you like to remove now?$\n$\n Click 'Yes' to delete,  'No' to overwrite"
#French
LangString MSG_LaunchConfig ${LANG_FRENCH} "Début configuration MPDisplay"
LangString MSG_InsatllPathNotFound ${LANG_FRENCH} "Installation n'a pas pu localiser MPDisplay++ utilisateur des données de tracé, Abandon!"
LangString MSG_DeleteOrOverwrite ${LANG_FRENCH} "Skin ${SKIN_NAME} existe déjà, voulez-vous retirer maintenant?$\n$\n Cliquez sur 'Oui' pour supprimer, 'Non' à remplacer"
#Italian
LangString MSG_LaunchConfig ${LANG_ITALIAN} "Esegui la configurazione di MPDisplay"
LangString MSG_InsatllPathNotFound ${LANG_ITALIAN} "Il setup non può individuare il precorso userdata di MPDisplay++, annullamento in corso!"
LangString MSG_DeleteOrOverwrite ${LANG_ITALIAN} "La skin ${SKIN_NAME} esiste già, vuoi rimuoverla ora?$\n$\n Clcca 'Si' per rimuovere, 'No' per sovrascrivere"
##------------------------------------------------------------------------------##
##------------------------------------------------------------------------------##




