@echo off
REM Одна команда для Windows: powershell -ExecutionPolicy Bypass -File start.ps1
cd /d "%~dp0"
powershell -NoProfile -ExecutionPolicy Bypass -File "%~dp0start.ps1"
pause
