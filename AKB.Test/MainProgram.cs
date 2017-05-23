using System;
using System.Collections.Generic;
using System.IO;
using AKB.Common.Data;
using AKB.Test.Model;

namespace AKB.Test
{
    internal class MainProgram
    {
        private static readonly IDbHelper _dbHelper = FactoryProducer.GetFactory("DB").GetDbHelper(ConfigHelper.GetDbHelperName());

        private static void Main(string[] args)
        {
            //InsertBlob();
            TestSelect();
        }

        private static void InsertBlob()
        {
            var filePath = @"D:\setup\android-studio-bundle-162.3871768-windows.exe";
            filePath = @"D:\setup\sqldeveloper-4.2.0.16.356.1154-no-jre.zip";
            var contentFile = File.ReadAllBytes(filePath);

            var obj = new TblBinaryModel();
            obj.Name = "File 1";
            obj.Data = contentFile;
            var lst = new List<TblBinaryModel>
            {
                obj
            };
            _dbHelper.InsertBatch(lst);
            Console.WriteLine("Insert done...");
        }

        private static void TestSelect()
        {
            var dt = _dbHelper.GetTable("select * From TTKhachHang");
            var b = "";
        }

    }
}
