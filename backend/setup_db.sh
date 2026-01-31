#!/bin/bash
set -e

echo "Restoring packages..."
dotnet restore

echo "Updating database..."
dotnet ef database update \
  --project EduVN.Infrastructure \
  --startup-project EduVN.API

echo "✅ Database updated successfully."
