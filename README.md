# HostsManager
This program allows you to update your hosts-file with a blacklist.<br>
The blacklisted traffic is being redirected to a IP/Path (localhost).<br>
Therfore you are protected from tracking and from spam ads and from whatever you want.<br>
(In short...)<br>

![Screenshot](http://i.imgur.com/QR935Oo.png "Screenshot")<br>

<br>
<h3><b>In long ...:</b></h3>
- The Hosts-File-Downloader & -Updater pulls a new/updated hosts-file from any online source (URL).<br>
- The Hosts-Files-List is saved by the app.<br>
More to the hosts-file: https://en.wikipedia.org/wiki/Hosts_(file)<br>
<br>
<br>
<h3><b>Possible Hosts-Files:</b></h3>
https://github.com/StevenBlack/hosts<br>
https://github.com/LV-Crew/hosts<br>
<br>
<br>
<h3><b>Antivirus:</b></h3>
Many antivirus programs do block the hosts file in a way, that we can't unblock it.<br>
Please refer to your antivirus program's manual to find out how to disable hosts file protection.<br>
<br>
Avira Antivirus: https://answers.avira.com/de/question/avira-blocks-hosts-file-what-can-i-do-90<br>
Others: https://www.devside.net/wamp-server/unlock-and-unblock-the-windows-hosts-file<br>
<br>
<br>
<h3><b>Settings:</b></h3>

![Screenshot](http://i.imgur.com/8cXddH1.png "Screenshot")<br>

<br>
Overwrite hosts file URLs:<br>
=======================================================================<br>
This allows you to choose the URLs of the hosts files to be imported manually, overwriting the system setting.<br>
<br>
Overwrite IP:<br>
=======================================================================<br>
Most host file are in the format 0.0.0.0 <hostname>, which redirects the traffic to localhost.<br>
LV-Crew HostsManager allows you to overwrite this adress with an adress you choose.<br>
By default, it redirects to a white page.<br>
<br>
Hosts file editor:<br>
=======================================================================<br>
Choose whether to use the internal text editor or wordpad for editing the hosts file.<br>
<br>
Autmatically update hosts file hourly:<br>
=======================================================================<br>
This allows you to automatically update the hosts file. <br>
It creates a new task in windows' task planer.<br>
<br>
<br>
<h3><b>Roundmap:</b></h3>
Some ideas and a 'small' roadmap in a to-dos-stlye... ;-)<br>
<br>
To-Dos:<br>
- NSIS-Installer<br>
- Internal Editor<br>
- Web-Page<br>
- DNS for URL 34.213.32.36<br>
<br>
To-Dos Branding:<br>
- Link to  http://HostsManager.LV-Crew.org<br>
- Link http://HostsManager.LV-Crew.org to 34.213.32.36 to https://github.com/LV-Crew/HostsManager<br>
- 'LV-Crew'-Logo<br>
<br>
To-Dos Beat:<br>
- https://sourceforge.net/projects/hostsmanager/<br>
- https://scottlerch.github.io/HostsFileEditor/<br>
- http://hostsman2.it-mate.co.uk/<br>
<br>
To-Dos .NET:<br>
- Use wget<br>
- Remove .NET<br>
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
<br>
<h3><b>Ideas:</b></h3>
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
- 2 counter für alles und seite<br>
<br>
<br>
<h3><b>Changelog:</b></h3>
v2017.08.06b by Dennis M. Heine & Tobias B. Besemer<br>
- Branding is saved in INI<br>
Release: https://github.com/LV-Crew/HostsManager/releases/latest<br>
<br>
Till v2017.08.04b by Dennis M. Heine & Tobias B. Besemer<br>
- Initial Releases<br>
- Initial Branding with Logo & About Dialog<br>
- Detects Antiviruses and warns<br>
- Separate options dialog<br>
- Download-URL saved in registry<br>
- Default source: https://raw.githubusercontent.com/StevenBlack/hosts/master/hosts<br>
- External Editor Wordpad<br>
- Internal editor has been activated<br>
- File permissions are being reset after update<br>
- Added auto update<br>
- Added CR/LF-Detection/Addition<br>
- Also: Error-Page on http://34.213.32.36/<br>
- Also: Error-Page with SSL<br>
- Also: certutil already included (imports the SSL certificate into the Mozilla-Firefox certificate store)<br>
- Also: http://HostsManager.LV-Crew.org -> http://34.213.32.36/<br>
- InstallForge-Installer (http://installforge.net/)<br>
- Install to %PROGRAMFILES(X86)%\LV-Crew\HostsManager\<br>
- Register in Windows (system integration)<br>
- Publisher in Windows is 'LV-Crew'<br>
Release: https://github.com/LV-Crew/HostsManager/releases/<br>
<br>
<br>
<h3><b>Some Dev notes:</b></h3>
- Version-Bump to 2017.xx.xxa (yyyy.mm.dd + letter)<br>
- File-Names are LV-Crew.HostsManager.xxx.xyz<br>
