using BackEndForGame.Contracts;
using BackEndForGame.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BackEndForGame.Controllers
{
    public class InventoryController : ControllerBase
    {
        private InventoryService _inventoryService;
        private ItemService _itemService;
        public InventoryController(InventoryService service, ItemService itemservice)
        {
            _inventoryService = service;
            _itemService = itemservice;
        }

        [HttpGet("GetItemsFromInventory")]
        [Authorize]
        public ActionResult<List<ItemData>>? GetItems()
        {
            string? uid = User?.Identities?.FirstOrDefault()?.Claims?.ToList().FirstOrDefault()?.Value;

            if (uid == null)
                return BadRequest();

            var t = _inventoryService.GetItemsFromInventory(new Guid(uid));

            if (t == null)
                return NotFound();

            return t;
        }

        [HttpPost("AddItemToInventory")]
        [Authorize]
        public ActionResult<bool> AddItem(ItemData data)
        {
            string? uid = User?.Identities?.FirstOrDefault()?.Claims?.ToList().FirstOrDefault()?.Value;

            if (uid == null)
                return BadRequest();

            var t = _itemService.GetItems().Where(x => x.Name == data.Name);

            if (t == null)
                return false;

            var f = _inventoryService.AddItem(data, uid);

            if (f == false)
                return BadRequest();

            return Ok();
        }
    }
}
