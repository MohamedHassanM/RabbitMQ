namespace RabbitMQ.Model
{
    [Serializable]
    public class Message
    {
        public Message(int ID, string Title, string Body, string To, string CC)
        {
            this.ID = ID;
            this.Title = Title+" "+ ID;
            this.Body = Body + " " + ID;
            this.To = To + " " + ID;
            this.CC = CC + " " + ID;
        } 

        public int ID { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public string To { get; set; }
        public string CC { get; set; }
    }
}