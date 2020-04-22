  @echo off

echo "Publish Package"


echo "pack ZP.Lib.NetCore"
cd /d ./ZP.Lib.NetCore/
dotnet pack .\ZP.Lib.NetCore.csproj -c Release -p:NuspecFile=.\ZP.Lib.NetCore.nuspec
xcopy .\bin\Release\*.nupkg ..\..\..\Publish\ /s /e /c /y /h /r
cd /d ../

echo "pack ZP.Lib.Main"
cd /d ./ZP.Lib.Main/
del /q /s .\bin\Release\*.nupkg
dotnet pack .\ZP.Lib.Server.csproj -c Release -p:NuspecFile=.\ZP.Lib.Server.nuspec
xcopy .\bin\Release\*.nupkg ..\..\..\Publish\ /s /e /c /y /h /r
cd /d ../

echo "pack ZP.Lib.Card"
cd /d ./ZP.Lib.Card/
dotnet pack .\ZP.Lib.Card.csproj -c Release -p:NuspecFile=.\ZP.Lib.Card.nuspec
xcopy .\bin\Release\*.nupkg ..\..\..\Publish\ /s /e /c /y /h /r
cd /d ../

echo "pack ZP.Lib.Matrix"
cd /d ./ZP.Lib.Matrix/
dotnet pack .\ZP.Lib.Matrix.csproj -c Release -p:NuspecFile=.\ZP.Lib.Matrix.nuspec
xcopy .\bin\Release\*.nupkg ..\..\..\Publish\ /s /e /c /y /h /r
cd /d ../

echo "pack ZP.Lib.Standard"
cd /d ./ZP.Lib.Standard/
dotnet pack .\ZP.Lib.Standard.csproj -c Release -p:NuspecFile=.\ZP.Lib.Standard.nuspec
xcopy .\bin\Release\*.nupkg ..\..\..\Publish\ /s /e /c /y /h /r
cd /d ../


echo "pack ZP.Lib.Web"
cd /d ./ZP.Lib.Web/
dotnet pack .\ZP.Lib.Web.csproj -c Release -p:NuspecFile=.\ZP.Lib.Web.nuspec
xcopy .\bin\Release\*.nupkg ..\..\..\Publish\ /s /e /c /y /h /r
cd /d ../


echo "Unit Test nuget pakcages"
cd /d ./ZP.Lib.Server.Test/
dotnet test ZP.Lib.Server.TestLocalNupack.csproj -c Release