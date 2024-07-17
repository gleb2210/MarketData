using WebAssessment.Models;

namespace WebAssessment.Simulator;

public interface IMarketSimulator
{
    double RefreshRateMilliseconds { get; set; }

    event EventHandler<ShopItemChangedEventArgs>? ShopItemChanged;

    IEnumerable<ShopItem> GetShopItems(int rows);
}