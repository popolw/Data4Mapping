using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using System.Reflection;
using System.Linq;

namespace Data4Mapping
{
    /// <summary>
    /// ModelBinding辅助类
    /// </summary>
    public class ModelBindingHelper
    {
        private static readonly IDictionary<string, ModelBinding> Cache = new Dictionary<string, ModelBinding>();


        /// <summary>
        /// 创建一个ModelBinding的方法
        /// </summary>
        /// <param name="type">要填充的实体的Type</param>
        /// <param name="reader">IDataReader</param>
        /// <param name="prefix">字段前缀</param>
        /// <returns>ModelBinding</returns>
        public static ModelBinding GetBinding(Type type, IDataReader reader, string prefix)
        {

            var binding = new ModelBinding(type, reader, prefix);
            return binding;

        }


        /// <summary>
        /// 创建一个ModelBinding的方法
        /// </summary>
        /// <param name="type">要填充的实体的Type</param>
        /// <param name="table">数据表DataTable</param>
        /// <param name="prefix">字段前缀</param>
        /// <returns>ModelBinding</returns>
        public static ModelBinding GetBinding(Type type, DataTable table, string prefix)
        {
             var binding = new ModelBinding(type, table, prefix);
             return binding;
        }

    }


    /// <summary>
    /// 数据映射类
    /// </summary>
    public static class DataMapping
    {

        private static readonly IDictionary<string, Func<object, object>> Cac = new Dictionary<string, Func<object, object>>();

        private static object BindModel(object obj, IDataReader reader,string prefix)
        {
            var type = obj.GetType();
            var binding = ModelBindingHelper.GetBinding(type, reader, prefix);
            //binding.Prefix = prefix;
            return binding.Bind(obj);
        }

        private static object BindModel(object obj, DataRow row, string prefix = "")
        {
            var type = obj.GetType();
            var binding = ModelBindingHelper.GetBinding(type, row.Table, prefix);
            binding.Prefix = prefix;
            return binding.Bind(obj, row);
        }


        /// <summary>
        /// 从reader读取数据并映射到范型T这里要求T必须有一个默认的无参构造函数
        /// 映射完成后会自动关闭IDataReader对象
        /// </summary>
        /// <param name="reader">IDataReader</param>
        /// <returns>映射之后的实例</returns>
        public static T ToModel<T>(IDataReader reader) where T : new()
        {
            var obj = new T();
            return (T)ToModel(obj, reader);
        }


        /// <summary>
        /// 从reader读取数据并映射到范型T这里要求T必须有一个默认的无参构造函数
        /// 如：
        ///    var cmdt = @"select p.BlocNo AS B_BlocNo from S_UIFilterFieldPlan p left join S_UIFilterFields f on p.SUIFilterFieldID=f.SUIFilterFieldID";
        ///    var reader = DBHelper.ExecuteReader(cmdt, Provider);
        ///    var obj = DataMapping.ToModel(S_UIFilterFieldPlan)(reader, s => new Dictionary(object, string) { { s.UIFilterFields, "B_" } });
        /// </summary>
        /// <typeparam name="T">要映射的数据类型T</typeparam>
        /// <param name="reader">IDataReader</param>
        /// <param name="expression">指定有外键关系时外键表使用的别名的一个表达式</param>
        /// <returns>映射之后的实例</returns>
        public static T ToModel<T>(IDataReader reader, Expression<Func<T, object>> expression) where T : new()
        {
            var obj = new T();
            ModelBindingProxy proxy = new ModelBindingProxy(typeof(T), expression);
            proxy.InitBinding(reader);
            using (reader)
            {
                if (reader.Read())
                {
                    return (T)proxy.Bind(obj);
                }
            }
            return default(T);
        }



        /// <summary>
        /// 从DataRow读取数据并映射到范型T这里要求T必须有一个默认的无参构造函数
        /// 如：
        ///    var cmdt = @"select p.BlocNo AS B_BlocNo from S_UIFilterFieldPlan p left join S_UIFilterFields f on p.SUIFilterFieldID=f.SUIFilterFieldID";
        ///    var row = DBHelper.ExecuteDataRow(cmdt, Provider);
        ///    var obj = DataMapping.ToModel(S_UIFilterFieldPlan)(row, s => new Dictionary(object,string) { { s.UIFilterFields, "B_" } });
        /// </summary>
        /// <typeparam name="T">要映射的数据类型T</typeparam>
        /// <param name="reader">System.Data.DataRow</param>
        /// <param name="expression">指定有外键关系时外键表使用的别名的一个表达式</param>
        /// <returns>映射之后的实例</returns>
        public static T ToModel<T>(DataRow row, Expression<Func<T, object>> expression) where T : new()
        {
            var obj = new T();
            ModelBindingProxy proxy = new ModelBindingProxy(typeof(T), expression);
            proxy.InitBinding(row.Table);
            return (T)proxy.Bind(obj, row);
        }

