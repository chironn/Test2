using DES.Core.Interfaces;

namespace DES.Converts.BYDQService.Entity
{
    public class FromIncomBufferEntity : ILogicEntity
    {
        #region Implementation of ILogicEntity

        public string FindKey { get; set; }

        #endregion

        public ushort RoutingKey { get; set; }

        public byte[] Buffer { get; set; }
    }
}
