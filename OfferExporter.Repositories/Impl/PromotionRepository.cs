using OfferExporter.Common;
using OfferExporter.Models;
using OfferExporter.Repositories.Abstract;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfferExporter.Repositories.Impl
{
    public class PromotionRepository : IPromotionRepository
    {
        public async Task<IList<GetAllPromotionTargetsResultRow>> GetAllOffersResultRows()
        {
            try
            {
                var connectionString = GlobalConfiguration.GenerateConnectionString();

                var promotionTargetsResultRows = new List<GetAllPromotionTargetsResultRow>();

                using (var connection = new SqlConnection(connectionString))
                using (var command = new SqlCommand(GlobalConfiguration.GetAllPromotionTargetsCommand(), connection))
                {
                    await connection.OpenAsync();

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var row = new GetAllPromotionTargetsResultRow
                            {
                                Id = reader.GetByte(reader.GetOrdinal("Id")),
                                Name = reader.GetString(reader.GetOrdinal("Name")),
                            };
                            promotionTargetsResultRows.Add(row);
                        }
                    }

                    await connection.CloseAsync();
                    await connection.DisposeAsync();
                }

                Console.WriteLine($"{DateTime.Now.ToString("HH:mm:ss.fff")} Read offers from database: {promotionTargetsResultRows.Count} found");
                return promotionTargetsResultRows;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{DateTime.Now.ToString("HH:mm:ss.fff")} Read offers from database failed ! {ex.Message}");
                throw;
            }
        }
    }
}
