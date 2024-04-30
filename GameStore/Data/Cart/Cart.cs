namespace GameStore.Data.Cart
{
    public class Cart
    {
        private readonly List<CartItem> lines = [];

        public IReadOnlyList<CartItem> Lines
        {
            get { return lines; }
        }

        public virtual void AddItem(Game game, int quantity)
        {
            CartItem? line = lines.Find(p => p.Game.Id == game.Id);

            if (line is null)
            {
                lines.Add(new CartItem
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
            => lines.RemoveAll(l => l.Game.Id == game.Id);

        public decimal ComputeTotalValue()
            => lines.Sum(e => e.Game.Price * e.Quantity);

        public virtual void Clear() => lines.Clear();
    }
}
