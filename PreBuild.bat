taskkill /F /IM xdesproc.exe 2>&1 | exit /B 0
net stop "MPDisplayServer"
exit /b 0

