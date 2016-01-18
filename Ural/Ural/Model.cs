using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ural
{
    public class Model
    {
        public data data { get; set; }

    }

    public class data
    {
        public List<procedures1> procedures { get; set; }
    }

    public class procedures1
    {
        public string serviceTitle { get; set; }
        public string serviceId { get; set; }
        public string procedureTitle { get; set; }
        public string deptTitle { get; set; }
        public string deptId { get; set; }
        public string procedureId { get; set; }
    }
}
