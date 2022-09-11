using OfferExporter.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfferExporter.Services.Abstract
{
    public interface IProductService
    {
       
        IList<Product> BuildProducts(IList<GetAllOffersResultRow> allOffersResultRows);
       
    }
}
