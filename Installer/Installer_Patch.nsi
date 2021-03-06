# Installer for MPDisplay++
# Created by Sa_ddam213

# Installer Vars
!define VERSION "0.8.0.1"
;!define BUILD_FOLDER "C:\Users\Dev64\Desktop\MPDisplayRelease"
!define BUILD_FOLDER "C:\Users\Dev64\Desktop\MPDisplay 0.8.0.0\MPDisplay Current Release"
!define GROUP_NAME "MPDisplay Team"
!define WEB_SITE "http://www.mpdisplay2.de/"
!define COPYRIGHT "MPDisplay Team  � 2012"
!define DESCRIPTION "MPDisplay++ - TouchScreen/External Display Interface For MediaPortal"
!define MUI_ABORTWARNING
!define MUI_UNABORTWARNING
!define MUI_ICON    "${BUILD_FOLDER}\MPDicon.ico"
!define MUI_UNICON  "${BUILD_FOLDER}\MPDicon.ico"

# Registry Vars
!define REG_HKLM "HKLM"
!define REG_APP_PATH "Software\MPDisplay"
!define REG_UNINSTALL_PATH "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APP_NAME}"
!define REG_MEDIAPORTAL_PATH "SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\MediaPortal"
!define REG_DOTNET4_PATH "SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full"

# FileName Vars
!define APP_NAME "MPDisplay++"
!define MAIN_APP_EXE "MPDisplay++.exe"
!define PROGRAM_DATA "$APPDATA\MPDisplay++\"
!define INSTALLER_EXE_NAME "Patch_MPDisplay++_${VERSION}.exe"
!define UNINSTALLER_EXE_NAME "Uninstall_${APP_NAME}.exe"

# Set Installer Output Location #
!system "md ${BUILD_FOLDER}\Packages\Releases\${VERSION}"
OutFile "${BUILD_FOLDER}\Packages\Releases\${VERSION}\${INSTALLER_EXE_NAME}"

# Includes
!include "MUI2.nsh"
!include "UAC.nsh"
!include "FileFunc.nsh"
!include "MUI_EXTRAPAGES.nsh"
!include "MUI_EXTRAPAGESCHANGELOG.nsh"


# Other Shit
SetCompress force
ShowInstDetails show
ShowUninstDetails show
Name "${APP_NAME}"
Caption "${APP_NAME}"
BrandingText "${APP_NAME}"
XPStyle on
InstallDirRegKey "${REG_HKLM}" "${REG_APP_PATH}" ""
InstallDir "$PROGRAMFILES\MPDisplay++"
RequestExecutionLevel user

# Forms/Pages
!define MUI_HEADERIMAGE
!define MUI_UNHEADERIMAGE
!define MUI_HEADERIMAGE_BITMAP "${BUILD_FOLDER}\header.bmp"
!define MUI_UNHEADERIMAGE_BITMAP "${BUILD_FOLDER}\header.bmp"
!define MUI_WELCOMEFINISHPAGE_BITMAP "${BUILD_FOLDER}\headerSide.bmp"
!define MUI_UNWELCOMEFINISHPAGE_BITMAP "${BUILD_FOLDER}\headerSide.bmp"
!define MUI_HEADERIMAGE_RIGHT
!define MUI_UNHEADERIMAGE_RIGHT
!define MUI_FINISHPAGE_SHOWREADME
!define MUI_FINISHPAGE_SHOWREADME_NOTCHECKED
!define MUI_FINISHPAGE_SHOWREADME_TEXT "$(DESC_LaunchConfig)"
!define MUI_FINISHPAGE_SHOWREADME_FUNCTION LaunchConfig

!define MUI_PAGE_CUSTOMFUNCTION_PRE WelcomePagePre
!insertmacro MUI_PAGE_WELCOME
!insertmacro MUI_PAGE_LICENSE "${BUILD_FOLDER}\License.rtf"
!insertmacro MUI_PAGE_CHANGELOG "${BUILD_FOLDER}\ChangeLog.rtf"
!insertmacro MUI_PAGE_INSTFILES
!insertmacro MUI_PAGE_FINISH


# Define the list of languages #
!insertmacro MUI_LANGUAGE "English"
!insertmacro MUI_LANGUAGE "German"
!insertmacro MUI_LANGUAGE "Dutch"
!insertmacro MUI_LANGUAGE "French"
!insertmacro MUI_LANGUAGE "Italian"

