using Microsoft.AspNetCore.SignalR;
using WebAssessment.Models;

namespace WebAssessment.SignalR
{
    public class MarketHub : Hub
    {
        public void SendUpdatesToClients(List<long> deleteIds, List<ShopItem> updatedItems)
        {
            Clients?.All.SendAsync("ReceivedMessage", deleteIds, updatedItems);
        }
    }
}
