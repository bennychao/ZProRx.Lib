using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using ZP.Lib.Battle;

namespace ZP.Lib.Matrix.Test.Entity
{
    public class TUnit :
    ZBattle.BaseBattleUnit,

    //base interface
    ZBattle.IBattleUnit,
    ZBattle.IEquipable,

    //extensions interface
    ZBattle.ISkillable,
    ZBattle.IMovable
    {
        public float MoveSpeed => throw new NotImplementedException();

        public bool IsMoving => throw new NotImplementedException();

        public IObservable<bool> MovingStatusObervable => throw new NotImplementedException();

        public void BeAssisted(ZBattleTemplate<BattleAction, float, Vector2>.IAssistable assister)
        {
            throw new NotImplementedException();
        }

        public void BeAttacked(ZBattleTemplate<BattleAction, float, Vector2>.IAttackable attacker)
        {
            throw new NotImplementedException();
        }

        public void BeDeselected()
        {
            throw new NotImplementedException();
        }

        public void BeSelected()
        {
            throw new NotImplementedException();
        }

        public void Build()
        {
            throw new NotImplementedException();
        }

        public void Destroy()
        {
            throw new NotImplementedException();
        }

        public void MoveTo(Vector2 pos)
        {
            throw new NotImplementedException();
        }

        public void Rotate(float a)
        {
            throw new NotImplementedException();
        }

        public void Run()
        {
            throw new NotImplementedException();
        }

        public ZBattleTemplate<BattleAction, float, Vector2>.ISkillUnit SelectSkill(int index)
        {
            throw new NotImplementedException();
        }
    }
}
