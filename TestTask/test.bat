echo off
cls
echo file1.txt:
TYPE bin\Debug\file1.txt
echo.
echo.
echo file2.txt:
TYPE bin\Debug\file2.txt
echo.
echo.
bin\Debug\TestTask.exe bin\Debug\file1.txt bin\Debug\file2.txt