using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Linq;
using System.Linq.Expressions;

namespace Data4Mapping
{

    /// <summary>
    /// 模型绑定类
    /// </summary>
    public class ModelBinding
    {
        private Type _type;
        private readonly string[] _columns;
        internal object _reader;
        private string _prefix = string.Empty;


        internal Func<string, string> _func;
        internal DataRow BindingRow { get; private set; }

        internal string Prefix { get; set; }

        private readonly IDictionary<string, PropertySetter> _dictionary;

        private static readonly IDictionary<string, IDictionary<string, PropertySetter>> Cache = new Dictionary<string, IDictionary<string, PropertySetter>>();

        internal ModelBinding(Type type, object reader):this(type,reader,s=>string.Empty)
        {
            
        }


        internal ModelBinding(Type type, object reader, Func<string, string> func):this(type, reader, string.Empty)
        {
            _func = func;

        }


        internal ModelBinding(Type type, object reader, string prefix)
        {
            _type = type;
            _reader = reader;
            _prefix = prefix;
            var count = 0;
            if (reader is IDataReader)
            {
                count = ((IDataReader)reader).FieldCount;
                _columns = new string[count];
                for (var i = 0; i < count; i++)
                {
                    _columns[i] = ((IDataReader)reader).GetName(i).Trim();
                }
            }
            else if (reader is DataTable)
            {
                count = ((DataTable)reader).Columns.Count;
                _columns = new string[count];
                for (var i = 0; i < count; i++)
                {
                    _columns[i] = ((DataTable)reader).Columns[i].ColumnName.Trim();
                }
            }
            else if(reader is DataRow)
            {
                count = ((DataRow)reader).Table.Columns.Count;
                _columns = new string[count];
                for (var i = 0; i < count; i++)
                {
                    _columns[i] = ((DataRow)reader).Table.Columns[i].ColumnName.Trim();
                }
            }
            else
            {
                throw new NotSupportedException(reader.GetType().Name);
            }

            var key = type.FullName + "|" + reader.GetType().Name + "|" + string.Join("$", _columns) + "|Prefix|" + _prefix.ToUpper();

            //如果缓存中已经有对应的PropertySetter字典则使用缓存
            if (Cache.ContainsKey(key))
            {
                _dictionary = Cache[key];
            }
            else //否则初始化一些应该有的数据
            {
                _dictionary = new Dictionary<string, PropertySetter>();
                var properties = type.GetProperties();
                foreach (var item in properties) GetAttribute(item);
                Cache[key] = _dictionary;
            }
        }


        /// <summary>
        /// 对一个实例进行绑定赋值
        /// </summary>
        /// <param name="obj">要绑定赋值的实例</param>
        /// <returns>赋值后的实例</returns>
        public object Bind(object obj)
        {
            foreach (var column in _columns)
            {
                if (!_dictionary.ContainsKey(column)) continue;
                var item = _dictionary[column];
                if (string.IsNullOrWhiteSpace(Prefix))
                {
                    Prefix = string.Empty;
                }
                if (_reader is IDataReader)
                {
                    item.SetValue(obj, ((IDataReader)_reader)[column]);
                }
                else if (_reader is DataRow)
                {
                    item.SetValue(obj, ((DataRow)_reader)[column]);
                }
            }
            return obj;
        }



        /// <summary>
        ///  对一个实例进行绑定赋值
        /// </summary>
        /// <param name="obj">要绑定赋值的实例</param>
        /// <param name="row">System.Data.Row</param>
        /// <returns>赋值后的实例</returns>
        public object Bind(object obj,DataRow row)
        {
            foreach (var column in _columns)
            {
                if (!_dictionary.ContainsKey(column)) continue;
                var item = _dictionary[column];
                item.Binding.BindingRow = row;
                if (string.IsNullOrWhiteSpace(Prefix))
                {
                    Prefix = string.Empty;
                }
                item.SetValue(obj, row[Prefix + column]);
            }
            return obj;
        }

        /// <summary>
        /// 对一个范型T绑定赋值
        /// </summary>
        /// <typeparam name="T">要绑定赋值的范型</typeparam>
        /// <param name="obj">对应范型的实例</param>
        /// <returns>赋值后的范型T实例</returns>
        public T GenericBind<T>(T obj)
        {
            return (T)Bind(obj);
        }

        /// <summary>
        /// 对一个范型T绑定赋值
        /// </summary>
        /// <typeparam name="T">要绑定赋值的范型</typeparam>
        /// <param name="obj">对应范型的实例</param>
        /// <param name="row">System.Data.Row</param>
        /// <returns>赋值后的范型T实例</returns>
        public T GenericBind<T>(T obj,DataRow row)
        {
            return (T)Bind(obj, row);
        }




        private void GetAttribute(PropertyInfo property)
        {
            if (!property.CanWrite) return;

            if (property.PropertyType == typeof(IEnumerable) || property.PropertyType == typeof(IEnumerable<>)) return;

            var attr = ReflectionHelper.FindAttribute<Data4ColumnAttribute>(property);

            if (attr == null)
            {
                var setter = new PropertySetter(property, _type, property.Name, _reader,this);
                _dictionary.Add((_prefix + property.Name).Trim(), setter);
            }
            else if (attr.Enable)
            {
                var name = string.IsNullOrEmpty(attr.ColumnName) ? property.Name : attr.ColumnName;
                var setter = new PropertySetter(property, _type, name, _reader,this);
                _dictionary.Add((_prefix + name).Trim(), setter);
            }
  

        }

    }
}