        public static T ToModel<T>(DataRow row) where T : new()
        {
            var obj = new T();
            return (T)ToModel(obj, row);
        }

        /// <summary>
        /// 从reader读取数据并映射到实例
        /// 映射完成后会自动关闭IDataReader对象
        /// </summary>
        /// <param name="obj">要映射的实例</param>
        /// <param name="reader">IDataReader</param>
        /// <returns>映射之后的实例</returns>
        public static object ToModel(object obj, IDataReader reader)
        {
            using (reader)
            {
                if (reader.Read()) return BindModel(obj, reader, string.Empty);
            }
            return null;
        }


        public static object ToModel(object obj, DataRow row)
        {
            return BindModel(obj, row);
        }


        public static object ToModel(Type type, IDataReader reader, string prefix = "")
        {
            using (reader)
            {
                if (reader.Read())
                {
                    var obj = ActivatorHelper.CreateInstance(type);
                    return BindModel(obj, reader, prefix);
                }
            }
            return null;
        }


        public static object ToModel(object obj, IDataReader reader, ModelBindingProxy proxy)
        {
            proxy.InitBinding(reader);
            using (reader)
            {
                if (reader.Read()) return proxy.Bind(obj);
            }
            return null;
        }




        /// <summary>
        /// 从reader读取数据并映射到实例
        /// 要使用此方法您需要首先调用Reader.Reader()方法 
        /// Reader不会自动释放需要您手动close或使用using进行释放
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="reader"></param>
        /// <returns></returns>
        public static object ToModelNotCloseReader(object obj, IDataReader reader, string prefix = "")
        {
            return BindModel(obj, reader, prefix);
        }

        /// <summary>
        /// 从reader读取数据并映射到范型T这里要求T必须有一个默认的无参构造函数
        /// 要使用此方法您需要首先调用Reader.Reader()方法 
        /// Reader不会自动释放需要您手动close或使用using进行释放
        /// </summary>
        /// <typeparam name="T">范型T</typeparam>
        /// <param name="reader">IDataReader</param>
        /// <param name="prefix"></param>
        /// <returns>填充后的实体</returns>
        public static T ToModelNotCloseReader<T>(IDataReader reader, string prefix = "") where T : new()
        {
            var obj = new T();
            return (T)BindModel(obj, reader, prefix);
        }


        /// <summary>
        ///  从reader读取数据并映射到范型T这里要求T必须有一个默认的无参构造函数
        /// 映射完成后会自动关闭IDataReader对象
        /// </summary>
        /// <typeparam name="T">范型T</typeparam>
        /// <param name="reader">IDataReader</param>
        /// <returns>对个T实例的IEnumerable(T)枚举器</returns>
        public static IEnumerable<T> ToEnumerable<T>(IDataReader reader) where T : new()
        {
            var binding = ModelBindingHelper.GetBinding(typeof(T), reader, string.Empty);
            var list = new List<T>();
            using (reader)
            {
                while (reader.Read())
                {
                    var obj = binding.GenericBind(new T());
                    list.Add(obj);
                }
            }
            return list;
        }

        /// <summary>
        /// 从reader读取数据并映射到范型T这里要求T必须有一个默认的无参构造函数
        /// 如：
        ///    var cmdt = @"select p.BlocNo AS B_BlocNo from S_UIFilterFieldPlan p left join S_UIFilterFields f on p.SUIFilterFieldID=f.SUIFilterFieldID";
        ///    var reader = DBHelper.ExecuteReader(cmdt, Provider);
        ///    var obj = DataMapping.ToModel(S_UIFilterFieldPlan)(reader, s => new Dictionary(object, string) { { s.UIFilterFields, "B_" } });
        /// </summary>
        /// <typeparam name="T">要映射的数据类型T</typeparam>
        /// <param name="reader">IDataReader</param>
        /// <param name="expression">指定有外键关系时外键表使用的别名的一个表达式</param>
        /// <returns>对个T实例的IEnumerable(T)枚举器</returns>
        //public static IEnumerable<T> ToEnumerable<T>(IDataReader reader, Expression<Func<T, object>> expression) where T : new()
        //{
           
