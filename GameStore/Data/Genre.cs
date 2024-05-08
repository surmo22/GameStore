namespace GameStore.Data
{
    public class Genre
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public ICollection<Game>? Games { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
