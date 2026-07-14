#!/bin/bash
set -e

DB_HOST="${DB_HOST:-organizationdb}"
SA_PASSWORD="${Organization_DB_PASSWORD}"
SQL_FILE="/scripts/OrganizationServiceDB.sql"

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

echo "Ensuring OrganizationDB exists..."
/opt/mssql-tools18/bin/sqlcmd -S "$DB_HOST" -U sa -P "$SA_PASSWORD" -C -Q \
  "IF DB_ID('OrganizationDB') IS NULL CREATE DATABASE OrganizationDB;"

echo "Waiting for OrganizationDB to be online..."
until /opt/mssql-tools18/bin/sqlcmd -S "$DB_HOST" -U sa -P "$SA_PASSWORD" -C -d OrganizationDB -Q "SELECT 1" > /dev/null 2>&1; do
  sleep 2
done

echo "Checking if schema already exists..."
EXISTS=$(/opt/mssql-tools18/bin/sqlcmd -S "$DB_HOST" -U sa -P "$SA_PASSWORD" -C -d OrganizationDB -h -1 -W -Q \
  "SET NOCOUNT ON; SELECT COUNT(*) FROM sys.tables WHERE name = 'Organizations'" 2>/dev/null | tr -d '[:space:]' || echo "0")

if [ "$EXISTS" = "1" ]; then
  echo "Schema already initialized. Skipping SQL script."
else
  echo "Running $SQL_FILE..."
  /opt/mssql-tools18/bin/sqlcmd -S "$DB_HOST" -U sa -P "$SA_PASSWORD" -C -b -i "$SQL_FILE"
fi

echo "Database initialization complete."
