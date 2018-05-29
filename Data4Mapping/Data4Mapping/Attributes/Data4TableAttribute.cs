using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Data4Mapping
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class Data4TableAttribute : Data4BaseAttribute
    {
        public string Name { get; set; }

        public Data4TableAttribute() :this(string.Empty){ }

        public Data4TableAttribute(string name) 
        {
            Name = name;
        }

    }
}
