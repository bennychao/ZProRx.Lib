# ZP.Lib.Card

Card的概念来源于卡牌游戏，这里把游戏或者应用中的对象，抽象为Card，其有一定的属性，可以通过Json进行详细定义。支持面向对象，可以进行派生，Card间可以进行相互关联。

Card库主要静态的关系，对于Card的动态行为，定义不多。因为不同游戏、应用系统，的动态行为千差万别，
后续动态行为，可以参考[ZP.Lib.Battle](../ZP.Lib.Battle/Readme.md)主要定义了2D回合/简单即时战略游戏（帧同步）类型。



## 主要元素
本库，除了定义通用的卡片类之后，对于一些游戏等应用中的通用对象也通过卡片类进行抽象，比如宝箱、货币等。
以Card核心展开以下，扩展出以下元素
- Card Card的实体类，包括Card的各种属性定义。
- CardLink Card关联类，通过CardRef与Card进行关联，通常用于与玩家进行关联，保存与玩家关联的数据，如：当前Card的数量等。
- Box 宝箱，其也是一个特殊的卡片类。
- Currency 货币，其也是一个特殊的卡片类。
- Product


## 主要机制
- 研制与回收


## ZCardsFactory

卡片工厂，用于加载、创建以及销毁卡片。

[CardAssetConfigAttribute]可以指定Cards Json和Config文件的位置
如果没有指定使用的默认的格式进行查找

```csharp
    $"[APP]/Jsons/{typeof(TCard).Name}s" :        //default asset path
    $"[APP]/Jsons/Config/{typeof(TCard).Name}s.json"    //default Config path 对于如果是Unity Client端是不能设置json后缀名的
```

## BUG
1. 目前ZCardsFactory 中不支持使用的把CardGroup做为子类，这样JsonLoad会失败。
2. ZCardsFactory 中还是使用的宏判断，是否使用的Json扩展名，如果在Unity Client 端使用的扩展名时，会出现资源加载失败，也Server端的不一致，需要进行判断。这个可以通过添加资源管理中心来进行对应。

---

[返回ZP.Lib](../Readme.md)