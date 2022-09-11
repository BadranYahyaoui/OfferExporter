using Microsoft.Extensions.Configuration;
using OfferExporter.Common;
using OfferExporter.Models;
using OfferExporter.Repositories.Abstract;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace OfferExporter.Repositories.Impl
{
    public class OfferRespository : IOfferRespository
    {
        private readonly IConfiguration _configuration;

        public OfferRespository(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task<IList<GetAllOffersResultRow>> GetAllOffersResultRows()
        {
            try
            {
                var connectionString = GlobalConfiguration.GenerateConnectionString();
                var offerResultRows = new List<GetAllOffersResultRow>();
                //var spName
                using (var connection = new SqlConnection(connectionString))
                using (var command = new SqlCommand(GlobalConfiguration.GetAllOffersCommand(), connection))
                {
                    await connection.OpenAsync();

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var row = new GetAllOffersResultRow
                            {
                                ProductPrid = reader.GetInt32(reader.GetOrdinal("ProductPrid")),
                                ReferentialId = reader.GetByte(reader.GetOrdinal("ReferentialId")),
                                ReferentialName = reader.GetString(reader.GetOrdinal("ReferentialName")),
                                ReferentialIsExportable = reader.GetBoolean(reader.GetOrdinal("ReferentialIsExportable")),
                                OfferId = reader.GetInt32(reader.GetOrdinal("OfferId")),
                                OfferIsActive = reader.GetBoolean(reader.GetOrdinal("OfferIsActive")),
                                OfferPrice = reader.GetDecimal(reader.GetOrdinal("OfferPrice")),
                                OfferQuantity = reader.GetInt16(reader.GetOrdinal("OfferQuantity")),
                                SellerId = reader.GetInt32(reader.GetOrdinal("SellerId")),
                                SellerName = reader.GetString(reader.GetOrdinal("SellerName")),
                                SellerIsActive = reader.GetBoolean(reader.GetOrdinal("SellerIsActive")),
                                PromotionId = reader.IsDBNull("PromotionId") ? null : reader.GetInt32(reader.GetOrdinal("PromotionId")),
                                PromotionReducedPrice = reader.IsDBNull("PromotionReducedPrice") ? null : reader.GetDecimal(reader.GetOrdinal("PromotionReducedPrice")),
                                PromotionTargetId = reader.IsDBNull("PromotionTargetId") ? null : reader.GetByte(reader.GetOrdinal("PromotionTargetId"))
                            };
                            offerResultRows.Add(row);
                        }
                    }

                    await connection.CloseAsync();
                    await connection.DisposeAsync();
                }

                Console.WriteLine($"{DateTime.Now.ToString("HH:mm:ss.fff")} Read offers from database: {offerResultRows.Count} found");
                return offerResultRows;

            }
            catch (Exception ex)
            {
                Console.WriteLine($"{DateTime.Now.ToString("HH:mm:ss.fff")} Read offers from database failed ! {ex.Message}");
                throw; // EXIT
            }
        }
    }
}
