using System;
using System.ComponentModel;

namespace Data4Mapping
{


    /// <summary>
    /// 自增长列类型
    /// </summary>
    [Description("自长增列类型")]
    public enum Data4IdentityType
    {
        None=0,
        /// <summary>
        /// 数据库自增长机制
        /// </summary>
        [Description("数据库自增长机制")]
        Database = 1,
        /// <summary>
        /// 自定义自增长机制
        /// </summary>
        [Description("自定义自增长机制")]
        Custom = 9
    }

    /// <summary>
    /// 列类型
    /// </summary>
    [Flags]
    public enum Data4ColumnType
    {
        None=0,
        /// <summary>
        /// 自增长
        /// </summary>
        Identity=1,
        /// <summary>
        /// 主键
        /// </summary>
        PrimaryKey=2,
        /// <summary>
        /// 获取数据库时
        /// </summary>
        DBDateTime=4,
        /// <summary>
        /// 不允许空值
        /// </summary>
        NotAllowNull=8,

        /// <summary>
        /// 必须与数据库同步
        /// </summary>
        MustSynced=16
    }

   

    [System.AttributeUsage(AttributeTargets.Assembly)]
    public class Data4ColumnBaseAttribute : Data4BaseAttribute
    {
        public Data4ColumnType ColumnType { get; set; }

    }



     [System.AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class Data4IdentityAttribute : Data4ColumnBaseAttribute
    {
        public Data4IdentityType IdentityType { get; set; }

        public Data4IdentityAttribute()
        {
            this.ColumnType =  this.ColumnType| Data4ColumnType.Identity;
            IdentityType = Data4IdentityType.Database;
        }

        public Data4IdentityAttribute(Data4IdentityType type)
        {
            this.ColumnType = this.ColumnType | Data4ColumnType.Identity;
            IdentityType = type;
        }


    }

     [System.AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
     public class Data4PrimaryKeyAttribute : Data4ColumnBaseAttribute
     { 
         public Data4PrimaryKeyAttribute()
         {
             this.ColumnType = this.ColumnType | Data4ColumnType.PrimaryKey;
             
         }

         public Data4PrimaryKeyAttribute(Data4IdentityType type)
         {
             this.ColumnType = this.ColumnType | Data4ColumnType.PrimaryKey;            
         }
     }




    /// <summary>
    /// 字段描述属性类
    /// </summary>
    [System.AttributeUsage(AttributeTargets.Property)]
    public class Data4ColumnAttribute : Data4IdentityAttribute
    {
        private string _columnName = string.Empty;
        /// <summary>
        /// 字段名
        /// </summary>
        public string ColumnName
        {
            get { return _columnName; }
            private set { _columnName = value; }
        }


        private bool _enable = true;
        /// <summary>
        /// 是否启用映射
        /// </summary>
        public  bool Enable
        {
            get { return _enable; }
            private  set { _enable = value; }
        }

        public Data4ColumnAttribute()
            : this(string.Empty, true, Data4ColumnType.None)
        {

        }

        public Data4ColumnAttribute(bool enable)
            : this(string.Empty, enable, Data4ColumnType.None)
       {
           
       }
        public Data4ColumnAttribute(bool enable, Data4ColumnType columnType)
            : this(string.Empty, enable, columnType)
        {
           
        }

       public Data4ColumnAttribute(string name)
           : this(name, true, Data4ColumnType.None)
        {
            
        }

       public Data4ColumnAttribute(string name, Data4ColumnType columnType)
           : this(name, true, columnType)
       {
          
       }

        public Data4ColumnAttribute(string name, bool enable)
            : this(name, enable, Data4ColumnType.None)
        {
            
        }
        public Data4ColumnAttribute(Data4ColumnType columnType)
            : this(string.Empty, true, columnType)
        {

        }

        public Data4ColumnAttribute(string name, bool enable, Data4ColumnType columnType)
        {
            Enable = enable;
            ColumnName = name;
            this.ColumnType = columnType;
        }



    }
}
