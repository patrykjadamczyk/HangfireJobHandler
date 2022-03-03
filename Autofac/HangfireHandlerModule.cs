﻿using Autofac;
using Dapper;
using System;
using System.Data.SqlClient;
using System.Threading;

namespace HangfireJobHandler.Autofac
{
    public class HangfireHandlerModule : Module
    {
        readonly string CheckDbSql = @$"SELECT * FROM sys.databases WHERE name = '{Environment.GetEnvironmentVariable("HANGFIRE_DATABASE")}'";

        readonly string ValidateTableSql = $@"IF NOT EXISTS (SELECT * 
                    FROM INFORMATION_SCHEMA.TABLES 
                    WHERE TABLE_SCHEMA = '{Environment.GetEnvironmentVariable("HANGFIRE_SCHEMA")}' 
                    AND TABLE_NAME = '{Environment.GetEnvironmentVariable("HANGFIRE_JOB_TABLE")}')
                    BEGIN
                        CREATE TABLE [{Environment.GetEnvironmentVariable("HANGFIRE_SCHEMA")}].[{Environment.GetEnvironmentVariable("HANGFIRE_JOB_TABLE")}] (
                            [Id] [bigint] IDENTITY(1,1) NOT NULL,
                            [CreatedAt] [datetime] NOT NULL,
                            [JobId] [varchar](40) NOT NULL,
                            CONSTRAINT [PK_{Environment.GetEnvironmentVariable("HANGFIRE_JOB_TABLE")}_Job] PRIMARY KEY CLUSTERED 
                            (
	                            [Id] ASC
                            )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
                            ) ON [PRIMARY];
                            ALTER TABLE [{Environment.GetEnvironmentVariable("HANGFIRE_SCHEMA")}].[{Environment.GetEnvironmentVariable("HANGFIRE_JOB_TABLE")}]
                            ADD CONSTRAINT UC_{Environment.GetEnvironmentVariable("HANGFIRE_JOB_TABLE")}_JobId UNIQUE (JobId),
						    CONSTRAINT [DF_{Environment.GetEnvironmentVariable("HANGFIRE_JOB_TABLE")}_created_at]  DEFAULT (getdate()) FOR [CreatedAt]
                    END";
        protected override void Load(ContainerBuilder builder)
        {
            using (var connection = new SqlConnection($"{Environment.GetEnvironmentVariable("SQL_CONNECTIONSTRING")};database=master;"))
            {
                while (connection.QuerySingle(CheckDbSql) < 1)
                {
                    Console.WriteLine($"Database {Environment.GetEnvironmentVariable("HANGFIRE_DATABASE")} not found, waiting 10 seconds to retry...");
                    Thread.Sleep(10000);
                }
            }
            using (var connection = new SqlConnection($"{Environment.GetEnvironmentVariable("SQL_CONNECTIONSTRING")};database={Environment.GetEnvironmentVariable("HANGFIRE_DATABASE")};"))
            {
                connection.Execute(ValidateTableSql);
            }

            builder.RegisterType<JobHandler>().As<IJobHandler>().SingleInstance();
        }
    }
}
