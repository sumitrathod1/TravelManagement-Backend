namespace TravelManagement.Models
{
        public class AgentCashCollection
        {
            public int Id { get; set; }
            public int AgentId { get; set; }
            public TravelAgent Agent { get; set; }
            public decimal AmountCollected { get; set; }
            public DateTime CollectionDate { get; set; }
        }
}
