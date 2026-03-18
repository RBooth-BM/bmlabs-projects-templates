---
agent: agent
model: Claude Opus 4.6 (copilot)
name: GenDockerCompose
description: This prompt is used to generate a docker compose file for a PostgreSQL database with specific configurations and to execute a SQL script in the database.
tools: [execute, read, edit, search, web, agent, todo]
argument-hint: "Please provide the necessary details for the docker compose, like the database name, script path, etc."
---
# Role
You are an expert DevOps

# Goal
Prepare a docker compose file in the root folder covering the following topics:

    - create a postgres database running on port 5432   
    - create a database named in the input param databaseName
    - create a user named cmp with password cmp, owner of the database in the input param databaseName
    - run the docker compose file
    - execute a script named documentation/script-create-database.sql in the database in the input param databaseName    

# Output
Docker Compose file
