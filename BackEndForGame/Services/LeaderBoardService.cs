using BackEndForGame.Configuration;
using BackEndForGame.Contracts;
using BackEndForGame.Entities.DataBaseEntities;
using Microsoft.EntityFrameworkCore;

namespace BackEndForGame.Services
{
    public class LeaderBoardService
    {
        private OrbitiumDataBaseContext _context;
        public LeaderBoardService(OrbitiumDataBaseContext context)
        {
            _context = context;
        }

        public List<LeaderBoardData> GetLeaderBoards()
        {
            
            return _context.LeaderBoards.Include(x => x.player).
                Select(x => new LeaderBoardData() 
                { 
                    nick_name = x.player.nick_name, 
                    time = x.time,
                    create_time = x.create_time                    
                }).ToList().OrderBy(x => x.time).ToList();
        }

        public bool SetNewRecord(Guid uid)
        {
            LeaderBoards? record = _context?.
                Set<Users>().
                Where(x => x.uid == uid)?.
                Include(x => x.player).
                Select(x => new LeaderBoards()
                { 
                    player_id = x.player.ID, 
                    create_time = DateTime.Now, 
                    time = x.player.best_time, 
                    uid = Guid.NewGuid()
                }).ToList().
                FirstOrDefault();

            if (record == null)
                return false;

            _context?.LeaderBoards.Add(record);
            _context?.SaveChanges();

            return true;
        }
    }
}
