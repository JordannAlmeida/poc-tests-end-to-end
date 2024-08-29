using Testcontainers.MsSql;

namespace BlondDonateContainerTests.Factory
{
    public static class SqlServerLoad
    {
        private static readonly string sqlCreateTableUsers = @"
            CREATE TABLE TBL_USER (
                ID VARCHAR(14) PRIMARY KEY,
                NAME VARCHAR(255) NOT NULL,
                AGE INT NOT NULL
            );";

        private static readonly string sqlCreateTableBloodDonate = @"
            CREATE TABLE TBL_BLOOD_DONATE (
                ID INT PRIMARY KEY IDENTITY,
                USER_ID VARCHAR(14) NOT NULL,
                BLOOD_TYPE VARCHAR(2) NOT NULL,
                RH_FACTOR CHAR(1) NOT NULL,
                QUANTITY_ML FLOAT NOT NULL,
                DATE_OF_DONATE DATE NOT NULL,
                FOREIGN KEY (USER_ID) REFERENCES TBL_USER(ID)
            );";

        public async static Task InitializeDatabaseTestContainerAsync(this MsSqlContainer msSqlContainer)
        {
            if (msSqlContainer == null)
            {
                throw new Exception("MsSqlContainer is not started");
            }
            var execResultCreateTableUser = await msSqlContainer.ExecScriptAsync(sqlCreateTableUsers);
            var execResultCreateTableBloodDonate =  await msSqlContainer.ExecScriptAsync(sqlCreateTableBloodDonate).ConfigureAwait(false);
            Console.WriteLine($"Executado scripts - execResultCreateTableUser: {execResultCreateTableUser.ExitCode} -  execResultCreateTableBloodDonate: {execResultCreateTableBloodDonate.ExitCode}");
        }

        public async static Task ResetData(this MsSqlContainer msSqlContainer)
        {
            await msSqlContainer.ExecScriptAsync("DELETE FROM TBL_USER");
            await msSqlContainer.ExecScriptAsync("DELETE FROM TBL_BLOOD_DONATE");
        }
    }
}
