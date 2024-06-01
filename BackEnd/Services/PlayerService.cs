using BackEndForGame.Configuration;
using BackEndForGame.Contracts;
using BackEndForGame.Entities.DataBaseEntities;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Data.Entity;

namespace BackEndForGame.Services
{
    public class PlayerService
    {
        private OrbitiumDataBaseContext _context;
        private InventoryService _inventoryService;
        public PlayerService(OrbitiumDataBaseContext context)
        {
            _context = context;
            _inventoryService = _context.GetService <InventoryService>();
        }

        public bool CreatePlayer(RegisterData Data)
        {
            Users? USER = _context.Users.
                Where(x => x.login == Data.Login).
                FirstOrDefault();

            if (USER == null)
                return false;

            _context.Players.Add(
                new Players { 
                ID = 0, 
                uid = Guid.NewGuid(),
                nick_name = Data.Nick, 
                orbs = 0, best_time = 0, 
                user_id = USER.ID,
                });

            _context.SaveChanges();

            var ind = _context.Players.Where(x => x.nick_name == Data.Nick).FirstOrDefault().ID;

            if (_inventoryService.CreateInventory(ind) == false)
                return false;
           
            return true;
        }

        public PlayerData? GetPlayerData(Guid uid)
        {
            var a = _context.Users.Include(x => x.player).FirstOrDefault();
            return _context.Users.Include(x => x.player).Where(x => x.uid == uid).
                Select(x => new PlayerData() 
                { 
                    Nick = x.player.nick_name,
                    Orbs = x.player.orbs,
                    Time = x.player.best_time }).
                FirstOrDefault();
        }

        public bool UpdatePlayerStatistic(PlayerData data)
        {
            Players? _player = _context.Players.Where(x => x.nick_name == data.Nick).ToList().FirstOrDefault();

            if (_player == null)
                return false;

            _player.orbs = data.Orbs;
            _player.best_time = data.Time;
            _context.SaveChanges();

            return true;
        }
    }
}