##-------------------------------------------------------------------------------------------------##
##---------*-LANGUAGE-*----------------------------------------------------------------------------##
##-------------------------------------------------------------------------------------------------##
##-------------------------------------------------------------------------------------------------##

#ENGLISH#
##-------------------------------------------------------------------------------------------------##
##-------------------------------------------------------------------------------------------------##

LangString ChangelogTextBottom ${LANG_ENGLISH} "Bug Tracking Powered By 'Mantis' BugTracking System"
LangString ChangelogHeader ${LANG_ENGLISH} "Changelog"
LangString ChangelogSubHeader ${LANG_ENGLISH} "${APP_NAME} Changelog"
LangString ChangelogTextTop ${LANG_ENGLISH} "Please review the Changelog below"


LangString DESC_SECTION_FullInstall ${LANG_ENGLISH} "Full Installation"
LangString DESC_SECTION_MediaPortalPlugin ${LANG_ENGLISH} "Mediaportal Plugin Only"
LangString DESC_SECTION_MPDisplayGUI ${LANG_ENGLISH} "MPDisplay++ GUI Only"
LangString DESC_SECTION_DesktopShortcutInstall ${LANG_ENGLISH} "Desktop Shortcuts"
LangString DESC_SECTION_FirewallException  ${LANG_ENGLISH} "Configure Firewall"

LangString DESC_LanguageFile ${LANG_ENGLISH} "Language_en"
LangString DESC_MediaPortalPlugin ${LANG_ENGLISH} "MediaPortal Plugin/Server Only$\n$\nUse This Option If Your TouchScreen/Display Is On A Different PC"
LangString DESC_MPDisplayGUI ${LANG_ENGLISH} "MPDisplay++ TouchScreen/Display Application Only$\n$\nUse This Option If You Are Installing MPDisplay++ On A Different PC To Mediaportal"
LangString DESC_FullInstall ${LANG_ENGLISH} "MPDisplay++ Full Installation$\n$\nUse This Option If Your TouchScreen/Display Is Attached To Your MediaPortal HTPC"
LangString DESC_DesktopShortcutInstall ${LANG_ENGLISH} "Desktop Shortcuts"
LangString DESC_FirewallException ${LANG_ENGLISH} "Add Firewall Exceptions For MPDisplay++"
LangString DESC_LaunchConfig ${LANG_ENGLISH} "Launch MPDisplay Config"

LangString MSGDotNet ${LANG_ENGLISH} "${APP_NAME} requires that the .NET Framework 4.0 is installed. The .NET Framework will be downloaded and installed automatically during installation of ${APP_NAME}."
LangString MSGDotNet_Cancel ${LANG_ENGLISH} "Setup Cannot Procced Until .NET Framework 4.0 is installed $\n$\nSetup Will Now Exit"
LangString MSGUnIninstall_ConfirmBackup ${LANG_ENGLISH} "All Current MPDisplay++ Settings/Skins Will Be Lost $\n$\n Would You Like To Backup These Settings Now?."
LangString MSGUnIninstall_Confirm ${LANG_ENGLISH} "Are You Sure You Want To Uninstall Now?."
LangString MSGUnInstall ${LANG_ENGLISH} "The current version of MPDisplay++ must be removed before install $\n$\nWould you like to uninstall now?"
LangString MSGUnInstall_Cancel ${LANG_ENGLISH} "Setup Cannot Procced Until Existing MPDisplay++ Instance is Removed $\n$\nSetup Will Now Exit"
LangString MSGDotNet_NeedRestart ${LANG_ENGLISH} "Failed To Install MPDisplay Server $\n$\nRetry?$\n$\nNote: If You Have Just Installed .NET Framework 4.0 a restart may be required before MPDisplay Server is installed"
LangString MSGMediaPortalNotFound ${LANG_ENGLISH} "Installer Could Not Locate Your MediaPortal Installation, Please Select Your MediaPortal Directory  $\n$\n eg: C:\Program Files\Team MediaPortal\MediaPortal"

LangString MSGUAC_Retry ${LANG_ENGLISH} "This Application Requires Admin Privileges, Try Again?"
LangString MSGUAC_Abort ${LANG_ENGLISH} "This Application Requires Admin Privileges, Aborting!"
LangString MSGUAC_NoLogon ${LANG_ENGLISH} "Logon service not running, aborting!"
LangString MSGUAC_Error ${LANG_ENGLISH} "Unable To Elevate To Admin Privileges , Error: "
##-------------------------------------------------------------------------------------------------##
##-------------------------------------------------------------------------------------------------##


