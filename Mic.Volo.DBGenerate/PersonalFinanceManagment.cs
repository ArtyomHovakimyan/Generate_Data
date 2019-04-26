using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mic.Volo.DBGenerate
{
    class PersonalFinanceManagment:IDisposable
    {
        private readonly string _connectionString = @"Data Source = (localdb)\MSSQLLocalDB;Integrated Security = True; Pooling=False";
        private SqlConnection _connection;

        public string DBName { get; set; }
        public PersonalFinanceManagment(string connectionString)
        {
            _connectionString = connectionString;
            _connection = new SqlConnection(_connectionString);
        }
        public async Task CreateDBAsync(string dbName)
        {
            await _connection.OpenAsync();
            using (SqlCommand command = new SqlCommand($"Create database [{dbName}];", _connection))
            {
                await command.ExecuteNonQueryAsync();
            }
            StringBuilder sql = new StringBuilder();
            sql.AppendLine($"Use [{dbName}];");
            sql.AppendLine(@"CREATE TABLE [dbo].[Category] (
                [Id]    UNIQUEIDENTIFIER NOT NULL DEFAULT newid(),
                [Title] NVARCHAR(MAX)   NOT NULL,
                CONSTRAINT[PK_Category] PRIMARY KEY CLUSTERED([Id] ASC)
            );");
            sql.AppendLine(@"CREATE TABLE [dbo].[Wallet] (
                [Id]          UNIQUEIDENTIFIER NOT NULL DEFAULT newid(),
                [CategoryId]  UNIQUEIDENTIFIER NOT NULL,
                [Amount]      MONEY            NOT NULL,
                [Comment]     NVARCHAR (MAX)   NULL,
                [Day]         DATETIME2 (7)    NOT NULL,
                [DateCreated] DATETIME2 (7)    CONSTRAINT [DF_Wallet_DateCreated] DEFAULT (getutcdate()) NOT NULL,
                CONSTRAINT [PK_Wallet] PRIMARY KEY CLUSTERED ([Id] ASC),
                CONSTRAINT [FK_Wallet_Category] FOREIGN KEY ([CategoryId]) REFERENCES [dbo].[Category] ([Id]) ON DELETE CASCADE ON UPDATE CASCADE
            );");
            using (SqlCommand command = new SqlCommand(sql.ToString(), _connection))
            {
                await command.ExecuteNonQueryAsync();
            }
            _connection.Close();
        }
        public async Task InitDbAsync(string dbName, int n)
        {

            await _connection.OpenAsync();
            //
            //this.CreateDBAsync(dbName).Wait();
            Dictionary<string, Guid> categories = new Dictionary<string, Guid>
            {
                {"Salary",Guid.NewGuid() },
                {"Credit",Guid.NewGuid() },
                {"Gasoline",Guid.NewGuid() },
                {"Entartainment",Guid.NewGuid() },
                {"Food",Guid.NewGuid() },
                {"Other",Guid.NewGuid() }
            };
            Random rnd = new Random();
            int year = 2018;
            StringBuilder sql = new StringBuilder();

            sql.AppendLine($"Use [{dbName}];");

            foreach (var category in categories)
            {
                sql.AppendLine($"Insert into [dbo].[Category]([Id],[Title]) Values ('{category.Value}','{category.Key}');");
            }
            for (int month = 1; month <= 12; month++)
            {
                sql.AppendLine($"Insert into [dbo].[Wallet]([CategoryId],[Amount],[Day]) Values ('{categories["Salary"]}',{750000},'{year}-{month.ToString().PadLeft(2, '0')}-01');");
                for (int day = 0; day <= DateTime.DaysInMonth(year, month); day++)
                {
                    sql.AppendLine($"Insert into [dbo].[Wallet]([CategoryId],[Amount],[Day]) Values ('{categories["Food"]}',{rnd.Next(10, 50)},'{year}-{month}-{day}');");
                }
            }
            using (SqlCommand command = new SqlCommand(sql.ToString(), _connection))
            {
                await command.ExecuteNonQueryAsync();
            }
            _connection.Close();
        }
        public async Task DeletDbAsync(string dbname)
        {
            await _connection.OpenAsync();
            StringBuilder sql = new StringBuilder();
            _connection.ChangeDatabase("master");
            //sql.AppendLine("Use [master];");
            sql.AppendLine($"DROP DATABASE [{dbname}]");
            using (SqlCommand command = new SqlCommand(sql.ToString(), _connection))
            {
                await command.ExecuteNonQueryAsync();
            }
            _connection.Close();
            _connection.Dispose();
        }
        public void Dispose()
        {
            _connection.Dispose();
        }
    }
}
