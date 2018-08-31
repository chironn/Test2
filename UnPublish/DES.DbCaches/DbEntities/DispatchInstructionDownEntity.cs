using DES.DbCaches.Entities;
using System;

namespace DES.DbCaches.DbEntities
{
    public class DispatchInstructionDownEntity : BYDQBaseEntity
    {
        public DispatchInstructionDownEntity()
        {
            FunctionNumber = 3008;
        }
        public string UUID { get; set; }
        public DateTime Time1 { get; set; }
        public DateTime Time2 { get; set; }
        public string Path { get; set; }
    }
}
