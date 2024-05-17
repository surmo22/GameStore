namespace GameStore.Data
{
    public class Key
    {
        public int Id { get; set; }
        public Game Game { get; set; }
        public bool Activated { get; set; } = false;
        public string Value { get; set; } = "MyKey";
    }
}
