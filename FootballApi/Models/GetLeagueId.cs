namespace GetLeagueModels
{
    public class GetLeague
    {
        public List<ResponseLeague> Response { get; set; }
        public GetLeague()
        {
            Response = new List<ResponseLeague>();
        }
    }

    public class ResponseLeague
    {
        public League1 league { get; set; }
        public ResponseLeague()
        {
            League1 league = new League1();
        }
    }

    public class League1
    {
        public int id { get; set; }
        public string name { get; set; }
    }

}