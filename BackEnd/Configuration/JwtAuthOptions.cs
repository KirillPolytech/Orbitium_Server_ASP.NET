namespace BackEndForGame.Configuration
{
    public class JwtAuthOptions
    {
        public required string key {  get; set; }
        public required string Issuer { get; set; }
        public required string Audience { get; set; }
    }
}
