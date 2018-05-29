using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Data;
using System.ComponentModel;

namespace Data4Mapping
{
    /// <summary>
    /// 类型转换类
    /// </summary>
    public static class ConvertExtension
    {
        /// <summary>
        /// 将一个值转换到指定类型的方法
        /// </summary>
        /// <param name="value">要转换的值obj</param>
        /// <param name="convertType">要转换到的类型</param>
        /// <returns>被转换类型后的值</returns>
        public static object ConvertTo(this object value, Type convertType)
        {
            //判断convertsionType类型是否为泛型，因为nullable是泛型类,
            if (convertType.IsGenericType &&
                //判断convertsionType是否为nullable泛型类
            convertType.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
            {
                if (value == null || value.ToString().Length == 0)
                {
                    return null;
                }

                //如果convertsionType为nullable类，声明一个NullableConverter类，该类提供从Nullable类到基础基元类型的转换
                NullableConverter nullableConverter = new NullableConverter(convertType);
                //将convertsionType转换为nullable对的基础基元类型
                convertType = nullableConverter.UnderlyingType;
            }
            var obj = Convert.ChangeType(value, convertType);
            return obj;

        }
    }

    /// <summary>
    /// 赋值类
    /// </summary>
    internal class PropertySetter
    {
        private readonly Type _type;
        private Action<object, object> _action;
        public  PropertyInfo Property { get; private set; }
        private string _name;
        internal ModelBinding Binding { get; private set; }

        public object Reader { get; private set; }

 

        

        public PropertySetter(PropertyInfo property, Type type, string name,object reader,ModelBinding binding)
        {
            this.Reader = reader;
            _type = type;
            Property = property;
            _name = name;
            Binding = binding;
            CreateExpress();
        }



        private void CreateExpress()
        {
            var paramobj = Expression.Parameter(typeof(object), "obj");
            var paramval = Expression.Parameter(typeof(object), "val");

            var bodyobj = Expression.Convert(paramobj, _type);
            var bodyval = Expression.Convert(paramval, Property.PropertyType);

            var body = Expression.Call(bodyobj, Property.GetSetMethod(), bodyval);
            _action = Expression.Lambda<Action<object, object>>(body, paramobj, paramval).Compile();

        }






        /// <summary>
        /// 填充值的方法
        /// </summary>
        /// <param name="obj">赋值的对象</param>
        /// <param name="value">要设置的值</param>
        public void SetValue(object obj, object value)
        {
            MappingHelper.SetValue(obj, value, Property, this._action);

        }

    }
}
