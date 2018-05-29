using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using System.Reflection;

namespace Data4Mapping
{
    /// <summary>
    /// ModelBinding代理类
    /// </summary>
    public class ModelBindingProxy
    {
        private Type _type;
        private Func<string, string> _func;
        private ModelBinding _binding;
        private Expression _expression;

        private Dictionary<MemberInfo, string> _list = new Dictionary<MemberInfo, string>();

        /// <summary>
        /// 实体绑定辅助代理类
        /// </summary>
        /// <param name="type">要进行绑定的类型Type</param>
        public ModelBindingProxy(Type type): this(type, t => string.Empty)
        {

        }

        /// <summary>
        /// 要进行绑定的类型Type
        /// </summary>
        /// <param name="type">要进行绑定的类型Type</param>
        /// <param name="func">一个委托用于处理别名问题</param>
        public ModelBindingProxy(Type type, Func<string, string> func)
        {
            this._type = type;
            this._func = func;
        }



        internal ModelBindingProxy(Type type, LambdaExpression expression)
        {
            _type = type;
            _expression = expression;
            var lambda = expression as LambdaExpression;
            if (lambda == null) throw new Exception("错误的表达式只能是LambdaExpression");
            MBETranslator translator = new MBETranslator();
            _func = translator.Translate(expression);
        }

        internal void InitBinding(IDataReader reader)
        {
            _binding = new ModelBinding(_type, reader, _func);
        }

        internal void InitBinding(DataTable table)
        {
            _binding = new ModelBinding(_type, table, _func);
        }

        internal object Bind(object obj)
        {
            return _binding.Bind(obj);
        }

        internal object Bind(object obj,DataRow row)
        {
            return _binding.Bind(obj, row);
        }

    }
}