#GERMAN#
##-------------------------------------------------------------------------------------------------##
##-------------------------------------------------------------------------------------------------##
LangString ChangelogTextBottom ${LANG_GERMAN} "Bug Tracking Powered By 'Mantis' BugTracking System"
LangString ChangelogHeader ${LANG_GERMAN} "�nderungsliste"
LangString ChangelogSubHeader ${LANG_GERMAN} "${APP_NAME} �nderungsliste"
LangString ChangelogTextTop ${LANG_GERMAN} "Nachfolgend die �nderungsliste"

LangString DESC_SECTION_FullInstall ${LANG_GERMAN} "Komplettinstalltion"
LangString DESC_SECTION_MediaPortalPlugin ${LANG_GERMAN} "nur das Mediaportal Plugin"
LangString DESC_SECTION_MPDisplayGUI ${LANG_GERMAN} "nur MPDisplay++"
LangString DESC_SECTION_DesktopShortcutInstall ${LANG_GERMAN} "Desktop Verkn�pfungen"
LangString DESC_SECTION_FirewallException  ${LANG_GERMAN} "Konfiguriere Firewall"

LangString DESC_LanguageFile ${LANG_GERMAN} "Language_de"
LangString DESC_MediaPortalPlugin ${LANG_GERMAN} "nur MediaPortal Plugin/Server$\n$\nVerwende diese Option, wenn Dein Touchscreen/Monitor sich auf einem anderen Computer befindet."
LangString DESC_MPDisplayGUI ${LANG_GERMAN} "nur MPDisplay++ TouchScreen/Display Anwendung$\n$\nVerwende diese Option wenn Du MPD++ auf einem andere Computer ohne MediaPortal installieren willst."
LangString DESC_FullInstall ${LANG_GERMAN} "MPDisplay++ Komplettinstallation$\n$\nVerwende diese Option wenn Dein Touchscreen/Monitor direkt an Deinem MediaPortal HTPC angeschlossen ist."
LangString DESC_DesktopShortcutInstall ${LANG_GERMAN} "Desktop Verkn�pfungen"
LangString DESC_FirewallException ${LANG_GERMAN} "F�ge Firewall Ausnahmen f�r MPDisplay++ hinzu."
LangString DESC_LaunchConfig ${LANG_GERMAN} "Starte MPDisplay Konfiguration."


LangString MSGDotNet ${LANG_GERMAN} "${APP_NAME} .NET Framework 4.0 muss installiert sein. Das .NET Framework wird automatisch w�hrend des Installationsprozesses von ${APP_NAME} heruntergeladen und installiert."
LangString MSGDotNet_Cancel ${LANG_GERMAN} "Setup kann nicht durchgef�hrt werden solange das .NET Framework 4.0 nicht installiert ist. $\n$\nSetup wird beendet!"
LangString MSGUnIninstall_Confirm ${LANG_GERMAN} "Alle Einstellungen/Skins von MPDisplay++ werden gel�scht. $\n$\nBist Du sicher das die Anwendung deinstalliert werden soll?"
LangString MSGUnIninstall_ConfirmBackup ${LANG_GERMAN} "Alle Einstellungen/Skins von MPDisplay++ werden gel�scht. $\n$\nWillst Du ein Backup durchf�hren lassen?"
LangString MSGUnInstall ${LANG_GERMAN} "Die aktuell installierte Version von MPD++ muss vorher deinstalliert werden. $\n$\nSoll die alte Version jetzt deinstalliert werden?"
LangString MSGUnInstall_Cancel ${LANG_GERMAN} "Das Setup kann nicht fortgef�hrt werden sollange noch ein MPD++ Prozess durchgef�hrt wird. $\n$\nSetup wird beendet!"
LangString MSGDotNet_NeedRestart ${LANG_GERMAN} "Fehler bei der Installation des MPDisplay++ Servers. $\n$\nWiederholen?$\n$\nHinweis: Wenn Du gerade erst das .NET Framework 4.0 installiert hast, muss der Rechner eventuell erst neu gestartet werden."
LangString MSGMediaPortalNotFound ${LANG_GERMAN} "Der Installer kann das Installationsverzeichnis von MediaPortal nicht finden. Bitte suchen Sie es manuell.  $\n$\nz.B.: C:\Program Files\Team MediaPortal\MediaPortal"

