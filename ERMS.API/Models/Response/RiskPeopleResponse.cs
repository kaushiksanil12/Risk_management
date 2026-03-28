namespace ERMS.API.Models.Response
{
    public class RiskPeopleResponse
    {
        public int    Id               { get; set; }
        public int    RiskId           { get; set; }
        public int    OwnerId          { get; set; }
        public string OwnerName        { get; set; } = string.Empty;
        public string OwnerUsername    { get; set; } = string.Empty;
        public int    ChampionId       { get; set; }
        public string ChampionName     { get; set; } = string.Empty;
        public string ChampionUsername { get; set; } = string.Empty;
    }
}
