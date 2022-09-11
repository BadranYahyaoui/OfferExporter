using System;
using System.Runtime.Serialization;

namespace OfferExporter.Models
{
    public class Offer
    {
        [IgnoreDataMember]
        public int Id { get; set; }

        [DataMember(Name = "seller")]
        public string Company { get; set; }

        [DataMember(Name = "price")]
        public decimal Price { get; set; }

        [DataMember(Name = "reducedPrice")]
        public decimal? ReducedPrice { get; set; }

        [DataMember(Name = "qty")]
        public int Quantity { get; set; }

        [DataMember(Name = "discountFor")]
        public string DiscountFor { get; set; }
    }


}
