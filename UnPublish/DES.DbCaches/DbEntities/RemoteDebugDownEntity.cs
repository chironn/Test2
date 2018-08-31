using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DES.DbCaches.Entities;

namespace DES.DbCaches.DbEntities
{
    public class RemoteDebugDownEntity:BYDQBaseEntity
    {
        public RemoteDebugDownEntity()
        {
            FunctionNumber = 7002;
        }
        public string UUID { get; set; }
        public int CNT { get; set; }
        public int SEQ { get; set; }
        public int DiagnosticDataType { get; set; }
        public int SecurityAuthentication { get; set; }
        public int DiagnosticMode { get; set; }
        public int SendCANID { get; set; }
        public int RecCANID { get; set; }
        public int FrameType { get; set; }
        public int KeyK { get; set; }
        public int SIDCNT { get; set; }
        public byte[] DiagnosticDataList { get; set; }
    }
}
