start Server\bin\Debug\Server.exe

set loopcount=2
:loop
start Client\Builds\Client.exe
set /a loopcount=loopcount-1
if %loopcount%==0 goto exitloop
goto loop
:exitloop