using System.Data;
using Npgsql;
using NUnit.Framework;
using Playground.Data.Contracts;
using Playground.Data.Dapper.Tests.Postgresql;
using Playground.Tests;

namespace Playground.Data.Dapper.Tests
{
    public class ConnectionTests : TestBase
    {
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            // create table
            DatabaseHelper.CreateTestTable();
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            // drop table
            DatabaseHelper.DropTestTable();
        }

        private IDbConnection _realConnection;
        private IConnection _sut;

        public override void SetUp()
        {
            base.SetUp();

            var connectionString = DatabaseHelper.GetConnectionString();

            _realConnection = new NpgsqlConnection(connectionString);
            _sut = new Connection(_realConnection);
        }

        [TearDown]
        public void TearDown()
        {
            _sut.Dispose();
        }

        [Test]
        public void ExecuteCommand_InsertsRowsInDatabase_WhenInsertStatementIsPassed()
        {
            // arrange
            const string sql = "INSERT INTO test(id, name) VALUES (@Id, '@Name')";
            
            // act
            _sut.ExecuteCommand(sql, new {Id = 1, Name = "dude"});

            // assert
        }
    }
}