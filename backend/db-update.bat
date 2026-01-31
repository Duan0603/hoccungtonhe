@echo off
dotnet restore
dotnet ef database update --project EduVN.Infrastructure --startup-project EduVN.API
echo Database updated successfully.
pause
