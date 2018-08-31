using DES.Core.Interfaces;
using DES.Entities.BYDQ;

namespace DES.Converts.BYDQService.Entity
{
    /// <summary>
    /// 
    /// </summary>
    public class FromInComEntity : ILogicEntity
    {
        #region Implementation of ILogicEntity

        public string FindKey { get; set; }

        #endregion

        public BaseEntity RealEntity { get; set; }
    }
}
