using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Data4Mapping
{
    /// <summary>
    /// 辅助类
    /// </summary>   
    public static class MappingHelper
    {

        /// <summary>
        /// 获取属性对应映射的字段名称
        /// </summary>
        /// <param name="property">PropertyInfo</param>
        /// <returns>数据库字段名称</returns>
        public static string GetColumnName(this PropertyInfo property)
        {
            var attr = ReflectionHelper.FindAttribute<Data4ColumnAttribute>(property);

            return attr == null ? property.Name : attr.ColumnName;

        }


        /// <summary>
        /// 根据实体的Type获取当前这个实体属性对应映射的数据库的字段名称集合
        /// </summary>
        /// <param name="type">实体的类型Type</param>
        /// <param name="prefix">字段前缀</param>
        /// <returns>字段集合</returns>
        public static IList<PropertyDbColumn> GetColumns(Type type, string prefix)
        {
            var list = new List<PropertyDbColumn>();

            var properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);

            foreach (PropertyInfo property in properties)
            {
                var attr = ReflectionHelper.FindAttribute<Data4ColumnAttribute>(property);
                string columnName = string.Empty;
                if (attr != null && attr.Enable)
                {
                    columnName = attr.ColumnName;
                }
                else if (attr == null)
                {
                    columnName = property.Name;
                }
                else
                {
                    continue;
                }
                PropertyDbColumn pc = new PropertyDbColumn(columnName, type, property, prefix);
                list.Add(pc);
            }

            return list;
        }


        /// <summary>
        /// 属性赋值的方法被赋值的属性要求必须有Set器
        /// </summary>
        /// <param name="obj">要设值得实体即要被赋值的目标源</param>
        /// <param name="value">要设置的值</param>
        /// <param name="property">设置对应的属性</param>
        /// <param name="action">对应的设置的实现委托</param>
        internal static void SetValue(object obj, object value, PropertyInfo property, Action<object, object> action)
        {
            #region
            //var type = property.PropertyType;

            //if (value == null) return;
            ////bugfix dbnull
            //if (value == DBNull.Value) return;


            //if (type == typeof(int)) value = Convert.ToInt32(value);
            //else if (type == typeof(long)) value = Convert.ToInt64(value);
            //else if (type == typeof(float)) value = Convert.ToSingle(value);
            //else if (type == typeof(double)) value = Convert.ToDouble(value);
            //else if (type == typeof(decimal)) value = Convert.ToDecimal(value);
            //else if (type == typeof(DateTime)) value = Convert.ToDateTime(value);
            //else if (type == typeof(byte)) value = Convert.ToByte(value);
            //else if (type == typeof(bool)) value = Convert.ToBoolean(value);

            ////nullenable
            //if (type == typeof(int?)) value = value == null ? null : (int?)Convert.ToInt32(value);
            //else if (type == typeof(long?)) value = value == null ? null : (long?)Convert.ToInt64(value);
            //else if (type == typeof(float?)) value = value == null ? null : (float?)Convert.ToSingle(value);
            //else if (type == typeof(double?)) value = value == null ? null : (double?)Convert.ToDouble(value);
            //else if (type == typeof(decimal?)) value = value == null ? null : (decimal?)Convert.ToDecimal(value);
            //else if (type == typeof(DateTime?)) value = value == null ? null : (DateTime?)Convert.ToDateTime(value);
            //else if (type == typeof(byte?)) value = value == null ? null : (byte?)Convert.ToByte(value);
            //else if (type == typeof(bool?)) value = value == null ? null : (bool?)Convert.ToBoolean(value);

            ////可空枚举特殊处理
            //else if (type.IsGenericType && type.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
            //{
            //    var pType = type.GetGenericArguments()[0];
            //    if (pType.IsEnum)
            //    {
            //        if (value != null)
            //        {
            //            value = Enum.ToObject(pType, Convert.ToInt32(value));
            //        }
            //        else
            //        {
            //            //都是Null了就返回了撒
            //            return;
            //        }
            //    }
            //}

            //if (type.IsEnum && !type.IsGenericType)
            //{
            //    value = Convert.ToInt32(value);
            //}

            //if (value.GetType() == typeof(System.Byte) && type.IsEnum)
            //{
            //    value = Convert.ToInt32(value);
            //}

            //try
            //{
            //    if (action != null)
            //    {
            //        action(obj, value);
            //    }
            //}
            //catch (Exception ex)
            //{
            //    var message = string.Format("PropertyName：{0} PropertyType：{1} ValueType：{2} Value：{3} 赋值失败", property.Name, type.Name, value.GetType().Name, value);
            //    throw new Exception(message, ex);
            //}
            #endregion

            var type = property.PropertyType;

            //处理DbNull和NULL的情况
            if (value == null || value == DBNull.Value) return;

            //如果是泛型
            if (type.IsGenericType)
            {
                var typeArry = type.GetGenericArguments();

                if (typeArry.Length > 1)
                {
                    return;
                }
                type = typeArry[0];
            }

            if (type == typeof(int)) value = Convert.ToInt32(value);
            else if (type == typeof(long)) value = Convert.ToInt64(value);
            else if (type == typeof(float)) value = Convert.ToSingle(value);
            else if (type == typeof(double)) value = Convert.ToDouble(value);
            else if (type == typeof(decimal)) value = Convert.ToDecimal(value);
            else if (type == typeof(DateTime)) value = Convert.ToDateTime(value);
            else if (type == typeof(byte)) value = Convert.ToByte(value);
            else if (type == typeof(bool))
            {
                if (object.Equals(value, "0"))
                {
                    value = false;
                }
                else if (object.Equals(value, "1"))
                {
                    value = true;
                }
                else
                {
                    value = Convert.ToBoolean(value);
                }
            }
            else if (type.IsEnum)
            {
                value = Enum.Parse(type, value.ToString());
            }

            //try set value
            try
            {
                if (action != null)
                {
                    action(obj, value);
                }
            }
            catch (Exception ex)
            {
                var message = string.Format("PropertyName：{0} PropertyType：{1} ValueType：{2} Value：{3} 赋值失败", property.Name, type.Name, value.GetType().Name, value);
                throw new Exception(message, ex);
            }

        }

        private static IEnumerable<T> CastTo<T, T1>(IEnumerable ie, Func<object, T1> func)
        {
            var p = Expression.Parameter(typeof(T), "p");
            var f = Expression.Lambda<Func<T1, T>>(Expression.Convert(p, typeof(T)), p).Compile();

            foreach (var item in ie)
            {
                yield return f(func(item));
            }
        }


        /// <summary>
        /// 将数据转换成指定类型T的方法
        /// </summary>
        /// <typeparam name="T">要转换到的类型</typeparam>
        /// <param name="source">要转换的数据</param>
        /// <returns>转换后的数据</returns>
        internal static IEnumerable<T> CastTo<T>(this IEnumerable source)
        {
            var code = Type.GetTypeCode(typeof(T));

            switch (code)
            {
                case TypeCode.Object:
                    return source.Cast<T>();
                case TypeCode.DBNull:
                    return CastTo<T, DBNull>(source, s => DBNull.Value);
                case TypeCode.Boolean:
                    return CastTo<T, bool>(source, s => Convert.ToBoolean(s));
                case TypeCode.Char:
                    return CastTo<T, char>(source, s => Convert.ToChar(s));
                case TypeCode.SByte:
                    return CastTo<T, sbyte>(source, s => Convert.ToSByte(s));
                case TypeCode.Byte:
                    return CastTo<T, byte>(source, s => Convert.ToByte(s));
                case TypeCode.Int16:
                    return CastTo<T, short>(source, s => Convert.ToInt16(s));
                case TypeCode.UInt16:
                    return CastTo<T, ushort>(source, s => Convert.ToUInt16(s));
                case TypeCode.Int32:
                    return CastTo<T, int>(source, s => Convert.ToInt32(s));
                case TypeCode.UInt32:
                    return CastTo<T, uint>(source, s => Convert.ToUInt32(s));
                case TypeCode.Int64:
                    return CastTo<T, long>(source, s => Convert.ToInt64(s));
                case TypeCode.UInt64:
                    return CastTo<T, ulong>(source, s => Convert.ToUInt64(s));
                case TypeCode.Single:
                    return CastTo<T, float>(source, s => Convert.ToSingle(s));
                case TypeCode.Double:
                    return CastTo<T, double>(source, s => Convert.ToDouble(s));
                case TypeCode.Decimal:
                    return CastTo<T, decimal>(source, s => Convert.ToDecimal(s));
                case TypeCode.DateTime:
                    return CastTo<T, DateTime>(source, s => Convert.ToDateTime(s));
                case TypeCode.String:
                    return CastTo<T, string>(source, s => s.ToString());
                default:
                    throw new NotSupportedException();
            }

        }



    }
}
