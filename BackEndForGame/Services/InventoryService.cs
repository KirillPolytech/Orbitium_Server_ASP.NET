using BackEndForGame.Configuration;
using BackEndForGame.Contracts;
using BackEndForGame.Entities.DataBaseEntities;
using Microsoft.EntityFrameworkCore;

namespace BackEndForGame.Services
{
    public class InventoryService
    {
        private OrbitiumDataBaseContext _context;
        public InventoryService(OrbitiumDataBaseContext context)
        {
            _context = context;
        }

        public List<ItemData>? GetItemsFromInventory(Guid uid)
        {
            Users? usr = _context.
                Set<Users>().
                Where(x => x.uid == uid).
                Include(x => x.player).
                Include(x => x.player.inventory).
                FirstOrDefault();      

            if (usr == null)
                return null;

            Inventories? inv = usr.player.inventory;

            if (inv == null)
                return null;
            
            return inv.items?.Select( x => new ItemData() { Name = x.name, Price = x.price } )?.ToList();
        }

        public bool AddItem(ItemData data, string uid)
        {
            
            var a = _context.Users.
                Where(x => x.uid == new Guid(uid)).
                Include(x => x.player).
                Include(x => x.player.inventory).
                FirstOrDefault();

            var z = a.player.inventory.items.Where(x => x.name == data.Name).FirstOrDefault();

            if (z != null)
                return false;

            var item = new Items()
            {
                inventory_id = a.player.inventory.ID,
                name = data.Name,
                price = data.Price,
                uid = Guid.NewGuid(),
                create_time = DateTime.Now

            };

            _context.Items.Add(item);            

            _context.SaveChanges();

            return true;
        }

        public bool CreateInventory(int id)
        {
            _context.Inventories.Add(new Inventories() 
            { 
                player_id = id, 
                uid = Guid.NewGuid(),
                create_time = DateTime.Now,
            });

            _context.SaveChanges();

            return true;
        }
    }
}