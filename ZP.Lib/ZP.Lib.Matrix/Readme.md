# ZP.Lib.Matrix

是Soc与Web的基础，包含一些Soc或者Web都使用的一些共通类。

## Channel
相关Channel的定义参考[ZP.Lib.Soc](../ZP.Lib.Soc/Readme.md)
本库中定义各Pipeline，包括：合同、帧同步、广播、组播、单播等
相应的ServerChannel，定义在[ZP.Lib.Soc](../ZP.Lib.Soc/Readme.md)中。
这是因为ServerChannel只会在后端Server工程中定义，包括[ZP.Lib.Soc](../ZP.Lib.Soc/Readme.md)包即可。

另外，还有SystemRuntimePipeline参考 [ZP.Lib.Battle](../ZP.Lib.Battle/Readme.md)

 
### clientIds
属性表示，
- 对于普通的Channel，连接到当前Channel的Client，Channel可以在Server也可以在Client
- 对于Pipeline Client 端来说，其值是连接上的整体Server端与Client整体的Client，其也是多个的。其中其第一个为本Client所以ClientID。
- 对于Suite Server Channel，即每一个Client对应一个Channel单体，这样其ClientId为连接的客户端

**OnConnectedObservable** 
用于接收被连接的事件。即有Client连入。
对于Pipeline其含义表示，Client端与Server端会做为一体，当有Client接入时，其就会接收到消息，但其ClientIds不会变。


## 时序
有关通道与管道有着很重要的时序性。

### 生命周期
- Listen：指通道处于一个低功耗的状态，只是接收主要的Topic事件，包括：Connect、GetStatus等。
- Opened：通道处于全速工作状态，所以方法都可以进行调用
- Closed：通道处于Closed的状态

```csharp
    public enum ChannelStatusEnum
    {
        Disabled = 0,
        Listen,
        Opened,
        Closed,
        Suspended,
    }
```

### 时序注意事项
1. 对于通道或者管道来说，Server端（对于Channel也可以是Client端）要先启动，进入Listen状态后，CLient端才可以调用Connect进行连接。[TODO] 对应的异常需要进行处理。
2. 对于管道，Open状态后，有一定时间开启各方法的监听，这期间会调用失败。对于Client端调用Connect后，在OnOpened中进行处理是安全的。而在ServerChannel的OnOpened中相反是不安全的。比如：SceneManageChannel。即ServerChannel的Open要比Client的要早一些。
3. [TODO] 针对这种可能要一种同步机制。

---

[返回ZP.Lib](../Readme.md)