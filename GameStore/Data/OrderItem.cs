using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameStore.Data
{
    public class OrderItem
    {
        public int Id { get; set; }
#pragma warning disable CS8618 
        public IdentityUser IdentityUser { get; set; }
        [ForeignKey("Game")]
        public int GameId { get; set; }
        public Game Game { get; set; }
        [ForeignKey("Key")]
        public int KeyId { get; set; }
        public Key Key { get; set; }
#pragma warning restore CS8618 
    }
}
