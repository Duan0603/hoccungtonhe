#!/bin/bash

echo "==================================================="
echo "  hoccungtonhe - Database Setup script"
echo "==================================================="

# Dừng script nếu có lỗi
set -e

# Di chuyển vào thư mục API
cd backend/EduVN.API || {
  echo "[ERROR] Cannot find backend/EduVN.API"
  exit 1
}

echo "[1/2] Installing dotnet-ef tool (if missing)..."
if ! command -v dotnet-ef &> /dev/null
then
  dotnet tool install --global dotnet-ef
else
  echo "dotnet-ef already installed"
fi

echo "[2/2] Updating Database (Migrations + Seeding)..."
dotnet ef database update \
  --project ../EduVN.Infrastructure \
  --startup-project .

echo
echo "[SUCCESS] Database is ready!"
echo "Test Accounts:"
echo "- Admin: admin@hoccungtonhe.com | Admin@123"
echo "- Teacher: teacher@hoccungtonhe.com | Teacher@123"
echo "- Student: student@hoccungtonhe.com | Student@123"
echo

read -p "Press Enter to continue..."
