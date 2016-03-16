using System;
using System.Collections.Generic;
using System.Linq;

namespace Svyaznoy.Core.Web
{
    public static class Compositor
    {
        private readonly static Dictionary<Type, Object>  Container = new Dictionary<Type, object>();


        public static void Add<T>(T instance)
            where T: class
        {
            Container[typeof (T)] = instance;
        }

        public static T Get<T>()
            where T : class
        {
            var type = typeof (T);
            if (Container.ContainsKey(type))
            {
                return Container[type] as T;
            }
            else
            {
                return null;
            }
        }
    }
}