using System.Text;

namespace Dythervin.ObjectPool
{
    public static class SharedPools
    {
        public static readonly ObjectPoolAuto<StringBuilder> StringBuilder =
            new ObjectPoolAuto<StringBuilder>(onRelease: builder => builder.Clear(), collectionCheckDefault: false);
    }
}