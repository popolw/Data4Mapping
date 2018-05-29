using System;

namespace Data4Mapping
{
    /// <summary>
    /// 日志属性类型
    /// </summary>
    [Flags]
    public enum Data4LogType
    {
        /// <summary>
        /// 未定义
        /// </summary>
        None = 0,
        /// <summary>
        /// 主键ID
        /// </summary>
        ID = 1,

        /// <summary>
        /// 编号
        /// </summary>
        Code = 2,

        /// <summary>
        /// 名称
        /// </summary>
        Name=4,

        /// <summary>
        /// 必须的
        /// </summary>
        Must = 8,
        /// <summary>
        /// 其它
        /// </summary>
        Other = 16,

        /// <summary>
        /// 简单的
        /// </summary>
        Simple = ID | Code|Name,

        /// <summary>
        /// 主要的
        /// </summary>
        Primary = ID | Code |Name| Must,

        /// <summary>
        /// 完整的
        /// </summary>
        Full = ID | Code | Name | Must | Other
    }

    /// <summary>
    /// 日志Attribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class Data4LogAttribute : Data4BaseAttribute
    {
        public Data4LogAttribute()
        {
        }

        public Data4LogAttribute(Data4LogType propType)
        {
            _logType = propType;
        }

        private Data4LogType _logType = Data4LogType.None;
        /// <summary>
        /// 日志类型
        /// </summary>
        public Data4LogType LogType
        {
            get { return _logType; }
            set { _logType = value; }
        }


    }
}
