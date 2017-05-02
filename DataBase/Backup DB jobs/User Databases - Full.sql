USE [msdb]
GO

/****** Object:  Job [HPDBA_DatabaseBackup - User Databases - Full]    Script Date: 10/13/2014 3:05:53 PM ******/
BEGIN TRANSACTION
DECLARE @ReturnCode INT
SELECT @ReturnCode = 0
/****** Object:  JobCategory [Database Maintenance]    Script Date: 10/13/2014 3:05:54 PM ******/
IF NOT EXISTS (SELECT name FROM msdb.dbo.syscategories WHERE name=N'Database Maintenance' AND category_class=1)
BEGIN
EXEC @ReturnCode = msdb.dbo.sp_add_category @class=N'JOB', @type=N'LOCAL', @name=N'Database Maintenance'
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback

END

DECLARE @jobId BINARY(16)
EXEC @ReturnCode =  msdb.dbo.sp_add_job @job_name=N'HPDBA_DatabaseBackup - User Databases - Full', 
		@enabled=1, 
		@notify_level_eventlog=2, 
		@notify_level_email=2, 
		@notify_level_netsend=0, 
		@notify_level_page=0, 
		@delete_level=0, 
		@description=N'Backup all databases with FULL recovery Model which are ONLINE', 
		@category_name=N'Database Maintenance', 
		@owner_login_name=N'sa', 
		@notify_email_operator_name=N'SOESQL', @job_id = @jobId OUTPUT
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback
/****** Object:  Step [Check for backup directories]    Script Date: 10/13/2014 3:05:56 PM ******/
EXEC @ReturnCode = msdb.dbo.sp_add_jobstep @job_id=@jobId, @step_name=N'Check for backup directories', 
		@step_id=1, 
		@cmdexec_success_code=0, 
		@on_success_action=3, 
		@on_success_step_id=0, 
		@on_fail_action=2, 
		@on_fail_step_id=0, 
		@retry_attempts=0, 
		@retry_interval=1, 
		@os_run_priority=0, @subsystem=N'TSQL', 
		@command=N'
SET NOCOUNT ON
CREATE TABLE #SubDirs (
	Directory varchar(100)
	)
DECLARE  @BackupDirectory nvarchar(260) 
	,@GetSubDirectories varchar(255) 
	,@DatabaseName varchar(255)
	,@CreateSubDirsCmd VARCHAR(255)
	,@Database varchar(250)
--*** Set Backup folder path ***
SET @BackupDirectory = ''I:\SOEBackups''
SELECT @GetSubDirectories = ''xp_cmdshell ''''dir "'' + @BackupDirectory + ''" /AD /B''''''
INSERT #SubDirs Execute (@GetSubDirectories)
IF EXISTS (SELECT name from master.dbo.sysdatabases WHERE name NOT IN (SELECT Directory FROM #SubDirs WHERE Directory IS NOT NULL) AND name NOT in (''Northwind'', ''Pubs'',''SYSAdmin''))
	BEGIN
	DECLARE CreateSubDirs CURSOR FOR SELECT name FROM master.dbo.sysdatabases WHERE dbid > 4 AND name NOT IN (SELECT Directory FROM #SubDirs WHERE Directory IS NOT NULL) AND name NOT in (''Northwind'', ''pubs'',''SYSAdmin'')
	OPEN  CreateSubDirs
	FETCH NEXT FROM CreateSubDirs INTO  @Database
	WHILE @@FETCH_STATUS = 0
		BEGIN
		SELECT @CreateSubDirsCmd = ''EXECUTE xp_cmdshell ''''md "'' + @BackupDirectory + ''\'' + @Database + ''"'''', no_output''
		PRINT @CreateSubDirsCmd
		EXECUTE (@CreateSubDirsCmd)
		PRINT ''FULL database backup job created directory '' + @BackupDirectory + @Database
		FETCH NEXT FROM CreateSubDirs INTO  @Database
		END
	CLOSE CreateSubDirs
	DEALLOCATE CreateSubDirs
	END
