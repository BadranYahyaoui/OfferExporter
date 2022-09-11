using OfferExporter.Models;
using OfferExporter.Repositories.Abstract;
using OfferExporter.Services.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfferExporter.Services.Impl
{
    public class OfferService : IOfferService
    {
        private readonly IOfferRespository _offerRespository;

        public OfferService(IOfferRespository offerRespository)
        {
            _offerRespository = offerRespository;
        }

     

        public IList<GetAllOffersResultRow> RemoveActiveSeller(IList<GetAllOffersResultRow> allOffersResultRows)
        {
            for (int j = allOffersResultRows.Count - 1; j >= 0; j--)
            {
                var row = allOffersResultRows[j];
                if (!row.SellerIsActive)
                    allOffersResultRows.Remove(row);
            }
            return allOffersResultRows;
        }

        public IList<GetAllOffersResultRow> RemoveEmptyRows()
        {
            var offerResultRows = _offerRespository.GetAllOffersResultRows().Result;

            for (int i = offerResultRows.Count - 1; i >= 0; i--)
            {
                var row = offerResultRows[i];

                if (!row.OfferIsActive)
                {
                    offerResultRows.Remove(row);
                    continue;
                }

                if (row.OfferPrice <= 0)
                {
                    offerResultRows.Remove(row);
                    continue;
                }

                if (row.PromotionReducedPrice.HasValue)
                {
                    if (row.PromotionReducedPrice <= 0)
                    {
                        offerResultRows.Remove(row);
                        continue;
                    }

                    // Quick-fix: when incoherent promotion we ignore it
                    if (row.OfferPrice <= row.PromotionReducedPrice)
                    {
                        row.PromotionId = null;
                        row.PromotionReducedPrice = null;
                        row.PromotionTargetId = null;
                    }
                }

                if (row.OfferQuantity <= 0)
                {
                    offerResultRows.Remove(row);
                    continue;
                }
            }

            return offerResultRows;
        }

        public IList<GetAllOffersResultRow> RemoveUnexportedData(IList<GetAllOffersResultRow> allOffersResultRows)
        {
            for (int k = allOffersResultRows.Count - 1; k >= 0; k--)
            {
                var row = allOffersResultRows[k];
                if (!row.ReferentialIsExportable)
                {
                    allOffersResultRows.Remove(row);
                    continue;
                }
            }
            return allOffersResultRows;
        }
        
    }
}
