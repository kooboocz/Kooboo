//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Lib.Reflection
{
  public static class Dynamic
    {
        private static object _lock = new object();

        public static object GetObjectMember(object obj, string FullPropertyName)
        {
            if (obj == null)
            {
                return null;
            }

            if (string.IsNullOrEmpty(FullPropertyName))
            {
                return obj;
            }

            var dest = obj;

            var SubProperties = FullPropertyName.Split('.');

            foreach (var item in SubProperties)
            {
                if (dest == null)
                {
                    return null;
                }
                Type objtype = dest.GetType();

                string key = objtype.Name + "." + item;

                if (!GetValueFuncs.ContainsKey(key))
                {
                    lock (_lock)
                    {
                        if (!GetValueFuncs.ContainsKey(key))
                        {
                            var GetValueFunc = Reflection.TypeHelper.GetGetObjectValue(item, objtype);
                            GetValueFuncs[key] = GetValueFunc;
                        }
                    }
                }

                var function = GetValueFuncs[key];

                if (function == null)
                {
                    return null;
                }
                else
                {
                    dest = function(dest);
                }
            }

            return dest;

        }

        private static Dictionary<string, Func<object, object>> GetValueFuncs = new Dictionary<string, Func<object, object>>();
          
    }
}
