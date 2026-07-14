#!/bin/bash
set -e

DB_HOST="${DB_HOST:-jobdb}"
SA_PASSWORD="${Job_DB_PASSWORD}"
SQL_FILE="/scripts/JobServiceDB.sql"

echo "Waiting for SQL Server at $DB_HOST..."
until /opt/mssql-tools18/bin/sqlcmd -S "$DB_HOST" -U sa -P "$SA_PASSWORD" -C -Q "SELECT 1" > /dev/null 2>&1; do
  sleep 2
done

echo "Waiting for all databases to come online..."
until /opt/mssql-tools18/bin/sqlcmd -S "$DB_HOST" -U sa -P "$SA_PASSWORD" -C -h -1 -W -Q \
  "SET NOCOUNT ON; SELECT COUNT(*) FROM sys.databases WHERE database_id > 4 AND state_desc <> 'ONLINE'" \
  | tr -d '[:space:]' | grep -q '^0$'; do
  sleep 2
done

echo "Ensuring JobDB exists..."
/opt/mssql-tools18/bin/sqlcmd -S "$DB_HOST" -U sa -P "$SA_PASSWORD" -C -Q \
  "IF DB_ID('JobDB') IS NULL CREATE DATABASE JobDB;"

echo "Waiting for JobDB to be online..."
until /opt/mssql-tools18/bin/sqlcmd -S "$DB_HOST" -U sa -P "$SA_PASSWORD" -C -d JobDB -Q "SELECT 1" > /dev/null 2>&1; do
  sleep 2
done

echo "Checking if schema already exists..."
EXISTS=$(/opt/mssql-tools18/bin/sqlcmd -S "$DB_HOST" -U sa -P "$SA_PASSWORD" -C -d JobDB -h -1 -W -Q \
  "SET NOCOUNT ON; SELECT COUNT(*) FROM sys.tables WHERE name = 'Jobs'" 2>/dev/null | tr -d '[:space:]' || echo "0")

if [ "$EXISTS" = "1" ]; then
  echo "Schema already initialized. Skipping SQL script."
else
  echo "Running $SQL_FILE..."
  /opt/mssql-tools18/bin/sqlcmd -S "$DB_HOST" -U sa -P "$SA_PASSWORD" -C -b -i "$SQL_FILE"
fi

echo "Database initialization complete."
