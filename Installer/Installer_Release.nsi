# Installer for MPDisplay++
# Created by Sa_ddam213

# Installer Vars
!define VERSION "1.0.0_D2"
!define BUILD_FOLDER "C:\Entwicklung\MPDisplay"
!define GROUP_NAME "MPDisplay Team"
!define WEB_SITE "http://www.mpdisplay2.de/"
!define COPYRIGHT "MPDisplay Team  © 2015"
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
!define MAIN_APP_EXE "MPDisplay.exe"
!define PROGRAM_DATA "$APPDATA\MPDisplay++\"
!define INSTALLER_EXE_NAME "Setup_MPDisplay++_${VERSION}.exe"
!define UNINSTALLER_EXE_NAME "Uninstall_${APP_NAME}.exe"
!define LOGLEVEL "Info"

# Set Installer Output Location #
!system "md ${BUILD_FOLDER}\Packages\ReleaseBuilds\${VERSION}\"
OutFile "${BUILD_FOLDER}\Packages\ReleaseBuilds\${VERSION}\${INSTALLER_EXE_NAME}"

# Includes
!include "MUI2.nsh"
!include "UAC.nsh"
!include "FileFunc.nsh"
!include "MUI_EXTRAPAGES.nsh"
!include "MUI_EXTRAPAGESCHANGELOG.nsh"
!include "WinVer.nsh"


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
!insertmacro MUI_PAGE_COMPONENTS
!insertmacro MUI_PAGE_DIRECTORY
!insertmacro MUI_PAGE_INSTFILES
!insertmacro MUI_PAGE_FINISH

!insertmacro MUI_UNPAGE_WELCOME
!insertmacro MUI_UNPAGE_INSTFILES
!insertmacro MUI_UNPAGE_FINISH

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
LangString DESC_SECTION_MediaPortalPlugin ${LANG_ENGLISH} "Plugin/Server Only"
LangString DESC_SECTION_MPDisplayGUI ${LANG_ENGLISH} "MPDisplay++ Only"
LangString DESC_SECTION_DesktopShortcutInstall ${LANG_ENGLISH} "Desktop Shortcuts"
LangString DESC_SECTION_FirewallException  ${LANG_ENGLISH} "Configure Firewall"

LangString DESC_UnInstall_Full ${LANG_ENGLISH} "There is 3rd party skins installed, Would you like to remove these also?"

LangString DESC_LanguageFile ${LANG_ENGLISH} "Language_en"
LangString DESC_MediaPortalPlugin ${LANG_ENGLISH} "MediaPortal Plugin/Server Only$\n$\nUse This Option If Your TouchScreen/Display Is On A Different PC"
LangString DESC_MPDisplayGUI ${LANG_ENGLISH} "MPDisplay++ TouchScreen/Display Application Only$\n$\nUse This Option If You Are Installing MPDisplay++ On A Different PC To Mediaportal"
LangString DESC_FullInstall ${LANG_ENGLISH} "MPDisplay++ Full Installation$\n$\nUse This Option If Your TouchScreen/Display Is Attached To Your MediaPortal HTPC"
LangString DESC_DesktopShortcutInstall ${LANG_ENGLISH} "Desktop Shortcuts"
LangString DESC_FirewallException ${LANG_ENGLISH} "Add Firewall Exceptions For MPDisplay++"
LangString DESC_LaunchConfig ${LANG_ENGLISH} "Launch MPDisplay Config"

LangString MSGDotNet ${LANG_ENGLISH} "${APP_NAME} requires that the .NET Framework 4.5 is installed. The .NET Framework will be downloaded and installed automatically during installation of ${APP_NAME}."
LangString MSGDotNet_Cancel ${LANG_ENGLISH} "Setup Cannot Procced Until .NET Framework 4.5 is installed $\n$\nSetup Will Now Exit"
LangString MSGUnIninstall_ConfirmBackup ${LANG_ENGLISH} "All Current MPDisplay++ Settings/Skins Will Be Lost $\n$\n Would You Like To Backup These Settings Now?."
LangString MSGUnIninstall_Confirm ${LANG_ENGLISH} "Are You Sure You Want To Uninstall Now?."
LangString MSGUnInstall ${LANG_ENGLISH} "The current version of MPDisplay++ must be removed before install. $\n$\nMPDisplay++ settings will be preserved. $\n$\nWould you like to uninstall now?"
LangString MSGUnInstall_Cancel ${LANG_ENGLISH} "Setup Cannot Procced Until Existing MPDisplay++ Instance is Removed $\n$\nSetup Will Now Exit"
LangString MSGDotNet_NeedRestart ${LANG_ENGLISH} "Failed To Install MPDisplay Server $\n$\nRetry?$\n$\nNote: If You Have Just Installed .NET Framework 4.5 a restart may be required before MPDisplay Server is installed"
LangString MSGMediaPortalNotFound ${LANG_ENGLISH} "Installer Could Not Locate Your MediaPortal Installation, Please Select Your MediaPortal Directory  $\n$\n eg: C:\Program Files\Team MediaPortal\MediaPortal"
LangString MSGXPWarn ${LANG_ENGLISH} "MPDisplay does not support Windows XP operating system"

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
LangString ChangelogHeader ${LANG_GERMAN} "Änderungsliste"
LangString ChangelogSubHeader ${LANG_GERMAN} "${APP_NAME} Änderungsliste"
LangString ChangelogTextTop ${LANG_GERMAN} "Nachfolgend die Änderungsliste"

LangString DESC_SECTION_FullInstall ${LANG_GERMAN} "Komplettinstallation"
LangString DESC_SECTION_MediaPortalPlugin ${LANG_GERMAN} "nur Plugin/Server Installation"
LangString DESC_SECTION_MPDisplayGUI ${LANG_GERMAN} "nur MPDisplay++ Installation"
LangString DESC_SECTION_DesktopShortcutInstall ${LANG_GERMAN} "Desktop Verknüpfungen"
LangString DESC_SECTION_FirewallException  ${LANG_GERMAN} "Konfiguriere Firewall"

