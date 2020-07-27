using muscshop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace muscshop.ViewModel
{
    public class ShoppingCartViewModel
    {
        public List<Cart> CartItems { get; set; }

        public decimal CartTotal { get; set; }


    }
}