using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DES.DbCaches.Entities;

namespace DES.DbCaches.DbEntities
{
    public class RemoteDebugUPEntity:BYDQBaseEntity
    {
        public RemoteDebugUPEntity()
        {
            FunctionNumber = 7002;
        }
        public string UUID { get; set; }
        public int CNT { get; set; }
        public int SEQ { get; set; }
        public int DiagnosticResult { get; set; }
        public byte[] DiagnosticData { get; set; }
        public string DiagnosticDataToHex { get; set; }
    }
}
