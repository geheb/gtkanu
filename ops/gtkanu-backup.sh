#!/bin/sh

TEMPDIR="$(mktemp -d)"
trap 'rm -rf -- "$TEMPDIR"' EXIT
BACKUPFILE="gtkanu.7z"

echo "create backup"
7z a -mhe=on "$TEMPDIR/$BACKUPFILE" /opt/gtkanu/ -p"***"

SHAREID="***"
PASS="***"
DAY="$(date +'%d')"
BASEADDR="https://nextcloud"
echo "delete old folder"
curl -u "$SHAREID:$PASS" "$BASEADDR/public.php/webdav/$DAY" -H 'X-Requested-With: XMLHttpRequest' -X DELETE
echo "create folder"
curl -u "$SHAREID:$PASS" "$BASEADDR/public.php/webdav/$DAY" -H 'X-Requested-With: XMLHttpRequest' -X MKCOL
echo "upload file"
curl -T "$TEMPDIR/$BACKUPFILE" -u "$SHAREID:$PASS" "$BASEADDR/public.php/webdav/$DAY/$BACKUPFILE" -H 'X-Requested-With: XMLHttpRequest'
