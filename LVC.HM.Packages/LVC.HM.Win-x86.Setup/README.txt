HostsManager

This program allows you to update your hosts-file with a blacklist.
The blacklisted traffic is being redirected to a IP/Path (localhost).
Therfore you are protected from tracking and from spam ads and from whatever you want.
(In short...)

Screenshot

In long ...:

    The Hosts-File-Downloader & -Updater pulls a new/updated hosts-file from any online source (URL).
    The Hosts-Files-List is saved by the app.
    More to the hosts-file: https://en.wikipedia.org/wiki/Hosts_(file)



Possible Hosts-Files:
https://github.com/StevenBlack/hosts
https://github.com/LV-Crew/hosts


Settings

Screenshot

Overwrite hosts file URLs:
=================================================================
This allows you to choose the URLs of the hosts files to be imported manually, overwriting the system setting.

Overwrite IP:
=================================================================
Most host file are in the format 0.0.0.0 , which redirects the traffic to localhost.
LV-Crew HostsManager allows you to overwrite this adress with an adress you choose.
By default, it redirects to a white page.

Hosts file editor:
=================================================================
Choose whether to use the internal text editor or wordpad for editing the hosts file.

Autmatically update hosts file hourly:
=================================================================
This allows you to automatically update the hosts file.
It creates a new task in windows' task planer.

Roundmap

Some ideas and a 'small' roadmap in a to-dos-stlye... ;-)

To-Dos:

    NSIS-Installer
    Internal Editor
    Web-Page
    DNS for URL 34.213.32.36


To-Dos Branding:
- Link to http://HostsManager.LV-Crew.org
- Link http://HostsManager.LV-Crew.org to 34.213.32.36 to https://github.com/LV-Crew/HostsManager
- 'LV-Crew'-Logo

To-Dos Beat:
- https://sourceforge.net/projects/hostsmanager/
- https://scottlerch.github.io/HostsFileEditor/
- http://hostsman2.it-mate.co.uk/

To-Dos .NET:
- Use wget
- Remove .NET

To-Dos Data Storage:
- Hosts-Files-List (multiple sources)
- Load local files in Hosts-Files-List, too
- Use INI, CFG, SQLite
- Tags and descriptions to sources
- Redirects per list
- Redirected to any IP/Path (localhost, too)

More To-Dos Hosts-Files-List:
- Add Line-Breaks in hosts-files (CR+LF)

To-Dos more Windows Integration:
- Place data in %PROGRAMDATA%\LV-Crew\HostsManager\

To-Dos better Windows Integration:
- Systray-Icon
- Schedult System Task
- Start at boot

To-Dos Multi-Plattform/Multi-Language:
- Use Qt
- 64-bit-Version
- NullSoft-Installer
- Linux-/macOS-/BSD-/Android-Port
- Multi-Language support

To-Dos Error-Server:
- Counter for Error-Page
- Maybee Avira & Google-Ads
- Error-Pages per lists

Ideas

    White-/Gray-/Blacklister read data e.g. from Cookies of Browsers


Some more ideas:
- Delete blacklisted Domains from the Cookies of the Browsers
- Delete blacklisted Domains from the Java-/Flash-Cookies
- Block IP4 & IP6
- Work with squid
- Use blacklists from squid
- Use lists from Firefox
- Import whitelist into CCleaner (Registry/INI)
- Import whitelist into NoScript (SQLite)
- Monitor port-forrwards (UPnP) from routers (AVM)
- Bad-URLs-List from Avira

More notes for later:
- hosts temp off
- Textfeld für eigenes hosts
- Whitelist from Blacklist rauslöschen
- C:\Program Files\CCleaner\ccleaner.ini
- create backup for first run (.HostsManager)
- Thunderbird
- Monitising
- In-App Advertisment Avira
- Diladele.Squid.Service.exe
- Uninstaller
- Use VCRedist 2017
- Cookies & blocked sources (pages)
- wefisy
- Integrate LV-Crew/certgen (https://github.com/LV-Crew/certgen)
- Better Antivirus detection and warning
- 2 counter für alles und seite
- 2x umbenennen zu LV-Crew.xxx


Changelog:
v2017.08.04b by Dennis M. Heine & Tobias B. Besemer
- File permissions are being reset after update
- Internal editor has been activated
- Added auto update
- Added CR/LF-Detection/Addition
- Better Branding

Till v2017.08.03a by Dennis M. Heine & Tobias B. Besemer
- Initial Releases
- Initial Branding with Logo & About Dialog
- Detects Antiviruses and warns
- Separate options dialog
- Download-URL saved in registry
- Default source: https://raw.githubusercontent.com/StevenBlack/hosts/master/hosts
- External Editor Wordpad
- Also: Error-Page on http://34.213.32.36/
- Also: Error-Page with SSL
- Also: certutil already included (imports the SSL certificate into the Mozilla-Firefox certificate store)
- Also: http://HostsManager.LV-Crew.org -> http://34.213.32.36/
- No more: InstallForge-Installer (http://installforge.net/)
- No more: Install to %PROGRAMFILES(X86)%\LV-Crew\HostsManager\
- No more: Register in Windows (system integration)
- No more: Publisher in Windows is 'LV-Crew'
Release: https://github.com/LV-Crew/HostsManager/releases/download/2017.08.03a/LV-Crew.HostsManager.2017.08.03a.zip

Some Dev notes:
- Version-Bump to 2017.xx.xxa (yyyy.mm.dd + letter)