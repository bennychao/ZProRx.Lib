using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using ZP.Lib;

namespace ZP.Lib.Server.Test.Entity
{
    internal class RTWeaponObj : ZPropertyViewItemBehaviour, IZPropertyViewItem
    {
        public bool bBind = false;

        void Start()
        {
            var cur = GetComponent<Collider>();
            cur.isTrigger = true;
            cur.enabled = false;
        }

        public bool Bind(IZProperty property)
        {
            base.BindBase(property);
            //throw new NotImplementedException();
            var weapon = property.Value as Weapon;
            Assert.IsTrue(weapon != null);
            Assert.AreEqual(weapon.power.Value, 12.5f, 0.01f);

            bBind = true;

            return true;
        }

        public void Unbind()
        {
            //throw new NotImplementedException();
            bBind = false;
        }

        public void UpdateValue(object data)
        {
            //throw new NotImplementedException();

        }

        private void OnTriggerEnter(Collision collision)
        {
            //Debug.Log("OnCollisionEnter " + collision.collider.name);
            var cur =  GetComponent<Collider>();
            Assert.IsTrue(collision.collider.bounds.Intersects(cur.bounds));
        }
    }


    internal class RTWeaponObj2 : ZPropertyViewItemBehaviour, IZPropertyViewItem
    {
        public bool bBind = false;
        public bool Bind(IZProperty property)
        {
            base.BindBase(property);
            //throw new NotImplementedException();
            var weapon = property.Value as Weapon;
            Assert.IsTrue(weapon != null);
            Assert.AreEqual(weapon.power.Value, 12.5f, 0.01f);

            bBind = true;

            return true;
        }

        public void Unbind()
        {
            //throw new NotImplementedException();
            bBind = false;
        }

        public void UpdateValue(object data)
        {
            //throw new NotImplementedException();

        }
    }

    internal class RTWeaponObj3 : ZPropertyViewItemBehaviour, IZPropertyViewItem
    {
        public bool bBind = false;
        public bool Bind(IZProperty property)
        {
            base.BindBase(property);
            //throw new NotImplementedException();
            var weapon = property.Value as Weapon;
            Assert.IsTrue(weapon != null);
            Assert.AreEqual(weapon.power.Value, 12.5f, 0.01f);

            bBind = true;

            return true;
        }

        public void Unbind()
        {
            //throw new NotImplementedException();
            bBind = false;
        }

        public void UpdateValue(object data)
        {
            //throw new NotImplementedException();

        }
    }
}
