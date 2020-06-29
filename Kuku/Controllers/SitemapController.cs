using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kuku.Models;
using Microsoft.AspNetCore.Mvc;

namespace Kuku.Controllers
{
    public class SitemapController : Controller
    {
        private readonly EFContext db;
        public SitemapController(EFContext context)
        {
            db = context;
        }

        [Route("sitemap.xml")]
        public ActionResult Sitemap()
        {
            string baseUrl = "http://kuku.mba/";

            // get a list of published articles
            var recipes = db.Recipes.ToList();

            // get last modified date of the home page
            var siteMapBuilder = new SitemapBuilder();

            // add the home page to the sitemap
            siteMapBuilder.AddUrl(baseUrl, modified: DateTime.UtcNow, changeFrequency: ChangeFrequency.Weekly, priority: 1.0);

            siteMapBuilder.AddUrl(baseUrl + "Home/Contact", modified: DateTime.UtcNow, changeFrequency: null, priority: 0.5);

            siteMapBuilder.AddUrl(baseUrl + "Home/About", modified: DateTime.UtcNow, changeFrequency: null, priority: 0.5);

            // add details recipe to the sitemap
            foreach (var recipe in recipes)
            {
                siteMapBuilder.AddUrl(baseUrl + "Home/DetailsRecipe/" + recipe.RecipeId, modified: recipe.CreatedDate, changeFrequency: null, priority: 0.5);
            }

            // generate the sitemap xml
            string xml = siteMapBuilder.ToString();
            return Content(xml, "text/xml");
        }
    }
}