LangString DESC_UnInstall_Full ${LANG_GERMAN} "Es sind noch weitere Skins installiert. Sollen diese ebenfalls entfernt werden?"

LangString DESC_LanguageFile ${LANG_GERMAN} "Language_de"
LangString DESC_MediaPortalPlugin ${LANG_GERMAN} "nur MediaPortal Plugin/Server$\n$\nVerwende diese Option, wenn Dein Touchscreen/Monitor sich auf einem anderen Computer befindet."
LangString DESC_MPDisplayGUI ${LANG_GERMAN} "nur MPDisplay++ TouchScreen/Display Anwendung$\n$\nVerwende diese Option wenn Du MPD++ auf einem andere Computer ohne MediaPortal installieren willst."
LangString DESC_FullInstall ${LANG_GERMAN} "MPDisplay++ Komplettinstallation$\n$\nVerwende diese Option wenn Dein Touchscreen/Monitor direkt an Deinem MediaPortal HTPC angeschlossen ist."
LangString DESC_DesktopShortcutInstall ${LANG_GERMAN} "Desktop Verknüpfungen"
LangString DESC_FirewallException ${LANG_GERMAN} "Füge Firewall Ausnahmen für MPDisplay++ hinzu."
LangString DESC_LaunchConfig ${LANG_GERMAN} "Starte MPDisplay Konfiguration."


LangString MSGDotNet ${LANG_GERMAN} "${APP_NAME} .NET Framework 4.5 muss installiert sein. Das .NET Framework wird automatisch während des Installationsprozesses von ${APP_NAME} heruntergeladen und installiert."
LangString MSGDotNet_Cancel ${LANG_GERMAN} "Setup kann nicht durchgeführt werden solange das .NET Framework 4.5 nicht installiert ist. $\n$\nSetup wird beendet!"
LangString MSGUnIninstall_Confirm ${LANG_GERMAN} "Alle Einstellungen/Skins von MPDisplay++ werden gelöscht. $\n$\n Bist Du sicher das die Anwendung deinstalliert werden soll?"
LangString MSGUnIninstall_ConfirmBackup ${LANG_GERMAN} "Alle Einstellungen/Skins von MPDisplay++ werden gelöscht. $\n$\n Willst Du ein Backup durchführen lassen?"
LangString MSGUnInstall ${LANG_GERMAN} "Die aktuell installierte Version von MPD++ muss vorher deinstalliert werden. $\n$\nDie MPDisplay++ Einstellungen bleiben dabei erhalten. $\n$\nSoll die alte Version jetzt deinstalliert werden?"
LangString MSGUnInstall_Cancel ${LANG_GERMAN} "Das Setup kann nicht fortgeführt werden sollange noch ein MPD++ Prozess durchgeführt wird. $\n$\nSetup wird beendet!"
LangString MSGDotNet_NeedRestart ${LANG_GERMAN} "Fehler bei der Installation des MPDisplay++ Servers. $\n$\nWiederholen?$\n$\nHinweis: Wenn Du gerade erst das .NET Framework 4.5 installiert hast, muss der Rechner eventuell erst neu gestartet werden."
LangString MSGMediaPortalNotFound ${LANG_GERMAN} "Der Installer kann das Installationsverzeichnis von MediaPortal nicht finden. Bitte suchen Sie es manuell.  $\n$\n z.B.: C:\Program Files\Team MediaPortal\MediaPortal"
LangString MSGXPWarn ${LANG_GERMAN} "MPDisplay unterstützt kein WindowsXP!"

LangString MSGUAC_Retry ${LANG_GERMAN} "Diese Anwendung benötigt Administratorrechte, noch mal versuchen?"
LangString MSGUAC_Abort ${LANG_GERMAN} "Diese Anwendung benötigt Administratorrechte, Abbruch!"
LangString MSGUAC_NoLogon ${LANG_GERMAN} "Logon service läuft nicht, Abbruch!"
LangString MSGUAC_Error ${LANG_GERMAN} "Benutzerrechte können nicht auf Adminrechte angehoben werden, Fehler: "
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

LangString DESC_UnInstall_Full ${LANG_DUTCH} "There is 3rd party skins installed, Would you like to remove these also?"

LangString DESC_LanguageFile ${LANG_DUTCH} "Language_nl"
LangString DESC_MediaPortalPlugin ${LANG_DUTCH} "MediaPortal Plugin/Server Only$\n$\nUse This Option If Your TouchScreen/Display Is On A Different PC"
LangString DESC_MPDisplayGUI ${LANG_DUTCH} "MPDisplay++ TouchScreen/Display Application Only$\n$\nUse This Option If You Are Installing MPDisplay++ On A Different PC To Mediaportal"
LangString DESC_FullInstall ${LANG_DUTCH} "MPDisplay++ Full Installation$\n$\nUse This Option If Your TouchScreen/Display Is Attached To Your MediaPortal HTPC"
LangString DESC_DesktopShortcutInstall ${LANG_DUTCH} "Desktop Shortcuts"
LangString DESC_FirewallException ${LANG_DUTCH} "Add Firewall Exceptions For MPDisplay++"
LangString DESC_LaunchConfig ${LANG_DUTCH} "Launch MPDisplay Config"


