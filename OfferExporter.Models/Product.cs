using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace OfferExporter.Models
{
    public class Product
    {
        [DataMember(Name = "prid")]
        public string Key { get => $"{ReferentialId}-{Prid}"; }

        [IgnoreDataMember]
        public int Prid { get; set; }

        [IgnoreDataMember]
        public int ReferentialId { get; set; }

        [IgnoreDataMember]
        public string ReferentialName { get; set; }

        [DataMember(Name = "offers")]
        public IList<Offer> Offers { get; set; } = new List<Offer>();

        public override bool Equals(object? obj)
        {
            return obj is Product product
                && product.Prid == Prid
                && product.ReferentialId == ReferentialId;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 13;
                hash = hash * 11 + Prid.GetHashCode();
                hash = hash * 17 + ReferentialId.GetHashCode();
                return hash;
            }
        }
    }
}
