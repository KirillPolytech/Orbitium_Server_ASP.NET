using BackEndForGame.Contracts;
using BackEndForGame.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BackEndForGame.Controllers
{
    public class ItemController : ControllerBase
    {
        private ItemService _itemService;
        public ItemController(ItemService _service)
        {
            _itemService = _service;
        }


        [HttpGet("GetInventory")]
        [Authorize]
        public ActionResult<List<ItemData>> GetItemNames()
        {
            return new ActionResult<List<ItemData>>(_itemService.GetItems());
        }

        [HttpPost("BuyItem")]
        [Authorize]
        public ActionResult<bool> BuyItem([FromBody] string name)
        {
            string? uid = User?.Identities?.FirstOrDefault()?.Claims?.ToList().FirstOrDefault()?.Value;

            if (uid == null)
                return NotFound();

            return new ActionResult<bool>(_itemService.BuyItem(new Guid(uid), name));
        }
    }
}