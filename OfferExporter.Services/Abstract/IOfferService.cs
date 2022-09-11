using OfferExporter.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfferExporter.Services.Abstract
{
    public interface IOfferService
    {
        IList<GetAllOffersResultRow> RemoveEmptyRows();
        IList<GetAllOffersResultRow> RemoveActiveSeller(IList<GetAllOffersResultRow> allOffersResultRows);
        IList<GetAllOffersResultRow> RemoveUnexportedData(IList<GetAllOffersResultRow> allOffersResultRows);
        
    }
}
