# ZP Workspace 

一个完整的ZP源码工程

## 目录结构

- ZP.Lib [ZP.Lib](./ZP.Lib/ReadMe.md)
- ZP.Matrix [ZP.Matrix](./ZP.Matrix/Readme.md)
- ZP.Unity [ZProRx.Unity](./ZP.Unity/Readme.md) 一些常用插件对ZP的扩展支持
- ZProRx.Test.Unity ZP基本应用的Demo
- Dudu.Unity Dudu案例Unity Client端工程，ZP.Lib 以源码的形式Plugin。
- ZP.Server.Demo 为Dudu案例 Soc Server端工程及源码。
- ZP.WebServer.Demo 为Dudu案例 Web Server端工程及源码。
- ZProRx.Matrix.Server 用于测试ZProRx.Matrix 相关功能的Soc Server商。配合ZProRx.Test.Unity 使用。
- .vscode VS工程的设置文件，如下包括了一些开发中使用的Task，如Unity.ZP.Lib.Plugin.Publish用于发布源码到指定的Unity应用中。主要用于基于GitHub源码进行开发。

由于开源进度的原因，以上目录可能包含不全。

Unity工程目前要求是要与ZP.Lib在同一目录下，
通过Task 实现把Plugin 注入到Unity工程中。

![](./Docs/img/Readme_2019-10-29-15-27-05.png)



## Release Note

### [v1.0.6]
[2020-04-22] 发布ZProRx.Lib 1.0.6 版本

变更内容：
1. ZP.Lib.Main: 追加Tag[Attribute]、减少外部对于UniRx的依赖。
2. 完善FSM状态机、可以用于Battle等场景。
3. 数据库支持SQLite，并在ZP.Lib.Web提供对应的Model支持。
4. 扩展对NetCore3.1的支持，包括NLog/Nacos等中间件完善。
5. 完善版本发布流程，包括：nuget package 的自动化测试等。
6. Bug修改参考 ZP.Lib.Main DevLog.md BugList 46+
   
主要包括：
1. ZP.Lib.Main 1.0.6 (ZP.Lib.Server) 
   ZProRx框架核心包发布，包括：ZP的基础设施（构建、访问、池化、持久化、关系）基于ZP的反映式框架、网络框架、后端框架、Unity可视化分离架构等。 详细说明请参考： 
   https://github.com/bennychao/ZProRx.Lib/blob/master/ZP.Lib/ZP.Lib.Main/Readme.md
   https://gitee.com/benny8080/ZProRx.Lib/tree/master/ZP.Lib

   下载安装：
   .Net CLI >dotnet add package ZP.Lib.Server --version 1.0.6 
   或 VS 2019 Nuget 包管理中搜索"ZP.Lib.Server"并安装（推荐）

2. ZProRx.Lib Plugin 1.0.6 
   下载地址： 
   https://github.com/bennychao/ZProRx.Lib/releases/ 
   https://gitee.com/benny8080/ZProRx.Lib/releases   
   https://assetstore.unity.com/ 中 搜索 ZProRx.Lib (开发中)

3. ZProRx.Test.Unity 
   案例 
   源码地址：
   https://github.com/bennychao/ZProRx.Lib/ZProRx.Test.Unity
   https://gitee.com/benny8080/ZProRx.Lib/tree/master/ZProRx.Test.Unity
      
   https://gitee.com/benny8080/ZProRx.Lib.git  


### [v1.0.5]
[2020-03-09] 发布ZProRx.Lib 1.0.5 版本

变更内容：
1. 开源ZP.Lib.Standard 是Soc应用与Web应用的共通底层库，只依赖以Standard 2.0 目前集成了配置服务Nacos（阿里开源配置框架）。
2. 开源ZP.Lib.NetCore 提供了用于NetCore下的一些扩展，更方便在NetCore上使用的ZProxRx。主要用于基于NetCore的控制台应用开发。
3. 开源ZP.Lib.Web 用于提供Http后端的ZP共通库。主要用于基于NetCore的Web 应用开发。
4. 追加ZProRx.Test.Web Demo 用于提供Web应用的Demo,可以与ZProRx.Test.Unity中的场景进行联动
5. ZProRx.Test.Unity 追加两个Test场景：TestSocketStage 和 TestWebStage，详细参考 ZProRx.Lib.Readme 
6. Bug修改参考 ZP.Lib.Server DevLog.md BugList 46
   
主要包括：
1. ZP.Lib.Main 1.0.5 (ZP.Lib.Server) 
   ZProRx框架核心包发布，包括：ZP的基础设施（构建、访问、池化、持久化、关系）基于ZP的反映式框架、网络框架、后端框架、Unity可视化分离架构等。 详细说明请参数： https://github.com/bennychao/ZProRx.Lib/blob/master/ZP.Lib/ZP.Lib.Main/Readme.md
   下载安装：
   .Net CLI >dotnet add package ZP.Lib.Server --version 1.0.5 
   或 VS 2019 Nuget 包管理中搜索"ZP.Lib.Server"并安装（推荐）

2. ZProRx.Lib Plugin 1.0.5 
   下载地址： 
   https://github.com/bennychao/ZProRx.Lib/releases/ 
   https://github.com/bennychao/ZProRx.Lib/Publish/ZProRx.Lib.package 
   https://assetstore.unity.com/ 中 搜索 ZProRx.Lib (开发中)

3. ZProRx.Test.Unity 
   案例 
   源码地址：
   https://github.com/bennychao/ZProRx.Lib/ZProRx.Test.Unity
   https://gitee.com/benny8080/ZProRx.Lib.git  



### [v1.0.4]

[2020-02-26]
发布ZProRx.Lib 1.0.4 版本

变更内容：
1. ZProRx.Test.Unity Demo UI 追加Card的支持，用于展示List的相关操作 （非ZP.Lib.Card）
2. 开源ZP.Lib.Card 模块，主要提供Card的抽象定义以及相关代码支持。详细参考[ZP.Lib.Card](./ZP.Lib/ZP.Lib.Card/Readme.md)
3. Bug修改参考 ZP.Lib.Server DevLog.md BugList 32 - 46

主要包括：
1. ZP.Lib.Main 1.0.4 (ZP.Lib.Server)
   ZProRx框架核心包发布，包括：ZP的基础设施（构建、访问、池化、持久化、关系）基于ZP的反映式框架、网络框架、后端框架、Unity可视化分离架构等。
   详细说明请参数：
   https://github.com/bennychao/ZProRx.Lib/blob/master/ZP.Lib/ZP.Lib.Main/Readme.md

   下载安装：
  - .Net CLI
  `>dotnet add package ZP.Lib.Server --version 1.0.3`
  - 或 VS 2019 Nuget 包管理中搜索"ZP.Lib.Server"并安装（推荐）
  
2. ZProRx.Lib Plugin 1.0.4 
   下载地址：
   https://github.com/bennychao/ZProRx.Lib/releases/
   https://github.com/bennychao/ZProRx.Lib/Publish/ZProRx.Lib.package

3. ZProRx.Test.Unity 案例
    源码地址：https://github.com/bennychao/ZProRx.Lib/ZProRx.Test.Unity



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

