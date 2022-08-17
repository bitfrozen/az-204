@echo off
powershell.exe -ExecutionPolicy bypass -Command "& '%~dp0build.ps1'" -Publish %*