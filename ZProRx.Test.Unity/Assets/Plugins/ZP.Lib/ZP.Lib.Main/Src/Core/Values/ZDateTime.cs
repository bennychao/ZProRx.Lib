using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;



namespace ZP.Lib
{



    /// <summary>
    /// ZD ate time.
    /// </summary>
    [PropertyValueChangeAnchorClass(".ms", ".time", ".date")]
    public class ZDateTime
    {
        public ZProperty<long> ms = new ZProperty<long>();
        public ZProperty<string> time = new ZProperty<string>("00:00:00");
        public ZProperty<string> date = new ZProperty<string>("1980/01/01");

        DateTime rawTime;

        public void OnLoad()
        {
            //Convert.ToDateTime(string)
            //string格式有要求，必须是yyyy - MM - dd hh: mm: ss

            DateTime dt;

            DateTimeFormatInfo dtFormat = new System.Globalization.DateTimeFormatInfo();

            dtFormat.ShortDatePattern = "yyyy/MM/dd hh:mm:ss";

            dt = Convert.ToDateTime(date.Value + " " + time.Value, dtFormat);

            //return dt;
            rawTime = dt.AddMilliseconds(ms);
        }

        public DateTime ToDate()
        {

            return rawTime;
        }

        public DateTime ToTime()
        {
            return DateTime.Now;
        }

        public int Compare(DateTime curtime)
        {
            return this.ToDate().CompareTo(curtime);
        }

        public override string ToString()
        {
            //return base.ToString();
            return date.Value + " " + time.Value;
        }

        public void ConvertFromString(string strTime) {
            DateTimeFormatInfo dtFormat = new System.Globalization.DateTimeFormatInfo();

            dtFormat.ShortDatePattern = "yyyy/MM/dd hh:mm:ss";

            var now = Convert.ToDateTime(strTime, dtFormat);


            //time.Value = now.ToLongTimeString().ToString();//10:46:56
            //date.Value = now.ToShortDateString().ToString();

            date.Value = now.ToString("yyyy/MM/dd");//10:46:56
            time.Value = now.ToString("hh:mm:ss");

            rawTime = now;
        }


        static public ZDateTime Create(string strTime) {
            var ret = ZPropertyMesh.CreateObject<ZDateTime>();
            ret.ConvertFromString(strTime);

            return ret;
        }

        public static ZDateTime Now()
        {
            var t = ZPropertyMesh.CreateObject<ZDateTime>();
            t.ToNow();
            return t;
        }

        public TimeSpan Duration(ZDateTime target)
        {
            return target.ToDate() - ToDate();
        }


        public int Compare(ZDateTime curtime)
        {
            return this.ToDate().CompareTo(curtime.ToDate());
        }

        public void ToNow()
        {
            var now = DateTime.Now;
            date.Value = now.ToString("yyyy/MM/dd");//10:46:56
            time.Value = now.ToString("hh:mm:ss");

            rawTime = now;
        }

    }


    public class ZDateTimeProperty : ZProperty<ZDateTime>
    {

    }
}

