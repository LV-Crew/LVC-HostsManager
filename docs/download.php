<?php
/* Datenbankserver - In der Regel die IP */
$db_server = 'localhost';
/* Datenbankname */
$db_name = 'counter';
/* Datenbankuser */
$db_user = 'root';
/* Datenbankpasswort */
$db_passwort = '';
         
/* Erstellt Connect zu Datenbank her */
$db = @ mysql_connect ( $db_server, $db_user, $db_passwort );

$db_select = @ mysql_select_db( $db_name );

mysql_query("UPDATE counter SET count=count+1");
header('Location: https://github.com/LV-Crew/HostsManager/releases/latest'); // redirect to the real file to be downloaded
