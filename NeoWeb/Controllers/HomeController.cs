﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NeoWeb.Models;
using NeoWeb.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Http;
using System.Globalization;

namespace NeoWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            try
            {
                ViewBag.News = _context.News.OrderByDescending(p => p.Time).Take(3).ToList();
            }
            catch (Exception)
            {
                ViewBag.News = new List<News>();
                //网站第一次运行，未创建数据库时会有异常
            }
            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SetLanguage(string culture, string returnUrl)
        {
            if (string.IsNullOrEmpty(culture) || string.IsNullOrEmpty(returnUrl))
                return RedirectToAction("Index");
            try
            {
                Response.Cookies.Append(
                        CookieRequestCultureProvider.DefaultCookieName,
                        CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                        new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
                    );
            }
            catch (InvalidOperationException)
            {
                return RedirectToAction("Index"); ;
            }
            catch (CultureNotFoundException)
            {
                return RedirectToAction("Index");
            }

            return LocalRedirect(returnUrl);
        }
    }
}
