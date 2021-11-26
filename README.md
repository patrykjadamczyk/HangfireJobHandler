# HangfireJobHandler
Extension library to add custom queue logic

# ATTENTION!
This library will create and use the destination database on first usage. Only MSSQL database supported.</br>
Be sure to provide correct connectionstring with all required user rights to maintain the database (create, read, upsert, delete).

# CONFIGURATION:
## 1. Environment variables:
`SQL_CONNECTIONSTRING` - connectionstring to the database WITHOUT database name parameter</br>
`HANGFIRE_SCHEMA` - hangfire schema name</br>
`HANGFIRE_DATABASE` - hangfire database name</br>
`HANGFIRE_JOB_TABLE` - Hangfire job table where the job identifiers will be stored.</br>
## 2. Queue
This library is using the queue named `handler` - it needs to be inserted into the Hangfire queue settings.
## 3. ContinuationsSupportIncludingFailedStateAttribute
Out of the box, Hangfire method `ContinueJobWith()` doesn't apply to `Failed` jobs 
- decorating the background job method with this attribute allows to extend the `ContinueJobWith` execution also to `Failed` jobs
so they will be also removed from the handler job table and not blocking incoming jobs with same identifier.

