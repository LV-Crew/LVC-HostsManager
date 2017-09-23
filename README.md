# HostsManager
This program allows you to update your hosts file with a downloaded blacklist.<br>
The blacklisted traffic is being redirected to a whitepage (or a URL/localhost).<br>
Therefore tracking is being reduced to a minimum.<br>
You are also being protected from spam ads and more.<br>
(In short...)<br>
<br>
<br>
<h3><b>And now in long... ;-)</b></h3>
- The Hosts-File-Downloader & -Updater pulls a new/updated hosts-file from any online source (URL).<br>
- The Hosts-Files-List is saved by the app.<br>
More to the hosts-file: https://en.wikipedia.org/wiki/Hosts_(file)<br>
Screenshots & Manual: http://hostsmanager.lv-crew.org/readme.html<br>
LV-Crew HostsManager is broth to you by Dennis M. Heine & Tobias B. Besemer.<br>
LV-Crew HostsManager can be downloaded here: https://github.com/LV-Crew/HostsManager/releases/<br>
<br>
<br>
<h3><b>What you need:</b></h3>
Microsoft Windows 7-10 or equal<br>
Microsoft .NET v4.0 or above<br>
<br>
<br>
<h3><b>Which host files we use by default:</b></h3>
https://hosts-file.net/<br>
https://github.com/StevenBlack/hosts<br>
<br>
<br>
<h3><b>Which addional hosts files can be used:</b></h3>
https://github.com/LV-Crew/hosts<br>
<br>
<br>
<h3><b>What we already included in the program:</b></h3>
- Own hostnames can be saved in blacklist.xml<br>
<br>
<br>
<h3><b>And what else?</b></h3>
- Detects Antiviruses and warns<br>
- Branding is saved in a INI<br>
<br>
<br>
- Download-URL saved in registry<br>
- Default source: https://raw.githubusercontent.com/StevenBlack/hosts/master/hosts<br>
- External Editor Wordpad<br>
- Internal editor has been activated<br>
- File permissions are being reset after update<br>
- Added auto update<br>
- Added CR/LF-Detection/Addition<br>
- Also: Error-Page on http://46.163.76.11/<br>
- Also: Error-Page with SSL<br>
- Also: certutil already included (imports the SSL certificate into the Mozilla-Firefox certificate store)<br>
- Also: http://HostsManager.LV-Crew.org -> http://46.163.76.11/<br>
- InstallForge-Installer (http://installforge.net/)<br>
- Install to %PROGRAMFILES(X86)%\LV-Crew\HostsManager\<br>
- Register in Windows (system integration)<br>
- Publisher in Windows is 'LV-Crew'<br>
<br>
<br>
<h3><b>Some Dev notes:</b></h3>
- Versioning with 2017.xx.xxa (yyyy.mm.dd + letter)<br>
- File-Names are LV-Crew.HostsManager.xxx.xyz<br>
- Make.Packages.bat use WinZip Command Line Tool (http://www.winzip.com/win/en/downcl.html)<br>
- Build with Microsoft Visual Studio 2017 Community Edition<br>
- Default Icons from https://icons8.com<br>
- Homepage favicons generated with https://www.favicon-generator.org/<br>
<br>
<br>
<h3><b>Things to to:</b></h3>
Include Readme.html?<br>
Remove Readme.txt?<br>
Use local Readme.html?<br>
Include Readme.md?<br>
Include LICENSE?<br>
Updater http://wingup.org/<br>
New Whitepage<br>
Ads for Whitepage<br>
<br>
<br>
<h3><b>Roundmap:</b></h3>
Some ideas and a 'small' roadmap in a to-dos-stlye... ;-)<br>
<br>
<br>
To-Dos:<br>
- NSIS-Installer<br>
- Internal Editor<br>
- Web-Page<br>
- DNS for URL 46.163.76.11<br>
<br>
<br>
To-Dos Branding:<br>
- Link to  http://HostsManager.LV-Crew.org<br>
- Link http://HostsManager.LV-Crew.org to 46.163.76.11 to https://github.com/LV-Crew/HostsManager<br>
- 'LV-Crew'-Logo<br>
<br>
<br>
To-Dos Beat:<br>
- https://sourceforge.net/projects/hostsmanager/<br>
- https://scottlerch.github.io/HostsFileEditor/<br>
- http://hostsman2.it-mate.co.uk/<br>
<br>
<br>
To-Dos Data Storage:<br>
- Hosts-Files-List (multiple sources)<br>
- Load local files in Hosts-Files-List, too<br>
- Use INI, CFG, SQLite<br>
- Tags and descriptions to sources<br>
- Redirects per list<br>
- Redirected to any IP/Path (localhost, too)<br>
<br>
<br>
More To-Dos Hosts-Files-List:<br>
- Add Line-Breaks in hosts-files (CR+LF)<br>
<br>
<br>
To-Dos more Windows Integration:<br>
- Place data in %PROGRAMDATA%\LV-Crew\HostsManager\<br>
<br>
<br>
To-Dos better Windows Integration:<br>
- Systray-Icon<br>
- Schedult System Task<br>
- Start at boot<br>
<br>
<br>
To-Dos Multi-Plattform/Multi-Language:<br>
- 64-bit-Version<br>
- NullSoft-Installer<br>
- Linux-/macOS-/BSD-/Android-Port<br>
- Multi-Language support<br>
<br>
<br>
To-Dos Error-Server:<br>
- Counter for Error-Page<br>
- Maybe Avira & Google-Ads<br>
- Error-Pages per lists<br>
<br>
<br>
<h3><b>Ideas:</b></h3>
Some ideas:<br>
- White-/Gray-/Blacklister read data e.g. from Cookies of Browsers<br>
<br>
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
- Bad-URLs-List from Avira<br>
<br>
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
