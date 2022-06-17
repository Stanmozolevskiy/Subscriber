
namespace DataModels
{
    
    public class Credantials
    {
        public string Phone { get; set; }
        public string Subject { get; set; }
    }

    public class Data
    {
        public string Body { get; set; }
        public string Url { get; set; }
    }

    public class SubscriptionName
    {
        public SubscriptionName(string name)
        {
            Name = name;
        }
        public string Name { get; set; }
    }

}
