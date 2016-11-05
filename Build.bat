@echo off
:: current date
set "curDate=%date:~0,4%%date:~5,2%%date:~8,2%"
echo %curDate%

cd %~dp0
%windir%\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe ".\HKSJ.WBVV.sln" /m /t:Clean;Build /p:Configuration=Release /distributedFileLogger
if %errorlevel% NEQ 0 echo solution compile failed
if %errorlevel% NEQ 0 goto END

echo compile is successful, starting deploy

echo 1.deleting PrecompiledWeb directory
rd /s/q PrecompiledWeb

echo 2.deploying file to PrecompiledWeb directory
%windir%\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe ".\HKSJ.WBVV.Api.Host\HKSJ.WBVV.Api.Host.csproj" /t:PublishToFileSystem /p:Configuration=Release;PublishDir="..\PrecompiledWeb\Host" /distributedFileLogger
if %errorlevel% NEQ 0 echo Api Host deploy failed
if %errorlevel% NEQ 0 goto END

%windir%\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe ".\HKSJ.WBVV.MVC.Client\HKSJ.WBVV.MVC.Client.csproj" /t:PublishToFileSystem /p:Configuration=Release;PublishDir="..\PrecompiledWeb\Web" /distributedFileLogger
if %errorlevel% NEQ 0 echo Client deploy failed
if %errorlevel% NEQ 0 goto END

%windir%\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe ".\HKSJ.WBVV.MVC.Manage\HKSJ.WBVV.MVC.Manage.csproj" /t:PublishToFileSystem /p:Configuration=Release;PublishDir="..\PrecompiledWeb\Admin" /distributedFileLogger
if %errorlevel% NEQ 0 echo Manage deploy failed
if %errorlevel% NEQ 0 goto END

echo 3.creating Index server related directories if necessary
IF NOT EXIST .\PrecompiledWeb\Host\IndexData md .\PrecompiledWeb\Host\IndexData

echo 4. copy memcached.exe to host folder
copy .\memcached.exe .\PrecompiledWeb\Host\

:END
if %errorlevel% NEQ 0 (
    if "%autoBuild%" EQU "1" (exit /b %errorlevel%)
    pause
)