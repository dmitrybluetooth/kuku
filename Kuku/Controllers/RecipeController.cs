using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Kuku.Models;
using Kuku.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Processing.Transforms;
using SixLabors.Primitives;

namespace Kuku.Controllers
{
    [Authorize(Roles = "admin")]
    public class RecipeController : Controller
    {
        private readonly UserManager<User> _userManager;
        IHostingEnvironment _appEnvironment;

        private EFContext db;
        public RecipeController(EFContext context, UserManager<User> userManager, IConfiguration configuration, IHostingEnvironment appEnvironment)

        {
            db = context;
            _userManager = userManager;
            Configuration = configuration;
            _appEnvironment = appEnvironment;
        }
        public IConfiguration Configuration { get; }


        //public async Task<IActionResult> Index()
        //{
        //    return View(await db.Recipes.ToListAsync());
        //}

        public async Task<IActionResult> MeasuringSystem()
        {
            return View(await db.MeasuringSystems.ToListAsync());
        }

        public async Task<IActionResult> NationalCuisine()
        {
            return View(await db.NationalCuisines.ToListAsync());
        }

        public async Task<IActionResult> ProductType()
        {
            return View(await db.ProductTypes.ToListAsync());
        }

        public async Task<IActionResult> Product(string productName)
        {
            List<MeasuringSystem> measuringSystems = await db.MeasuringSystems.ToListAsync();
            List<ProductType> productTypes = await db.ProductTypes.ToListAsync();
            IQueryable<Product> products = db.Products;

            if (!String.IsNullOrEmpty(productName))
            {
                products = products.Where(p => p.ProductName.Contains(productName));
            }

            ProductsListViewModel viewModels = new ProductsListViewModel
            {
                Products = products.ToList(),
                Name = productName
            };
            return View(viewModels);
        }

        public async Task<IActionResult> TypeOfDish()
        {
            return View(await db.TypeOfDishes.ToListAsync());
        }

        public async Task<IActionResult> RecipeDetail()
        {
            return View(await db.RecipeDetails.ToListAsync());
        }


