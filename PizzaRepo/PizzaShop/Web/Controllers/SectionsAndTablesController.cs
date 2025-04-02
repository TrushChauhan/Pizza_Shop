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
        public async Task<IActionResult> Index(){
            var sections = await _sectionTableService.GetSectionsAsync();
            return View("SectionsAndTables",sections);
        }
    }
}