LangString MSGUAC_Retry ${LANG_GERMAN} "Diese Anwendung ben�tigt Administratorrechte, noch mal versuchen?"
LangString MSGUAC_Abort ${LANG_GERMAN} "Diese Anwendung ben�tigt Administratorrechte, Abbruch!"
LangString MSGUAC_NoLogon ${LANG_GERMAN} "Logon service l�uft nicht, Abbruch!"
LangString MSGUAC_Error ${LANG_GERMAN} "Benutzerrechte k�nnen nicht auf Adminrechte angehoben werden, Fehler: "
##-------------------------------------------------------------------------------------------------##
##-------------------------------------------------------------------------------------------------##

#DUTCH#
##-------------------------------------------------------------------------------------------------##
##-------------------------------------------------------------------------------------------------##
LangString ChangelogTextBottom ${LANG_DUTCH} "Bug Tracking Powered By 'Mantis' BugTracking System"
LangString ChangelogHeader ${LANG_DUTCH} "Changelog"
LangString ChangelogSubHeader ${LANG_DUTCH} "${APP_NAME} Changelog"
LangString ChangelogTextTop ${LANG_DUTCH} "Please review the Changelog below"

LangString DESC_SECTION_FullInstall ${LANG_DUTCH} "Full Installation"
LangString DESC_SECTION_MediaPortalPlugin ${LANG_DUTCH} "Mediaportal Plugin Only"
LangString DESC_SECTION_MPDisplayGUI ${LANG_DUTCH} "MPDisplay++ GUI Only"
LangString DESC_SECTION_DesktopShortcutInstall ${LANG_DUTCH} "Desktop Shortcuts"
LangString DESC_SECTION_FirewallException  ${LANG_DUTCH} "Configure Firewall"

LangString DESC_LanguageFile ${LANG_DUTCH} "Language_nl"
LangString DESC_MediaPortalPlugin ${LANG_DUTCH} "MediaPortal Plugin/Server Only$\n$\nUse This Option If Your TouchScreen/Display Is On A Different PC"
LangString DESC_MPDisplayGUI ${LANG_DUTCH} "MPDisplay++ TouchScreen/Display Application Only$\n$\nUse This Option If You Are Installing MPDisplay++ On A Different PC To Mediaportal"
LangString DESC_FullInstall ${LANG_DUTCH} "MPDisplay++ Full Installation$\n$\nUse This Option If Your TouchScreen/Display Is Attached To Your MediaPortal HTPC"
LangString DESC_DesktopShortcutInstall ${LANG_DUTCH} "Desktop Shortcuts"
LangString DESC_FirewallException ${LANG_DUTCH} "Add Firewall Exceptions For MPDisplay++"
LangString DESC_LaunchConfig ${LANG_DUTCH} "Launch MPDisplay Config"


LangString MSGDotNet ${LANG_DUTCH} "${APP_NAME} requires that the .NET Framework 4.0 is installed. The .NET Framework will be downloaded and installed automatically during installation of ${APP_NAME}."
LangString MSGDotNet_Cancel ${LANG_DUTCH} "Setup Cannot Procced Until .NET Framework 4.0 is installed $\n$\nSetup Will Now Exit"
LangString MSGUnIninstall_Confirm ${LANG_DUTCH} "All Current MPDisplay++ Settings/Skins Will Be Lost $\n$\nAre You Sure You Want To Uninstall Now?gtgttt."
LangString MSGUnIninstall_ConfirmBackup ${LANG_DUTCH} "All Current MPDisplay++ Settings/Skins Will Be Lost $\n$\nWould You Like To Backup These Settings Now?."
LangString MSGUnInstall ${LANG_DUTCH} "The current version of MPDisplay++ must be removed before install $\n$\nWould you like to uninstall now?"
LangString MSGUnInstall_Cancel ${LANG_DUTCH} "Setup Cannot Procced Until Existing MPDisplay++ Instance is Removed $\n$\nSetup Will Now Exit"
LangString MSGDotNet_NeedRestart ${LANG_DUTCH} "Failed To Install MPDisplay Server $\n$\nRetry?$\n$\nNote: If You Have Just Installed .NET Framework 4.0 a restart may be required before MPDisplay Server is installed"
LangString MSGMediaPortalNotFound ${LANG_DUTCH} "Installer Could Not Locate Your MediaPortal Installation, Please Select Your MediaPortal Directory  $\n$\neg: C:\Program Files\Team MediaPortal\MediaPortal"

