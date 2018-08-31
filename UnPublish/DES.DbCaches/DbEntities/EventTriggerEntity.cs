using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DES.DbCaches.Entities;

namespace DES.DbCaches.DbEntities
{
    public class EventTriggerEntity : BYDQBaseEntity
    {
        public string UUID { get; set; }

        public string DbDT { get; set; }

        public int EventCode { get; set; }
    }
}
