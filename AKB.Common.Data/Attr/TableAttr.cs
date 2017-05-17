using System;

namespace AKB.Common.Data.Attr
{
    [AttributeUsage(AttributeTargets.Class)]
    public class TableAttr : Attribute
    {
        public string Name { get; set; }

        public TableAttr(string name)
        {
            Name = name;
        }
    }
}