LangString MSGUAC_Retry ${LANG_DUTCH} "This Application Requires Admin Privileges, Try Again?"
LangString MSGUAC_Abort ${LANG_DUTCH} "This Application Requires Admin Privileges, Aborting!"
LangString MSGUAC_NoLogon ${LANG_DUTCH} "Logon service not running, aborting!"
LangString MSGUAC_Error ${LANG_DUTCH} "Unable To Elevate To Admin Privileges , Error: "
#French#
##-------------------------------------------------------------------------------------------------##
##-------------------------------------------------------------------------------------------------##
LangString ChangelogTextBottom ${LANG_FRENCH} "Bug Tracking Powered By 'Mantis' BugTracking System"
LangString ChangelogHeader ${LANG_FRENCH} "liste des modifications"
LangString ChangelogSubHeader ${LANG_FRENCH} "${APP_NAME} liste des modifications"
LangString ChangelogTextTop ${LANG_FRENCH} "Ci-dessous la liste des changements"

LangString DESC_SECTION_FullInstall ${LANG_FRENCH} "Installation termin�e"
LangString DESC_SECTION_MediaPortalPlugin ${LANG_FRENCH} "que le plugin MediaPortal"
LangString DESC_SECTION_MPDisplayGUI ${LANG_FRENCH} "que le plugin MediaPortal"
LangString DESC_SECTION_DesktopShortcutInstall ${LANG_FRENCH} "Raccourcis Bureau"
LangString DESC_SECTION_FirewallException ${LANG_FRENCH} "configurer un pare-"

LangString DESC_LanguageFile ${LANG_FRENCH} "Language_fr"
LangString DESC_MediaPortalPlugin ${LANG_FRENCH} "que le plugin MediaPortal/serveur$\n$\nUtilisez cette option si votre �cran / moniteur est sur ??un autre ordinateur."
LangString DESC_MPDisplayGUI ${LANG_FRENCH} "seulement MPDisplay++ � �cran tactile/affichage de l'application$\n$\nUtilisez cette option si vous souhaitez installer MPD++ sur un autre ordinateur sans Media Portal."
LangString DESC_FullInstall ${LANG_FRENCH} "MPDisplay + + installation compl�te$\n$\nUtilisez cette option si votre �cran/moniteur directement � votre MediaPortal HTPC est connect�."
LangString DESC_DesktopShortcutInstall ${LANG_FRENCH} "Raccourcis Bureau"
LangString DESC_FirewallException ${LANG_FRENCH} "Ajouter des exceptions de pare-feu pour MPDisplay++ ajout�."
LangString DESC_LaunchConfig ${LANG_FRENCH} "D�but configuration MPDisplay."


LangString MSGDotNet ${LANG_FRENCH} "${APP_NAME} .NET Framework 4.0 doit �tre install�. Le. NET Framework sera automatiquement pendant le processus d'installation ${APP_NAME} t�l�charg� et install�."
LangString MSGDotNet_Cancel ${LANG_FRENCH} "L'installation ne peut pas �tre effectu�e jusqu'� ce que le. NET Framework 4.0 n'est pas install�. $\n$\nL'installation est termin�e!"
LangString MSGUnIninstall_Confirm ${LANG_FRENCH} "Tous les r�glages et peaux de MPDisplay++ sera supprim�. $\n$\nEtes-vous s�r que l'application sera d�sinstall�e?"
LangString MSGUnIninstall_ConfirmBackup ${LANG_FRENCH} "Tous les r�glages et peaux de MPDisplay++ sera supprim�. $\n$\nVoulez-vous �tre d'effectuer une sauvegarde?"
LangString MSGUnInstall ${LANG_FRENCH} "La version actuellement install�e de MPD++ doit �tre d�sinstall� avant. $\n$\nDevrait maintenant �tre d�sinstall� l'ancienne version?"
LangString MSGUnInstall_Cancel ${LANG_FRENCH} "Ne devrait pas �tre la configuration de proc�der dans un autre MPD++ processus est effectu�e. $\n$\nL'installation est termin�e!"
LangString MSGDotNet_NeedRestart ${LANG_FRENCH} "Erreur lors de l'installation de la MPDisplay++ serveur. $\n$\nR�p�ter?$\n$\nRemarque:. Si vous avez juste le NET Framework 4.0 est install�, l'ordinateur doit �tre d�marr� avant toute nouvelle."
LangString MSGMediaPortalNotFound ${LANG_FRENCH} "Le programme d'installation ne peut pas trouver le r�pertoire d'installation de MediaPortal. S'il vous pla�t chercher manuellement. $\n$\n par exemple: C:\Program Files\Team MediaPortal\MediaPortal"

