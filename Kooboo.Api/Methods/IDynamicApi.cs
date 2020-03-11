//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Api.Methods
{
  public interface  IDynamicApi
    {
        DynamicApi GetMethod(string name);  
    }

    public class DynamicApi
    {
        public Type Type { get; set; }

        public MethodInfo Method { get; set; }
    }
}
