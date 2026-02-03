#!/bin/bash

echo "backup app ..."

TEMPDIR="$(mktemp -d)"
trap 'rm -rf -- "$TEMPDIR"' EXIT
BACKUPFILE="gtkanu.tar.xz"
cd $TEMPDIR

XZ_OPT=-9 tar -Jchf $BACKUPFILE -C /opt/gtkanu .

echo "push to nextcloud ..."

SHAREID="***"
PASS="***"
DAY="$(date +'%d')"
BASEADDR="https://nextcloud"
curl -u "$SHAREID:$PASS" "$BASEADDR/public.php/webdav/$DAY" -H 'X-Requested-With: XMLHttpRequest' -X DELETE
curl -u "$SHAREID:$PASS" "$BASEADDR/public.php/webdav/$DAY" -H 'X-Requested-With: XMLHttpRequest' -X MKCOL
curl -T $BACKUPFILE -u "$SHAREID:$PASS" "$BASEADDR/public.php/webdav/$DAY/$BACKUPFILE" -H 'X-Requested-With: XMLHttpRequest'