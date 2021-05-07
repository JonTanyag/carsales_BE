using System;
using System.Collections.Generic;
using System.Text;

namespace carsales.data.Models
{
    public class SalesPersonModel
    {
        public string Name { get; set; }
        public List<GroupModel> Groups { get; set; }
        public bool isAvailable { get; set; }
    }
}
