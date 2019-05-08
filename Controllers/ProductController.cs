using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NG_Core_Auth.Data;
using NG_Core_Auth.Models;

namespace NG_Core_Auth.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ProductController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("[action]")]
        [Authorize(Policy = "RequiredLoggedIn")]
        public IActionResult GetProducts()
        {
            return Ok(_context.Products.ToList());
        }

        // add
        [HttpPost("[action]")]
        [Authorize(Policy = "RequiredAdministratorRole")]
        public async Task<IActionResult> AddProduct([FromBody] ProductModel formdata)
        {
            var newProduct = new ProductModel
            {
                Name = formdata.Name,
                ImageUrl = formdata.ImageUrl,
                Description = formdata.Description,
                OutOfStock = formdata.OutOfStock,
                Price = formdata.Price
            };

            await _context.Products.AddAsync(newProduct);
            await _context.SaveChangesAsync();

            return Ok(newProduct);
        }

        //update
        [HttpPut("[action]/{id}")]
        [Authorize(Policy = "RequiredAdministratorRole")]
        public async Task<IActionResult> UpdateProduct([FromRoute] int id, [FromBody] ProductModel formdata)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var findProduct = _context.Products.FirstOrDefault(p => p.ProductId == id);
            if (findProduct == null)
            {
                return NotFound();
            }

            findProduct.Name = formdata.Name;
            findProduct.ImageUrl = formdata.ImageUrl;
            findProduct.Description = formdata.Description;
            findProduct.OutOfStock = formdata.OutOfStock;
            findProduct.Price = formdata.Price;

            _context.Entry(findProduct).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok(new JsonResult(findProduct));
        }

        [HttpDelete("[action]/{id}")]
        [Authorize(Policy = "RequiredAdministratorRole")]
        public async Task<IActionResult> DeleteProduct([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // find the product
            var findProduct = await _context.Products.FindAsync(id);

            if (findProduct == null)
            {
                return NotFound();
            }

            _context.Products.Remove(findProduct);
            await _context.SaveChangesAsync();

            return Ok(new JsonResult("Product with id: " + id + " has been deleted!!!"));
        }


        //[HttpGet("[action]")]
        //public async Task<ActionResult<IEnumerable<ProductModel>>> GetProducts()
        //{
        //    return await _context.Products.ToListAsync();
        //}

        //[HttpPost("[action]")]
        //public async Task<IActionResult> AddProduct([FromBody] ProductModel product)
        //{
        //    _context.Products.Add(product);
        //    await _context.SaveChangesAsync();

        //    return CreatedAtAction("GetProducts", new { id = product.ProductId }, product);
        //}

        //// GET: api/Product
        //[HttpGet]
        //public async Task<ActionResult<IEnumerable<ProductModel>>> GetProducts()
        //{
        //    return await _context.Products.ToListAsync();
        //}

        //// GET: api/Product/5
        //[HttpGet("{id}")]
        //public async Task<ActionResult<ProductModel>> GetProductModel(int id)
        //{
        //    var productModel = await _context.Products.FindAsync(id);

        //    if (productModel == null)
        //    {
        //        return NotFound();
        //    }

        //    return productModel;
        //}

        //// PUT: api/Product/5
        //[HttpPut("{id}")]
        //public async Task<IActionResult> PutProductModel(int id, ProductModel productModel)
        //{
        //    if (id != productModel.ProductId)
        //    {
        //        return BadRequest();
        //    }

        //    _context.Entry(productModel).State = EntityState.Modified;

        //    try
        //    {
        //        await _context.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!ProductModelExists(id))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return NoContent();
        //}

        //// POST: api/Product
        //[HttpPost]
        //public async Task<ActionResult<ProductModel>> PostProductModel(ProductModel productModel)
        //{
        //    _context.Products.Add(productModel);
        //    await _context.SaveChangesAsync();

        //    return CreatedAtAction("GetProductModel", new { id = productModel.ProductId }, productModel);
        //}

        //// DELETE: api/Product/5
        //[HttpDelete("{id}")]
        //public async Task<ActionResult<ProductModel>> DeleteProductModel(int id)
        //{
        //    var productModel = await _context.Products.FindAsync(id);
        //    if (productModel == null)
        //    {
        //        return NotFound();
        //    }

        //    _context.Products.Remove(productModel);
        //    await _context.SaveChangesAsync();

        //    return productModel;
        //}

        //private bool ProductModelExists(int id)
        //{
        //    return _context.Products.Any(e => e.ProductId == id);
        //}
    }
}