LangString MSGDotNet ${LANG_DUTCH} "${APP_NAME} requires that the .NET Framework 4.5 is installed. The .NET Framework will be downloaded and installed automatically during installation of ${APP_NAME}."
LangString MSGDotNet_Cancel ${LANG_DUTCH} "Setup Cannot Procced Until .NET Framework 4.5 is installed $\n$\nSetup Will Now Exit"
LangString MSGUnIninstall_Confirm ${LANG_DUTCH} "All Current MPDisplay++ Settings/Skins Will Be Lost $\n$\n Are You Sure You Want To Uninstall Now?gtgttt."
LangString MSGUnIninstall_ConfirmBackup ${LANG_DUTCH} "All Current MPDisplay++ Settings/Skins Will Be Lost $\n$\n Would You Like To Backup These Settings Now?."
LangString MSGUnInstall ${LANG_DUTCH} "The current version of MPDisplay++ must be removed before install. $\n$\nMPDisplay++ settings will be preserved. $\n$\nWould you like to uninstall now?"
LangString MSGUnInstall_Cancel ${LANG_DUTCH} "Setup Cannot Procced Until Existing MPDisplay++ Instance is Removed $\n$\nSetup Will Now Exit"
LangString MSGDotNet_NeedRestart ${LANG_DUTCH} "Failed To Install MPDisplay Server $\n$\nRetry?$\n$\nNote: If You Have Just Installed .NET Framework 4.5 a restart may be required before MPDisplay Server is installed"
LangString MSGMediaPortalNotFound ${LANG_DUTCH} "Installer Could Not Locate Your MediaPortal Installation, Please Select Your MediaPortal Directory  $\n$\n eg: C:\Program Files\Team MediaPortal\MediaPortal"
LangString MSGXPWarn ${LANG_DUTCH} "MPDisplay does not support Windows XP operating system"

LangString MSGUAC_Retry ${LANG_DUTCH} "This Application Requires Admin Privileges, Try Again?"
LangString MSGUAC_Abort ${LANG_DUTCH} "This Application Requires Admin Privileges, Aborting!"
LangString MSGUAC_NoLogon ${LANG_DUTCH} "Logon service not running, aborting!"
LangString MSGUAC_Error ${LANG_DUTCH} "Unable To Elevate To Admin Privileges , Error: "
##-------------------------------------------------------------------------------------------------##
##-------------------------------------------------------------------------------------------------##


#French#
##-------------------------------------------------------------------------------------------------##
##-------------------------------------------------------------------------------------------------##
LangString ChangelogTextBottom ${LANG_FRENCH} "Bug Tracking Powered By 'Mantis' BugTracking System"
LangString ChangelogHeader ${LANG_FRENCH} "liste des modifications"
LangString ChangelogSubHeader ${LANG_FRENCH} "${APP_NAME} liste des modifications"
LangString ChangelogTextTop ${LANG_FRENCH} "Ci-dessous la liste des changements"

LangString DESC_SECTION_FullInstall ${LANG_FRENCH} "Installation terminée"
LangString DESC_SECTION_MediaPortalPlugin ${LANG_FRENCH} "que le plugin MediaPortal"
LangString DESC_SECTION_MPDisplayGUI ${LANG_FRENCH} "que le plugin MediaPortal"
LangString DESC_SECTION_DesktopShortcutInstall ${LANG_FRENCH} "Raccourcis Bureau"
LangString DESC_SECTION_FirewallException ${LANG_FRENCH} "configurer un pare-"

LangString DESC_UnInstall_Full ${LANG_FRENCH} "There is 3rd party skins installed, Would you like to remove these also?"

LangString DESC_LanguageFile ${LANG_FRENCH} "Language_fr"
LangString DESC_MediaPortalPlugin ${LANG_FRENCH} "que le plugin MediaPortal/serveur$\n$\nUtilisez cette option si votre écran / moniteur est sur ??un autre ordinateur."
LangString DESC_MPDisplayGUI ${LANG_FRENCH} "seulement MPDisplay++ à écran tactile/affichage de l'application$\n$\nUtilisez cette option si vous souhaitez installer MPD++ sur un autre ordinateur sans Media Portal."
LangString DESC_FullInstall ${LANG_FRENCH} "MPDisplay + + installation complète$\n$\nUtilisez cette option si votre écran/moniteur directement à votre MediaPortal HTPC est connecté."
LangString DESC_DesktopShortcutInstall ${LANG_FRENCH} "Raccourcis Bureau"
LangString DESC_FirewallException ${LANG_FRENCH} "Ajouter des exceptions de pare-feu pour MPDisplay++ ajouté."
LangString DESC_LaunchConfig ${LANG_FRENCH} "Début configuration MPDisplay."


LangString MSGDotNet ${LANG_FRENCH} "${APP_NAME} .NET Framework 4.5 doit être installé. Le. NET Framework sera automatiquement pendant le processus d'installation ${APP_NAME} téléchargé et installé."
LangString MSGDotNet_Cancel ${LANG_FRENCH} "L'installation ne peut pas être effectuée jusqu'à ce que le. NET Framework 4.0 n'est pas installé. $\n$\nL'installation est terminée!"
LangString MSGUnIninstall_Confirm ${LANG_FRENCH} "Tous les réglages et peaux de MPDisplay++ sera supprimé. $\n$\n Etes-vous sûr que l'application sera désinstallée?"
LangString MSGUnIninstall_ConfirmBackup ${LANG_FRENCH} "Tous les réglages et peaux de MPDisplay++ sera supprimé. $\n$\n Voulez-vous être d'effectuer une sauvegarde?"
LangString MSGUnInstall ${LANG_FRENCH} "La version actuellement installée de MPD++ doit être désinstallé avant. $\n$\nTous les réglages de MPDisplay++ sereont préservées. $\n$\nDevrait maintenant être désinstallé l'ancienne version?"
LangString MSGUnInstall_Cancel ${LANG_FRENCH} "Ne devrait pas être la configuration de procéder dans un autre MPD++ processus est effectuée. $\n$\nL'installation est terminée!"
LangString MSGDotNet_NeedRestart ${LANG_FRENCH} "Erreur lors de l'installation de la MPDisplay++ serveur. $\n$\nRépéter?$\n$\nRemarque:. Si vous avez juste le NET Framework 4.0 est installé, l'ordinateur doit être démarré avant toute nouvelle."
LangString MSGMediaPortalNotFound ${LANG_FRENCH} "Le programme d'installation ne peut pas trouver le répertoire d'installation de MediaPortal. S'il vous plaît chercher manuellement. $\n$\n par exemple: C:\Program Files\Team MediaPortal\MediaPortal"
LangString MSGXPWarn ${LANG_FRENCH} "MPDisplay does not support Windows XP operating system"

