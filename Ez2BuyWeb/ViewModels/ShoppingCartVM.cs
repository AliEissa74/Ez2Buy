using Ez2Buy.DataAccess.Models;

namespace Ez2BuyWeb.ViewModels
{
    public class ShoppingCartVM
    {
        public IEnumerable<ShoppingCart> shoppingCartList { get; set; }
        public double OrderTotal { get; set; }
    }
}
