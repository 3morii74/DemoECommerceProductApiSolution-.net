using eCommmerce.SharedLibrary.Logs;
using eCommmerce.SharedLibrary.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using productApi.Domain.Entities;
using ProductApi.Application.DTOs;
using ProductApi.Application.DTOs.Conversions;
using ProductApi.Application.Inerfaces;
using System.Collections.Generic;

namespace ProductApi.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController(IProduct productInterface) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDTO>>> GetProducts()
        {
            //get data from repo
            var products = await productInterface.GetAllAsync();
            if (!products.Any())
            {
                return NotFound("NO products Detected in database");
            }
            // convert data from entity to DTO
            var (_, list) = ProductConversion.FromEntity(null!, products);
            return list!.Any() ? Ok(list) : NotFound("No products found");
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ProductDTO>> GetProduct(int id)
        {
            var product = await productInterface.FindByIdAsync(id);
            if (product is null)
            {
                return NotFound($"NO product by this id {id}");
            }
            var (_product, _) = ProductConversion.FromEntity(product, null!);
            return _product is not null ? Ok(_product) : NotFound("No product found");
        }

        [HttpPost]
        public async Task<ActionResult<Response>> CreateProduct(ProductDTO product)
        {
            // el validation elly gwa el ProductDTO b t record fi el model state automatically
            // Check if the model state is valid (i.e., all data annotations are passed)
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Convert the DTO to an entity
            var getEntity = ProductConversion.ToEntity(product);

            // Create the product asynchronously using the product interface
            var response = await productInterface.CreateAsync(getEntity);

            // Return the response based on the success flag
            return response.flag is true ? Ok(response) : BadRequest(response);
        }

        [HttpPut]
        public async Task<ActionResult<Response>> UpdateProduct(ProductDTO product)
        {
            // Check if the model state is valid (i.e., all data annotations are passed)
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Convert the DTO to an entity
            var getEntity = ProductConversion.ToEntity(product);

            // Update the product asynchronously using the product interface
            var response = await productInterface.UpdateAsync(getEntity);

            // Return the response based on the success flag
            return response.flag is true ? Ok(response) : BadRequest(response);
        }

        [HttpDelete]
        public async Task<ActionResult<Response>> DeleteProduct(ProductDTO productDto)
        {
            try
            {
                // Retrieve the existing product from the database
                var existingProduct = await productInterface.FindByIdAsync(productDto.Id);
                if (existingProduct == null)
                {
                    return NotFound(new Response(false, "Product not found."));
                }

                // Delete the product asynchronously using the product interface
                var response = await productInterface.DeleteAsync(existingProduct);

                // Return the response based on the success flag
                return response.flag ? Ok(response) : BadRequest(response);
            }
            catch (Exception ex)
            {
                // Log the exception for debugging
                LogException.LogExceptions(ex);

                // Return a generic error message to the client
                return StatusCode(500, new Response(false, "An error occurred while deleting the product."));
            }
        }
    }
}