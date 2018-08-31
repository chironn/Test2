using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DES.DbCaches.Entities;

namespace DES.DbCaches.DbEntities
{
    public class DataMonitorEntity : BYDQBaseEntity
    {
        public string UUID { get; set; }
        public int SignType { get; set; }
        public int DbCNT { get; set; }
        public int DbSEQ { get; set; }
        public byte[] CanMessageData { get; set; }
    }
}
