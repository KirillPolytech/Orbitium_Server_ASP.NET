namespace BackEndForGame.Contracts
{
    public class PlayerData
    {
        public required  string Nick { get; set; }
        public required decimal Time { get; set; }
        public required int Orbs { get; set; }
    }
}