using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace ZP.Lib
{

    public interface IZProperty : IZPropertable
    {
     string SimplePropertyID { get; }
     Action<object> OnValueChanged{ set; get;}

      string Name {get;}
      string Description { get;}
      

      object Value{ set; get;}

      Type  GetDefineType ();
      bool IsInstanceOf(Type interfaceType);

      void Copy (IZProperty prop);
      IZPropertyViewItem ViewItem{ get;}
    }

    public interface IZProperty<T> : IZProperty
    {
      new T Value { set; get; }

      new Action<T> OnValueChanged { set; get; }
    }

}


