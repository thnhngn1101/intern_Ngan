using System.Text;
using Application.Settings;
using BPMaster.Services;
using Common.Controllers;
using DotNetTraining.Domains.Dtos;
using DotNetTraining.Domains.Entities;
using DotNetTraining.Services;
using iText.Commons.Actions.Data;
using Microsoft.AspNetCore.Mvc;

[Route("api/product")]
[ApiController]
public class ProductController : BaseV1Controller<ProductService, ApplicationSetting>
{
    private readonly ProductService _productService;

    public ProductController(IServiceProvider services, IHttpContextAccessor httpContextAccessor) : base(services, httpContextAccessor)
    {
        this._productService = services.GetService<ProductService>()!;
    }

    [HttpGet("getAllProduct")]
    public async Task<IActionResult> GetAllProduct()
    {
        var product = await _productService.GetAllProduct();
        return Success(product);
    }

    [HttpGet("{productId}")]
    public async Task<IActionResult> GetProductById(Guid productId)
    {
        return Success(await _productService.GetProductById(productId));
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateProduct([FromBody] ProductDto dto)
    {
        return CreatedSuccess(await _productService.CreateProduct(dto));
    }

    [HttpDelete("{productId}")]
    public async Task<IActionResult> DeleteProduct(Guid productId)
    {

        await _productService.DeleteProduct(productId);
        return Success("Delete Success");
    }

    [HttpPut("{productId}")]
    public async Task<IActionResult> UpdateProduct(Guid productId, [FromBody] ProductDto productdto)
    {
        return Success(await _productService.UpdateProduct(productId, productdto));
    }


}


