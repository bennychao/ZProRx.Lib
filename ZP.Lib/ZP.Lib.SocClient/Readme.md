# ZP.Lib.SocClient

## 主要功能
即，本地客户端，为Soc端 AI提供一个运行环境（套间），基本操作与真正的Client没有区别，代码可以差分进行复用。
套间相关定义参考[ZP.Lib.Soc](../ZP.Lib.Soc/Readme.md)

本地客户端使用套间的目的
- Battle等单体是可以做成套间隔离的。当然目前看还是考虑性能，使用Room单体合适一些。//TODO 目前还不是套间的单体。
- 未来考虑把Client也分离为单独的进程运行。


### 实现方案：

通过底层判断，当前Topic是否在本地，如果在本地，通过Subject直接进行发送，不需要通过Socket。参考类TTPServer，TTPClient端不会有类似的处理，本地通过也只是对于
SocServer端才会有的情况。


### ZServerScene
由于性能和开销的考虑，不能把ZServerScene每一个SocClient都有单独的。所以考虑把ZServerScene做成Room级的单体。
主要就是要把给SocClient追加SceneMgrPipeline就可以了。


### 目前限制：
对于本地通道端，还不能接收到Socket 层的Connected消息。只能使用的Channel层的Connect Topic消息。
先不支持单进程时的Scene支持。本地通道内是无法访问到ZServerScene的单体的。

---

[返回ZP.Lib](../Readme.md)