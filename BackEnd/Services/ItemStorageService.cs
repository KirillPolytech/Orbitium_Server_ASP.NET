using BackEndForGame.Configuration;
using BackEndForGame.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace BackEndForGame.Services
{
    public class ItemStorageService : ControllerBase
    {
        private OrbitiumDataBaseContext _context;
        public ItemStorageService(OrbitiumDataBaseContext context)
        {
            _context = context;
        }

        public List<ItemData> GetItems()
        {
            return _context.ItemStorage.Select(x => new ItemData()
            {
                Name = x.name,
                Price = x.price
            }).ToList();
        }
    }
}