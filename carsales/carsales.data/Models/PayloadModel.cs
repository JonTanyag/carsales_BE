using System;
using System.Collections.Generic;
using System.Text;

namespace carsales.data.Models
{
    public class PayloadModel
    {
        public string Customer { get; set; }
        public CarType CarType { get; set; }
        public bool speaksGreek { get; set; }
    }
}
