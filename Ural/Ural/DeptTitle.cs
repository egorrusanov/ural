using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ural
{
    public class DeptTitle
    {
        public dataService data { get; set; }
    }

    public class dataService
    {
        public serviceStruct r_state_structure { get; set; }
    }

    public class serviceStruct
    {
        public string title { get; set; }
    }
}
