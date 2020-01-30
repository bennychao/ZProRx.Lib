
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZP.Lib;
using UniRx;
using ZP.Lib.Core.Relation;

public enum GroupTypeEnum
{
	ArcherTower,
	Mage,
	MageTower,
	RedTower
}

public enum CardCategoryEnum
{
    Normal,
    Super,
    Platina
}

[PropertyAddComponentClass(typeof(UICardItem))]
public class TestCard: IIndexable
{
    [PropertyImageRes(ImageResType.LocalRes, "ZProApp/Test/")]
    public ZProperty<string> Image = new ZProperty<string>();

    private ZProperty<int> cardId = new ZProperty<int>();

    [PropertyDescription("blood", "bloodDes")]
    private ZProperty<int> blood = new ZProperty<int>();

    [PropertyDescription("attack", "bloodDes")]
    private ZProperty<int> attack = new ZProperty<int>();

    public ZProperty<Vector3> Color = new ZProperty<Vector3>(new Vector3(1, 1, 1));

    public ZProperty<CardCategoryEnum> Category = new ZProperty<CardCategoryEnum>();

    private ZDirectEvent onSelect = new ZDirectEvent();

    public int Index {
        get => cardId.Value;
        set => cardId.Value = value;
    }
}