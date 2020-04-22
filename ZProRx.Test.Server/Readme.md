# ZProRx.Test.Server 

## 简介
用于测试Unity端与Soc Server以及基本的网络通信功能。



## 运行
使用的参数："{\"WorkerParam\":\"run\",\"Port\":5050,\"UnitType\":\"hall\",\"Count\":2, \"IsPrivateClub\":true}"

- 方法1
在[运行][配置][Default][参数]中追加以上字符串。

- 方法2：
  Windows下运行命令：
```s
    dotnet .\ZProRx.Test.Soc.dll '{\"WorkerParam\":\"run\",\"Port\":5050,\"UnitType\":\"hall\",\"Count\":2,\"IsPrivateClub\":true}'
```

    Mac/Linux 下运行如下命令:
```s
    dotnet ./ZProRx.Test.Soc.dll '{"WorkerParam":"run","Port":5050,"UnitType":"hall","Count":2,"IsPrivateClub":true}'
```