LangString MSGUAC_Retry ${LANG_FRENCH} "Cette application nécessite des privilèges d'administrateur, essayez à nouveau?"
LangString MSGUAC_Abort ${LANG_FRENCH} "Cette application nécessite des droits d'administrateur, de la démolition!"
LangString MSGUAC_NoLogon ${LANG_FRENCH} "Service Ouverture de session ne fonctionne pas, arrêtez-vous!"
LangString MSGUAC_Error ${LANG_FRENCH} "Les droits des utilisateurs ne peut être soulevée en cas d'erreur de droits d'administrateur: "
##-------------------------------------------------------------------------------------------------##
##-------------------------------------------------------------------------------------------------##


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

LangString DESC_UnInstall_Full ${LANG_ITALIAN} "There is 3rd party skins installed, Would you like to remove these also?"

LangString DESC_LanguageFile ${LANG_ITALIAN} "Language_it"
LangString DESC_MediaPortalPlugin ${LANG_ITALIAN} "Solo Plugin/Server MediaPortal$\n$\nScegli quest'opzione se il tuo TouchScreen/Display è su un altro PC."
LangString DESC_MPDisplayGUI ${LANG_ITALIAN} "Solo applicazione TouchScreen/Display di MPDisplay++$\n$\nScegli questa opzione se stai installando MPDisplay++ su un PC diverso da quello dove è installato MediaPortal."
LangString DESC_FullInstall ${LANG_ITALIAN} "Installazione completa di MPDisplay++$\n$\nScegli questa opzione se il tuo TouchScreen/Display è collegato al tuo HTPC MediaPortal."
LangString DESC_DesktopShortcutInstall ${LANG_ITALIAN} "Collegamenti Desktop"
LangString DESC_FirewallException ${LANG_ITALIAN} "Aggiungi eccezioni al firewall per MPDisplay++."
LangString DESC_LaunchConfig ${LANG_ITALIAN} "Esegui la configurazione di MPDisplay."


LangString MSGDotNet ${LANG_ITALIAN} "${APP_NAME} richiede che il .NET Framework 4.5 sia installato. Il .NET Framework sarà scaricato e installato automaticamente durante l'installazione di ${APP_NAME}."
LangString MSGDotNet_Cancel ${LANG_ITALIAN} "Il Setup non può procedere finchè .NET Framework 4.5 non verrà installato $\n$\nIl Setup si chiuderà ora!"
LangString MSGUnIninstall_Confirm ${LANG_ITALIAN} "Tutti gli attuali Settaggi/Skin di MPDisplay++ saranno persi $\n$\n Sicuro di voler disinstallare ora?"
LangString MSGUnIninstall_ConfirmBackup ${LANG_ITALIAN} "Tutti gli attuali Settaggi/Skin di MPDisplay++ saranno persi $\n$\n Vuoi fare un backup di questi settaggi ora?"
LangString MSGUnInstall ${LANG_ITALIAN} "L'attuale versione di MPDisplay++ deve essere rimossa prima dell'installazione $\n$\nVuoi disinstallare ora?"
LangString MSGUnInstall_Cancel ${LANG_ITALIAN} "Il setup non può procedere prima che l'attuale istanza di MPDisplay++ venga rimossa $\n$\nIl Setup si chiuderà ora"
LangString MSGDotNet_NeedRestart ${LANG_ITALIAN} "Errore nell'installazione di MPDisplay Server $\n$\nRiprova?$\n$\nNota: se hai appena installato .NET Framework 4.5 potrebbe essere necessario un riavvio prima che MPDisplay Server venga installato"
LangString MSGMediaPortalNotFound ${LANG_ITALIAN} "L'installer non ha individuato l'installazione di MediaPortal, selezionare a mano la cartella di installazione $\n$\n ES: C:\Program Files\Team MediaPortal\MediaPortal"
LangString MSGXPWarn ${LANG_ITALIAN} "MPDisplay does not support Windows XP operating system"

LangString MSGUAC_Retry ${LANG_ITALIAN} "Questa applicazione richiede privilegi amministrativi, riprovare?"
LangString MSGUAC_Abort ${LANG_ITALIAN} "Questa applicazione richiede privilegi amministrativi, annullamento in corso!"
LangString MSGUAC_NoLogon ${LANG_ITALIAN} "Servizio Logon non è in esecuzione, annullamento in corso!"
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
  
FunctionEnd

Function WelcomePagePre
   # Check UAC
   !insertmacro Check_UAC "Installer"
   
  !insertmacro MUI_LANGDLL_DISPLAY



    ${If} ${AtLeastWinVista}

		# Set Registry Permissons
		AccessControl::GrantOnRegKey \
		${REG_HKLM} "${REG_UNINSTALL_PATH}" "(S-1-5-32-545)" "FullAccess"
		AccessControl::GrantOnRegKey \
		${REG_HKLM} "${REG_APP_PATH}" "(S-1-5-32-545)" "FullAccess"
		
		# Check if MPDisplay++ Is Already Installed, and Uninstall
		ReadRegDWORD $1 ${REG_HKLM} "${REG_UNINSTALL_PATH}" UnInstallString
		${If} $1 != ''
		
			MessageBox MB_YESNO|MB_ICONEXCLAMATION "$(MSGUnInstall)" IDYES UninstallYes IDNO UninstallNo
		
			UninstallNo:
				MessageBox MB_OK|MB_ICONINFORMATION $(MSGUnInstall_Cancel)
				Quit
		
			UninstallYes:
			        WriteRegDWORD ${REG_HKLM} "${REG_APP_PATH}"  "KeepSettings" 1
				ExecWait '$1 _?=${PROGRAM_DATA}' $3
				#If Uninstall was canceled(0) Exit installer
				${If} $3 == 0
					Quit
				${EndIf}
		${EndIf}
		
		
		# .NET4.5
		ClearErrors
		ReadRegDWORD $0 HKLM "SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full" "Release"
		IfErrors NotDetected
		${If} $0 >= 378389
			DetailPrint "Microsoft .NET Framework 4.5 is installed ($0)"
		${Else}
		NotDetected:
			MessageBox MB_YESNO|MB_ICONINFORMATION $(MSGDotNet) IDYES InstallDotNet IDNO Cancel 
			Cancel:
				MessageBox MB_OK|MB_ICONINFORMATION $(MSGDotNet_Cancel)
					Quit

			; Install .NET4.5.
			InstallDotNet:
				DetailPrint "Installing Microsoft .NET Framework 4.5"
				SetDetailsPrint listonly
				SetOutPath "$TEMP"	
					File "${BUILD_FOLDER}\dotNetFx45_Full_setup.exe"
				ExecWait "$TEMP\dotNetFx45_Full_setup.exe"	
				${If} $0 == 3010 
				${OrIf} $0 == 1641
					DetailPrint "Microsoft .NET Framework 4.5 installer requested reboot."
					SetRebootFlag true 
					Quit
				${EndIf}
				SetDetailsPrint lastused
				DetailPrint "Microsoft .NET Framework 4.5 installer returned $0"
		${EndIf}
	
	
	${Else}
	    MessageBox MB_OK|MB_ICONINFORMATION $(MSGXPWarn)
    ${EndIf} 
	

