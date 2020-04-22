  @echo off

echo "Clone To ZProRx.Lib"

del /q /s ..\..\ZProRx.Lib\ZP.Lib\*
rd  /s /q ..\..\ZProRx.Lib\ZP.Lib\

xcopy ..\ZP.Lib\* ..\..\ZProRx.Lib\ZP.Lib\ /s /e /c /y /h /r 

del /q /s ..\..\ZProRx.Lib\ZP.Lib\ZP.Lib.Battle\*
rd  /s /q ..\..\ZProRx.Lib\ZP.Lib\ZP.Lib.Battle\

del /q /s ..\..\ZProRx.Lib\ZP.Lib\ZP.Lib.Battle.NetCore\*
rd  /s /q ..\..\ZProRx.Lib\ZP.Lib\ZP.Lib.Battle.NetCore\

del /q /s ..\..\ZProRx.Lib\ZProRx.Test.Unity\*
rd  /s /q ..\..\ZProRx.Lib\ZProRx.Test.Unity\

del /q /s ..\..\ZProRx.Lib\ZProRx.Test.Server\*
rd  /s /q ..\..\ZProRx.Lib\ZProRx.Test.Server\

del /q /s ..\..\ZProRx.Lib\ZProRx.Test.Web\*
rd  /s /q ..\..\ZProRx.Lib\ZProRx.Test.Web\

xcopy ..\ZProRx.Test.Unity\* ..\..\ZProRx.Lib\ZProRx.Test.Unity\ /s /e /c /y /h /r
xcopy ..\ZProRx.Test.Server\* ..\..\ZProRx.Lib\ZProRx.Test.Server\ /s /e /c /y /h /r
xcopy ..\ZProRx.Test.Web\* ..\..\ZProRx.Lib\ZProRx.Test.Web\ /s /e /c /y /h /r

xcopy ..\.vscode\* ..\..\ZProRx.Lib\.vscode\ /s /e /c /y /h /r
xcopy ..\Docs\* ..\..\ZProRx.Lib\Docs\ /s /e /c /y /h /r
xcopy ..\.favorites.json ..\..\ZProRx.Lib\.favorites.json /s /e /c /y /h /r
xcopy ..\.gitattributes ..\..\ZProRx.Lib\.gitattributes /s /e /c /y /h /r
xcopy ..\.gitignore ..\..\ZProRx.Lib\.gitignore /s /e /c /y /h /r
xcopy ..\ZProRx.Lib.Open.sln ..\..\ZProRx.Lib\ZProRx.Lib.Open.sln /s /e /c /y /h /r
xcopy ..\Readme.md ..\..\ZProRx.Lib\Readme.md /s /e /c /y /h /r

echo "Unit Test ZProRx.Lib"

cd /d ..\..\ZProRx.Lib\ZP.Lib\ZP.Lib.Server.Test\
dotnet test ZP.Lib.Server.Test.csproj