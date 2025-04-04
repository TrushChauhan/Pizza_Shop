using AspNetCoreHero.ToastNotification.Abstractions;
using Entity.Models;
using Entity.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.Implementations;
using Service.Interfaces;
using Web.Filters;

namespace Web.Controllers
{
    public class TaxController : Controller
    {

        private readonly ITaxService _taxService;
        public TaxController(ITaxService taxService)
        {
            _taxService = taxService;
        }
        [Authorize]
        public async Task<IActionResult> Taxes(string search = null)
        {   ViewBag.SearchTerm=search;
            List<TaxViewModel> taxes = await _taxService.GetTaxesTableAsync(search);
            return View(taxes);
        }

        [HttpPost]
        [PermissionAuthorize("TaxAndFee", "delete")]
        public async Task<IActionResult> DeleteTax(int taxid)
        {
            try
            {
                await _taxService.DeleteTaxAsync(taxid);
                return Ok();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return BadRequest();
            }
        }
        [HttpPost]
        public async Task<IActionResult> AddTax([FromBody] TaxViewModel model)
        {
            try
            {
                await _taxService.AddTaxAsync(model);
                return Ok();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500, "Internal server error");
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetTax(int id)
        {
            var tax = await _taxService.GetTaxByIdAsync(id);
            if (tax == null)
            {
                return NotFound();
            }
            return Ok(tax);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateTax([FromBody] TaxViewModel model)
        {
            try
            {
                bool result = await _taxService.UpdateTaxAsync(model);
                if (!result)
                {
                    return NotFound();
                }
                return Ok();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500, "Internal server error");
            }
        }
    }
}