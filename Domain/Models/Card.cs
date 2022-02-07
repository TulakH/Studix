using Domain.Interfarces;

namespace Domain.Models
{
    public class Card : IEntity
    {
        public Guid Id { get; set; }
        public string? GroupName {get; set;}

        public string? Front {get; set;}
        
        public string? Back {get; set;}
    }
}