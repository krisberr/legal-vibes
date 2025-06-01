# SQL Server Management Scripts

This directory contains scripts to help manage SQL Server services for development.

## Available Scripts

### `start-sql.bat`
Starts the SQL Server services required for development:
- SQL Server (MSSQLSERVER)
- SQL Server Browser

### `stop-sql.bat`
Stops the SQL Server services to free up system resources:
- SQL Server (MSSQLSERVER)
- SQL Server Browser

## Usage

1. Right-click on either script
2. Select "Run as administrator"
3. Follow the prompts in the console window

## Important Notes

- These scripts must be run as Administrator
- The scripts will check for administrative privileges
- If services fail to start/stop, check Windows Services (services.msc) for more details
- Your database and all data will remain intact when stopping services
- The application's connection strings don't need to be changed when restarting services 