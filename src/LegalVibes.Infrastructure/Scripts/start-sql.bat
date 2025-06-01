@echo off
echo Starting SQL Server services...

NET SESSION >nul 2>&1
if %errorLevel% == 0 (
    echo Administrative permissions confirmed.
) else (
    echo Please run this script as Administrator.
    pause
    exit /b 1
)

echo Starting SQL Server (main database engine)...
net start MSSQLSERVER
if %errorLevel% == 0 (
    echo SQL Server started successfully.
) else (
    echo Failed to start SQL Server.
    pause
    exit /b 1
)

echo.
echo Attempting to start SQL Server Browser (optional service)...
echo Note: This service is not required for local development.
net start SQLBrowser
if %errorLevel% == 0 (
    echo SQL Server Browser started successfully.
) else (
    echo SQL Server Browser is not available or disabled - this is OK for local development.
    echo This service is only needed for multiple SQL instances or network browsing.
)

echo.
echo Main SQL Server is running!
echo You can now start developing with Legal Vibes!
pause 