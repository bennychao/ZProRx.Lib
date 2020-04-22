using System;
namespace ZP.Lib
{
    /// <summary>
    /// 接口引用支持
    /// </summary>
    public interface IRefable
    {
        int RefID { set; get; }
        void BindRef();

        object ToMap();
    }
}
