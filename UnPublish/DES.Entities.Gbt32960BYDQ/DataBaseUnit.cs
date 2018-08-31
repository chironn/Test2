using System;

namespace DES.Entities.BYDQ
{
    /// <summary>
    /// 信息体对象属性
    /// </summary>
    public class DataBaseUnit :  ICloneable
    {
        /// <summary>
        /// 信息类型标志
        /// </summary>
        public byte DataType { get; set; }


        protected virtual Object OnClone()
        {
            return MemberwiseClone();

        }

        public object Clone()
        {
            return OnClone();
        }
    }
}
