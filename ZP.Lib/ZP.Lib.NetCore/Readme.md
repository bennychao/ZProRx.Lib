# ZP.Lib.NetCore

## 简介
提供了用于NetCore下的一些扩展，更方便在NetCore上使用ZProxRx框架。

## 主要功能与特性
 - [Swagger](#swagger%e6%94%af%e6%8c%81)
 - [ZsonResult](#zsonresult)
 - [shutdown](#shutdown)

## Swagger支持

 - 修改UI Title，通过注入的方式
  
  ```csharp
  c.InjectJavascript("/swagger_custom.js"); // 加载中文包
  ```
接入方式如下：
```csharp
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddMvc().
            SetCompatibilityVersion(CompatibilityVersion.Version_3_0).
            AddZPBinder(); //Support FromBody with ZP Class

        services.AddMatrix(Configuration);

        services.AddZPSwagger<Program>(Configuration);
    }
```
```csharp
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime appLifeTime
        , ILoggerFactory loggerFactory)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        app.UseMatrix();
        //app.UseMvc();
        app.UseRouting();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });

        app.UseZPSwagger<Program>();
    }
```


## ZsonResult 
Zson是对Json的简单封装的定义，这里也可NetCore进行扩展，可以用于Web API的返回值，与作为[FromBody]参数。
返回值如果使用Matrix框架，默认是支持。
如果需要参数支持，需要调用` IMvcBuilder.AddZPBinder `MVCBuilder的扩展

```csharp
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_3_0).AddZPBinder();
```

### 常用类
- ZsonResult 派生至NetCore的ActionResult类，用于Zson格式数据的返回。
- ZsonResult<T> T为相应的ZP数据类。
- ZsonHub<T> 如果返回数据为基本类型或者其它的非ZP类，可以使用的Hub进行封装，同样也可以以Zson的格式返回。
- ZsonListHub<T> 类似上者，支持返回List的Zson数据。

什么是ZP类，参考[ZP.Lib.Main](../ZP.Lib.Main/Readme.md)


## ZP Model
用于Mysql的封装，基于ZP.Lib.Server
包括创建、查询、更新等基本功能。
支持IOC 构造方法。可以自动与Web API Controller进行绑定，也可以导入一些依赖

Model类定义,由于连接字符串，一般在appsettings.json中定义，所以需要为Model提供IConfiguration 对象做为构造参数。这里有框架自动进行依赖注入。
```csharp
    public class BoxesModel : BaseModel
    {
        public BoxesModel(IConfiguration configuration) : base(configuration)
        {
        }

```
默认Model的类名为其Table的名，如上BoxesModel 类，对应的Table名为Boxs。

Controller IOC
```csharp   
    public BoxesController(IModelsProvider modelsProvider)
    {
        base.modelsProvider = modelsProvider;
    }
```

获取Model对象，以及对数据库的访问
```csharp   
    var model = modelsProvider.GetModel<BoxesModel>();
    model.AddBox(pid, link);
```
详细完整的例子，请参考ZProRx.Test.Web 工程源码

  
## Shutdown
在类库中提供 SystemController定义，其中包括方法(Web API)
 `/api/v1/com/System/shutdown`
 用于退出Web 应用。

另外，如果有需要释放资源等操作，需要在
```csharp
    appLifeTime.ApplicationStopped.Register(() =>
    {
        Debug.Log("On Stop App!");
        app.StopRoomBuilder();
    });
```

## BugList
1. [TODO] Swagger 支持目前Version还没有与工程版本联动。


## NetCore 3.1 升级问题汇总
1.  NLog 只支持到3.0 ，未支持3.1 版本。
    loggerFactory.AddNLog();
    env.ConfigureNLog("sysnlog.config");

[TODO][2020-03-06]
在WebServer中还是不好用。需要考虑是否把其合并到UseMatrix中。

2. [Fixed]ZsonResultExecutor 未进行修改完。      
3. AddJsonFormatters 删除了，啥影响啊？      
4. [Fixed]CreateDefaultBuilder 与 CreateHostBuilder 可以共用吧？ 应该是同时支持的。
5. fact.AddConsole(); h目前 不好用了。
6. [Fixed][FromBody] 接收不到啊。 use API ReadToEndAsync
7. AddMvc 可以 变为 AddController
---

[返回ZP.Lib](../Readme.md)