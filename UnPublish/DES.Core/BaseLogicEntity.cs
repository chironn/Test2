/* ==============================================================================
* 类型名称：BaseLogicEntity  
* 类型描述：
* 创 建 者：linfk
* 创建日期：2017/11/29 11:55:51
* =====================
* 修改者：
* 修改描述：
# 修改日期
* ==============================================================================*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DES.Core.Interfaces;

namespace DES.Core
{
    /// <summary>
    /// 
    /// </summary>
    public class BaseLogicEntity<TEntity> : ILogicEntity
    {
        public string FindKey { get; set; }

        public TEntity Entity { get; set; }
    }
}