LangString MSGUAC_Retry ${LANG_FRENCH} "Cette application n�cessite des privil�ges d'administrateur, essayez � nouveau?"
LangString MSGUAC_Abort ${LANG_FRENCH} "Cette application n�cessite des droits d'administrateur, de la d�molition!"
LangString MSGUAC_NoLogon ${LANG_FRENCH} "Service Ouverture de session ne fonctionne pas, arr�tez-vous!"
LangString MSGUAC_Error ${LANG_FRENCH} "Les droits des utilisateurs ne peut �tre soulev�e en cas d'erreur de droits d'administrateur: "

#Italian#
##-------------------------------------------------------------------------------------------------##
##-------------------------------------------------------------------------------------------------##
LangString ChangelogTextBottom ${LANG_ITALIAN} "Bug Tracking Powered By 'Mantis' BugTracking System"
LangString ChangelogHeader ${LANG_ITALIAN} "Changelog"
LangString ChangelogSubHeader ${LANG_ITALIAN} "${APP_NAME} Changelog"
LangString ChangelogTextTop ${LANG_ITALIAN} "Si prega di verificare il changelog seguente"

LangString DESC_SECTION_FullInstall ${LANG_ITALIAN} "Installazione Completa"
LangString DESC_SECTION_MediaPortalPlugin ${LANG_ITALIAN} "Solo Plugin di MediaPortal"
LangString DESC_SECTION_MPDisplayGUI ${LANG_ITALIAN} "Solo interfaccia grafica di MPDisplay++ (GUI)"
LangString DESC_SECTION_DesktopShortcutInstall ${LANG_ITALIAN} "Collegamenti sul Desktop"
LangString DESC_SECTION_FirewallException ${LANG_ITALIAN} "Configura Firewall"

LangString DESC_LanguageFile ${LANG_ITALIAN} "Language_it"
LangString DESC_MediaPortalPlugin ${LANG_ITALIAN} "Solo Plugin/Server MediaPortal$\n$\nScegli quest'opzione se il tuo TouchScreen/Display � su un altro PC."
LangString DESC_MPDisplayGUI ${LANG_ITALIAN} "Solo applicazione TouchScreen/Display di MPDisplay++$\n$\nScegli questa opzione se stai installando MPDisplay++ su un PC diverso da quello dove � installato MediaPortal."
LangString DESC_FullInstall ${LANG_ITALIAN} "Installazione completa di MPDisplay++$\n$\nScegli questa opzione se il tuo TouchScreen/Display � collegato al tuo HTPC MediaPortal."
LangString DESC_DesktopShortcutInstall ${LANG_ITALIAN} "Collegamenti Desktop"
LangString DESC_FirewallException ${LANG_ITALIAN} "Aggiungi eccezioni al firewall per MPDisplay++."
LangString DESC_LaunchConfig ${LANG_ITALIAN} "Esegui la configurazione di MPDisplay."


LangString MSGDotNet ${LANG_ITALIAN} "${APP_NAME} richiede che il .NET Framework 4.0 sia installato. Il .NET Framework sar� scaricato e installato automaticamente durante l'installazione di ${APP_NAME}."
LangString MSGDotNet_Cancel ${LANG_ITALIAN} "Il Setup non pu� procedere finch� .NET Framework 4.0 non verr� installato $\n$\nIl Setup si chiuder� ora!"
LangString MSGUnIninstall_Confirm ${LANG_ITALIAN} "Tutti gli attuali Settaggi/Skin di MPDisplay++ saranno persi $\n$\n Sicuro di voler disinstallare ora?"
LangString MSGUnIninstall_ConfirmBackup ${LANG_ITALIAN} "Tutti gli attuali Settaggi/Skin di MPDisplay++ saranno persi $\n$\n Vuoi fare un backup di questi settaggi ora?"
LangString MSGUnInstall ${LANG_ITALIAN} "L'attuale versione di MPDisplay++ deve essere rimossa prima dell'installazione $\n$\nVuoi disinstallare ora?"
LangString MSGUnInstall_Cancel ${LANG_ITALIAN} "Il setup non pu� procedere prima che l'attuale istanza di MPDisplay++ venga rimossa $\n$\nIl Setup si chiuder� ora"
LangString MSGDotNet_NeedRestart ${LANG_ITALIAN} "Errore nell'installazione di MPDisplay Server $\n$\nRiprova?$\n$\nNota: se hai appena installato .NET Framework 4.0 potrebbe essere necessario un riavvio prima che MPDisplay Server venga installato"
LangString MSGMediaPortalNotFound ${LANG_ITALIAN} "L'installer non ha individuato l'installazione di MediaPortal, selezionare a mano la cartella di installazione $\n$\n ES: C:\Program Files\Team MediaPortal\MediaPortal"

