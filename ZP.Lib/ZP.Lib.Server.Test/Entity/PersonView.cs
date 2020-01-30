using System;
using System.Collections.Generic;
using System.Text;
using ZP.Lib;

namespace ZP.Lib.Server.Test.Entity
{
    internal class PersonView
    {
        public ZPropertyList<PersonLink> personLinks = new ZPropertyList<PersonLink>();

        public ZPropertyList<PersonLink> personLinks2 = new ZPropertyList<PersonLink>();

        public ZPropertyRefList<PersonLink> personRefLinks = new ZPropertyRefList<PersonLink>();

        public ZPropertyRefList<PersonLink> personRefLinks2 = new ZPropertyRefList<PersonLink>();

        public ZPropertyRefList<Person> personRefLinksView = new ZPropertyRefList<Person>();

        public ZPropertyRefList<PersonLink> personLinkRefLinksView2 = new ZPropertyRefList<PersonLink>();


        public ZProperty<int> totalBlood = new ZProperty<int>();

        public ZProperty<float> totalfloatBlood = new ZProperty<float>();
    }
}
