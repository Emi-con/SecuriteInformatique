using Sec.Market.MVC.Models;

namespace Sec.Market.MVC.Interfaces
{
    public interface IOrderService
    {
        Task<List<Order>> ObtenirSelonUser();

        Task<Order> Obtenir(int id);

        Task Ajouter(OrderData orderData);

        Task Supprimer(int id);
    }
}