FunctionEnd

##-------------------------------------------------------------------------------------------------##
##-------------------------------------------------------------------------------------------------##
##-------------------------------------------------------------------------------------------------##
##-------------------------------------------------------------------------------------------------##




##-------------------------------------------------------------------------------------------------##
##---------*FullInstall----------------------------------------------------------------------------##
##-------------------------------------------------------------------------------------------------##
##-------------------------------------------------------------------------------------------------##
Section "$(DESC_SECTION_FullInstall)" FullInstall
    SectionIn 1
	# Install GUI
	SetOutPath "$INSTDIR\"
		CreateDirectory "$INSTDIR\"
			File "${BUILD_FOLDER}\GUI\*.*"
			File "${BUILD_FOLDER}\Server\*.*"
			File "${BUILD_FOLDER}\Config\*.*"
			
    # Install User Data		
	SetOutPath "${PROGRAM_DATA}"
		CreateDirectory "${PROGRAM_DATA}"
	# Set Access On Data Folder so we can edit files	
    AccessControl::GrantOnFile \
    "${PROGRAM_DATA}" "(S-1-5-32-545)" "FullAccess"
    
        SetOverwrite off
     	File /r "${BUILD_FOLDER}\Data\MPDisplay.xml"
	File /r "${BUILD_FOLDER}\Data\AdvancedPluginSettings.xml"
	File /r "${BUILD_FOLDER}\Data\AddImageSettings.xml"
	SetOverwrite on
	
	File /r "${BUILD_FOLDER}\Data\Skin\*.*"
		
	# Install Language Files
	SetOutPath "${PROGRAM_DATA}Language\"
		CreateDirectory "${PROGRAM_DATA}Language"
			File /r "${BUILD_FOLDER}\Data\Language\*.*"

	# Install skinEditor data Files
	SetOutPath "${PROGRAM_DATA}SkinEditor\"
		CreateDirectory "${PROGRAM_DATA}SkinEditor"
			File /r "${BUILD_FOLDER}\Data\SkinEditor\*.*"
    
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
			  
	# Write Registry Entries
        WriteRegStr ${REG_HKLM} "${REG_APP_PATH}" "LogLevel" "${LOGLEVEL}"
	WriteRegStr ${REG_HKLM} "${REG_APP_PATH}" "MPDisplayPath" "$INSTDIR"	
	WriteRegStr ${REG_HKLM} "${REG_APP_PATH}" "MPDisplayExePath" "$INSTDIR\${MAIN_APP_EXE}"
	WriteRegStr ${REG_HKLM} "${REG_APP_PATH}" "MPDisplayConfigExePath" "$INSTDIR\MPDisplayConfig.exe"
	WriteRegStr ${REG_HKLM} "${REG_APP_PATH}" "MPDServerExePath" "$INSTDIR\MPDisplayServer.exe"	
        WriteRegStr ${REG_HKLM} "${REG_APP_PATH}" "SkinEditorExePath" "$INSTDIR\SkinEditor.exe"
	WriteRegStr ${REG_HKLM} "${REG_APP_PATH}" "ProgramDataPath" "${PROGRAM_DATA}"
        WriteRegStr ${REG_HKLM} "${REG_APP_PATH}" "InstallType" "Full"
	WriteRegStr ${REG_HKLM} "${REG_APP_PATH}" "LanguageFile" "$(DESC_LanguageFile)"
	 
	# Install And Start Service
	ReInstallService:
	nsSCM::Install "MPDisplayServer" "MPDisplayServer" 16 2 "$INSTDIR\MPDisplayServer.exe" "" "" "" ""
	nsSCM::Start "MPDisplayServer"	
		Pop $0 ; return error/success
			${If} $0 == 'error'
				MessageBox MB_YESNO|MB_ICONINFORMATION  "$(MSGDotNet_NeedRestart)" IDYES ReInstallService IDNO Cancel
					Cancel:
				    Abort
			${EndIf}
			
SectionEnd	
##-------------------------------------------------------------------------------------------------##
##-------------------------------------------------------------------------------------------------##


