﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ImageCRUD.Models;
using Microsoft.EntityFrameworkCore;
using ImageCRUD.Utility;

namespace ImageCRUD.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _db;

        public HomeController(ILogger<HomeController> logger,ApplicationDbContext db)
        {
            _logger = logger;
             _db = db;
        }

        public IActionResult Index()
        {
            HomeVM homeVM = new HomeVM()
            {
                products = _db.Product.Include(u => u.Category),
                categories = _db.Category
            };
            return View(homeVM);
        }
       
        public IActionResult filter(string category)
        {
            var product = _db.Product.Where(u => u.Category.category_name == category);
            HomeVM homeVM = new HomeVM()
            {
                products = product.Include(u => u.Category),
                categories = _db.Category
            };
            return View("Index",homeVM);
        }
        public IActionResult Details(int id)
        {
            List<ShoppingCart> ShoppingCartList = new List<ShoppingCart>();
            if (HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.ShoppingCart) != null &&
                HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.ShoppingCart).Count() > 0)
            {

                ShoppingCartList = HttpContext.Session.Get<List<ShoppingCart>>(WC.ShoppingCart);
            }

            DetailVM DetailVM = new DetailVM()
            {

                Product = _db.Product.Include(u => u.Category).Where(u => u.product_id == id).FirstOrDefault(),
                ExistsInCart = false

            };
            foreach(var obj in ShoppingCartList)
            {
                if (obj.ProductId == DetailVM.Product.product_id)
                {
                    DetailVM.ExistsInCart = true;
                }
            }
            return View(DetailVM);
        }
        [HttpPost]
        [ActionName("Details")]
        public IActionResult DetailsPOST(int id)
        {
            List<ShoppingCart> ShoppingCartList = new List<ShoppingCart>();
            if(HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.ShoppingCart) != null &&
                HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.ShoppingCart).Count()>0)
            {

                ShoppingCartList = HttpContext.Session.Get<List<ShoppingCart>>(WC.ShoppingCart);
            }
            ShoppingCartList.Add(new ShoppingCart { ProductId = id });
            HttpContext.Session.Set(WC.ShoppingCart, ShoppingCartList);
            return RedirectToAction(nameof(Index));
        }
        
        public IActionResult RemoveFromCart(int id)
        {
            List<ShoppingCart> ShoppingCartList = new List<ShoppingCart>();
            if (HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.ShoppingCart) != null &&
                HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.ShoppingCart).Count() > 0)
            {

                ShoppingCartList = HttpContext.Session.Get<List<ShoppingCart>>(WC.ShoppingCart);
            }
            var objToRemove = ShoppingCartList.SingleOrDefault(u => u.ProductId == id);
            if (objToRemove != null)
            {
                ShoppingCartList.Remove(objToRemove);
            }
            HttpContext.Session.Set(WC.ShoppingCart, ShoppingCartList);
            return RedirectToAction(nameof(Index));
        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
