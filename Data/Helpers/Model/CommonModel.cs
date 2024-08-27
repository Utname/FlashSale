using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Helpers.Model
{
    public class CommonModel
    {
        public int id { get; set; }
        public string name { get; set; }
    }

    public class CommonModelRef
    {
        public string id { get; set; }
        public string name { get; set; }
    }

    public class CommonModelImage
    {
        public int id { get; set; }
        public string name { get; set; }
        public string image { get; set; }
    }

    public class CommonModelImageCover : CommonModelImage
    {
        public string coverImage { get; set; }

    }
}
