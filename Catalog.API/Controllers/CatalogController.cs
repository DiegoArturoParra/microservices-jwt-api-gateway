using Catalog.API.Infrastructure.Repository;
using Catalog.API.Model;
using Catalog.API.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace Catalog.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class CatalogController : ControllerBase
    {

        private readonly IUnitOfWork _uow;
        public CatalogController(IUnitOfWork uow)
        {
            _uow = uow;

        }

        [HttpGet]
        [Authorize]
        [Route("listar")]
        [ProducesResponseType(typeof(PaginatedItemsViewModel<Product>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> Products([FromQuery] int pageSize = 10, [FromQuery] int pageIndex = 0)
        {
            var totalItem = await _uow.Repository<Product>().LongCountAsync();
            var itemsOnPage = await _uow.Repository<Product>().GetAll().Include(y => y.Brand).OrderBy(c => c.Name).Skip(pageSize * pageIndex).Take(pageSize).ToListAsync();
            var model = new PaginatedItemsViewModel<Product>(pageIndex, pageSize, totalItem, itemsOnPage);
            return Ok(model);
        }

        [HttpPost]
        [Authorize]
        [Route("add-brands")]
        public async Task<IActionResult> AddBrand()
        {
            AddBrands();
            await _uow.Repository<Brand>().AddRange(_brands);
            await _uow.SaveChangesAsync();
            return Ok();
        }

        [HttpPost]
        [Authorize]
        [Route("add-products")]
        public async Task<IActionResult> AddProductsWithBrand()
        {
            AddProducts();
            await _uow.Repository<Product>().AddRange(_products);
            await _uow.SaveChangesAsync();
            return Ok();
        }

        private List<Product> _products;
        private List<Brand> _brands;


        private void AddBrands()
        {
            _brands = new List<Brand>
            {
                new Brand
                {
                    BrandName = "Samsung",
                    Description = "Samsung android phone"
                },
                 new Brand
                 {
                    BrandName = "Dell",
                    Description = "Dell laptop"
                },
                 new Brand
                 {
                    BrandName = "Apple",
                    Description = "Apple smartwatch"
                },
                 new Brand
                 {
                    BrandName = "HarperCollins",
                    Description = "Publisher of To Kill a Mockingbird"
                },
                 new Brand
                 {
                    BrandName = "Prentice Hall",
                    Description = "Publisher of Clean Code"
                },
                 new Brand
                 {
                    BrandName = "Scribner",
                    Description = "Publisher of The Joy of Cooking"
                }
            };
        }

        private void AddProducts()
        {
            _products = new List<Product> {

                new Product
                {
                    Name = "Samsung A72",
                    Description = "Samsung A72 android phone",
                    Price = 20000,
                    BrandId = 1,

                },
                 new Product
                {
                    Name = "Dell XPS 13",
                    Description = "Dell XPS 13 laptop",
                    Price = 1200,
                   BrandId = 2,
                },
                 new Product
                {
                    Name = "Apple Watch Series 7",
                    Description = "Apple Watch Series 7 smartwatch",
                    Price = 400,
                    BrandId = 3,
                },
                 new Product
                {
                    Name = "To Kill a Mockingbird",
                    Description = "Harper Lee's classic novel",
                    Price = 15,
                    BrandId = 4,
                },
                 new Product
                {
                    Name = "Clean Code",
                    Description = "A book on writing clean code by Robert C. Martin",
                    Price = 30,
                     BrandId = 5,
                },
                 new Product
                {
                    Name = "The Joy of Cooking",
                    Description = "A comprehensive cookbook",
                    Price = 25,
                    BrandId = 6,
                }
            };
        }
    }
}