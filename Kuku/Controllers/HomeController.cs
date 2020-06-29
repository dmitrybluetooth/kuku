using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Kuku.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Kuku.ViewModels;
using System.Collections.Generic;
using System;

namespace Kuku.Controllers
{
    public class HomeController : Controller
    {
        private EFContext db;
        public int maxCountView = 3;

        public HomeController(EFContext context)

        {
            db = context;
        }

        //www.site.ua/filter/cuisines:12,15/
        //www.site.ua/filter/cuisines:12,15/product:12,15/?filters=kjhfncdhg
        //www.site.ua/filter/product:12,15/
        [Route("filter")]
        public IActionResult Filters(string flp,string flc, string fld, int? page)
        {
            //IQueryable<Recipe_Product> recipe_Products = db.Recipe_Products.Include(p => p.Recipe);
            //if (id != null && id != 0)
            //{
            //    recipe_Products = recipe_Products.Where(p => p.RecipeId == id);
            //}
            //var products = db.Recipe_Products.Select(sc => sc.Product).ToList();

            string SqlFilterProduct = "join Recipe_Products frp on frp.RecipeId = r.RecipeId ";
            string SqlFilterNationalCuisines = "join Recipe_NationalCuisines frn on frn.RecipeId = r.RecipeId ";
            string SqlFilterTypeOfDishes = "join Recipe_TypeOfDishes frd on frd.RecipeId = r.RecipeId ";
            string SqlFilterRecept = "";
            string[] up = { };
            string[] uc = { };
            string[] ud = { };
            List<string> sfl = new List<string>();
            string pfl = "";
            bool tpl = false;
            if (flp != null)
            {
//                SqlFilterProduct = "join Recipe_Products frp on frp.RecipeId = r.RecipeId and frp.ProductId in (" + flp + ") ";
                up = flp.Split(',');
                sfl.Add("flp=" + flp);
                tpl = true;
                for (int i = 0; i < up.Length; i++) {
                    string fs = "frp" + i;
                    SqlFilterRecept += "join Recipe_Products " + fs + " on "+ fs+".RecipeId = r.RecipeId and "+fs+".ProductId = " + up[i] + " ";
                }
            }
            else
            {
                flp = "0";
                SqlFilterRecept += SqlFilterProduct;
            }
            if (flc != null)
            {
//                SqlFilterNationalCuisines = "join Recipe_NationalCuisines frn on frn.RecipeId = r.RecipeId and frn.NationalCuisineId in (" + flc + ") ";
                uc = flc.Split(',');
                sfl.Add("flc=" + flc);
                tpl = true;
                for (int i = 0; i < uc.Length; i++)
                {
                    string fs = "frc" + i;
                    SqlFilterRecept += "join Recipe_NationalCuisines " + fs + " on " + fs + ".RecipeId = r.RecipeId and " + fs + ".NationalCuisineId = " + uc[i] + " ";
                }
            }
            else
            {
                flc = "0";
                SqlFilterRecept += SqlFilterNationalCuisines;
            }
            if (fld != null)
            {
//                SqlFilterTypeOfDishes = "join Recipe_TypeOfDishes frd on frd.RecipeId = r.RecipeId and frd.TypeOfDishId in (" + fld + ") ";
                ud = fld.Split(',');
                sfl.Add("fld=" + fld);
                tpl = true;
                for (int i = 0; i < ud.Length; i++)
                {
                    string fs = "frd" + i;
                    SqlFilterRecept += "join Recipe_TypeOfDishes " + fs + " on " + fs + ".RecipeId = r.RecipeId and " + fs + ".TypeOfDishId = " + ud[i] + " ";
                }
            }
            else
            {
                fld = "0";
                SqlFilterRecept += SqlFilterTypeOfDishes;
            }
            if (tpl) {
                pfl = "/filter?" + String.Join("&", sfl);
            }
                
            string SqlTopFilter = "SELECT Distinct TOP (10) " +
                "Products.ProductId as itemId, 'Top products' as itemType, Products.ProductName as itemName, " +
                "COUNT(Distinct Recipe_Products.RecipeId) AS itemCount, 0 as itemSort, " +
                "CASE WHEN Products.ProductId in (" + flp + ") THEN 'active' ELSE '' END as itemChecked, " +
                "'' as itemLink " +
                "FROM Products JOIN Recipe_Products ON Recipe_Products.ProductId = Products.ProductId join ProductTypes pt on pt.ProductTypeId = Products.ProductTypeId " +
                "WHERE Recipe_Products.RecipeId in (SELECT Distinct r.RecipeId FROM Recipes r " +
                SqlFilterRecept +
                ") GROUP BY Products.ProductId,pt.ProductTypeName,Products.ProductName ORDER BY itemChecked DESC, itemCount DESC ";

            List<Filter> TopProduct = db.Filters.FromSql(SqlTopFilter).ToList();
            List<Filter> TopFilterProduct = new List<Filter>();
            int index;
            string[] u;
            bool t;
            foreach (Filter filter in TopProduct)
            {
                t = false;
                List<string> s = new List<string>();
                filter.itemLink = "/";
                u = up;
                index = Array.IndexOf(u, filter.itemId + "");
                if (index > -1)
                {
                    filter.itemChecked = "active";
                    Delete(ref u, index);
                    if (u.Length > 0)
                    {
                        t = true;
                        s.Add("flp=" + String.Join(",", u));
                    }
                }
                else
                {
                    t = true;
                    if (u.Length > 0)
                    {
                        s.Add("flp=" + String.Join(",", u) + "," + filter.itemId);
                    }
                    else
                    {
                        s.Add("flp=" + filter.itemId);
                    }

                };
                if (uc.Length > 0)
                {
                    t = true;
                    s.Add("flc=" + String.Join(",", uc));
                }
                if (ud.Length > 0)
                {
                    t = true;
                    s.Add("fld=" + String.Join(",", ud));
                }
                if (t)
                {
                    filter.itemLink = "/filter?" + String.Join("&", s);
                }
                TopFilterProduct.Add(filter);
            }
            string SqlFilter = "SELECT Distinct " +
                "Products.ProductId as itemId, pt.ProductTypeName as itemType, Products.ProductName as itemName, " +
                "COUNT(Distinct Recipe_Products.RecipeId) AS itemCount, 1 as itemSort, " +
                "CASE WHEN Products.ProductId in (" + flp + ") THEN 'active' ELSE '' END as itemChecked, " +
                "'' as itemLink " +
                "FROM Products JOIN Recipe_Products ON Recipe_Products.ProductId = Products.ProductId join ProductTypes pt on pt.ProductTypeId = Products.ProductTypeId " +
                "WHERE Recipe_Products.RecipeId in (SELECT Distinct r.RecipeId FROM Recipes r " +
                SqlFilterRecept +
                ") GROUP BY Products.ProductId,pt.ProductTypeName,Products.ProductName " +
                "UNION " +
                "SELECT Distinct " +
                "NationalCuisines.NationalCuisineId as itemId, 'National Cuisines' as itemType, NationalCuisines.NationalCuisineName as itemName, " +
                "COUNT(Distinct Recipe_NationalCuisines.RecipeId) AS itemCount, 2 as itemSort, " +
                "CASE WHEN NationalCuisines.NationalCuisineId in (" + flc + ") THEN 'active' ELSE '' END as itemChecked, " +
                "'' as itemLink " +
                "FROM NationalCuisines JOIN Recipe_NationalCuisines ON Recipe_NationalCuisines.NationalCuisineId = NationalCuisines.NationalCuisineId " +
                "WHERE Recipe_NationalCuisines.RecipeId in (SELECT Distinct r.RecipeId FROM Recipes r " +
                SqlFilterRecept +
                ") GROUP BY NationalCuisines.NationalCuisineId,NationalCuisines.NationalCuisineName " +
                "UNION " +
                "SELECT Distinct " +
                "TypeOfDishes.TypeOfDishId as itemId, 'Type Of Dishes' as itemType, TypeOfDishes.TypeOfDishName as itemName, " +
                "COUNT(Distinct Recipe_TypeOfDishes.RecipeId) AS itemCount, 3 as itemSort, " +
                "CASE WHEN TypeOfDishes.TypeOfDishId in (" + fld + ") THEN 'active' ELSE '' END as itemChecked, " +
                "'' as itemLink " +
                "FROM TypeOfDishes JOIN Recipe_TypeOfDishes ON Recipe_TypeOfDishes.TypeOfDishId = TypeOfDishes.TypeOfDishId " +
                "WHERE Recipe_TypeOfDishes.RecipeId in (SELECT Distinct r.RecipeId FROM Recipes r " +
                SqlFilterRecept +
                ") GROUP BY TypeOfDishes.TypeOfDishId,TypeOfDishes.TypeOfDishName " +
                "ORDER BY itemSort, itemType, itemChecked DESC, itemCount DESC, itemName;"

            ;
            List<Filter> Filters = db.Filters.FromSql(SqlFilter).ToList();
            List<Recipe_Filter> Recipe_Filters = new List<Recipe_Filter>();
            List<Filter> Products = new List<Filter>();
            List<Filter> NationalCuisines = new List<Filter>();
            List<Filter> TypeOfDishes = new List<Filter>();
            index = -1;
            string asType = "";
            int itemsCount = 0;
            string itemClass = "";
            foreach (Filter filter in Filters)
            {
                
                if (asType == "") asType = filter.itemType;
                if (asType != filter.itemType)
                {
                    if (itemsCount <= this.maxCountView) { itemClass += " maxHeight-" + itemsCount; }
                    Recipe_Filters.Add(new Recipe_Filter { items = Products, itemType = asType, itemMD5 = this.MD5HashFilter(asType), itemsCount = itemsCount, itemClass = itemClass });
                    asType = filter.itemType;
                    Products = new List<Filter>();
                    itemsCount = 0;
                    itemClass = "";
                }
                itemsCount++;
                t = false;
                List<string> s = new List<string>();
                filter.itemLink = "/";
                switch (filter.itemType)
                {
                    case "National Cuisines":
                        u = uc;
                        index = Array.IndexOf(u, filter.itemId + "");
                        if (up.Length > 0)
                        {
                            t = true;
                            s.Add("flp=" + String.Join(",", up));
                        }
                        if (index > -1)
                        {
                            Delete(ref u, index);
                            filter.itemChecked = "active";
                            if (u.Length > 0)
                            {
                                
                                t = true;
                                s.Add("flc=" + String.Join(",", u));
                            }
                        }
                        else
                        {
                            t = true;
                            if (u.Length > 0)
                            {
                                s.Add("flc=" + String.Join(",", u) + "," + filter.itemId);
                            }
                            else
                            {
                                s.Add("flc=" + filter.itemId);
                            }

                        };
                        if (ud.Length > 0)
                        {
                            t = true;
                            s.Add("fld=" + String.Join(",", ud));
                        }
                        if (t)
                        {
                            filter.itemLink = "/filter?" + String.Join("&", s);
                        }


                        Products.Add(filter);
                        break;
                    case "Type Of Dishes":
                        u = ud;
                        index = Array.IndexOf(u, filter.itemId + "");
                        if (up.Length > 0)
                        {
                            t = true;
                            s.Add("flp=" + String.Join(",", up));
                        }
                        if (uc.Length > 0)
                        {
                            t = true;
                            s.Add("flc=" + String.Join(",", uc));
                        }
                        if (index > -1)
                        {
                            Delete(ref u, index);
                            filter.itemChecked = "active";
                            if (u.Length > 0)
                            {
                                
                                t = true;
                                s.Add("fld=" + String.Join(",", u));
                            }
                        }
                        else
                        {
                            t = true;
                            if (u.Length > 0)
                            {
                                s.Add("fld=" + String.Join(",", u) + "," + filter.itemId);
                            }
                            else
                            {
                                s.Add("fld=" + filter.itemId);
                            }

                        };
                        if (t)
                        {
                            filter.itemLink = "/filter?" + String.Join("&", s);
                        }

                        Products.Add(filter);
                        break;
                    default:
                        u = up;
                        index = Array.IndexOf(u, filter.itemId + "");
                        if (index > -1)
                        {
                            filter.itemChecked = "active";
                            Delete(ref u, index);

                            //Array.Clear(u, index, 1);
                            if (u.Length > 0)
                            {

                                t = true;
                                s.Add("flp=" + String.Join(",", u));
                            }
                        }
                        else
                        {
                            t = true;
                            if (u.Length > 0)
                            {
                                s.Add("flp=" + String.Join(",", u) + "," + filter.itemId);
                            }
                            else
                            {
                                s.Add("flp=" + filter.itemId);
                            }

                        };
                        if (uc.Length > 0)
                        {
                            t = true;
                            s.Add("flc=" + String.Join(",", uc));
                        }
                        if (ud.Length > 0)
                        {
                            t = true;
                            s.Add("fld=" + String.Join(",", ud));
                        }
                        if (t)
                        {
                            filter.itemLink = "/filter?" + String.Join("&", s);
                        }
                        Products.Add(filter);
                        break;
                }
                if (index > -1) { itemClass = "in"; }
            }
            if (asType != "")
            {
                if (itemsCount <= this.maxCountView) { itemClass += " maxHeight-" + itemsCount; }
                Recipe_Filters.Add(new Recipe_Filter { items = Products, itemType = asType, itemMD5 = this.MD5HashFilter(asType), itemsCount = itemsCount, itemClass = itemClass });
                Products = new List<Filter>();
            }
            string sqlRecept = "SELECT Distinct r.* FROM Recipes r " + SqlFilterRecept;
            IEnumerable<Recipe> recipes = db.Recipes.FromSql(sqlRecept).ToList().OrderByDescending(r => r.CreatedDate);
            var count = recipes.Count();

            var pager = new PageInfo(recipes.Count(), page, pfl);

            FilterViewModel viewModel = new FilterViewModel
            {
                TopFilterProduct = TopFilterProduct,
                Recipe_Filters = Recipe_Filters,
                Recipes = recipes.Skip((pager.CurrentPage - 1) * pager.PageSize).Take(pager.PageSize),
                Products = Products,
                TypeOfDishes = TypeOfDishes,
                NationalCuisines = NationalCuisines,
                PageInfo = pager
                //MeasuringSystems = measuringSystem
            };
            return View("Index",viewModel);
        }

