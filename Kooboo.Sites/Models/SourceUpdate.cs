//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Sites.Models
{
    public class SourceUpdate
    {
        public int StartIndex { get; set; }
        public int EndIndex { get; set; }
        public string NewValue { get; set; }
    }
}
