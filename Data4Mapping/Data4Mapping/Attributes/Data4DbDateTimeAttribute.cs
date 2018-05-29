using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Data4Mapping
{

    public enum Data4DbTimeModel
    {
        /// <summary>
        /// 只有添加时才负值
        /// </summary>
        Insert,

        /// <summary>
        /// 变更 必须显示赋值才更新
        /// </summary>
        Change,

        /// <summary>
        /// 不管是否赋值都更新
        /// </summary>
        Always 
    }

    public class Data4DbDateTimeAttribute:Attribute
    {
        public Data4DbTimeModel DbTimeModel { get; private set; }

        public Data4DbDateTimeAttribute() { }

        public Data4DbDateTimeAttribute(Data4DbTimeModel model) 
        {
            DbTimeModel = model;
        }
    }
}
