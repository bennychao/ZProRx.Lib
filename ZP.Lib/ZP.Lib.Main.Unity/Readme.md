# ZP.Lib.Main.Unity

基于ZP.Lib.Main，对Unity支持的扩展


## 切割日志

[2020-04-16]
切割失败

![](./Docs/img/Readme_2020-04-16-13-25-00.png)

主要是TransNode （Transform）耦合的太深了。需要想办法进行对应。

分步：

1. 切割ZP.Lib.Main
2. 消除ZP.Lib.Main对UnityEngine库的依赖。难点在Vector3的依赖上。