        [HttpGet]
        public IActionResult AddProduct(int? recipeid, int? productid)
        {
            if (recipeid != null)
            {
                Recipe_Product recipe_Product = db.Recipe_Products.FirstOrDefault(p => p.RecipeId == recipeid);
                if (recipe_Product != null)
                    return View(recipe_Product);
            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AddProduct(Recipe_Product recipe_Product)
        {
            ///вот сюда чёта добавить, чтобы всё работало
            db.Recipe_Products.Add(recipe_Product);
            await db.SaveChangesAsync();
            return View();
        }

        public IActionResult CreateMeasuringSystem()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateMeasuringSystem(MeasuringSystem measuringSystem)
        {
            db.MeasuringSystems.Add(measuringSystem);
            await db.SaveChangesAsync();
            return RedirectToAction("MeasuringSystem");
        }

        public IActionResult CreateNationalCuisine()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateNationalCuisine(NationalCuisine NationalCuisine)
        {
            db.NationalCuisines.Add(NationalCuisine);
            await db.SaveChangesAsync();
            return RedirectToAction("NationalCuisine");
        }

        [HttpGet]
        public ActionResult CreateProduct()
        {
            // Формируем список продуктов для передачи в представление
            SelectList productTypes = new SelectList(db.ProductTypes, "ProductTypeId", "ProductTypeName");
            ViewBag.ProductTypes = productTypes;

            SelectList measuringSystem = new SelectList(db.MeasuringSystems, "MeasuringSystemId", "MeasuringSystemName");
            ViewBag.MeasuringSystems = measuringSystem;

            return View();
        }
        [HttpPost]
        public ActionResult CreateProduct(Product product)
        {
            //Добавляем игрока в таблицу
            db.Products.Add(product);
            db.SaveChanges();
            // перенаправляем на главную страницу
            return RedirectToAction("Product");
        }

        public IActionResult CreateProductType()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateProductType(ProductType ProductType)
        {
            db.ProductTypes.Add(ProductType);
            await db.SaveChangesAsync();
            return RedirectToAction("ProductType");
        }

        public IActionResult CreateTypeOfDish()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateTypeOfDish(TypeOfDish TypeOfDish)
        {
            db.TypeOfDishes.Add(TypeOfDish);
            await db.SaveChangesAsync();
            return RedirectToAction("TypeOfDish");
        }

        public ActionResult CreateRecipe()
        {
            return View();
        }
        [HttpPost]
        public IActionResult CreateRecipe(IFormFile uploadedFile, SP_Recipe sp_Recipe)
        {
            // string connectionString = Configuration.GetConnectionString("DefaultConnection");
            // название процедуры
            //string sqlExpression = "SP_Product";

            using (SqlConnection connection = new SqlConnection(Configuration.GetConnectionString("DefaultConnection")))
            {
                // Sp_recipe file = new Sp_recipe { FileName = uploadedFile.FileName.Substring(uploadedFile.FileName.LastIndexOf('\\') + 1) };
                string shortFileName = uploadedFile.FileName.Substring(uploadedFile.FileName.LastIndexOf('\\') + 1);
                SP_Recipe file = new SP_Recipe { FileName = shortFileName };

                Directory.CreateDirectory(_appEnvironment.WebRootPath + "/Temp/");
                // путь к папке Temp
                string path = _appEnvironment.WebRootPath + "/Temp/";

                if (uploadedFile != null)
                {
                    // сохраняем файл в папку Temp в каталоге wwwroot
                    using (var fileStream = new FileStream(path + shortFileName, FileMode.Create))
                    {
                        uploadedFile.CopyTo(fileStream);
                    }

                    using (var img = Image.Load(path + shortFileName))
                    {
                        // as generate returns a new IImage make sure we dispose of it
                        using (Image<Rgba32> destRound = img.Clone(x => x.Resize(new Size(590, 0))))
                        {
                            destRound.Save(path + "bigImage_" + _userManager.GetUserName(HttpContext.User) + "_" + shortFileName);
                        }

                        using (Image<Rgba32> destRound = img.Clone(x => x.Resize(new Size(320, 0))))
                        {
                            destRound.Save(path + "previewImage_" + _userManager.GetUserName(HttpContext.User) + "_" + shortFileName);
                        }
                    }

                    byte[] bigImageData = System.IO.File.ReadAllBytes(path + "bigImage_" + _userManager.GetUserName(HttpContext.User) + "_" + shortFileName);
                    file.BigImageData = bigImageData;

                    byte[] previewImageData = System.IO.File.ReadAllBytes(path + "previewImage_" + _userManager.GetUserName(HttpContext.User) + "_" + shortFileName);
                    file.PreviewImageData = previewImageData;

                    byte[] originalImageData = null;
                    // считываем переданный файл в массив байтов
                    using (var binaryReader = new BinaryReader(uploadedFile.OpenReadStream()))
                    {
                        originalImageData = binaryReader.ReadBytes((int)uploadedFile.Length);
                    }
                    // установка массива байтов
                    file.OriginalImageData = originalImageData;

                    Directory.Delete(path, true);
                }

                connection.Open();
                SqlCommand command = new SqlCommand("SP_Recipe", connection);
                // указываем, что команда представляет хранимую процедуру
                command.CommandType = System.Data.CommandType.StoredProcedure;
                // параметр для ввода имени
                //string shortFileName = filename.Substring(filename.LastIndexOf('\\') + 1);
                SqlParameter fileNameParam = new SqlParameter
                {
                    ParameterName = "@FileName",
                    Value = file.FileName
                };
                // добавляем параметр
                command.Parameters.Add(fileNameParam);
                // параметр для ввода возраста
                SqlParameter originalImageDataParam = new SqlParameter
                {
                    ParameterName = "@OriginalImageData",
                    Value = file.OriginalImageData
                };
                // добавляем параметр
                command.Parameters.Add(originalImageDataParam);

                SqlParameter recipeNameParam = new SqlParameter
                {
                    ParameterName = "@RecipeName",
                    Value = sp_Recipe.RecipeName
                };
                // добавляем параметр
                command.Parameters.Add(recipeNameParam);

                SqlParameter DescriptionParam = new SqlParameter
                {
                    ParameterName = "@Description",
                    Value = sp_Recipe.Description
                };
                // добавляем параметр
                command.Parameters.Add(DescriptionParam);

                SqlParameter bigImageDataParam = new SqlParameter
                {
                    ParameterName = "@BigImageData",
                    Value = file.BigImageData
                };
                // добавляем параметр
                command.Parameters.Add(bigImageDataParam);

                SqlParameter previewImageDataParam = new SqlParameter
                {
                    ParameterName = "@PreviewImageData",
                    Value = file.PreviewImageData
                };
                // добавляем параметр
                command.Parameters.Add(previewImageDataParam);

                SqlParameter userIdParam = new SqlParameter
                {
                    ParameterName = "@UserId",
                    Value = _userManager.GetUserId(HttpContext.User)
                };
                // добавляем параметр
                command.Parameters.Add(userIdParam);

                //var result = command.ExecuteScalar();
                // если нам не надо возвращать id
                command.ExecuteNonQuery();
                connection.Close();
            }
            return RedirectToAction("Index", "Home");
        }

        public ActionResult CreateRecipeDetail()
        {
            return View(/*await db.Recipe.ToListAsync()*/);
        }

        [HttpPost]
        public IActionResult CreateRecipeDetail(IFormFile uploadedFile, SP_RecipeDetails sp_RecipeDetails, int? id)
        {
            if (id != null)
            {
                // string connectionString = Configuration.GetConnectionString("DefaultConnection");
                // название процедуры
                //string sqlExpression = "SP_Product";

                using (SqlConnection connection = new SqlConnection(Configuration.GetConnectionString("DefaultConnection")))
                {
                    // Sp_recipe file = new Sp_recipe { FileName = uploadedFile.FileName.Substring(uploadedFile.FileName.LastIndexOf('\\') + 1) };
                    string shortFileName = uploadedFile.FileName.Substring(uploadedFile.FileName.LastIndexOf('\\') + 1);
                    SP_RecipeDetails file = new SP_RecipeDetails { FileName = shortFileName };

                    Directory.CreateDirectory(_appEnvironment.WebRootPath + "/Temp/");
                    // путь к папке Temp
                    string path = _appEnvironment.WebRootPath + "/Temp/";

                    if (uploadedFile != null)
                    {
                        // сохраняем файл в папку Temp в каталоге wwwroot
                        using (var fileStream = new FileStream(path + shortFileName, FileMode.Create))
                        {
                            uploadedFile.CopyTo(fileStream);
                        }

                        using (var img = Image.Load(path + shortFileName))
                        {
                            // as generate returns a new IImage make sure we dispose of it
                            using (Image<Rgba32> destRound = img.Clone(x => x.Resize(new Size(590, 0))))
                            {
                                destRound.Save(path + "bigImage_" + _userManager.GetUserName(HttpContext.User) + "_" + shortFileName);
                            }

                            using (Image<Rgba32> destRound = img.Clone(x => x.Resize(new Size(320, 0))))
                            {
                                destRound.Save(path + "previewImage_" + _userManager.GetUserName(HttpContext.User) + "_" + shortFileName);
                            }
                        }

                        byte[] bigImageData = System.IO.File.ReadAllBytes(path + "bigImage_" + _userManager.GetUserName(HttpContext.User) + "_" + shortFileName);
                        file.BigImageData = bigImageData;

                        byte[] previewImageData = System.IO.File.ReadAllBytes(path + "previewImage_" + _userManager.GetUserName(HttpContext.User) + "_" + shortFileName);
                        file.PreviewImageData = previewImageData;

                        byte[] originalImageData = null;
                        // считываем переданный файл в массив байтов
                        using (var binaryReader = new BinaryReader(uploadedFile.OpenReadStream()))
                        {
                            originalImageData = binaryReader.ReadBytes((int)uploadedFile.Length);
                        }
                        // установка массива байтов
                        file.OriginalImageData = originalImageData;

                        Directory.Delete(path, true);
                    }
                    connection.Open();
                    SqlCommand command = new SqlCommand("SP_RecipeDetails", connection)
                    {
                        // указываем, что команда представляет хранимую процедуру
                        CommandType = System.Data.CommandType.StoredProcedure
                    };
                    // параметр для ввода имени
                    //string shortFileName = filename.Substring(filename.LastIndexOf('\\') + 1);
                    SqlParameter fileNameParam = new SqlParameter
                    {
                        ParameterName = "@FileName",
                        Value = file.FileName
                    };
                    // добавляем параметр
                    command.Parameters.Add(fileNameParam);
                    // параметр для ввода возраста
                    SqlParameter originalImageDataParam = new SqlParameter
                    {
                        ParameterName = "@OriginalImageData",
                        Value = file.OriginalImageData
                    };
                    // добавляем параметр
                    command.Parameters.Add(originalImageDataParam);
                    SqlParameter DescriptionParam = new SqlParameter
                    {
                        ParameterName = "@DescriptionRD",
                        Value = sp_RecipeDetails.DescriptionRD
                    };
                    // добавляем параметр
                    command.Parameters.Add(DescriptionParam);
                    SqlParameter bigImageDataParam = new SqlParameter
                    {
                        ParameterName = "@BigImageData",
                        Value = file.BigImageData
                    };
                    // добавляем параметр
                    command.Parameters.Add(bigImageDataParam);
                    SqlParameter previewImageDataParam = new SqlParameter
                    {
                        ParameterName = "@PreviewImageData",
                        Value = file.PreviewImageData
                    };
                    // добавляем параметр
                    command.Parameters.Add(previewImageDataParam);
                    SqlParameter RecipeIdParam = new SqlParameter
                    {
                        ParameterName = "@RecipeId",
                        Value = id

                    };
                    // добавляем параметр
                    command.Parameters.Add(RecipeIdParam);
                    //var result = command.ExecuteScalar();
                    // если нам не надо возвращать id
                    command.ExecuteNonQuery();
                    connection.Close();
                }
                return Ok("Recipe details added");
            }
            return NotFound();
        }

        public async Task<IActionResult> DetailsProduct(int? id)
        {
            if (id != null)
            {
                Product product = await db.Products.FirstOrDefaultAsync(p => p.ProductId == id);
                if (product != null)
                    return View(product);
            }
            return NotFound();
        }

        public async Task<IActionResult> DetailsMeasuringSystem(int? id)
        {
            if (id != null)
            {
                MeasuringSystem measuringSystem= await db.MeasuringSystems.FirstOrDefaultAsync(p => p.MeasuringSystemId == id);
                if (measuringSystem != null)
                    return View(measuringSystem);
            }
            return NotFound();
        }

        public async Task<IActionResult> DetailsNationalCuisine(int? id)
        {
            if (id != null)
            {
                NationalCuisine nationalCuisine = await db.NationalCuisines.FirstOrDefaultAsync(p => p.NationalCuisineId == id);
                if (nationalCuisine != null)
                    return View(nationalCuisine);
            }
            return NotFound();
        }

        public async Task<IActionResult> DetailsProductType(int? id)
        {
            if (id != null)
            {
                ProductType productType = await db.ProductTypes.FirstOrDefaultAsync(p => p.ProductTypeId == id);
                if (productType != null)
                    return View(productType);
            }
            return NotFound();
        }

        public async Task<IActionResult> DetailsTypeOfDish(int? id)
        {
            if (id != null)
            {
                TypeOfDish typeOfDish = await db.TypeOfDishes.FirstOrDefaultAsync(p => p.TypeOfDishId == id);
                if (typeOfDish != null)
                    return View(typeOfDish);
            }
            return NotFound();
        }

        [HttpGet]
        [ActionName("DeleteMeasuringSystem")]
        public async Task<IActionResult> ConfirmDeleteMeasuringSystem(int? id)
        {
            if (id != null)
            {
                MeasuringSystem measuringSystem= await db.MeasuringSystems.FirstOrDefaultAsync(p => p.MeasuringSystemId == id);
                if (measuringSystem != null)
                    return View(measuringSystem);
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> DeleteMeasuringSystem(int? id)
        {
            if (id != null)
            {

                MeasuringSystem measuringSystem = new MeasuringSystem { MeasuringSystemId = id.Value };
                db.Entry(measuringSystem).State = EntityState.Deleted;
                await db.SaveChangesAsync();
                return RedirectToAction("MeasuringSystem");
            }
            return NotFound();
        }

        [HttpGet]
        [ActionName("DeleteProduct")]
        public async Task<IActionResult> ConfirmDeleteProduct(int? id)
        {
            if (id != null)
            {
                Product product = await db.Products.FirstOrDefaultAsync(p => p.ProductId == id);
                if (product != null)
                    return View(product);
            }
            return NotFound();
        }
        [HttpPost]
        public async Task<IActionResult> DeleteProduct(int? id)
        {
            if (id != null)
            {

                Product product = new Product { ProductId = id.Value };
                db.Entry(product).State = EntityState.Deleted;
                await db.SaveChangesAsync();
                return RedirectToAction("Product");
            }
            return NotFound();
        }

        [HttpGet]
        [ActionName("DeleteProductType")]
        public async Task<IActionResult> ConfirmDeleteProductType(int? id)
        {
            if (id != null)
            {
                ProductType productType = await db.ProductTypes.FirstOrDefaultAsync(p => p.ProductTypeId == id);
                if (productType != null)
                    return View(productType);
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> DeleteProductType(int? id)
        {
            if (id != null)
            {

                ProductType productType = new ProductType { ProductTypeId = id.Value };
                db.Entry(productType).State = EntityState.Deleted;
                await db.SaveChangesAsync();
                return RedirectToAction("ProductType");
            }
            return NotFound();
        }

        [HttpGet]
        [ActionName("DeleteTypeOfDish")]
        public async Task<IActionResult> ConfirmDeleteTypeOfDish(int? id)
        {
            if (id != null)
            {
                TypeOfDish typeOfDish = await db.TypeOfDishes.FirstOrDefaultAsync(p => p.TypeOfDishId == id);
                if (typeOfDish != null)
                    return View(typeOfDish);
            }
            return NotFound();
        }
        [HttpPost]
        public async Task<IActionResult> DeleteTypeOfDish(int? id)
        {
            if (id != null)
            {

                TypeOfDish typeOfDish = new TypeOfDish { TypeOfDishId = id.Value };
                db.Entry(typeOfDish).State = EntityState.Deleted;
                await db.SaveChangesAsync();
                return RedirectToAction("TypeOfDish");
            }
            return NotFound();
        }

        [HttpGet]
        [ActionName("DeleteRecipe")]
        public async Task<IActionResult> ConfirmDeleteRecipe(int? id)
        {
            if (id != null)
            {
                Recipe recipe = await db.Recipes.FirstOrDefaultAsync(p => p.RecipeId == id);
                if (recipe != null)
                    return View(recipe);
            }
            return NotFound();
        }
        [HttpPost]
        public async Task<IActionResult> DeleteRecipe(int? id)
        {
            if (id != null)
            {
                /*NationalCuisine nationalCuisine = await db.NationalCuisine.FirstOrDefaultAsync(p => p.NationalCuisineId == id);
                if (nationalCuisine != null)
                {
                    db.NationalCuisine.Remove(nationalCuisine);
                    await db.SaveChangesAsync();
                    return RedirectToAction("NationalCuisine");
                }*/
                Recipe recipe = new Recipe { RecipeId = id.Value };
                db.Entry(recipe).State = EntityState.Deleted;
                await db.SaveChangesAsync();
                return RedirectToAction("Index", "Home");//в отличии от предыдущего, этот метод - оптимизированный и с проверкой на существование записи в БД
            }
            return NotFound();
        }

        [HttpGet]
        [ActionName("DeleteRecipeDetail")]
        public async Task<IActionResult> ConfirmDeleteRecipeDetail(int? recipedetailid, int? recipeid)
        {
            if (recipedetailid != null)
            {
                RecipeDetail recipeDetail = await db.RecipeDetails.FirstOrDefaultAsync(p => p.RecipeDetailId == recipedetailid);
                if (recipeDetail != null)
                    return View(recipeDetail);
            }
            return NotFound();
        }
        [HttpPost]
        public async Task<IActionResult> DeleteRecipeDetail(int? recipedetailid, int? recipeid)
        {
            if (recipedetailid != null)
            {

                RecipeDetail recipeDetail = new RecipeDetail { RecipeDetailId = recipedetailid.Value };
                db.Entry(recipeDetail).State = EntityState.Deleted;
                await db.SaveChangesAsync();
                return RedirectToAction("DetailsRecipe", "Home", new { id = recipeid });
            }
            return NotFound();
        }

        [HttpGet]
        [ActionName("DeleteRecipe_NationalCuisine")]
        public async Task<IActionResult> ConfirmDeleteRecipe_NationalCuisine(int? recipeid, int? nationalcuisineid)
        {
            Recipe recipeidcontext = db.Recipes.FirstOrDefault(p => p.RecipeId == recipeid);
            if (recipeidcontext == null)
            {
                return BadRequest("No such order found for this user.");
            }
            IQueryable<Recipe_NationalCuisine> recipe_NationalCuisines = db.Recipe_NationalCuisines.Include(p => p.NationalCuisine);

            if (recipeid != null && recipeid != 0)
            {
                recipe_NationalCuisines = recipe_NationalCuisines.Where(p => p.RecipeId == recipeid);
            }
            if (nationalcuisineid != null && nationalcuisineid != 0)
            {
                var cuisine = await recipe_NationalCuisines.FirstOrDefaultAsync(sc => sc.NationalCuisineId == nationalcuisineid);
                if (cuisine != null)
                    return View(cuisine);
            }
            return NotFound();
        }
        [HttpPost]
        public async Task<IActionResult> DeleteRecipe_NationalCuisine(Recipe_NationalCuisine cuisine, int? recipeid, int? nationalcuisineid)
        {
            if (recipeid == null && nationalcuisineid == null)
            {
                return BadRequest("Bad request");
            }
            Recipe_NationalCuisine recipe_NationalCuisine = new Recipe_NationalCuisine { RecipeId = cuisine.RecipeId, NationalCuisineId = cuisine.NationalCuisineId };
            db.Entry(recipe_NationalCuisine).State = EntityState.Deleted;
            await db.SaveChangesAsync();
            return RedirectToAction("DetailsRecipe", "Home", new { id = recipeid });

        }

        [HttpGet]
        [ActionName("DeleteRecipe_Product")]
        public async Task<IActionResult> ConfirmDeleteRecipe_Product(int? recipeid, int? productid)
        {
            Recipe recipeidcontext = db.Recipes.FirstOrDefault(p => p.RecipeId == recipeid);
            if (recipeidcontext == null)
            {
                return BadRequest("No such order found for this user.");
            }
            IQueryable<Recipe_Product> recipe_Products = db.Recipe_Products.Include(p => p.Product);

            if (recipeid != null && recipeid != 0)
            {
                recipe_Products = recipe_Products.Where(p => p.RecipeId == recipeid);
            }
            if (productid != null && productid != 0)
            {
                var product = await recipe_Products.FirstOrDefaultAsync(sc => sc.ProductId == productid);
                if (product != null)
                    return View(product);
            }
            return NotFound();
        }
        [HttpPost]
        public async Task<IActionResult> DeleteRecipe_Product(Recipe_Product product, int? recipeid, int? productid)
        {
            if (recipeid == null && productid == null)
            {
                return BadRequest("Bad request");
            }
            Recipe_Product recipe_Product = new Recipe_Product { RecipeId = product.RecipeId, ProductId = product.ProductId };
            db.Entry(recipe_Product).State = EntityState.Deleted;
            await db.SaveChangesAsync();
            return RedirectToAction("DetailsRecipe", "Home", new { id = recipeid });

        }

        [HttpGet]
        [ActionName("DeleteRecipe_TypeOfDish")]
        public async Task<IActionResult> ConfirmDeleteRecipe_TypeOfDish(int? recipeid, int? typeofdishid)
        {
            Recipe recipeidcontext = db.Recipes.FirstOrDefault(p => p.RecipeId == recipeid);
            if (recipeidcontext == null)
            {
                return BadRequest("No such order found for this user.");
            }
            IQueryable<Recipe_TypeOfDish> recipe_TypeOfDishes = db.Recipe_TypeOfDishes.Include(p => p.TypeOfDish);

            if (recipeid != null && recipeid != 0)
            {
                recipe_TypeOfDishes = recipe_TypeOfDishes.Where(p => p.RecipeId == recipeid);
            }
            if (typeofdishid != null && typeofdishid != 0)
            {
                var type = await recipe_TypeOfDishes.FirstOrDefaultAsync(sc => sc.TypeOfDishId == typeofdishid);
                if (type != null)
                    return View(type);
            }
            return NotFound();
        }
        [HttpPost]
        public async Task<IActionResult> DeleteRecipe_TypeOfDish(Recipe_TypeOfDish type, int? recipeid, int? typeofdishid)
        {
            if (recipeid == null && typeofdishid == null)
            {
                return BadRequest("Bad request");
            }
            Recipe_TypeOfDish recipe_TypeOfDish = new Recipe_TypeOfDish { RecipeId = type.RecipeId, TypeOfDishId = type.TypeOfDishId };
            db.Entry(recipe_TypeOfDish).State = EntityState.Deleted;
            await db.SaveChangesAsync();
            return RedirectToAction("DetailsRecipe", "Home", new { id = recipeid });

        }

        public async Task<IActionResult> EditMeasuringSystem(int? id)
        {
            if (id != null)
            {
                MeasuringSystem measuringSystem = await db.MeasuringSystems.FirstOrDefaultAsync(p => p.MeasuringSystemId == id);
                if (measuringSystem != null)
                    return View(measuringSystem);
            }
            return NotFound();
        }
        [HttpPost]
        public async Task<IActionResult> EditMeasuringSystem(MeasuringSystem measuringSystem)
        {
            db.MeasuringSystems.Update(measuringSystem);
            await db.SaveChangesAsync();
            return RedirectToAction("MeasuringSystem");
        }

        public async Task<IActionResult> EditNationalCuisine(int? id)
        {
            if (id != null)
            {
                NationalCuisine nationalCuisine = await db.NationalCuisines.FirstOrDefaultAsync(p => p.NationalCuisineId == id);
                if (nationalCuisine != null)
                    return View(nationalCuisine);
            }
            return NotFound();
        }
        [HttpPost]
        public async Task<IActionResult> EditNationalCuisine(NationalCuisine cuisine)
        {
            db.NationalCuisines.Update(cuisine);
            await db.SaveChangesAsync();
            return RedirectToAction("NationalCuisine");
        }

        public async Task<IActionResult> EditProduct(int? id)
        {
            SelectList productTypes = new SelectList(db.ProductTypes, "ProductTypeId", "ProductTypeName");
            ViewBag.ProductTypes = productTypes;

            SelectList measuringSystem = new SelectList(db.MeasuringSystems, "MeasuringSystemId", "MeasuringSystemName");
            ViewBag.MeasuringSystems = measuringSystem;

            if (id != null)
            {
                Product product = await db.Products.FirstOrDefaultAsync(p => p.ProductId == id);
                if (product != null)
                    return View(product);
            }
            return NotFound();
        }
        [HttpPost]
        public async Task<IActionResult> EditProduct(Product product)
        {
            db.Products.Update(product);
            await db.SaveChangesAsync();
            return RedirectToAction("Product");
        }

        public async Task<IActionResult> EditProductType(int? id)
        {
            if (id != null)
            {
                ProductType productType = await db.ProductTypes.FirstOrDefaultAsync(p => p.ProductTypeId == id);
                if (productType != null)
                    return View(productType);
            }
            return NotFound();
        }
        [HttpPost]
        public async Task<IActionResult> EditProductType(ProductType type)
        {
            db.ProductTypes.Update(type);
            await db.SaveChangesAsync();
            return RedirectToAction("ProductType");
        }

        public async Task<IActionResult> EditRecipe(int? id)
        {
            if (id != null)
            {
                Recipe recipe = await db.Recipes.FirstOrDefaultAsync(p => p.RecipeId == id);
                if (recipe != null)
                    return View(recipe);
            }
            return NotFound();
        }
        [HttpPost]
        public async Task<IActionResult> EditRecipe(IFormFile uploadedFile, Recipe recipe)
        {
            Recipe editRecipe = db.Recipes.Find(recipe.RecipeId);

            if (uploadedFile != null)
            {
                string shortFileName = uploadedFile.FileName.Substring(uploadedFile.FileName.LastIndexOf('\\') + 1);
                OriginalImage originalImage = await db.OriginalImages.FindAsync(editRecipe.OriginalImageId);
                originalImage.FileName = shortFileName;

                Directory.CreateDirectory(_appEnvironment.WebRootPath + "/Temp/");
                // путь к папке Temp
                string path = _appEnvironment.WebRootPath + "/Temp/";

                // сохраняем файл в папку Temp в каталоге wwwroot
                using (var fileStream = new FileStream(path + shortFileName, FileMode.Create))
                {
                    uploadedFile.CopyTo(fileStream);
                }

                using (var img = Image.Load(path + shortFileName))
                {
                    // as generate returns a new IImage make sure we dispose of it
                    using (Image<Rgba32> destRound = img.Clone(x => x.Resize(new Size(590, 0))))
                    {
                        destRound.Save(path + "bigImage_" + _userManager.GetUserName(HttpContext.User) + "_" + shortFileName);
                    }

                    using (Image<Rgba32> destRound = img.Clone(x => x.Resize(new Size(320, 0))))
                    {
                        destRound.Save(path + "previewImage_" + _userManager.GetUserName(HttpContext.User) + "_" + shortFileName);
                    }
                }

                byte[] bigImageData = System.IO.File.ReadAllBytes(path + "bigImage_" + _userManager.GetUserName(HttpContext.User) + "_" + shortFileName);

                byte[] previewImageData = System.IO.File.ReadAllBytes(path + "previewImage_" + _userManager.GetUserName(HttpContext.User) + "_" + shortFileName);

                byte[] originalImageData = null;
                // считываем переданный файл в массив байтов
                using (var binaryReader = new BinaryReader(uploadedFile.OpenReadStream()))
                {
                    originalImageData = binaryReader.ReadBytes((int)uploadedFile.Length);
                }
                // установка массива байтов
                originalImage.OriginalImageData = originalImageData;

                Directory.Delete(path, true);

                db.OriginalImages.Update(originalImage);

                editRecipe.RecipeName = recipe.RecipeName;
                editRecipe.Description = recipe.Description;
                editRecipe.BigImageData = bigImageData;
                editRecipe.PreviewImageData = previewImageData;

                db.Recipes.Update(editRecipe);
                await db.SaveChangesAsync();
            }
            else
            {
                editRecipe.RecipeName = recipe.RecipeName;
                editRecipe.Description = recipe.Description;

                db.Recipes.Update(editRecipe);
                await db.SaveChangesAsync();
            }
            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> EditRecipeDetail(int? recipedetailid, int? recipeid)
        {
            if (recipedetailid != null)
            {
                RecipeDetail recipeDetail = await db.RecipeDetails.FirstOrDefaultAsync(p => p.RecipeDetailId == recipedetailid);
                if (recipeDetail != null)
                    return View(recipeDetail);
            }
            return NotFound();
        }
        [HttpPost]
        public async Task<IActionResult> EditRecipeDetail(IFormFile uploadedFile, RecipeDetail recipeDetail)
        {
            RecipeDetail editRecipeDetail = db.RecipeDetails.Find(recipeDetail.RecipeDetailId);

            if (uploadedFile != null)
            {
                string shortFileName = uploadedFile.FileName.Substring(uploadedFile.FileName.LastIndexOf('\\') + 1);
                OriginalImage originalImage = await db.OriginalImages.FindAsync(editRecipeDetail.OriginalImageId);
                originalImage.FileName = shortFileName;

                Directory.CreateDirectory(_appEnvironment.WebRootPath + "/Temp/");
                // путь к папке Temp
                string path = _appEnvironment.WebRootPath + "/Temp/";

                // сохраняем файл в папку Temp в каталоге wwwroot
                using (var fileStream = new FileStream(path + shortFileName, FileMode.Create))
                {
                    uploadedFile.CopyTo(fileStream);
                }

                using (var img = Image.Load(path + shortFileName))
                {
                    // as generate returns a new IImage make sure we dispose of it
                    using (Image<Rgba32> destRound = img.Clone(x => x.Resize(new Size(590, 0))))
                    {
                        destRound.Save(path + "bigImage_" + _userManager.GetUserName(HttpContext.User) + "_" + shortFileName);
                    }

                    using (Image<Rgba32> destRound = img.Clone(x => x.Resize(new Size(320, 0))))
                    {
                        destRound.Save(path + "previewImage_" + _userManager.GetUserName(HttpContext.User) + "_" + shortFileName);
                    }
                }

                byte[] bigImageData = System.IO.File.ReadAllBytes(path + "bigImage_" + _userManager.GetUserName(HttpContext.User) + "_" + shortFileName);

                byte[] previewImageData = System.IO.File.ReadAllBytes(path + "previewImage_" + _userManager.GetUserName(HttpContext.User) + "_" + shortFileName);

                byte[] originalImageData = null;
                // считываем переданный файл в массив байтов
                using (var binaryReader = new BinaryReader(uploadedFile.OpenReadStream()))
                {
                    originalImageData = binaryReader.ReadBytes((int)uploadedFile.Length);
                }
                // установка массива байтов
                originalImage.OriginalImageData = originalImageData;

                Directory.Delete(path, true);

                db.OriginalImages.Update(originalImage);

                editRecipeDetail.Description = recipeDetail.Description;
                editRecipeDetail.BigImageData = bigImageData;
                editRecipeDetail.PreviewImageData = previewImageData;

                db.RecipeDetails.Update(editRecipeDetail);
                await db.SaveChangesAsync();
            }
            else
            {
                editRecipeDetail.Description = recipeDetail.Description;

                db.RecipeDetails.Update(editRecipeDetail);
                await db.SaveChangesAsync();
            }
            return RedirectToAction("DetailsRecipe", "Home", new { id = editRecipeDetail.RecipeId});
        }


        public async Task<IActionResult> EditTypeOfDish(int? id)
        {
            if (id != null)
            {
                TypeOfDish typeOfDish = await db.TypeOfDishes.FirstOrDefaultAsync(p => p.TypeOfDishId == id);
                if (typeOfDish != null)
                    return View(typeOfDish);
            }
            return NotFound();
        }
        [HttpPost]
        public async Task<IActionResult> EditTypeOfDish(TypeOfDish dish)
        {
            db.TypeOfDishes.Update(dish);
            await db.SaveChangesAsync();
            return RedirectToAction("TypeOfDish");
        }

        [HttpPost]
        public ActionResult FilterNationalCuisine(int? recipeid, string name)
        {
            Recipe recipeidcontext = db.Recipes.FirstOrDefault(p => p.RecipeId == recipeid);
            if (recipeidcontext == null)
            {
                return BadRequest("No such order found for this user.");
            }
            IQueryable<NationalCuisine> nationalCuisines = db.NationalCuisines;
            if (!String.IsNullOrEmpty(name))
            {
                nationalCuisines = nationalCuisines.Where(p => p.NationalCuisineName.Contains(name));
            }

            NationalCuisineListViewModel viewModel = new NationalCuisineListViewModel
            {
                NationalCuisines = nationalCuisines.ToList(),
                Name = name,
                Recipe = recipeidcontext
            };
            return View("SelectNationalCuisine", viewModel);
        }

        [HttpGet]
        public ActionResult SelectNationalCuisine(int? recipeid, string name)
        {
            Recipe recipeidcontext = db.Recipes.FirstOrDefault(p => p.RecipeId == recipeid);
            if (recipeidcontext == null)
            {
                return BadRequest("No such order found for this user.");
            }
            IQueryable<NationalCuisine> nationalCuisines = db.NationalCuisines;
            if (!String.IsNullOrEmpty(name))
            {
                nationalCuisines = nationalCuisines.Where(p => p.NationalCuisineName.Contains(name));
            }

            NationalCuisineListViewModel viewModel = new NationalCuisineListViewModel
            {
                NationalCuisines = nationalCuisines.ToList(),
                Name = name,
                Recipe = recipeidcontext
            };
            return View(viewModel);
            //return NotFound();
        }
        [HttpPost]
        public async Task<IActionResult> SelectNationalCuisine([FromQuery] Recipe recipe, [FromQuery] NationalCuisine nationalCuisine)
        {
            int nationalCuisineId = nationalCuisine.NationalCuisineId;
            int recipeId = recipe.RecipeId;
            Recipe_NationalCuisine recipe_NationalCuisine = new Recipe_NationalCuisine
            {
                NationalCuisineId = nationalCuisineId,
                RecipeId = recipeId
            };
            db.Recipe_NationalCuisines.Add(recipe_NationalCuisine);
            await db.SaveChangesAsync();
            return Ok("National cuisine added to recipe");
        }

        [HttpPost]
        public ActionResult FilterTypeOfDish(int? recipeid, string name)
        {
            Recipe recipeidcontext = db.Recipes.FirstOrDefault(p => p.RecipeId == recipeid);
            if (recipeidcontext == null)
            {
                return BadRequest("No such order found for this user.");
            }
            IQueryable<TypeOfDish> typeOfDishes = db.TypeOfDishes;
            if (!String.IsNullOrEmpty(name))
            {
                typeOfDishes = typeOfDishes.Where(p => p.TypeOfDishName.Contains(name));
            }

            TypeOfDishesListViewModel viewModel = new TypeOfDishesListViewModel
            {
                TypeOfDishes = typeOfDishes.ToList(),
                Name = name,
                Recipe = recipeidcontext
            };
            return View("SelectTypeOfDish", viewModel);
        }

        [HttpGet]
        public ActionResult SelectTypeOfDish(int? recipeid, string name)
        {
            Recipe recipeidcontext = db.Recipes.FirstOrDefault(p => p.RecipeId == recipeid);
            if (recipeidcontext == null)
            {
                return BadRequest("No such order found for this user.");
            }
            IQueryable<TypeOfDish> typeOfDishes = db.TypeOfDishes;
            if (!String.IsNullOrEmpty(name))
            {
                typeOfDishes = typeOfDishes.Where(p => p.TypeOfDishName.Contains(name));
            }

            TypeOfDishesListViewModel viewModel = new TypeOfDishesListViewModel
            {
                TypeOfDishes = typeOfDishes.ToList(),
                //ProductTypes = new SelectList(productTypes, "ProductTypeId", "ProductTypeName"),
                Name = name,
                Recipe = recipeidcontext
            };
            return View(viewModel);
            //return NotFound();
        }
        [HttpPost]
        public async Task<IActionResult> SelectTypeOfDish([FromQuery] Recipe recipe, [FromQuery] TypeOfDish typeOfDish)
        {
            int typeOfDishId = typeOfDish.TypeOfDishId;
            int recipeId = recipe.RecipeId;
            Recipe_TypeOfDish recipe_TypeOfDish = new Recipe_TypeOfDish
            {
                TypeOfDishId = typeOfDishId,
                RecipeId = recipeId
            };
            db.Recipe_TypeOfDishes.Add(recipe_TypeOfDish);
            await db.SaveChangesAsync();
            return Ok("Type of dish added to recipe");
        }

        [HttpPost]
        public ActionResult FilterProduct(int? recipeid, int? productTypeId, string name)
        {
            Recipe recipeidcontext = db.Recipes.FirstOrDefault(p => p.RecipeId == recipeid);
            if (recipeidcontext == null)
            {
                return BadRequest("No such order found for this user.");
            }

            IQueryable<Product> products = db.Products.Include(p => p.ProductType);
            if (productTypeId != null && productTypeId != 0)
            {
                products = products.Where(p => p.ProductTypeId == productTypeId);
            }
            if (!String.IsNullOrEmpty(name))
            {
                products = products.Where(p => p.ProductName.Contains(name));
            }

            List<ProductType> productTypes = db.ProductTypes.ToList();
            // устанавливаем начальный элемент, который позволит выбрать всех
            productTypes.Insert(0, new ProductType { ProductTypeName = "All type", ProductTypeId = 0 });

            ProductsListViewModel viewModel = new ProductsListViewModel
            {
                Products = products.ToList(),
                ProductTypes = new SelectList(productTypes, "ProductTypeId", "ProductTypeName"),
                Name = name,
                Recipe = recipeidcontext
            };
            return View("SelectProduct", viewModel);
        }

        [HttpGet]
        public ActionResult SelectProduct(int? recipeid, int? productTypeId, string name)
        {
            Recipe recipeidcontext = db.Recipes.FirstOrDefault(p => p.RecipeId == recipeid);
            if (recipeidcontext == null)
            {
                return BadRequest("No such order found for this user.");
            }
            IQueryable<Product> products = db.Products.Include(p => p.ProductType);
            if (productTypeId != null && productTypeId != 0)
            {
                products = products.Where(p => p.ProductTypeId == productTypeId);
            }
            if (!String.IsNullOrEmpty(name))
            {
                products = products.Where(p => p.ProductName.Contains(name));
            }

            List<ProductType> productTypes = db.ProductTypes.ToList();
            // устанавливаем начальный элемент, который позволит выбрать всех
            productTypes.Insert(0, new ProductType { ProductTypeName = "All type", ProductTypeId = 0 });

            ProductsListViewModel viewModel = new ProductsListViewModel
            {
                Products = products.ToList(),
                ProductTypes = new SelectList(productTypes, "ProductTypeId", "ProductTypeName"),
                Name = name,
                Recipe = recipeidcontext
            };
            return View(viewModel);
            //return NotFound();
        }
        [HttpPost]
        public async Task<IActionResult> SelectProduct([FromQuery] Recipe recipe, [FromQuery] Product product, ProductsListViewModel productsListViewModel)
        {
            int productId = product.ProductId;
            int recipeId = recipe.RecipeId;
            float quantity = productsListViewModel.Quantity;
            if (quantity == 0)
            {
                return BadRequest("Please specify a value other than '0' and using only digits (0-9) and decimal separator ','. Please go back and try again.");
            }
            Recipe_Product recipe_Product = new Recipe_Product
            {
                ProductId = productId,
                RecipeId = recipeId,
                Quantity = quantity,
                CreatedDate = DateTime.Now
            };
            db.Recipe_Products.Add(recipe_Product);
            await db.SaveChangesAsync();
            return Ok("Product added to recipe");
        }

    }
}