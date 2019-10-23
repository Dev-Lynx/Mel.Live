using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mel.Live.Extensions
{
    public static class TypeExtensions
    {
        #region Methods
        public static bool IsAssignableTo(this Type type, Type assignableType) =>
            assignableType.IsAssignableFrom(type);

        public static bool IsAssignableTo<TAssignable>(this Type type) =>
            IsAssignableTo(type, typeof(TAssignable));
        #endregion
    }
}
