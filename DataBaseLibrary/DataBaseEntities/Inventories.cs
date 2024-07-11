namespace BackEndForGame.Entities.DataBaseEntities
{
    public class Inventories
    {
        public int ID { get; set; }
        public required Guid uid { get; set; }
        public required int player_id { get; set; }
        public required DateTime create_time { get; set; }

        public Players? player { get; set; }
        public ICollection<Items> items { get; init; } = new List<Items>();
    }
}
