using System;

namespace Data4Mapping
{
    /// <summary>
    /// 父级字段关系标识
    /// </summary>
    public class Parent4ColumnAttribute:Attribute
    {
        /// <summary>
        /// 列名称
        /// </summary>
        public string ColumnName { get; private set; }

        /// <summary>
        /// 父级标识
        /// </summary>
        /// <param name="columnName">对应的父级字段名</param>
        public Parent4ColumnAttribute(string columnName)
        {
            this.ColumnName = columnName;
        }
    }
}
