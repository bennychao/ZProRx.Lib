Arch="ZP.Matrix.Architect"

#restore
dotnet restore ./ZP.Lib.Battle/ZP.Lib.Battle.Client.csproj
dotnet restore ./ZP.Lib.Matrix/ZP.Lib.Matrix.Client.csproj

dotnet publish ./ZP.Lib.Battle/ZP.Lib.Battle.Client.csproj -c Release -o ../../Publish/UnityPlugin
dotnet publish ./ZP.Lib.Matrix/ZP.Lib.Matrix.Client.csproj -c Debug -o ../../Publish/UnityPlugin

mkdir ../Assets/Plugins/ZPMatrix  #创建新的文件目录；

cp ../Publish/UnityPlugin/ZP.Lib.* ../Assets/Plugins/ZPMatrix/.


#LitJson.dll  M2Mqtt

cp ../Publish/UnityPlugin/LitJson.dll ../Assets/Plugins/ZPMatrix/.

cp ../Publish/UnityPlugin/M2Mqtt.dll ../Assets/Plugins/ZPMatrix/.