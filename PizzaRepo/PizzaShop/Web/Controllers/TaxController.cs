using Entity.Models;
using Microsoft.AspNetCore.Mvc;
using Service.Implementations;
using Service.Interfaces;

namespace Web.Controllers
{
public class TaxController : Controller
{
    private readonly ITaxService _taxService;
    
    public TaxController(ITaxService taxService){
        _taxService=taxService;
    }

    public async Task<IActionResult> Taxes(string search=null){
        var taxes= await _taxService.GetTaxesTableAsync(search);
        return View(taxes);
    }
    public async Task<IActionResult> DeleteTax(int taxid){
        await _taxService.DeleteTaxAsync(taxid);
        return Ok();
    }
}
}