##-------------------------------------------------------------------------------------------------##
##---------*MediaPortalPlugin----------------------------------------------------------------------##
##-------------------------------------------------------------------------------------------------##
##-------------------------------------------------------------------------------------------------##
Section /o "$(DESC_SECTION_MediaPortalPlugin)" MediaPortalPlugin
     SectionIn 1 RO
	 
		 # Install Server
	SetOutPath "$INSTDIR\"
		CreateDirectory "$INSTDIR\"
			File "${BUILD_FOLDER}\Server\*.*"
			File "${BUILD_FOLDER}\Config\*.*"
			
  	# Install User Data		
	SetOutPath "${PROGRAM_DATA}"
		CreateDirectory "${PROGRAM_DATA}"
	# Set Access On Data Folder so we can edit files	
    AccessControl::GrantOnFile \
    "${PROGRAM_DATA}" "(S-1-5-32-545)" "FullAccess"
                SetOverwrite off
	  	File /r "${BUILD_FOLDER}\Data\MPDisplay.xml"
		File /r "${BUILD_FOLDER}\Data\AdvancedPluginSettings.xml"
		File /r "${BUILD_FOLDER}\Data\AddImageSettings.xml"
		SetOverwrite on

	# Install Language Files
	SetOutPath "${PROGRAM_DATA}Language\"
		CreateDirectory "${PROGRAM_DATA}Language"
			File /r "${BUILD_FOLDER}\Data\Language\*.*"
		
	
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
	
  	# Write Registry Entries
    WriteRegStr ${REG_HKLM} "${REG_APP_PATH}" "LogLevel" "${LOGLEVEL}"
	WriteRegStr ${REG_HKLM} "${REG_APP_PATH}" "MPDisplayPath" "$INSTDIR"	
	WriteRegStr ${REG_HKLM} "${REG_APP_PATH}" "MPDServerExePath" "$INSTDIR\MPDisplayServer.exe"	
	WriteRegStr ${REG_HKLM} "${REG_APP_PATH}" "MPDisplayConfigExePath" "$INSTDIR\MPDisplayConfig.exe"
	WriteRegStr ${REG_HKLM} "${REG_APP_PATH}" "ProgramDataPath" "${PROGRAM_DATA}"
    WriteRegStr ${REG_HKLM} "${REG_APP_PATH}" "InstallType" "Plugin"
	WriteRegStr ${REG_HKLM} "${REG_APP_PATH}" "LanguageFile" "$(DESC_LanguageFile)"
	
	# Install And Start Service
	ReInstallService:
	nsSCM::Install "MPDisplayServer" "MPDisplayServer" 16 2 "$INSTDIR\MPDisplayServer.exe" "" "" "" ""
	nsSCM::Start "MPDisplayServer"	
		Pop $0 ; return error/success
			${If} $0 == 'error'
				MessageBox MB_YESNO|MB_ICONINFORMATION  "$(MSGDotNet_NeedRestart)" IDYES ReInstallService IDNO Cancel
					Cancel:
				    Abort
			${EndIf}
	
SectionEnd
##-------------------------------------------------------------------------------------------------##
##-------------------------------------------------------------------------------------------------##




##-------------------------------------------------------------------------------------------------##
##---------*MPDisplayGUI---------------------------------------------------------------------------##
##-------------------------------------------------------------------------------------------------##
##-------------------------------------------------------------------------------------------------##
Section /o "$(DESC_SECTION_MPDisplayGUI)" MPDisplayGUI
    SectionIn 1 RO
	
	# Install GUI
	SetOutPath "$INSTDIR\"
		CreateDirectory "$INSTDIR\"
			File "${BUILD_FOLDER}\GUI\*.*"
			File "${BUILD_FOLDER}\Config\*.*"
	
	# Install User Data		
	SetOutPath "${PROGRAM_DATA}"
		CreateDirectory "${PROGRAM_DATA}"
	# Set Access On Data Folder so we can edit files	
    AccessControl::GrantOnFile \
    "${PROGRAM_DATA}" "(S-1-5-32-545)" "FullAccess"
        SetOverwrite off
     	File /r "${BUILD_FOLDER}\Data\MPDisplay.xml"
	File /r "${BUILD_FOLDER}\Data\AddImageSettings.xml"
	SetOverwrite on
	File /r "${BUILD_FOLDER}\Data\Skin\*.*"
  		
	# Install Language Files
	SetOutPath "${PROGRAM_DATA}Language\"
		CreateDirectory "${PROGRAM_DATA}Language"
			File /r "${BUILD_FOLDER}\Data\Language\*.*"

	# Install skinEditor data Files
	SetOutPath "${PROGRAM_DATA}SkinEditor\"
		CreateDirectory "${PROGRAM_DATA}SkinEditor"
			File /r "${BUILD_FOLDER}\Data\SkinEditor\*.*"
	
	# Write Registry Entries
    WriteRegStr ${REG_HKLM} "${REG_APP_PATH}" "LogLevel" "${LOGLEVEL}"
	WriteRegStr ${REG_HKLM} "${REG_APP_PATH}" "MPDisplayPath" "$INSTDIR"	
	WriteRegStr ${REG_HKLM} "${REG_APP_PATH}" "MPDisplayExePath" "$INSTDIR\${MAIN_APP_EXE}"
	WriteRegStr ${REG_HKLM} "${REG_APP_PATH}" "MPDisplayConfigExePath" "$INSTDIR\MPDisplayConfig.exe"
    WriteRegStr ${REG_HKLM} "${REG_APP_PATH}" "SkinEditorExePath" "$INSTDIR\SkinEditor.exe"
	WriteRegStr ${REG_HKLM} "${REG_APP_PATH}" "ProgramDataPath" "${PROGRAM_DATA}"
    WriteRegStr ${REG_HKLM} "${REG_APP_PATH}" "InstallType" "GUI"
	WriteRegStr ${REG_HKLM} "${REG_APP_PATH}" "LanguageFile" "$(DESC_LanguageFile)"
	
SectionEnd
##-------------------------------------------------------------------------------------------------##
##-------------------------------------------------------------------------------------------------##



##-------------------------------------------------------------------------------------------------##
##---------*DesktopShortcutInstall-----------------------------------------------------------------##
##-------------------------------------------------------------------------------------------------##
##-------------------------------------------------------------------------------------------------##
Section "$(DESC_SECTION_DesktopShortcutInstall)" DesktopShortcutInstall
 SectionIn 1
  ${If} ${SectionIsSelected} ${MediaPortalPlugin}
  #do nothing
  ${Else}
    SetShellVarContext current	
		CreateShortCut "$DESKTOP\${APP_NAME}.lnk" "$INSTDIR\MPDisplay.exe"
		CreateShortCut "$DESKTOP\${APP_NAME}Config.lnk" "$INSTDIR\MPDisplayConfig.exe"
		CreateDirectory "$SMPROGRAMS\${APP_NAME}"
		CreateShortCut "$SMPROGRAMS\${APP_NAME}\${APP_NAME}.lnk" "$INSTDIR\MPDisplay.exe"
		CreateShortCut "$SMPROGRAMS\${APP_NAME}\${APP_NAME}Config.lnk" "$INSTDIR\MPDisplayConfig.exe"
        CreateShortCut "$SMPROGRAMS\${APP_NAME}\${APP_NAME}SkinEditor.lnk" "$INSTDIR\SkinEditor.exe"
		CreateShortCut "$SMPROGRAMS\${APP_NAME}\Uninstall.lnk" "$INSTDIR\${UNINSTALLER_EXE_NAME}"
	SetShellVarContext all	
  ${EndIf}
