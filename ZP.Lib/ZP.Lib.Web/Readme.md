# ZP.Lib.Web

用于提供Http后端的ZP共通库

**###主要功能####**

1. 共通配置支持[MatrixConfig]类
2. 提供Matrix的扩展，MatrixAppBuilderExtensions和MatrixCoreServiceCollectionExtensions

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

---

[返回ZP.Lib](../Readme.md)