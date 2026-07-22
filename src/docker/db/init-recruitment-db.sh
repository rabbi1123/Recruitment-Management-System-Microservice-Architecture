#!/bin/bash
set -e

DB_HOST="${DB_HOST:-recruitmentdb}"
SA_PASSWORD="${Recruitment_DB_PASSWORD}"
SQL_FILE="/scripts/RecruitmentServiceDB.sql"

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

echo "Ensuring RecruitmentDB exists..."
/opt/mssql-tools18/bin/sqlcmd -S "$DB_HOST" -U sa -P "$SA_PASSWORD" -C -Q \
  "IF DB_ID('RecruitmentDB') IS NULL CREATE DATABASE RecruitmentDB;"

echo "Waiting for RecruitmentDB to be online..."
until /opt/mssql-tools18/bin/sqlcmd -S "$DB_HOST" -U sa -P "$SA_PASSWORD" -C -d RecruitmentDB -Q "SELECT 1" > /dev/null 2>&1; do
  sleep 2
done

echo "Checking if schema already exists..."
EXISTS=$(/opt/mssql-tools18/bin/sqlcmd -S "$DB_HOST" -U sa -P "$SA_PASSWORD" -C -d RecruitmentDB -h -1 -W -Q \
  "SET NOCOUNT ON; SELECT COUNT(*) FROM sys.tables WHERE name = 'Applications'" 2>/dev/null | tr -d '[:space:]' || echo "0")

if [ "$EXISTS" = "1" ]; then
  echo "Schema already initialized. Skipping SQL script."
else
  echo "Running $SQL_FILE..."
  /opt/mssql-tools18/bin/sqlcmd -S "$DB_HOST" -U sa -P "$SA_PASSWORD" -C -b -i "$SQL_FILE"
fi

echo "Database initialization complete."
