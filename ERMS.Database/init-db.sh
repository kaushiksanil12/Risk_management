#!/bin/bash
set -e

# This script is used as the MariaDB Docker entrypoint init script.
# It runs all SQL files in order to create the database schema, 
# stored procedures, triggers, and seed data.

echo "=== ERMS Database Initialization ==="

# 01 — Skip CreateDatabase.sql as Docker already creates the DB via MARIADB_DATABASE
# The USE statement is not needed since we connect directly to the database.

echo "[1/6] Creating tables..."
mysql -u root -p"${MARIADB_ROOT_PASSWORD}" "${MARIADB_DATABASE}" < /docker-entrypoint-initdb.d/sql/02_CreateTables.sql

echo "[2/6] Creating stored procedures..."
mysql -u root -p"${MARIADB_ROOT_PASSWORD}" "${MARIADB_DATABASE}" < /docker-entrypoint-initdb.d/sql/03_CreateProcedures.sql

echo "[3/6] Creating triggers..."
mysql -u root -p"${MARIADB_ROOT_PASSWORD}" "${MARIADB_DATABASE}" < /docker-entrypoint-initdb.d/sql/04_CreateTriggers.sql

echo "[4/6] Inserting seed data..."
mysql -u root -p"${MARIADB_ROOT_PASSWORD}" "${MARIADB_DATABASE}" < /docker-entrypoint-initdb.d/sql/05_SeedData.sql

echo "[5/6] Creating email procedure..."
mysql -u root -p"${MARIADB_ROOT_PASSWORD}" "${MARIADB_DATABASE}" < /docker-entrypoint-initdb.d/sql/06_EmailProcedure.sql

echo "[6/6] Creating Risk Wizard tables & procedures..."
mysql -u root -p"${MARIADB_ROOT_PASSWORD}" "${MARIADB_DATABASE}" < /docker-entrypoint-initdb.d/sql/07_RiskWizard.sql

echo "=== ERMS Database Initialization Complete ==="
