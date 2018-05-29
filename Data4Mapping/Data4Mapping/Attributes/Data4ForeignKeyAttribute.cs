using System;

namespace Data4Mapping
{
    /// <summary>
    /// 外键标识Attribute
    /// </summary>
    public class Data4ForeignKeyAttribute:Attribute
    {
        /// <summary>
        /// 外键名称
        /// </summary>
        public string Name { get; private set; }

        public Data4ForeignKeyAttribute()
        {
            this.Name = string.Empty;
        }

        /// <summary>
        /// 外键标识Attribute
        /// </summary>
        /// <param name="name">外键名称</param>
        public Data4ForeignKeyAttribute(string name)
        {
            this.Name = name;
        }
    }
}
