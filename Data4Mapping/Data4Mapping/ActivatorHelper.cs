using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Data4Mapping
{
    internal class ActivatorHelper
    {
        /// <summary>
        /// 用以存储使创建类实例的委托
        /// </summary>
        private static readonly IDictionary<string, object> Cache = new Dictionary<string, object>();


        /// <summary>
        /// 根据一个类型并使用参数创建一个类型实例
        /// </summary>
        /// <param name="type">要创建实例的类型</param>
        /// <param name="arguments">要使用的参数</param>
        /// <returns></returns>
        public static object CreateInstance(Type type, params object[] arguments)
        {
            var key = type.Name;
            var types = Type.EmptyTypes;
            if (arguments != null)
            {
                types = new Type[arguments.Length];
                for (int i = 0; i < arguments.Length; i++)
                {
                    types[i] = arguments[i].GetType();
                    key = key + "|" + types[i].Name;
                }
            }

            //find in cache
            if (Cache.ContainsKey(key))
            {
                return (Cache[key] as Func<object[], object>)(arguments);
            }


            var constructor = type.GetConstructor(types);

            var parms = constructor.GetParameters();

            var exps = new List<Expression>();

            LabelTarget target = Expression.Label(typeof(object));
            var args = Expression.Parameter(typeof(object[]), "arguments");
            for (int i = 0; i < parms.Length; i++)
            {
                var method = typeof(object[]).GetMethod("GetValue", new[] { typeof(int) });
                MethodCallExpression med = Expression.Call(args, method, Expression.Constant(i));
                var c = Expression.Convert(med, parms[i].ParameterType);
                exps.Add(c);
            }
            var nexpression = Expression.New(constructor, exps);
            GotoExpression ret = Expression.Return(target, nexpression);
            LabelExpression lbl = Expression.Label(target, Expression.Constant(null));
            var block = Expression.Block(ret, lbl);

            var lam = Expression.Lambda<Func<object[], object>>(block, args);

            var del = lam.Compile();

            Cache.Add(key, del);

            return del(arguments);
        }



        /// <summary>
        /// 创建一个带参数的泛型T的实例的方法
        /// </summary>
        /// <typeparam name="T">要创建的泛型T</typeparam>
        /// <param name="arguments">参数</param>
        /// <returns>泛型实例T</returns>
        public static T CreateInstance<T>(params object[] arguments)
        {
            return (T)CreateInstance(typeof(T), arguments);
        }

        /// <summary>
        /// 根据类型创建实例
        /// </summary>
        /// <param name="type">要创建的实力类型</param>
        /// <returns>实例</returns>
        public static object CreateInstance(Type type)
        {
            var key = type.FullName;
            if (Cache.ContainsKey(key))
            {
                return (Cache[key] as Func<object>)();
            }

            var instance = Expression.New(type);
            var func = Expression.Lambda<Func<object>>(instance, null).Compile();
            Cache.Add(key, func);
            return func();
        }


        /// <summary>
        /// 创建一个泛型T的实例类型(必须有无参的构造函数)
        /// </summary>
        /// <typeparam name="T">要创建实例的泛型T</typeparam>
        /// <returns>泛型T实例</returns>
        public static T CreateInstance<T>()
        {
            return (T)CreateInstance(typeof(T));
        }
    }
}
