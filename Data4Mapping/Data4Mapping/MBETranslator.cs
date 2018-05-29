using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Data4Mapping
{
    internal class MBETranslator : ExpressionVisitor
    {

        private ParameterExpression _pa;

        private LabelTarget _labelTarget;

        private List<Expression> _arry = new List<Expression>();

        public MBETranslator()
        {
            _pa = Expression.Parameter(typeof(string), "value");

            _labelTarget = Expression.Label(typeof(string));

        }

        protected override Expression VisitMember(MemberExpression node)
        {
            return Expression.Constant(node.Member.Name);
        }



        protected override Expression VisitListInit(ListInitExpression node)
        {
            foreach (var item in node.Initializers)
            {

                var eq = Expression.Equal(this.Visit(item.Arguments[0]), _pa);

                var ife = Expression.IfThen(eq, Expression.Return(_labelTarget, item.Arguments[1]));

                _arry.Add(ife);

            }

            return node;
        }

        public Func<string, string> Translate(Expression expression)
        {
            this.Visit(expression);

            _arry.Add(Expression.Label(_labelTarget, Expression.Constant(String.Empty)));

            BlockExpression blocks = Expression.Block(_arry);

            var lambda = Expression.Lambda<Func<string, string>>(blocks, new ParameterExpression[] { _pa });

            return lambda.Compile();
        }


    }
}
