using GameStore.Infrastructure;
using Newtonsoft.Json;

namespace GameStore.Data.Cart
{
    public class SessionCart : Cart
    {
        [JsonIgnore]
        public ISession? Session { get; set; }

        public static Cart GetCart(IServiceProvider services)
        {
            ISession? session = services.GetRequiredService<IHttpContextAccessor>().HttpContext?.Session;
            SessionCart cart = session?.GetJson<SessionCart>("Cart") ?? new SessionCart();
            cart.Session = session;
            return cart;
        }

        public override void AddItem(Game game, int quantity)
        {
            base.AddItem(game, quantity);
            this.Session?.SetJson("Cart", this);
        }

        public override void RemoveLine(Game game)
        {
            base.RemoveLine(game);
            this.Session?.SetJson("Cart", this);
        }

        public override void Clear()
        {
            base.Clear();
            this.Session?.Remove("Cart");
        }
    }
}
