using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClassLibrary.BusinessObject
{
    public class DatabaseConfig
    {
        public Dictionary<string, DatabaseDetail> Database { get; set; }
        public class DatabaseDetail
        {
            public string ConnectionStrings { get; set; }
            public int? DatabaseProvider { get; set; }
        }
    }
}