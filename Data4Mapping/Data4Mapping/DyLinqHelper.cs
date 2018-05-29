using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Winsoft.Helper
{
    public static class DyLinqHelper
    {
        /// <summary>
        /// 移除
        /// </summary>
        /// <typeparam name="T">泛型T</typeparam>
        /// <param name="linq">Winsoft.DyLinq(T)</param>
        /// <param name="table">表名称</param>
        /// <param name="column">字段名称</param>
        public static void RemoveSelectNode<T>(DyLinq<T> linq, string table, string column)
        {
            var collection = linq._dytokens[DyToken.Select];
            var type = typeof(T).Assembly.GetTypes().FirstOrDefault(s => s.Name == table);
            if (type != null)
            {
                RomverTranslator translator = new RomverTranslator(type);

                foreach (var item in collection)
                {
                    var flag = translator.Translate(item, column);
                    if (flag) break;
                }
            }
        }
    }
}
