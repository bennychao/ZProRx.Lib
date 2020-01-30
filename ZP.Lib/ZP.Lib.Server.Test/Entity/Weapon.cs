using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using ZP.Lib;

namespace ZP.Lib.Server.Test.Entity
{
    //use ".xxx" to support sub Class from Weapon
    [RTTransformClass(".transform")]
    [PropertyAddComponentClass(typeof(RTWeaponObj), typeof(RTWeaponObj2))]
    internal class Weapon
    {

        [PropertyDescription("power", "a power data")]
        public ZProperty<float> power = new ZProperty<float>();

        public ZProperty<ZTransform3> transform = new ZProperty<ZTransform3>();

        public Weapon()
        {

        }

        public Weapon(float power)
        {
            this.power.Value = power;
        }


    }

}
