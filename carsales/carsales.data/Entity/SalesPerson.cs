using carsales.data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace carsales.data.Entity
{
    public class SalesPerson
    {
        public string Name { get; set; }
        public string[] Groups { get; set; }
        public bool isAvailable { get; set; }
    }
}
