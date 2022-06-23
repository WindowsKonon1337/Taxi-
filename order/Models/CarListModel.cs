using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace order.Models
{
    public class CarListModel
    {
        public string Model { get; set; }
        public decimal Cost { get; set; }
        public decimal CosPerMounth { get; set; }
        public decimal Fuel { get; set; }
    }
}
