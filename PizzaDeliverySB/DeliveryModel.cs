using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaDeliverySB
{
    class DeliveryModel
    {

        [DisplayName("Customer Name")]
        public string CustomerName { get; set; }

        [DisplayName("Product Name")]
        public string ProductName { get; set; }
    }
}
