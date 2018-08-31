/* ==============================================================================
* 类型名称：OperationResult  
* 类型描述：
* 创 建 者：linfk
* 创建日期：2017/12/4 10:22:44
* =====================
* 修改者：
* 修改描述：
# 修改日期
* ==============================================================================*/

namespace DES.DbCaches.Entities
{
    /// <summary>
    /// 操作结果
    /// </summary>
    public class OperationResult
    {

        /// <summary>
        /// 操作是否成功
        /// </summary>
        public bool Successed { get; protected set; }

        /// <summary>
        /// 错误消息
        /// </summary>
        public string ErrorMessage { get; protected set; }

        /// <summary>
        /// 返回错正确状态
        /// </summary>
        /// <returns>更新对象自己为正确状态并返回自己</returns>
        public OperationResult True()
        {
            Successed = true;
            return this;
        }

        /// <summary>
        /// 返回错错误状态,避免字符串频繁创建影响内存
        /// </summary>
        public OperationResult False()
        {
            Successed = false;
            return this;
        }
        /// <summary>
        /// 返回错错误状态
        /// </summary>
        /// <returns>更新对象自己为错误状态并返回自己</returns>
        public OperationResult False(string message)
        {
            False();
            ErrorMessage = message;
            return this;
        }
    }

    /// <summary>
    /// 带返回参数的操作结果
    /// </summary>
    /// <typeparam name="TData">返回参数类型</typeparam>
    public class OperationResult<TData> : OperationResult
    {

        public TData Data { get; protected set; }

        /// <summary>
        /// 返回错正确状态
        /// </summary>
        /// <returns>更新对象自己为正确状态并返回自己</returns>
        public OperationResult<TData> True(TData data)
        {
            True();
            Data = data;
            return this;
        }
        /// <summary>
        /// 返回错错误状态
        /// </summary>
        /// <param name="message">错误消息</param>
        /// <returns>更新对象自己为错误状态并返回自己</returns>
        public new OperationResult<TData> False(string message)
        {
            False();
            ErrorMessage = message;
            return this;
        }

        /// <summary>
        /// 返回错错误状态,避免字符串频繁创建影响内存
        /// </summary>
        /// <returns>更新对象自己为错误状态并返回自己</returns>
        public new OperationResult<TData> False()
        {
            Successed = false;
            return this;
        }
    }
}
