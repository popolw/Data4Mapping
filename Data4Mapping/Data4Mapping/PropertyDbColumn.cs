using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Data4Mapping
{
    /// <summary>
    /// 属性和数据库列映射
    /// </summary>
    public class PropertyDbColumn
    {
        private string _prefix;

        private Action<object, object> _action;

        /// <summary>
        /// 属性和数据库列映射
        /// </summary>
        /// <param name="columnName">列名称</param>
        /// <param name="type">实体Type</param>
        /// <param name="property">对应实体的属性</param>
        /// <param name="prefix">前缀</param>
        public PropertyDbColumn(string columnName,Type type, PropertyInfo property, string prefix)
        {
            this.ColumnName = columnName;
            this.Property = property;
            this._prefix = prefix;

        }

        /// <summary>
        /// 实体属性
        /// </summary>
        public PropertyInfo Property { get; private set; }

        /// <summary>
        /// 数据库列名称
        /// </summary>
        public string ColumnName { get; private set; }

        /// <summary>
        /// 全列名称(前缀+列名称)
        /// </summary>
        public string FullColumnName
        {
            get
            {
                return this._prefix + this.ColumnName;
            }
        }




        /// <summary>
        /// 设置值
        /// </summary>
        /// <param name="obj">要设置值的目标</param>
        /// <param name="value">要设置的值</param>
        public void SetValue(object obj,object value)
        {
            if (_action == null)
            {
                var paramobj = Expression.Parameter(typeof(object), "obj");
                var paramval = Expression.Parameter(typeof(object), "val");
                var bodyobj = Expression.Convert(paramobj, obj.GetType());
                var bodyval = Expression.Convert(paramval, Property.PropertyType);
                var body = Expression.Call(bodyobj, Property.GetSetMethod(), bodyval);
                _action = Expression.Lambda<Action<object, object>>(body, paramobj, paramval).Compile();
            }

            MappingHelper.SetValue(obj, value, this.Property, this._action);
        }

        /// <summary>
        /// 获取值
        /// </summary>
        /// <param name="obj">要获取值得对象</param>
        /// <returns></returns>
        public object GetValue(object obj)
        {
            return this.Property.GetValue(obj, null);
        }

        public override string ToString()
        {
            return string.Format("列名称：{0}。", this.ColumnName);
        }


    }
}