        private void Delete(ref string[] u, int index)
        {
            string[] n = new string[u.Length - 1];
            for (int i = 0; i < index; i++)
            {
                n[i] = u[i];
            }
            for (int i = index; i < n.Length; i++)
            {
                n[i] = u[i + 1];
            }
            u = n;
            //throw new NotImplementedException();
        }

        [Route("/")]
        public IActionResult Index(int? page)
        {
            //IQueryable<Recipe_Product> recipe_Products = db.Recipe_Products.Include(p => p.Recipe);
            //if (id != null && id != 0)
            //{
            //    recipe_Products = recipe_Products.Where(p => p.RecipeId == id);
            //}
            //var products = db.Recipe_Products.Select(sc => sc.Product).ToList();

            //const string SqlFilterProduct = "join Recipe_Products frp on frp.RecipeId = r.RecipeId and frp.ProductId in (13, 15, 16, 17, 21) ";
            const string SqlFilterProduct =
                "join Recipe_Products frp on frp.RecipeId = r.RecipeId ";
            //const string SqlFilterNationalCuisines = "join Recipe_NationalCuisines frn on frn.RecipeId = r.RecipeId and frn.NationalCuisineId in (10) ";
            const string SqlFilterNationalCuisines = "join Recipe_NationalCuisines frn on frn.RecipeId = r.RecipeId ";
            //const string SqlFilterTypeOfDishes = "join Recipe_TypeOfDishes frt on frt.RecipeId = r.RecipeId and frt.TypeOfDishId in (4, 6) ";
            const string SqlFilterTypeOfDishes = "join Recipe_TypeOfDishes frt on frt.RecipeId = r.RecipeId ";
            const string SqlTopFilter = "SELECT Distinct TOP (10) " +
                "Products.ProductId as itemId, 'Top products' as itemType, Products.ProductName as itemName, " +
                "COUNT(Distinct Recipe_Products.RecipeId) AS itemCount, 0 as itemSort, " +
                "'' as itemChecked, " +
                "CONCAT('/filter?flp=',Products.ProductId) as itemLink " + 
                "FROM Products JOIN Recipe_Products ON Recipe_Products.ProductId = Products.ProductId join ProductTypes pt on pt.ProductTypeId = Products.ProductTypeId " +
                "WHERE Recipe_Products.RecipeId in (SELECT Distinct r.RecipeId FROM Recipes r " +
                SqlFilterProduct +
                SqlFilterNationalCuisines +
                SqlFilterTypeOfDishes +
                ") GROUP BY Products.ProductId,pt.ProductTypeName,Products.ProductName ORDER BY itemCount DESC ";
            const string SqlFilter = "SELECT Distinct " +
                "Products.ProductId as itemId, pt.ProductTypeName as itemType, Products.ProductName as itemName, " +
                "COUNT(Distinct Recipe_Products.RecipeId) AS itemCount, 1 as itemSort, " +
                "'' as itemChecked, " +
                "CONCAT('/filter?flp=',Products.ProductId) as itemLink " +
                "FROM Products JOIN Recipe_Products ON Recipe_Products.ProductId = Products.ProductId join ProductTypes pt on pt.ProductTypeId = Products.ProductTypeId " +
                "WHERE Recipe_Products.RecipeId in (SELECT Distinct r.RecipeId FROM Recipes r " +
                SqlFilterProduct +
                SqlFilterNationalCuisines +
                SqlFilterTypeOfDishes +
                ") GROUP BY Products.ProductId,pt.ProductTypeName,Products.ProductName " +
                "UNION " +
                "SELECT Distinct " +
                "NationalCuisines.NationalCuisineId as itemId, 'National Cuisines' as itemType, NationalCuisines.NationalCuisineName as itemName, " +
                "COUNT(Distinct Recipe_NationalCuisines.RecipeId) AS itemCount, 2 as itemSort, " +
                "'' as itemChecked, " +
                "CONCAT('/filter?flc=',NationalCuisines.NationalCuisineId) as itemLink " +
                "FROM NationalCuisines JOIN Recipe_NationalCuisines ON Recipe_NationalCuisines.NationalCuisineId = NationalCuisines.NationalCuisineId " +
                "WHERE Recipe_NationalCuisines.RecipeId in (SELECT Distinct r.RecipeId FROM Recipes r " +
                SqlFilterProduct +
                SqlFilterNationalCuisines +
                SqlFilterTypeOfDishes +
                ") GROUP BY NationalCuisines.NationalCuisineId,NationalCuisines.NationalCuisineName " +
                "UNION " +
                "SELECT Distinct " +
                "TypeOfDishes.TypeOfDishId as itemId, 'Type Of Dishes' as itemType, TypeOfDishes.TypeOfDishName as itemName, " +
                "COUNT(Distinct Recipe_TypeOfDishes.RecipeId) AS itemCount, 3 as itemSort, " +
                "'' as itemChecked, " +
                "CONCAT('/filter?fld=',TypeOfDishes.TypeOfDishId) as itemLink " +
                "FROM TypeOfDishes JOIN Recipe_TypeOfDishes ON Recipe_TypeOfDishes.TypeOfDishId = TypeOfDishes.TypeOfDishId " +
                "WHERE Recipe_TypeOfDishes.RecipeId in (SELECT Distinct r.RecipeId FROM Recipes r " +
                SqlFilterProduct +
                SqlFilterNationalCuisines +
                SqlFilterTypeOfDishes +
                ") GROUP BY TypeOfDishes.TypeOfDishId,TypeOfDishes.TypeOfDishName " +
                "ORDER BY itemSort, itemType, itemChecked DESC, itemCount DESC, itemName;"
            ;
            List<Filter> TopFilterProduct = db.Filters.FromSql(SqlTopFilter).ToList();

            List<Filter> Filters = db.Filters.FromSql(SqlFilter).ToList();
            
            List<Recipe_Filter> Recipe_Filters = new List<Recipe_Filter>();
            List<Filter> Products = new List<Filter>();
            List<Filter> NationalCuisines = new List<Filter>();
            List<Filter> TypeOfDishes = new List<Filter>();
            string asType = "";
            int itemsCount = 0;
            string itemClass = "";
            foreach (Filter filter in Filters)
            {
                
                if (asType == "") asType = filter.itemType;
                if (asType != filter.itemType)
                {
                    if (itemsCount <= this.maxCountView) {itemClass = "maxHeight-" + itemsCount;}
                    Recipe_Filters.Add(new Recipe_Filter { items = Products, itemType = asType, itemMD5 = this.MD5HashFilter(asType), itemsCount = itemsCount, itemClass = itemClass });
                    asType = filter.itemType;
                    Products = new List<Filter>();
                    itemClass = "";
                    itemsCount = 0;
                }
                itemsCount++;
                Products.Add(filter);
            }
            if (asType != "")
            {
                if (itemsCount <= this.maxCountView) { itemClass = "maxHeight-" + itemsCount; }
                Recipe_Filters.Add(new Recipe_Filter { items = Products, itemType = asType, itemMD5 = this.MD5HashFilter(asType), itemsCount = itemsCount, itemClass = itemClass });
                Products = new List<Filter>();
            }

            IEnumerable<Recipe> recipes = db.Recipes.ToList().OrderByDescending(r => r.CreatedDate);
            var count = recipes.Count();

            var pager = new PageInfo(recipes.Count(), page);

            //IEnumerable<Recipe> recipes = db.Recipes.ToList();//.ToPagedList(pageNumber, pageSize);
            FilterViewModel viewModel = new FilterViewModel
            {
                TopFilterProduct = TopFilterProduct,
                Recipe_Filters = Recipe_Filters,
                Recipes = recipes.Skip((pager.CurrentPage - 1) * pager.PageSize).Take(pager.PageSize),
                Products = Products,
                TypeOfDishes = TypeOfDishes,
                NationalCuisines = NationalCuisines,
                PageInfo = pager
                //MeasuringSystems = measuringSystem
            };
            return View(viewModel);
        }
        public string MD5HashFilter(string input)
        {
            System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create();
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes("filter-" + input + DateTime.Now.ToShortTimeString());
            byte[] hash = md5.ComputeHash(inputBytes);
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }
            return sb.ToString();
        }
        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public async Task<ActionResult> DetailsRecipe(int? id)
        {
            Recipe recipe = await db.Recipes.FirstOrDefaultAsync(p => p.RecipeId == id);
            if (recipe == null)
            {
                return BadRequest("No such order found for this user.");
            }
            IQueryable<RecipeDetail> recipeDetails = db.RecipeDetails.Include(p => p.Recipe);
            if (id != null && id != 0)
            {
                recipeDetails = recipeDetails.Where(p => p.RecipeId == id);
            }
            IQueryable<Recipe_Product> recipe_Products = db.Recipe_Products.Include(p => p.Recipe);
            if (id != null && id != 0)
            {
                recipe_Products = recipe_Products.Where(p => p.RecipeId == id);
            }
            var products = db.Recipe_Products.Select(sc => sc.Product).ToList();
            List<MeasuringSystem> measuringSystems = await db.MeasuringSystems.ToListAsync();
            IQueryable<Recipe_TypeOfDish> recipe_TypeOfDishes = db.Recipe_TypeOfDishes.Include(p => p.Recipe);
            if (id != null && id != 0)
            {
                recipe_TypeOfDishes = recipe_TypeOfDishes.Where(p => p.RecipeId == id);
            }
            var typeOfDishes = db.Recipe_TypeOfDishes.Select(sc => sc.TypeOfDish).ToList();
            IQueryable<Recipe_NationalCuisine> recipe_NationalCuisines = db.Recipe_NationalCuisines.Include(p => p.Recipe);
            if (id != null && id != 0)
            {
                recipe_NationalCuisines = recipe_NationalCuisines.Where(p => p.RecipeId == id);
            }
            var nationalCuisines = db.Recipe_NationalCuisines.Select(sc => sc.NationalCuisine).ToList();

            string aspNetUserName = db.AspNetUsers.FirstOrDefault(a => a.Id == recipe.UserId).UserName;
            //string aspNetUserName = db.AspNetUsers.Where(a => a.Id == recipe.UserId).Select(a => a.UserName).FirstOrDefault();


            RecipeViewModel viewModel = new RecipeViewModel
            {
                Recipes = recipe,
                RecipesDetails = recipeDetails,
                Recipe_Products = recipe_Products.OrderBy(r => r.CreatedDate),
                Products = products,
                Recipe_TypeOfDishes = recipe_TypeOfDishes,
                TypeOfDishes = typeOfDishes,
                Recipe_NationalCuisenes = recipe_NationalCuisines,
                NationalCuisines = nationalCuisines,
                AspNetUserName = aspNetUserName
            };
            return View(viewModel);
        }
    }
}
