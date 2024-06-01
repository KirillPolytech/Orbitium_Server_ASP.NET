using BackEndForGame.Contracts;
using BackEndForGame.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BackEndForGame.Controllers
{
    public class ItemStorageController : ControllerBase
    {
        private ItemStorageService _storageService;
        public ItemStorageController(ItemStorageService service)
        {
            _storageService = service;
        }

        [HttpGet("GetItemsData")]
        [AllowAnonymous]
        public ActionResult<List<ItemData>> GetItemsData()
        {
            return _storageService.GetItems();
        }
    }
}
