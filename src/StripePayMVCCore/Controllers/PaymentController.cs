using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Stripe;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace StripePayMVCCore.Controllers
{
    public class PaymentController : Controller
    {
        // GET: /<controller>/
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Charge(string stripeEmail, string stripeToken)
        {
            StripeConfiguration.SetApiKey("sk_test_aJpRlSnGboF0waG5ROZRGbJM");
            var customers = new StripeCustomerService();
            var charges = new StripeChargeService();
            var orders = new StripeOrderService();
            var products = new StripeProductService();
            var SKUs = new StripeSkuService();

            var customer = customers.Create(new StripeCustomerCreateOptions
            {
                Email = stripeEmail,
                SourceToken = stripeToken
            });

            var customerId = customer.Id;

            var product = products.Create(new StripeProductCreateOptions
            {
                 Active = true,
                 Name = "Jacket Armani",
                 Attributes = new string[] { "size", "gender", "color" },
                 Description = "Super awesome, one-of-a-kind t-shirt"
            });

            var SKU = SKUs.Create(new StripeSkuCreateOptions
            {
                Product = product.Id,
                Attributes = new Dictionary<string, string>()
                {
                    { "size", "Medium" },
                    { "gender", "Female" },
                    { "color", "Black" }
                },
                Price = 456*100,
                Currency = "usd",
                Inventory = new StripeInventoryOptions
                {
                    Quantity = 500,
                    Type = "finite"
                }               
            });

            var order = orders.Create(new StripeOrderCreateOptions
            {
                 Currency = "usd",
                 CustomerId = customerId,
                 Email = stripeEmail,
                 Items = new List<StripeOrderItemOptions>
                 {
                     new StripeOrderItemOptions { Amount = 45600, Currency = "usd", Description = "Jacket Armani", Quantity = 3, Type = "sku", Parent = SKU.Id  }                     
                 } ,
                 Shipping = new StripeShippingOptions {  Country = "USA", CityOrTown= "New York", Name= "Shrikant S", State="New York", PostalCode="10001", Phone="12121212",
                 Line1 = "T1 Block Street", Line2 = "T2 Steet"}                  
            });

            var charge = charges.Create(new StripeChargeCreateOptions
            {
                Amount = 456,
                Description = "Fashion Payment",
                Currency = "usd",
                CustomerId = customer.Id
            });

           
            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
