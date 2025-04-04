using AspNetCoreHero.ToastNotification.Abstractions;
using Entity.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Service.Interfaces;

namespace Web.Controllers
{
    public class SectionsAndTablesController : Controller
    {

        private readonly ISectionAndTableService _sectionTableService;
        private readonly INotyfService _notify;

        public SectionsAndTablesController(ISectionAndTableService sectionAndTableService, INotyfService notyfService)
        {
            _sectionTableService = sectionAndTableService;
            _notify = notyfService;
        }
        public async Task<IActionResult> Index()
        {
            List<SectionViewModel> sections = await _sectionTableService.GetSectionsAsync();
            return View("SectionsAndTables", sections);
        }
        [HttpGet]
        public async Task<IActionResult> GetTablesBySection(int sectionId, int page = 1, int pageSize = 5, string searchTerm = "")
        {
            try
            {
                var (tables, totalItems) = await _sectionTableService.GetTablesBySectionAsync(sectionId, page, pageSize, searchTerm);

                return Ok(new
                {
                    tables = tables,
                    totalItems = totalItems,
                    currentPage = page,
                    pageSize = pageSize
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500, "Internal server error");
            }
        }
        [HttpPost]
        public async Task<IActionResult> AddSection([FromBody] SectionViewModel model)
        {
            try
            {
                await _sectionTableService.AddSectionAsync(model);
                _notify.Custom("Section Added Successfully", 5, "Green", "fa-solid fa-check");
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
                _notify.Custom("Section Deleted Successfully", 5, "Green", "fa-solid fa-check");
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
                _notify.Custom("Section Updated Successfully", 5, "Green", "fa-solid fa-check");
                return Ok();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500, "Internal server error");
            }
        }
        public async Task<IActionResult> GetAllSections()
        {
            List<SectionViewModel> Sections = await _sectionTableService.GetSectionsAsync();
            return Ok(Sections);
        }
        public async Task<IActionResult> AddTable([FromBody] DinetableViewModel table)
        {
            try
            {
                await _sectionTableService.AddTableAsync(table);
                _notify.Custom("Table Added Successfully", 5, "Green", "fa-solid fa-check");
                return Ok();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500, "Internal server error");
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetTable(int id)
        {
            DinetableViewModel table = await _sectionTableService.GetTableByIdAsync(id);
            if (table == null)
            {
                return NotFound();
            }
            return Ok(table);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateTable([FromBody] DinetableViewModel model)
        {
            try
            {
                bool result = await _sectionTableService.UpdateTableAsync(model);
                if (!result)
                {
                    return NotFound();
                }
                _notify.Custom("Table Updated Successfully", 5, "Green", "fa-solid fa-check");
                return Ok();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500, "Internal server error");
            }
        }
        [HttpPost]
        public async Task<IActionResult> DeleteTable(int tableid)
        {
            try
            {
                await _sectionTableService.DeleteTableAsync(tableid);
                _notify.Custom("Table Deleted Successfully", 5, "Green", "fa-solid fa-check");
                return Ok();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return BadRequest();
            }
        }
        [HttpPost]
        public async Task<IActionResult> MassDeleteTables(string tableIds)
        {
            try
            {
                var ids = tableIds.Split(',').Select(int.Parse).ToList();
                await _sectionTableService.MassDeleteTablesAsync(ids);
                _notify.Custom("Tables Deleted Successfully", 5, "Green", "fa-solid fa-check");
                return Ok();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return BadRequest();
            }
        }
    }
}