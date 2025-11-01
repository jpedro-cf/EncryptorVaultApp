#!/bin/sh
set -e

mkdir -p /app/secrets

echo "$PUBLIC_KEY" | base64 -d > /app/secrets/public.pem
echo "$PRIVATE_KEY" | base64 -d > /app/secrets/private.pem

chmod 600 /app/secrets/private.pem

exec dotnet EncryptionApp.dll