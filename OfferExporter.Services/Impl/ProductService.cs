using OfferExporter.Models;
using OfferExporter.Repositories.Abstract;
using OfferExporter.Services.Abstract;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfferExporter.Services.Impl
{
    public class ProductService : IProductService
    {
        private readonly IPromotionRepository _promotionRepository;

        public ProductService(IPromotionRepository promotionRepository)
        {
            _promotionRepository = promotionRepository;
        }
        public IList<Product> BuildProducts(IList<GetAllOffersResultRow> offerResultRows)
        {
            var promotionTargetsResultRows = _promotionRepository.GetAllOffersResultRows().Result;
            List<Product> products = new();

            for (int m = 0; m < offerResultRows.Count; m++)
            {
                var row = offerResultRows[m];

                //var product = new Product
                //{
                //    Prid = row.ProductPrid,
                //    ReferentialId = row.ReferentialId,
                //    ReferentialName = row.ReferentialName
                //};
                var product = CreateFromRow<Product>(row);//Products Mapping

                var found = products.FirstOrDefault(p => product.Equals(p));
                if (found is not null)
                {
                    product = found;
                }
                else
                {
                    products.Add(product);
                }

                product.Offers.Add(new Offer
                {
                    Id = row.OfferId,
                    Company = row.SellerName,
                    Price = row.OfferPrice,
                    ReducedPrice = row.PromotionReducedPrice,
                    Quantity = row.OfferQuantity,
                    DiscountFor = promotionTargetsResultRows.FirstOrDefault(x => x.Id == row.PromotionTargetId)?.Name //instead of inner join
                });

                if (m == (offerResultRows.Count - 1))
                {
                    products.Add(product);
                }
            }
            return OrderProducts(products);
        }

        private IList<Product> OrderProducts(IList<Product> products)
        {
            // Quick-fix: Sort the products and offers (so the generate file is always the same)

            return products
                .Select(product =>
                {
                    product.Offers = product.Offers
                        .OrderBy(offer => offer.Id)
                        .ToList();
                    return product;
                })
                .OrderBy(product => product.ReferentialId)
                .ThenBy(product => product.Prid)
                .ToList();
        }
        private IList<Product> ParallelOrderProducts(IList<Product> products)
        {
            // Quick-fix: Sort the products and offers (so the generate file is always the same)


            return products
                .AsParallel()
                .Select(product =>
                {
                    product.Offers = product.Offers
                        .OrderBy(offer => offer.Id)
                        .ToList();
                    return product;
                })
                .OrderBy(product => product.ReferentialId)
                .ThenBy(product => product.Prid)
                .ToList();
            
        }

        private static T CreateFromRow<T>(GetAllOffersResultRow row) where T : Product, new()
        {
            return new T
            {
                Prid = row.ProductPrid,
                ReferentialId = row.ReferentialId,
                ReferentialName = row.ReferentialName
            };
        }

    }
}
