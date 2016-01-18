using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ural
{
    public class ServicesList
    {
        public dataServices data { get; set; }
    }

    public class dataServices
    {
        public List<servicesStruct> list { get; set; }
        public string count { get; set; }
    }

    public class servicesStruct
    {
        public string full_title { get; set; }
        public string url { get; set; }
        public string short_title { get; set; }
        public string dept { get; set; }
        public string level_title { get; set; }
    }
}
