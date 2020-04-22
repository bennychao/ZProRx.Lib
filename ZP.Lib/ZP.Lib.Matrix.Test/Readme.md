# ZP.Lib.Matrix.Test

## 0.简介
用于测试Matrix Lib的功能：
包括：
SocApp的启动、关闭
Channel/Pipeline 相关的功能的建立、关闭、调用等操作


注意：
以上需要一个TestApp用于进行测试。

## 2.问题列表
1. Pipeline相关的Test,还没测试多个用例一起动作的情况。
2. 目前RuntimeSystem 中ondisconnected  的Ressult 还是有时不能清理干净。需要考虑，已经没有接收者时，如何进行相关的释放。
3. 需要追加测试多个房间的情况。
4. 追加测试物理特性的测试, 包括新的追加的 `IZRigidbodyComponent`和`IAnimatorComponent`。
5. Battle 相关的测试还未进行。可以以LocalBattle为主要测试时机。