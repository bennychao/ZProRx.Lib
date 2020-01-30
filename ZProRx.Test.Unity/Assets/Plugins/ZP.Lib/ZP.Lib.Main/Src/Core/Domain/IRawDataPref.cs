
using System;
using System.Collections.Generic;

namespace ZP.Lib
{
    public interface IRawDataPref
    {
        object RawData { get; set; }

        object ToValue();

        T QueryProperty<T>(string propertyId);

        T GetData<T>();
    }

}