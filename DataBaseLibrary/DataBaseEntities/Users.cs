namespace BackEndForGame.Entities.DataBaseEntities
{
    public class Users
    {
        public int ID { get; set; }
        public required Guid uid { get; set; }
        public required string login { get; set; }
        public required byte[] password { get; set; }
        public DateTime create_time { get; set; }

        public Players? player { get; set; }
    }
}