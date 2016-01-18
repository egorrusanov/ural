using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ural
{
    public class ListForms
    {
        public dataForm data { get; set; }

    }

    public class dataForm
    {
        public List<forms> list { get; set; }
    }

    public class forms
    {
        public string id { get; set; }
        public string title { get; set; }

        public List<procedures> procedures { get; set; }
    }

    public class procedures
    {
        public string serviceTitle { get; set; }
        public string serviceId { get; set; }
        public string procedureTitle { get; set; }
        public string deptTitle { get; set; }
        public string procedureId { get; set; }

    }
}
