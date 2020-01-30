
  @echo off

echo "Source Plugin To Unity Project " %1

echo.

echo "clear current plugins"
del /q /s ..\%1\Assets\Plugins\ZP.Lib\
rd  /s /q ..\%1\Assets\Plugins\ZP.Lib\

echo "Copy LitJson & M2Mqtt"
xcopy .\ServerLibrary\LitJson.dll ..\%1\Assets\Plugins\ /s /e /c /y /h /r
xcopy .\ClientLibrary\M2Mqtt.dll ..\%1\Assets\Plugins\ /s /e /c /y /h /r



echo "Copy ZP.Lib.Main"
xcopy .\ZP.Lib.Main\*.cs ..\%1\Assets\Plugins\ZP.Lib\ZP.Lib.Main\ /s /e /c /y /h /r
xcopy .\ZP.Lib.Main\ZP.Lib.Main.asmdef ..\%1\Assets\Plugins\ZP.Lib\ZP.Lib.Main\ /s /e /c /y /h /r

del /q /s ..\%1\Assets\Plugins\ZP.Lib\ZP.Lib.Main\bin\
del /q /s ..\%1\Assets\Plugins\ZP.Lib\ZP.Lib.Main\obj\
rd  /s /q ..\%1\Assets\Plugins\ZP.Lib\ZP.Lib.Main\bin\
rd  /s /q ..\%1\Assets\Plugins\ZP.Lib\ZP.Lib.Main\obj\

echo "Copy ZP.Lib.Card"
xcopy .\ZP.Lib.Card\*.cs ..\%1\Assets\Plugins\ZP.Lib\ZP.Lib.Card\ /s /e /c /y /h /r
xcopy .\ZP.Lib.Card\ZP.Lib.Card.asmdef ..\%1\Assets\Plugins\ZP.Lib\ZP.Lib.Card\ /s /e /c /y /h /r

del /q /s ..\%1\Assets\Plugins\ZP.Lib\ZP.Lib.Card\bin\
del /q /s ..\%1\Assets\Plugins\ZP.Lib\ZP.Lib.Card\obj\
rd  /s /q ..\%1\Assets\Plugins\ZP.Lib\ZP.Lib.Card\bin\
rd  /s /q ..\%1\Assets\Plugins\ZP.Lib\ZP.Lib.Card\obj\



pause