        //    ModelBindingProxy proxy = new ModelBindingProxy(typeof(T), expression);
        //    proxy.InitBinding(reader);
        //    var list = new List<T>();

        //    using (reader)
        //    {
        //        while (reader.Read())
        //        {
        //            var obj = new T();
        //            list.Add((T)proxy.Bind(obj));
        //        }
        //    }
        //    return list;
        //}


        /// <summary>
        /// 从reader读取数据并映射到类型type的类这里要求Type必须有一个默认的无参构造函数并且返回指定类型的集合
        /// </summary>
        /// <typeparam name="TType">要返回的实际类型(TType)</typeparam>
        /// <param name="type">要映射的类型</param>
        /// <param name="reader">IdataReader</param>
        /// <returns>对个T实例的IEnumerable(T)</returns>
        public static IEnumerable<T> ToEnumerable<T>(Type type, IDataReader reader)
        {
            return (IEnumerable<T>)ToEnumerable(type, typeof(T), reader);
        }





        /// <summary>
        /// 从reader读取数据并映射到类型type的类这里要求Type必须有一个默认的无参构造函数并且返回指定类型的集合
        /// </summary>
        /// <param name="type">要映射的类型</param>
        /// <param name="convertType">要转换到的类型</param>
        /// <param name="reader">IdataReader</param>
        /// <returns>IEnumerable</returns>
        public static IEnumerable ToEnumerable(Type type, Type convertType, IDataReader reader)
        {
            var list = System.Activator.CreateInstance(typeof(List<>).MakeGenericType(convertType)) as IList;

            var binding = ModelBindingHelper.GetBinding(type, reader, string.Empty);

            using (reader)
            {
                while (reader.Read())
                {
                    var obj = ActivatorHelper.CreateInstance(type);
                    var o = binding.Bind(obj);

                    if (Cac.ContainsKey(type.FullName + convertType.FullName))
                    {
                        var co = Cac[type.FullName + convertType.FullName];
                        list.Add(co(obj));
                    }
                    else
                    {
                        var paramobj = Expression.Parameter(typeof(object), "obj");
                        var covert = Expression.Convert(paramobj, convertType);
                        var lambda = Expression.Lambda<Func<object, object>>(covert, paramobj);

                        var co = lambda.Compile();
                        Cac.Add(type.FullName + convertType.FullName, co);

                        list.Add(co(obj));
                    }

                }
            }

            return list;
        }



        /// <summary>
        /// 从reader读取数据并映射到类型type的类这里要求Type必须有一个默认的无参构造函数
        /// 也可以是Type可是是如List(Student) 这种类型 但是Student类型必须是有无参构造函数的类
        /// </summary>
        /// <param name="type">要映射的类型System.Type</param>
        /// <param name="reader">IDataReader</param>
        /// <param name="prefix"></param>
        /// <returns>IEnumerable</returns>
        public static IEnumerable ToEnumerable(Type type, IDataReader reader, string prefix = "")
        {
            
            IList list = null;
            Type modelType = null;
            bool isArry = false;

            //直接接收泛型类型
            if (type.IsGenericType && typeof(IList).IsAssignableFrom(type))
            {
                list = ActivatorHelper.CreateInstance(type) as IList;
                modelType = type.GetGenericArguments()[0];
            }
            else if(typeof(Array).IsAssignableFrom(type))
            {
                isArry = true;
                modelType = type.GetElementType();
            }
            else
            {
                list = ActivatorHelper.CreateInstance(typeof(List<>).MakeGenericType(type)) as IList;
                modelType = type;
            }

            var binding = ModelBindingHelper.GetBinding(modelType, reader, string.Empty);
            binding.Prefix = prefix;

            using (reader)
            {
                while (reader.Read())
                {
                    var obj = ActivatorHelper.CreateInstance(modelType);
                    var o = binding.Bind(obj);

                    list.Add(o);
                }
            }

            return list;
        }






