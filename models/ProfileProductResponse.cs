using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace models
{
    public class ProfileProductResponse
    {
        public Profile Profile { get; set; }
        public List<Product> ProfileProducts { get; set; }
    }
}
