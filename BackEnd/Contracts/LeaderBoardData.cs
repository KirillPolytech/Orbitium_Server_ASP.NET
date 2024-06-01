namespace BackEndForGame.Contracts
{
    public class LeaderBoardData
    {
        public required string nick_name { get; set; }
        public required decimal time { get; set; }
        public required DateTime create_time { get; set; }
    }
}
