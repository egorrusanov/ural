using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ural
{
    public class ListId
    {
        public dataId data { get; set; }

    }

    public class dataId
    {
        public List<list> list { get; set; }
    }

    public class list
    {
        public string id { get; set; }
        public string service_id { get; set; }
        public string title { get; set; }
    }
}
