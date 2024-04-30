namespace GameStore.Data
{
    public class Cart
    {
        private readonly List<CartItem> lines = [];

        public IReadOnlyList<CartItem> Lines
        {
            get { return this.lines; }
        }

        public virtual void AddItem(Game game, int quantity)
        {
            CartItem? line = this.lines.Find(p => p.Game.Id == game.Id);

            if (line is null)
            {
                this.lines.Add(new CartItem
                {
                    Game = game,
                    Quantity = quantity,
                });
            }
            else
            {
                line.Quantity += quantity;
            }
        }

        public virtual void RemoveLine(Game game)
            => this.lines.RemoveAll(l => l.Game.Id == game.Id);

        public decimal ComputeTotalValue()
            => this.lines.Sum(e => e.Game.Price * e.Quantity);

        public virtual void Clear() => this.lines.Clear();
    }
}
