using Entity.Models;
using Entity.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.Implementations;
using Service.Interfaces;

namespace Web.Controllers
{
    public class SectionsAndTablesController : Controller
    {

        private readonly ISectionAndTableService _sectionTableService;

        public SectionsAndTablesController(ISectionAndTableService sectionAndTableService)
        {
            _sectionTableService = sectionAndTableService;
        }
        public async Task<IActionResult> Index()
        {
            List<SectionViewModel> sections = await _sectionTableService.GetSectionsAsync();
            return View("SectionsAndTables", sections);
        }
        [HttpPost]
        public async Task<IActionResult> AddSection([FromBody] SectionViewModel model)
        {
            try
            {
                await _sectionTableService.AddSectionAsync(model);
                return Ok();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500, "Internal server error");
            }
        }
        public async Task<IActionResult> DeleteSection(int sectionId)
        {
            try
            {
                await _sectionTableService.DeleteSectionAsync(sectionId);
                return Ok();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return BadRequest();
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetSection(int id)
        {
            SectionViewModel section = await _sectionTableService.GetSectionByIdAsync(id);
            if (section == null)
            {
                return NotFound();
            }
            return Ok(section);
        }
                [HttpPost]
        public async Task<IActionResult> UpdateSection([FromBody] SectionViewModel model)
        {
            try
            {
                bool result = await _sectionTableService.UpdateSectionAsync(model);
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