LangString MSGUAC_Retry ${LANG_ITALIAN} "Questa applicazione richiede privilegi amministrativi, riprovare?"
LangString MSGUAC_Abort ${LANG_ITALIAN} "Questa applicazione richiede privilegi amministrativi, annullamento in corso!"
LangString MSGUAC_NoLogon ${LANG_ITALIAN} "Servizio Logon non � in esecuzione, annullamento in corso!"
LangString MSGUAC_Error ${LANG_ITALIAN} "Impossibile ottenere privilegi amministrativi, Errore:"
##-------------------------------------------------------------------------------------------------##
##-------------------------------------------------------------------------------------------------##
##-------------------------------------------------------------------------------------------------##
##-------------------------------------------------------------------------------------------------##










# Function to Elevate UAC to Admin #
!macro Check_UAC InsatllType
	uac_tryagain:
	!insertmacro UAC_RunElevated
	${Switch} $0
	${Case} 0
		${IfThen} $1 = 1 ${|} Quit ${|} ;we are the outer process, the inner process has done its work, we are done
		${IfThen} $3 <> 0 ${|} ${Break} ${|} ;we are admin, let the show go on
		${If} $1 = 3 ;RunAs completed successfully, but with a non-admin user
			MessageBox mb_YesNo|mb_IconExclamation|mb_TopMost|mb_SetForeground "$(MSGUAC_Retry)" /SD IDNO IDYES uac_tryagain IDNO 0
		${EndIf}
		;fall-through and die
	${Case} 1223
		MessageBox mb_IconStop|mb_TopMost|mb_SetForeground "$(MSGUAC_Abort)"
		Quit
	${Case} 1062
		MessageBox mb_IconStop|mb_TopMost|mb_SetForeground "$(MSGUAC_NoLogon)"
		Quit
	${Default}
		MessageBox mb_IconStop|mb_TopMost|mb_SetForeground "$(MSGUAC_Error)$0"
		Quit
	${EndSwitch}
	 
	SetShellVarContext all
!macroend




##-------------------------------------------------------------------------------------------------##
##---------*INSTALL*-------------------------------------------------------------------------------##
##-------------------------------------------------------------------------------------------------##
##-------------------------------------------------------------------------------------------------## 
Function .onInit
    SetShellVarContext all	
	SetOverwrite on
	# Display the language selector #
    !insertmacro MUI_LANGDLL_DISPLAY
FunctionEnd

Function WelcomePagePre
   # Check UAC
   !insertmacro Check_UAC "Installer"
    # Set Registry Permissons
    AccessControl::GrantOnRegKey \
    ${REG_HKLM} "${REG_UNINSTALL_PATH}" "(S-1-5-32-545)" "FullAccess"
	AccessControl::GrantOnRegKey \
    ${REG_HKLM} "${REG_APP_PATH}" "(S-1-5-32-545)" "FullAccess"
	
FunctionEnd

##-------------------------------------------------------------------------------------------------##
##-------------------------------------------------------------------------------------------------##
##-------------------------------------------------------------------------------------------------##
##-------------------------------------------------------------------------------------------------##




##-------------------------------------------------------------------------------------------------##
##---------*FullInstall----------------------------------------------------------------------------##
##-------------------------------------------------------------------------------------------------##
##-------------------------------------------------------------------------------------------------##
Section -MainProgram
##-------------------------------------------------------------------------------------------------##
##-------------------------------------------------------------------------------------------------##
    ReadRegDWORD $1 ${REG_HKLM} "${REG_APP_PATH}" InstallType
	${If} $1 == 'Plugin'    
	
		# Find MediaPortal InstallPath
		ReadRegDWORD $0 ${REG_HKLM} "${REG_MEDIAPORTAL_PATH}" InstallPath
		${IF} $0 == ''
			; If MediaPortal Path Not Found, Show Folder Picker Dislog So User Can Select MediaPortal Path
			nsDialogs::SelectFolderDialog "$(MSGMediaPortalNotFound)" $PROGRAMFILES
			Pop $0
			${GetParent} $0 $1
			# Install Plugin
			 SetOutPath "$1\MediaPortal\Plugins\Process"
				File "${BUILD_FOLDER}\Plugin\*.dll"
			 WriteRegStr ${REG_HKLM} "${REG_APP_PATH}" "MPDPluginPath" "$1\MediaPortal\Plugins\Process"	
		
		${Else}  
			SetOutPath "$0\Plugins\Process"
				File "${BUILD_FOLDER}\Plugin\*.dll"
			WriteRegStr ${REG_HKLM} "${REG_APP_PATH}" "MPDPluginPath" "$0\Plugins\Process"	
		${EndIf}
