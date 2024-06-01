using BackEndForGame.Configuration;
using BackEndForGame.Contracts;
using BackEndForGame.Entities.DataBaseEntities;
using System.Data.Entity;

namespace BackEndForGame.Services
{
    public class ItemService
    {
        private OrbitiumDataBaseContext _context;
        public ItemService(OrbitiumDataBaseContext context)
        {
            _context = context;
        }

        public List<ItemData> GetItems()
        {
            return _context.Items.Select(x => new ItemData() {  Name = x.name, Price = x.price }).ToList();
        }

        public bool BuyItem(Guid uid, string name)
        {
            Users usr = _context.Users.Where(x => x.uid == uid).Include(x => x.player).Include(x => x.player.inventory).FirstOrDefault();

            if (usr == null)
                return false;

            //var itm = usr.player.inventory.items.Where(x => x.name == name).FirstOrDefault();

            //if (itm != null)
                //return false;

            Inventories inv = usr.player.inventory;

            Items item = _context.Items.Where(x => x.name == name).FirstOrDefault();

            if (item == null)
                return false;

            if (usr.player.orbs < item.price)
                return false;

            //inv.items.Add(item);
            usr.player.orbs -= item.price;

            return true;
        }
    }
}