SectionEnd


##-------------------------------------------------------------------------------------------------##
##-------------------------------------------------------------------------------------------------##


##-------------------------------------------------------------------------------------------------##
##---------*DesktopShortcutInstall-----------------------------------------------------------------##
##-------------------------------------------------------------------------------------------------##
##-------------------------------------------------------------------------------------------------##
Section "$(DESC_SECTION_FirewallException)" FirewallException
 SectionIn 1
  ${If} ${SectionIsSelected} ${FullInstall}
    SimpleFC::AdvAddRule "MPDisplayServer (TCP In)" "Allows incoming requests." "6" "1" "1" "7" "1" "$INSTDIR\MPDisplayServer.exe" "" "MPDisplayServer" "" "" "" ""
	SimpleFC::AdvAddRule "MPDisplayServer (TCP Out)" "Allows outgoing requests." "6" "2" "1" "7" "1" "$INSTDIR\MPDisplayServer.exe" "" "MPDisplayServer" "" "" "" ""
  ${EndIf}
  ${If} ${SectionIsSelected} ${MPDisplayGUI}
    SimpleFC::AdvAddRule "MPDisplay++ (TCP In)" "Allows incoming requests." "6" "1" "1" "7" "1" "$INSTDIR\MPDisplay.exe" "" "MPDisplay++" "" "" "" ""
	SimpleFC::AdvAddRule "MPDisplay++ (TCP Out)" "Allows outgoing requests." "6" "2" "1" "7" "1" "$INSTDIR\MPDisplay.exe" "" "MPDisplay++" "" "" "" ""
  ${EndIf}
  ${If} ${SectionIsSelected} ${MediaPortalPlugin}
	SimpleFC::AdvAddRule "MPDisplayServer (TCP In)" "Allows incoming requests." "6" "1" "1" "7" "1" "$INSTDIR\MPDisplayServer.exe" "" "MPDisplayServer" "" "" "" ""
	SimpleFC::AdvAddRule "MPDisplayServer (TCP Out)" "Allows outgoing requests." "6" "2" "1" "7" "1" "$INSTDIR\MPDisplayServer.exe" "" "MPDisplayServer" "" "" "" ""
  ${EndIf}
SectionEnd
##-------------------------------------------------------------------------------------------------##
##-------------------------------------------------------------------------------------------------##




##-------------------------------------------------------------------------------------------------##
##-------------------------------------------------------------------------------------------------##
##-----------FUNCTIONS-----------------------------------------------------------------------------##
##-------------------------------------------------------------------------------------------------##


Function .onSelChange
	SectionGetFlags ${FullInstall} $R0
	SectionGetFlags ${MPDisplayGUI} $R1
	SectionGetFlags ${MediaPortalPlugin} $R2
	IntOp $R0 $R0 & ${SF_SELECTED}
	IntOp $R1 $R1 & ${SF_SELECTED}
	IntOp $R2 $R2 & ${SF_SELECTED}
	${If} $R0 == ${SF_SELECTED}
		!insertmacro ClearSectionFlag ${MPDisplayGUI} ${SF_RO}
		!insertmacro UnSelectSection ${MPDisplayGUI}
		!insertmacro SetSectionFlag ${MPDisplayGUI} ${SF_RO}
	    !insertmacro ClearSectionFlag ${MediaPortalPlugin} ${SF_RO}
		!insertmacro UnSelectSection ${MediaPortalPlugin}
		!insertmacro SetSectionFlag ${MediaPortalPlugin} ${SF_RO}
	    !insertmacro ClearSectionFlag ${DesktopShortcutInstall} ${SF_RO}
	${EndIf}
	${If} $R0 != ${SF_SELECTED}
		!insertmacro ClearSectionFlag ${MPDisplayGUI} ${SF_RO}
		!insertmacro ClearSectionFlag ${MediaPortalPlugin} ${SF_RO}	
	${EndIf}
	${If} $R1 == ${SF_SELECTED}
		!insertmacro UnSelectSection ${MediaPortalPlugin}
		!insertmacro ClearSectionFlag ${DesktopShortcutInstall} ${SF_RO}
	${EndIf}  
	${If} $R2 == ${SF_SELECTED}
	    !insertmacro ClearSectionFlag ${DesktopShortcutInstall} ${SF_RO}
		!insertmacro UnSelectSection ${DesktopShortcutInstall}
		!insertmacro SetSectionFlag ${DesktopShortcutInstall} ${SF_RO}
		!insertmacro UnSelectSection ${MPDisplayGUI}
	${EndIf}
FunctionEnd

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

#Give the sections descriptions for the component checklist#
!insertmacro MUI_FUNCTION_DESCRIPTION_BEGIN
  !insertmacro MUI_DESCRIPTION_TEXT ${MPDisplayGUI} $(DESC_MPDisplayGUI)
  !insertmacro MUI_DESCRIPTION_TEXT ${MediaPortalPlugin} $(DESC_MediaPortalPlugin)
  !insertmacro MUI_DESCRIPTION_TEXT ${FullInstall} $(DESC_FullInstall)
  !insertmacro MUI_DESCRIPTION_TEXT ${DesktopShortcutInstall} $(DESC_DesktopShortcutInstall)
!insertmacro MUI_FUNCTION_DESCRIPTION_END


##-------------------------------------------------------------------------------------------------##
##-------------------------------------------------------------------------------------------------##




