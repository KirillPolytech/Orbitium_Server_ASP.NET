namespace BackEndForGame.Entities.DataBaseEntities
{
    public class Players
    {
        public int ID { get; set; }
        public required int user_id { get; set; }
        public required Guid uid { get; set; }
        public required string nick_name { get; set; }
        public required decimal best_time { get; set; }
        public required int orbs { get; set; }

        public Users? user { get; set; }
        public Inventories? inventory { get; set; }
        public LeaderBoards? leaderBoard { get; set; }
    }
}
