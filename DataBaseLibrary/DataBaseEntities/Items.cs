namespace BackEndForGame.Entities.DataBaseEntities
{
    public class Items
    {
        public int ID { get; set; }
        public required Guid uid { get; set; }
        public required string name { get; set; }
        public required int price { get; set; }
        public required DateTime create_time { get; set; }
        public required int inventory_id { get; set; }

        public Inventories? inventory { get; set; }
    }
}
