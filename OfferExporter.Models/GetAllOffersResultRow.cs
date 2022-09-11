using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfferExporter.Models
{
    public class GetAllOffersResultRow
    {
        public int ProductPrid { get; init; }
        public byte ReferentialId { get; init; }
        public string ReferentialName { get; init; }
        public bool ReferentialIsExportable { get; init; }
        public int OfferId { get; init; }
        public bool OfferIsActive { get; init; }
        public decimal OfferPrice { get; init; }
        public short OfferQuantity { get; init; }
        public int SellerId { get; init; }
        public string SellerName { get; init; }
        public bool SellerIsActive { get; init; }
        public int? PromotionId { get; set; } // Quick-fix: 'init' has been changed by 'set' so we can set the value to null
        public decimal? PromotionReducedPrice { get; set; } // Quick-fix: idem
        public byte? PromotionTargetId { get; set; } // Quick-fix: idem
    }
}
