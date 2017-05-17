using System;
using System.Runtime.Serialization;
using AKB.Common.Data.Attr;

namespace AKB.Test.Model
{
    [Serializable]
    [TableAttr("tbl_binary")]
    public class TblBinaryModel
    {
        [ColumnAttr(Name = "id", UsingForInsert = false)]
        public uint Id { get; set; }

        [ColumnAttr(Name = "name")]
        public string Name { get; set; }

        [ColumnAttr(Name = "data")]
        public byte[] Data { get; set; }
    }
}
