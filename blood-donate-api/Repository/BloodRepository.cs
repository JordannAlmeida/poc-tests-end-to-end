
using blood_donate_api.Models;
using blood_donate_api.Repository.Interfaces;
using Dapper;

namespace blood_donate_api.Repository
{
    public class BloodRepository : SqlServerDb, IBloodRepository
    {
        public BloodRepository(string connectionString) : base(connectionString)
        {
        }

        public async Task<IEnumerable<BloodStockModel>> GetBloodStockModelByTypeAndRhFactorAsync(string bloodType, string rhFactor)
        {
            string sql = @"Select
                           BLOOD_TYPE as BloodType,
                           RH_FACTOR as RhFactor,
                           QUANTITY_ML as Quantity,
                           DATE_OF_DONATE as DateOfDonate
                           FROM TBL_BLOOD_DONATE WHERE BLOOD_TYPE = @BloodType AND RH_FACTOR = @RhFactor";

            using var connection = await GetConnectionAsync();
            var parameters = new { BloodType = bloodType, RhFactor = rhFactor };

            return await connection.QueryAsync<BloodStockModel>(sql, parameters);
        }

        public async Task<int> RegisterDonateAsync(UserModel userModel, BloodStockModel bloodStockModel)
        {
            using var connection = await GetConnectionAsync();
            using var transaction = await connection.BeginTransactionAsync();
            try
            {
                var userId = await connection.QuerySingleOrDefaultAsync<string?>(
                    "SELECT ID FROM TBL_USER WHERE ID = @UniqueCode",
                    new { userModel.UniqueCode },
                    transaction
                );

                if (string.IsNullOrEmpty(userId))
                {
                    await connection.ExecuteAsync(
                        @"INSERT INTO TBL_USER (ID, NAME, AGE) 
                          VALUES (@UniqueCode, @Name, @Age)",
                        userModel,
                        transaction
                    );
                }

                var donationId = await connection.QuerySingleAsync<int>(
                    @"INSERT INTO TBL_BLOOD_DONATE 
                      (USER_ID, BLOOD_TYPE, RH_FACTOR, QUANTITY_ML, DATE_OF_DONATE)
                      OUTPUT Inserted.ID
                      VALUES (@UserId, @BloodType, @RhFactor, @Quantity, @DateOfDonate)",
                    new
                    {
                        UserId = userModel.UniqueCode,
                        bloodStockModel.BloodType,
                        bloodStockModel.RhFactor,
                        bloodStockModel.Quantity,
                        bloodStockModel.DateOfDonate
                    },
                    transaction
                );
                await transaction.CommitAsync();
                return donationId;
            }
            catch (Exception ex)
            {
                 Console.WriteLine(ex.Message);
                await transaction.RollbackAsync();
                return 0;
            }

        }
    }
}
