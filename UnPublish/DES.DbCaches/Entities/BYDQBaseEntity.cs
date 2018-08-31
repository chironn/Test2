using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DES.DbCaches.Entities
{
    public class BYDQBaseEntity
    {
        public int Encryption { get; set; }

        public int Type { get; set; }

        public int CMD { get; set; }

        public int FunctionNumber { get; set; }

        public int ResponseSign { get; set; }

        public int FunctionStatus { get; set; }

        public string UniqueIdentity { get; set; }

        public int ProductType { get; set; }

        public int CloudProductCode { get; set; }

        public string TimeStamp { get; set; }

        public DateTime Time { get; set; }

    }
}
