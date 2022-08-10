namespace DTO.Entidades
{
    public class RetornoToken
    {
        public bool sucess { get; set; }
        public string date_time { get; set; }
        public string domain { get; set; }
        public string username { get; set; }
        public string token { get; set; }
        public string validity { get; set; }
        public string message { get; set; }
    }
}