DROP TABLE #SubDirs
', 
		@database_name=N'master', 
		@output_file_name=N'I:\SOEBackups\FULL_User_Backup_$(ESCAPE_SQUOTE(STRTDT))_$(ESCAPE_SQUOTE(STRTTM)).txt', 
		@flags=4
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback
/****** Object:  Step [Backup database]    Script Date: 10/13/2014 3:05:56 PM ******/
EXEC @ReturnCode = msdb.dbo.sp_add_jobstep @job_id=@jobId, @step_name=N'Backup database', 
		@step_id=2, 
		@cmdexec_success_code=0, 
		@on_success_action=1, 
		@on_success_step_id=0, 
		@on_fail_action=2, 
		@on_fail_step_id=0, 
		@retry_attempts=0, 
		@retry_interval=1, 
		@os_run_priority=0, @subsystem=N'TSQL', 
		@command=N'
SET NOCOUNT ON
--********************************************************************************************
-- FOR RESTART SET THE @RESTART to the database name that had the backup succesfully complete 
--********************************************************************************************
CREATE TABLE #ExcludeDBs (
	dbname sysname
	)
INSERT #ExcludeDBs SELECT ''Northwind, Pubs,sql_dba''
DECLARE  @BackupCommand VARCHAR (1000)
	,@TimeStamp VARCHAR(20)
	,@DBName VARCHAR (128)
        ,@status BIT
	,@BackupDirectory VARCHAR(500)
	,@dbid int
	,@Restart VARCHAR(50)
--********************************************************************************************
-- FOR RESTART SET THE @RESTART to the database name that had the backup succesfully complete 
-- 
-- OTHERWISE @RESTART SHOULD BE = '' '' to a blank
--********************************************************************************************
SET @Restart = '' '' 
IF @Restart <> '' '' 
   BEGIN
	SELECT @dbid = dbid FROM master..sysdatabases where name = @Restart
   END
ELSE
   BEGIN
	SET @dbid = 4	
   END
--PRINT @dbid
--*** Set path for Backups ***
SET @BackupDirectory = ''I:\SOEBackups''
DECLARE DB_name_cursor CURSOR 
FOR SELECT name FROM master..sysdatabases 
WHERE dbid > @dbid and name not in (''Northwind'', ''pubs'',''SYSAdmin'') and DATABASEPROPERTYEX(name, ''STATUS'') = ''ONLINE'' 
order by name
OPEN DB_name_cursor
FETCH NEXT FROM DB_name_cursor INTO @DBName
WHILE (@@Fetch_Status = 0)
BEGIN
If @DBName NOT IN (SELECT dbname FROM #ExcludeDBs) 
	BEGIN


		SET @TimeStamp =  convert( char (8), getdate(),112) + substring( convert( char (15), getdate(),113), 13, 2) + substring( convert( char (18), getdate(),113), 16, 2)
 		SET @BackupCommand = ''backup database '' + ''['' +  @DBName + '']''+ '' to disk = '''''' + @BackupDirectory + ''\'' + @DBName + ''\'' + @DBName + ''_FULL_'' + @TimeStamp + ''.bak''''''
		PRINT @BackupCommand
  		EXEC (@BackupCommand)
		PRINT '' -------------------------------------*** -------------------------------------''

	END
FETCH NEXT FROM DB_name_cursor INTO @DBName	                 
END
CLOSE DB_name_cursor
DEALLOCATE DB_name_cursor
DROP TABLE #ExcludeDBs', 
		@database_name=N'master', 
		@output_file_name=N'I:\SOEBackups\FULL_User_Backup_$(ESCAPE_SQUOTE(STRTDT))_$(ESCAPE_SQUOTE(STRTTM)).txt', 
		@flags=6
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback
EXEC @ReturnCode = msdb.dbo.sp_update_job @job_id = @jobId, @start_step_id = 1
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback
EXEC @ReturnCode = msdb.dbo.sp_add_jobschedule @job_id=@jobId, @name=N'fullbackup', 
		@enabled=1, 
		@freq_type=8, 
		@freq_interval=1, 
		@freq_subday_type=1, 
		@freq_subday_interval=3, 
		@freq_relative_interval=0, 
		@freq_recurrence_factor=1, 
		@active_start_date=20020314, 
		@active_end_date=99991231, 
		@active_start_time=20000, 
		@active_end_time=235959, 
		@schedule_uid=N'6d2fce3c-8982-46f4-bc82-0193085e62aa'
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback
EXEC @ReturnCode = msdb.dbo.sp_add_jobserver @job_id = @jobId, @server_name = N'(local)'
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback
COMMIT TRANSACTION
GOTO EndSave
QuitWithRollback:
    IF (@@TRANCOUNT > 0) ROLLBACK TRANSACTION
EndSave:

GO

