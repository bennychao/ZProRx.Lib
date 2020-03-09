# ZProRx.Test.Web

## 一个基本的基于ZProRx框架的Web 应用的例子

## 工程配置

1. 新建立NetCore Web API （3.1 以上版本）应用
2. 生成XML用于Swigger，参考如下工程配置
```xml
  <PropertyGroup Condition=" '$(Configuration)|$(Platform)' == 'Debug|AnyCPU' ">
    <DocumentationFile>bin\Debug\netcoreapp3.1\ZProRx.Test.Web.xml</DocumentationFile>
    <NoWarn>1701;1702;1591</NoWarn>
  </PropertyGroup>

  <PropertyGroup Condition=" '$(Configuration)|$(Platform)' == 'Release|AnyCPU' ">
    <DocumentationFile>bin\Debug\netcoreapp3.1\ZProRx.Test.Web.xml</DocumentationFile>
    <NoWarn>1701;1702;1591</NoWarn>
  </PropertyGroup>
```

2. 引入UniRx以及UnityCore库
```xml
  <ItemGroup>
    <Reference Include="UniRx">
      <HintPath>..\ZP.Lib\ServerLibrary\UniRx.dll</HintPath>
    </Reference>
    <Reference Include="UnityEngine.CoreModule">
      <HintPath>..\ZP.Lib\ServerLibrary\UnityEngine.CoreModule.dll</HintPath>
    </Reference>
  </ItemGroup>
 ``` 

3. 引入 ZP.Lib.NetCore 和 ZP.Lib.Web 库，其中包括了ZP.Lib.Server 等核心 ZProRx库

```xml
  <ItemGroup>
    <ProjectReference Include="..\ZP.Lib\ZP.Lib.NetCore\ZP.Lib.NetCore.csproj" />
    <ProjectReference Include="..\ZP.Lib\ZP.Lib.Web\ZP.Lib.Web.csproj" />
  </ItemGroup>
```


## 注意问题：
1. 返回值的问题，确保不要返回Viod类型，即调用端会接收“”空返回。