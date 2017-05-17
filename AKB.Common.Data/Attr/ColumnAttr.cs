using System;

namespace AKB.Common.Data.Attr
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ColumnAttr : Attribute
    {
        public ColumnAttr()
        {
            UsingForInsert = true;
            UsingForUpdate = true;
            UsingForDelete = true;
            UsingForSelect = true;
        }

        public string Name { get; set; }

        public bool UsingForInsert { get; set; }
        public bool UsingForUpdate { get; set; }
        public bool UsingForDelete { get; set; }
        public bool UsingForSelect { get; set; }
    }
}
