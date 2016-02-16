using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Npgsql;
using NUnit.Framework;
using Playground.Data.Contracts;
using Playground.Data.Dapper.Tests.Postgresql;
using Playground.Tests;
using Dapper;
using FluentAssertions;

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

            var connectionStringBuilder = DatabaseHelper.GetConnectionStringBuilder();

            _realConnection = new NpgsqlConnection(connectionStringBuilder);
            _realConnection.Open();
            _sut = new Connection(_realConnection);
        }

        [TearDown]
        public void TearDown()
        {
            _sut.Dispose();
        }

        [Test]
        public async Task ExecuteCommand_InsertsRowsInDatabase_WhenInsertStatementIsPassed()
        {
            // arrange
            const string sql = "INSERT INTO test(id, name) VALUES (@Id, @Name)";
            var expected = new Test {Id = 1, Name = "dude"};

            // act
            _sut
                .ExecuteCommand(sql, expected)
                .Wait();

            // assert
            var actual = (await _realConnection
                .QueryAsync<Test>("SELECT * from test")
                .ConfigureAwait(false))
                .ToList();

            actual
                .Count()
                .Should()
                .Be(1);

            actual
                .First()
                .ShouldBeEquivalentTo(expected);
        }

        //[Test]
        //public async Task ExecuteCommand_InsertsRowsInDatabase_WhenInsertStatementIsPassed()
        //{
        //    // arrange
        //    const string sql = "UPDATE test SET Name = @Name WHERE Id = @Id";
        //    var initial = new Test { Id = 1, Name = "dude" };

        //    // act
        //    _sut
        //        .ExecuteCommand(sql, expected)
        //        .Wait();

        //    // assert
        //    var actual = (await _realConnection
        //        .QueryAsync<Test>("SELECT * from test")
        //        .ConfigureAwait(false))
        //        .ToList();

        //    actual
        //        .Count()
        //        .Should()
        //        .Be(1);

        //    actual
        //        .First()
        //        .ShouldBeEquivalentTo(expected);
        //}
    }
}