# ZP Workspace 

一个完整的ZP源码工程

## 目录结构

- ZP.Lib [ZP.Lib](./ZP.Lib/ReadMe.md)
- ZP.Matrix [ZP.Matrix](./ZP.Matrix/Readme.md)
- ZProRx.Test.Unity ZP基本应用的Demo
- Dudu.Unity Dudu案例Unity Client端工程，ZP.Lib 以源码的形式Plugin。
- ZP.Server.Demo 为Dudu案例 Soc Server端工程及源码。
- ZP.WebServer.Demo 为Dudu案例 Web Server端工程及源码。
- .vscode VS工程的设置文件，如下包括了一些开发中使用的Task，如Unity.ZP.Lib.Plugin.Publish用于发布源码到指定的Unity应用中。主要用于基于GitHub源码进行开发。

由于开源进度的原因，以上目录可能包含不全。

Unity工程目前要求是要与ZP.Lib在同一目录下，
通过Task 实现把Plugin 注入到Unity工程中。

![](./Docs/img/Readme_2019-10-29-15-27-05.png)



## Release Note

### [v1.0.3]

[2020-01-25 11:13:39]
发布ZProRx.Lib 1.0.3 版本

主要包括：
1. ZP.Lib.Main 1.0.3 (ZP.Lib.Server)
   ZProRx框架核心包发布，包括：ZP的基础设施（构建、访问、池化、持久化、关系）基于ZP的反映式框架、网络框架、后端框架、Unity可视化分离架构等。
   详细说明请参数：
   https://github.com/bennychao/ZProRx.Lib/blob/master/ZP.Lib/ZP.Lib.Main/Readme.md

   下载安装：
  - .Net CLI
  `>dotnet add package ZP.Lib.Server --version 1.0.3`
  - 或 VS 2019 Nuget 包管理中搜索"ZP.Lib.Server"并安装（推荐）
  
2. ZProRx.Lib Plugin 1.0.3 
   下载地址：
   https://github.com/bennychao/ZProRx.Lib/releases/
   https://github.com/bennychao/ZProRx.Lib/Publish/ZProRx.Lib.package

3. ZProRx.Test.Unity 案例
    源码地址：https://github.com/bennychao/ZProRx.Lib/ZProRx.Test.Unity