##-------------------------------------------------------------------------------------------------##
##---------*UNINSTALL*-----------------------------------------------------------------------------##
##-------------------------------------------------------------------------------------------------##
##-------------------------------------------------------------------------------------------------##
Section -WriteUninstaller
 	${If} ${SectionIsSelected} ${MediaPortalPlugin}
		WriteRegStr ${REG_HKLM} "${REG_UNINSTALL_PATH}"  "DisplayIcon" "$INSTDIR\MPDisplayServer.exe"
	${Else}
		WriteRegStr ${REG_HKLM} "${REG_UNINSTALL_PATH}"  "DisplayIcon" "$INSTDIR\${MAIN_APP_EXE}"
	${EndIf}
        WriteRegStr ${REG_HKLM} "${REG_UNINSTALL_PATH}"  "DisplayName" "${APP_NAME}"
        WriteRegStr ${REG_HKLM} "${REG_UNINSTALL_PATH}"  "UninstallString" "$INSTDIR\${UNINSTALLER_EXE_NAME}"
        WriteRegStr ${REG_HKLM} "${REG_UNINSTALL_PATH}"  "DisplayVersion" "${VERSION}"
        WriteRegStr ${REG_HKLM} "${REG_UNINSTALL_PATH}"  "Publisher" "${GROUP_NAME}"
        
	SetOutPath "$INSTDIR\"
	WriteUninstaller "$INSTDIR\${UNINSTALLER_EXE_NAME}"
SectionEnd

Section Uninstall
	SetAutoClose true

	    ReadRegDWORD $0 ${REG_HKLM} "${REG_APP_PATH}" MPDisplayPath
			Delete "$0\${UNINSTALLER_EXE_NAME}"
            ReadRegDWORD $3 ${REG_HKLM} "${REG_APP_PATH}" "KeepSettings"
            
        # Check User Rights----------------------------------------------------##
		!insertmacro Check_UAC "UnInstaller"
		SetShellVarContext all 
		AccessControl::GrantOnRegKey \
		${REG_HKLM} "${REG_APP_PATH}" "(S-1-5-32-545)" "FullAccess"
		##---------------------------------------------------------------------##
		
		
		# Remove Firewall Exeptions--------------------------------------------##
		SimpleFC::AdvExistsRule "MPDisplayServer (TCP In)"
		Pop $1 ; return error(1)/success(0)
		${If} $1 == 0
			 SimpleFC::AdvRemoveRule "MPDisplayServer (TCP In)"
		${EndIf}
		SimpleFC::AdvExistsRule "MPDisplayServer (TCP Out)"
		Pop $1 ; return error(1)/success(0)
		${If} $1 == 0
			 SimpleFC::AdvRemoveRule "MPDisplayServer (TCP Out)"
		${EndIf}
		
		SimpleFC::AdvExistsRule "MPDisplay++ (TCP In)"
		Pop $1 ; return error(1)/success(0)
		${If} $1 == 0
			 SimpleFC::AdvRemoveRule "MPDisplay++ (TCP In)"
		${EndIf}
		SimpleFC::AdvExistsRule "MPDisplay++ (TCP Out)"
		Pop $1 ; return error(1)/success(0)
		${If} $1 == 0
			 SimpleFC::AdvRemoveRule "MPDisplay++ (TCP Out)"
		${EndIf}
		##---------------------------------------------------------------------##
		
		#Stop and remove the service-------------------------------------------##
		nsSCM::Stop "MPDisplayServer"
		Sleep 3000
		nsSCM::Remove "MPDisplayServer"
		##---------------------------------------------------------------------##
		
		# Remove Plugin Files -------------------------------------------------##  
		ReadRegDWORD $0 ${REG_HKLM} "${REG_APP_PATH}" MPDPluginPath
		Delete "$0\MPDisplay.Common.dll"
		Delete "$0\MPDisplay.MessageFramework.dll"
		Delete "$0\MPDisplay.Plugin.dll"
		##---------------------------------------------------------------------##
		
		# Remove GUI/Server Files/Directory -----------------------------------##
		ReadRegDWORD $1 ${REG_HKLM} "${REG_APP_PATH}" MPDisplayPath
		Delete "$1\${UNINSTALLER_EXE_NAME}"
		RmDir /r "$1"
		RmDir "$1" #Directory will not be removed if not empty,(user safe, incase some fucktard installed in C:\ or something)
		##---------------------------------------------------------------------##
		
		#Remove Shortcuts------------------------------------------------------##
		SetShellVarContext current	
		Delete "$DESKTOP\${APP_NAME}.lnk"
		Delete "$DESKTOP\${APP_NAME}Config.lnk"
		RmDir /r  "$SMPROGRAMS\${APP_NAME}"
		##---------------------------------------------------------------------##
	
	    # Remove UserData Files   ---------------------------------------------##
		ReadRegDWORD $0 ${REG_HKLM} "${REG_APP_PATH}" ProgramDataPath
		RmDir /r "$0\Language"
		RmDir /r "$0\Logs"
                RmDir /r "$0\SkinEditor"
		RmDir /r "$0\Skin\Titan"
                RmDir /r "$0\Skin\Titan-WS"
		RmDir "$0\Skin"
	
		# Ask if the user wants to remove all 3rd party skins.
		${If} ${FileExists} "$0\Skin\*.*"
	          MessageBox MB_YESNO|MB_ICONQUESTION "$(DESC_UnInstall_Full)" IDYES BackupYes IDNO BackupNo
				  BackupYes:
						RmDir /r "$0\Skin"
				  BackupNo:
		${EndIf}	
		
		${If} $3 != 1
                      Delete "$0\MPDisplay.xml"
                      Delete "$0\AdvancedPluginSettings.xml"
                      Delete "$0\AddImageSettings.xml"
	              RmDir "$0"
	        ${EndIf}
	        
		DeleteRegKey ${REG_HKLM} "${REG_APP_PATH}" 
		DeleteRegKey ${REG_HKLM} "${REG_UNINSTALL_PATH}" 
	 
SectionEnd
##-------------------------------------------------------------------------------------------------##
##-------------------------------------------------------------------------------------------------##
##-------------------------------------------------------------------------------------------------##
##-------------------------------------------------------------------------------------------------##

