@echo off
echo Stopping SQL Server services...

NET SESSION >nul 2>&1
if %errorLevel% == 0 (
    echo Administrative permissions confirmed.
) else (
    echo Please run this script as Administrator.
    pause
    exit /b 1
)

echo Stopping SQL Server Browser (if running)...
echo Note: This is an optional service, so it's OK if it's not running.
net stop SQLBrowser 2>nul
if %errorLevel% == 0 (
    echo SQL Server Browser stopped successfully.
) else (
    echo SQL Server Browser was not running - this is normal.
)

echo.
echo Stopping SQL Server (main database engine)...
net stop MSSQLSERVER
if %errorLevel% == 0 (
    echo SQL Server stopped successfully.
) else (
    echo Failed to stop SQL Server - it might already be stopped.
)

echo.
echo Main SQL Server has been stopped.
echo SQL Server resources are now freed up!
pause 