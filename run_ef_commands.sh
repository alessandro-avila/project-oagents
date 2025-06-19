#!/bin/bash
 cd $(dirname $0)

# Exit immediately if a command exits with a non-zero status
set -e

# Define the DbContext name
DB_CONTEXT="TodoAppDbContext"

# Define the migration name
MIGRATION_NAME="InitialCreate"

# Add the initial migration using the specified DbContext
dotnet ef migrations add $MIGRATION_NAME --context $DB_CONTEXT

# Update the database applying the migration using the specified DbContext
dotnet ef database update --context $DB_CONTEXT