##-------------------------------------------------------------------------------------------------##
##-------------------------------------------------------------------------------------------------##
		
##-------------------------------------------------------------------------------------------------##
##-------------------------------------------------------------------------------------------------##
    ${ElseIf} $1 == 'Full' 	

	    # Copy Skin Files
		ReadRegDWORD $0 ${REG_HKLM} "${REG_APP_PATH}" ProgramDataPath
		SetOutPath "$0"
			AccessControl::GrantOnFile "$0\Skin\" "(S-1-5-32-545)" "FullAccess"
				File /r "${BUILD_FOLDER}\ProgData\*.*"
	
		nsSCM::Stop "MPDisplayServer"
		Sleep 3000
	
		# Install GUI
		ReadRegDWORD $0 ${REG_HKLM} "${REG_APP_PATH}" MPDisplayExePath
		${GetParent} $0 $1
		SetOutPath "$1"
				File "${BUILD_FOLDER}\GUI\*.*"
			
	 
		# Find MediaPortal InstallPath
		ReadRegDWORD $0 ${REG_HKLM} "${REG_MEDIAPORTAL_PATH}" InstallPath
		${IF} $0 == ''
			; If MediaPortal Path Not Found, Show Folder Picker Dislog So User Can Select MediaPortal Path
			nsDialogs::SelectFolderDialog "$(MSGMediaPortalNotFound)" $PROGRAMFILES
			Pop $0
			${GetParent} $0 $1
			# Install Plugin
			 SetOutPath "$1\MediaPortal\Plugins\Process"
				File "${BUILD_FOLDER}\Plugin\*.dll"
			 WriteRegStr ${REG_HKLM} "${REG_APP_PATH}" "MPDPluginPath" "$1\MediaPortal\Plugins\Process"	
		
		${Else}  
			SetOutPath "$0\Plugins\Process"
				File "${BUILD_FOLDER}\Plugin\*.dll"
			WriteRegStr ${REG_HKLM} "${REG_APP_PATH}" "MPDPluginPath" "$0\Plugins\Process"	
		${EndIf}
		
		# Install Server
		ReadRegDWORD $0 ${REG_HKLM} "${REG_APP_PATH}" MPDServerExePath
		${GetParent} $0 $1
		SetOutPath "$1"
				File "${BUILD_FOLDER}\Server\*.*"
		
		
		nsSCM::Start "MPDisplayServer"	
##-------------------------------------------------------------------------------------------------##
##-------------------------------------------------------------------------------------------------##		
		
##-------------------------------------------------------------------------------------------------##
##-------------------------------------------------------------------------------------------------##		
    ${ElseIf} $1 == 'GUI' 	
    
	    # Copy Skin Files
		ReadRegDWORD $0 ${REG_HKLM} "${REG_APP_PATH}" ProgramDataPath
		SetOutPath "$0"
			AccessControl::GrantOnFile "$0\Skin\" "(S-1-5-32-545)" "FullAccess"
				File /r "${BUILD_FOLDER}\ProgData\*.*"
	
	    # Install GUI
		ReadRegDWORD $0 ${REG_HKLM} "${REG_APP_PATH}" MPDisplayExePath
		${GetParent} $0 $1
		SetOutPath "$1"
			File "${BUILD_FOLDER}\GUI\*.*"
	
    ${EndIf}	
 ##-------------------------------------------------------------------------------------------------##
##-------------------------------------------------------------------------------------------------## 
  

			
SectionEnd	





# If FinishPage Checkbox is checked launch required config app
Function LaunchConfig
    ReadRegDWORD $1 ${REG_HKLM} "${REG_APP_PATH}" InstallType
	${If} $1 != 'Plugin'
        ReadRegDWORD $0 ${REG_HKLM} "${REG_APP_PATH}" MPDisplayConfigExePath
	    Exec "$0"	
    ${Else}
        ReadRegDWORD $0 ${REG_HKLM} "${REG_MEDIAPORTAL_PATH}" InstallPath
		Exec "$0\Configuration.exe"	
	${EndIf}
FunctionEnd




































