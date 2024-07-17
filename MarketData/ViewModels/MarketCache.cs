using System.Collections.ObjectModel;
using Telerik.DataSource.Extensions;

namespace WebAssessment.ViewModels;

using WebAssessment.Models;
using WebAssessment.SignalR;
using WebAssessment.Simulator;

// This is a simple way to communicate the simulated data, you do not need to use this in your implementation.
public class MarketCache
{
    private readonly IMarketSimulator _marketSimulator;

    public readonly ObservableCollection<ShopItem> ShopItems;
    private readonly MarketHub _hub;
    protected object CacheLock = new object();

    public MarketCache(IMarketSimulator marketSimulator, 
        MarketHub hub)
    {
        _marketSimulator = marketSimulator;
        _hub = hub;
        ShopItems = new ObservableCollection<ShopItem>(_marketSimulator.GetShopItems(1000)); //1000 items
        _marketSimulator.ShopItemChanged -= OnShopItemChanged;
        _marketSimulator.ShopItemChanged += OnShopItemChanged;
        _marketSimulator.ShopItemChanged -= OnShopItemChangedSignalR;
        _marketSimulator.ShopItemChanged += OnShopItemChangedSignalR;
    }


    private void OnShopItemChangedSignalR(object? sender, ShopItemChangedEventArgs eventArgs)
    {
        _hub.SendUpdatesToClients(eventArgs.DeletedIds, eventArgs.NewValues);
    }


    private void OnShopItemChanged(object? sender, ShopItemChangedEventArgs eventArgs)
    {
        UpdateChangedItems(eventArgs.NewValues, eventArgs.DeletedIds);
    }

    private void UpdateChangedItems(List<ShopItem> newValues, List<long> deletedIds)
    {
        lock (CacheLock)
        {
            var newIds = newValues.ToDictionary(x => x.Id);
            var deleteIdSet = deletedIds.ToHashSet();
            int count = ShopItems.Count;
            for (int i = count - 1; i >= 0; i--)
            {
                var item = ShopItems[i];
                if (newIds.Remove(item.Id, out var newItem))
                {
                    ShopItems[i] = newItem;
                }
                else if (deleteIdSet.Remove(item.Id))
                {
                    ShopItems.RemoveAt(i);
                }
            }
            ShopItems.AddRange(newIds.Values);
        }
    }
}
