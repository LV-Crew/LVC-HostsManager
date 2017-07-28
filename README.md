# HostsManager
This program allows you to update your hosts-file with a blacklist.<br>
The blacklisted traffic is being redirected to a IP/Path (localhost).<br>
Therfore you are protected from tracking and from spam ads and from whatever you want.<br>
(In short...)<br>
<br>
In long ...:<br>
- The Hosts-File-Downloader & -Updater pulls a new/updated hosts-file from any online source (URL).<br>
- The Hosts-Files-List is saved by the app.<br>
More to the hosts-file: https://en.wikipedia.org/wiki/Hosts_(file)<br>
<br>
<br>
Possible Hosts-Files:<br>
https://github.com/StevenBlack/hosts<br>
https://github.com/LV-Crew/hosts<br>
<br>
<br>
Some ideas and a 'small' roadmap in a to-dos-stlye... ;-)<br>
<br>
To-Dos Branding:<br>
- About Dialog<br>
- Link to https://github.com/LV-Crew/HostsManager<br>
- 'LV-Crew' as Publisher<br>
- 'LV-Crew'-Logo<br>
<br>
To-Dos Data Storage:<br>
- Hosts-Files-List (multiple sources)<br>
- Load local files in Hosts-Files-List, too<br>
- Use INI, CFG, SQLite<br>
- Tags and descriptions to sources<br>
- Redirects per list<br>
- Redirected to any IP/Path (localhost, too)<br>
<br>
More To-Dos Hosts-Files-List:<br>
- Add Line-Breaks in hosts-files (CR+LF)<br>
<br>
To-Dos more Windows Integration:<br>
- Place data in %PROGRAMDATA%\LV-Crew\HostsManager\<br>
<br>
To-Dos better Windows Integration:<br>
- Systray-Icon<br>
- Schedult System Task<br>
- Start at boot<br>
<br>
To-Dos Multi-Plattform/Multi-Language:<br>
- Use Qt<br>
- Use wget<br>
- 64-bit-Version<br>
- NullSoft-Installer<br>
- Linux-/macOS-/BSD-/Android-Port<br>
- Multi-Language support<br>
<br>
To-Dos Error-Server:<br>
- Counter for Error-Page<br>
- Maybee Avira & Google-Ads<br>
- Error-Pages per lists<br>
<br>
Some ideas:<br>
- White-/Gray-/Blacklister read data e.g. from Cookies of Browsers<br>
<br>
Some more ideas:<br>
- Delete blacklisted Domains from the Cookies of the Browsers<br>
- Delete blacklisted Domains from the Java-/Flash-Cookies<br>
- Block IP4 & IP6<br>
- Work with squid<br>
- Use blacklists from squid<br>
- Use lists from Firefox<br>
- Import whitelist into CCleaner (Registry/INI)<br>
- Import whitelist into NoScript (SQLite)<br>
- Monitor port-forrwards (UPnP) from routers (AVM)<br>
- Bad-URLs-List from Avira<br>
<br>
More notes for later:<br>
- hosts temp off<br>
- Textfeld für eigenes hosts<br>
- Whitelist from Blacklist rauslöschen<br>
- C:\Program Files\CCleaner\ccleaner.ini<br>
- create backup for first run (.HostsManager)<br>
- Thunderbird<br>
- Monitising<br>
- In-App Advertisment Avira<br>
- Diladele.Squid.Service.exe<br>
- Uninstaller<br>
- Use VCRedist 2017<br>
- Cookies & blocked sources (pages)<br>
- wefisy<br>
- Integrate LV-Crew/certgen (https://github.com/LV-Crew/certgen)<br>
- Better Antivirus detection and warning<br>
<br>
<br>
Changelog:<br>
v2017.07.28a by torify<br>
- Version-Bump to 2017.xx.xxa (yyyy.mm.dd + letter)<br>
- ...<br>
<br>
Till v2017.07.28a by torify & Tobias-B-Besemer<br>
- Initial Releases<br>
- InstallForge-Installer (http://installforge.net/)<br>
- Install to %PROGRAMFILES(X86)%\LV-Crew\HostsManager\<br>
- Register in Windows (system integration)<br>
- Publisher in Windows is 'LV-Crew'<br>
- Initial Branding with Logo<br>
- First Menu<br>
- Separate options dialog<br>
- Download-URL saved in registry<br>
- Default source: https://raw.githubusercontent.com/StevenBlack/hosts/master/hosts<br>
- Detects Avira Antivirus and warns<br>
- Also: certutil already included (imports the SSL certificate in the Mozilla-Firefox certificate store)<br>
- Also: Error-Page on http://34.213.32.36/<br>
- Also: Error-Page with SSL<br>
Release: https://github.com/LV-Crew/HostsManager/releases/download/v1.1.1/setup.exe<br>
