using OfferExporter.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OfferExporter.Repositories.Abstract
{
    public interface IPromotionRepository
    {
        public Task<IList<GetAllPromotionTargetsResultRow>> GetAllOffersResultRows();

    }
}