        /// <summary>
        /// 从DataTable取数据并映射到范型T这里要求T必须有一个默认的无参构造函数
        /// </summary>
        /// <typeparam name="T">范型T</typeparam>
        /// <param name="table">表</param>
        /// <returns>对个T实例的IEnumerable(T)枚举器</returns>
        public static IEnumerable<T> ToEnumerable<T>(DataTable table) where T : new()
        {
            var binding = ModelBindingHelper.GetBinding(typeof(T), table, string.Empty);
            var list = new List<T>();

            foreach (DataRow row in table.Rows)
            {
                var obj = binding.GenericBind(new T(), row);
                list.Add(obj);
            }

            return list;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="row"></param>
        /// <param name="obj"></param>
        /// <param name="prefix"></param>
        /// <returns></returns>
        public static object ToModel(Type type, DataRow row, object obj, string prefix)
        {
            var binding = new ModelBinding(type, row, prefix);
            return binding.Bind(obj);
        }

        /// <summary>
        /// 从DataTable读取数据并映射到范型T这里要求T必须有一个默认的无参构造函数
        /// 如：
        ///    var cmdt = @"select p.BlocNo AS B_BlocNo from S_UIFilterFieldPlan p left join S_UIFilterFields f on p.SUIFilterFieldID=f.SUIFilterFieldID";
        ///    var table = DBHelper.ExecuteTable(cmdt, Provider);
        ///    var obj = DataMapping.ToModel(S_UIFilterFieldPlan)(table, s => new Dictionary(object, string） { { s.UIFilterFields, "B_" } });
        /// </summary>
        /// <typeparam name="T">要映射的数据类型T</typeparam>
        /// <param name="reader">System.Data.DataRow</param>
        /// <param name="expression">指定有外键关系时外键表使用的别名的一个表达式</param>
        /// <returns>映射之后的实例</returns>
        //public static IEnumerable<T> ToEnumerable<T>(DataTable table, Expression<Func<T, object>> expression) where T : new()
        //{
        //    ModelBindingProxy proxy = new ModelBindingProxy(typeof(T), expression);
        //    proxy.InitBinding(table);

        //    var list = new List<T>();

        //    foreach (DataRow row in table.Rows)
        //    {
        //        var obj = (T)proxy.Bind(new T(), row);
        //        list.Add(obj);
        //    }

        //    return list;
        //}

    }

    /// <summary>
    /// DataReader扩展类
    /// </summary>
    public static class IDataReaderExtension
    {
        /// <summary>
        /// 重IDataReader中获取指定列的数据集合的方法读取完后Reader会自动关闭
        /// </summary>
        /// <typeparam name="T">指定列的实际数据类型</typeparam>
        /// <param name="reader">System.Data.IDataReader</param>
        /// <param name="columnName">列名称</param>
        /// <returns>指定列的数据集合</returns>
        public static IList<T> GetAssignColumnValues<T>(this IDataReader reader,string columnName)
        {
            IList<T> list = new List<T>();
            if (reader == null) return list;

            using (reader)
            {
                while (reader.Read())
                {
                    var obj = (T)reader[columnName];
                    list.Add(obj);
                }
            }

            return list;
        }


        public static T ToModel<T>(this IDataReader reader,Func<IDataReader,T> func)
        {
            using (reader)
            {
                if (reader.Read())
                {
                    return func(reader);
                }
            }
            return default(T);
        }

        public static IList<T> ToList<T>(this IDataReader reader, Func<IDataReader, T> func)
        {
            var list = new List<T>();
            using (reader)
            {
                while (reader.Read())
                {
                    var obj = func(reader);
                    list.Add(obj);
                }
            }
            return list;
        }





        /// <summary>
        /// 获取IDataReader中指定列名的值使用该方法可以避免IDataReader[column]为DbNull和Null的情况
        /// </summary>
        /// <typeparam name="T">返回的数据T</typeparam>
        /// <param name="reader">IDataReader</param>
        /// <param name="column">列名称</param>
        /// <param name="func">这个委托用来做自定义类型转换</param>
        /// <returns>数据T</returns>
        public static T GetValue<T>(this IDataReader reader, string column, Func<object, T> func = null)
        {
            var obj = reader[column];

            if (obj == null || obj == DBNull.Value)
            {
                return default(T);
            }
            if (func != null)
            {
                return func(obj);
            }
            else
            {
                if (typeof(T).IsEnum)
                {
                    return (T)Enum.Parse(typeof(T), obj.ToString());
                }

                return (T)Convert.ChangeType(obj, typeof(T));
            }
        }

    }

}


