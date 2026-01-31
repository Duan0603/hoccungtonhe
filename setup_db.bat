@echo off
echo ===================================================
echo   hoccungtonhe - Database Setup script
echo ===================================================

cd backend/EduVN.API

echo [1/2] Installing dotnet-ef tool (if missing)...
dotnet tool install --global dotnet-ef >nul 2>&1

echo [2/2] Updating Database (Migrations + Seeding)...
dotnet ef database update --project ..\EduVN.Infrastructure --startup-project .

if %errorlevel% neq 0 (
    echo.
    echo [ERROR] Database update failed!
    echo Please check if PostgreSQL is running.
    echo.
    exit /b %errorlevel%
)

echo.
echo [SUCCESS] Database is ready!
echo Test Accounts:
echo - Admin: admin@hoccungtonhe.com | Admin@123
echo - Teacher: teacher@hoccungtonhe.com | Teacher@123
echo - Student: student@hoccungtonhe.com | Student@123
echo.
pause
