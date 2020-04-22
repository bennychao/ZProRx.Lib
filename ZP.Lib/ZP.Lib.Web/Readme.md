# ZP.Lib.Web

用于提供Http后端的ZP共通库

**###主要功能####**

1. 共通配置支持[MatrixConfig]类
2. 提供Matrix的扩展，MatrixAppBuilderExtensions和MatrixCoreServiceCollectionExtensions
3. 集成NLog功能

## 构建Web 应用


## Web API 返回值

可以返回ZP类
这里提供了一些封装，定义在 ZPropertyNetCore 类中。

```csharp
    [HttpPut("{id}")]
    public IActionResult Put(int id, [FromBody]TestData value)
    {
        var ret = ZPropertyMesh.CreateObject<TestData>();
        ret.testNum.Value = 100;
        return ZPropertyNetCore.ZsonOkResult(ret);
    }
```

## 配置NLog
目前在本包中已经支持了NLog，在应用的StartUp 中进行配置就可以加支使用了。
需要定义相应的配置文件

```xml
<?xml version="1.0" encoding="utf-8" ?>
<nlog xmlns="http://www.nlog-project.org/schemas/NLog.xsd"
      xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
     autoReload="true"
       internalLogLevel="Warn"
       internalLogFile="internal-nlog.txt">
    <!--define various log targets-->
    <targets>
        <!--write logs to file-->
        <target xsi:type="File" name="allfile" fileName="nlog-all-${shortdate}.log"
                 layout="${longdate}|${logger}|${uppercase:${level}}|${message} ${exception}" />

        <target xsi:type="File" name="ownFile-web" fileName="nlog-my-${shortdate}.log"
                 layout="${longdate}|${logger}|${uppercase:${level}}|${message} ${exception}" />
        <target xsi:type="Null" name="blackhole" />
    </targets>
    <rules>
        <!--All logs, including from Microsoft-->
        <logger name="*" minlevel="Trace" writeTo="allfile" />

        <!--Skip Microsoft logs and so log only own logs-->
        <logger name="Microsoft.*" minlevel="Trace" writeTo="blackhole" final="true" />
        <logger name="*" minlevel="Trace" writeTo="ownFile-web" />
    </rules>
</nlog>
```
使用代码如下：

```csharp
App.UseZLog("sysnlog.config");
```

打Log有两种方式：

- ILogger<XXX> logger
- Debug.Log

可以看到如下不同的Log风格：
```s
2020-04-15 11:06:56.4283|ZProRx.Test.Web.Controllers.TestDataModelController|WARN|Test 
2020-04-15 11:06:56.4283|Http|WARN|>> [2020/4/15 11:06:56432] Debug:Test 
```

但对于ZProRx框架中的Log都是通过Debug的方式输出的。

所以以上配置了UseZLog后，可以看到框架输出的一些异常Log！！！

---

[返回ZP.Lib](../Readme.md)