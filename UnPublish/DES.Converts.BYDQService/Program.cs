using DES.Core;
using DES.Entities.BYDQ;

namespace DES.Converts.BYDQService
{
    class Program
    {
        static void Main(string[] args)
        {
            //绑定父类与子类序列化关系
            BaseEntity.RegisteredProtoBuf();
            Shell.Run(new MainService());
        }
    }
}
