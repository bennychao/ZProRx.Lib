using System;
using System.Collections.Generic;
using System.Text;
using ZP.Lib;

namespace ZP.Lib.Server.Test.Entity
{
    internal class PersonLink : IIndexable
    {

        [PropertyLinkSync]
        [PropertyLink(".PersonRef.blood")]
        public ZLinkProperty<int> blood3 = new ZLinkProperty<int>();

        //[PropertyLinkSync] not need set, will throw exception
        [PropertyLink(".PersonRef.testList")]
        public ZListCursor<int> zListCursor = new ZListCursor<int>();

        //view
        public ZProperty<bool> bDead = new ZProperty<bool>();

        public ZProperty<int> testCount = new ZProperty<int>();

        public ZProperty<int> linkId = new ZProperty<int>();

        public ZPropertyInterfaceRef<Person> PersonRef = new ZPropertyInterfaceRef<Person>(
                //bind function
                (id) =>{

                    var person = ZPropertyMesh.CreateObject<Person>();
                    ZPropertyPrefs.Load(person, "../../../Assets/TestPerson.json");

                    person.blood2.Upgrade(1);
                    return person;
                });

        public int Index
        {
            get => linkId.Value;
            set => linkId.Value = value;
        }
    }
}
