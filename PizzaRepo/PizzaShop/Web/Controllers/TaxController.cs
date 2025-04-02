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
        {
            var taxes = await _taxService.GetTaxesTableAsync(search);
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
            await _taxService.AddTaxAsync(model);
            return Ok();
        }
    }
}