using System.Collections.Generic;
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
    public class ConnectionSimpleTests : SimpleTestBase
    {
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            // create table
            DatabaseHelper.CreateTestTable();

            // create stored procedure
            DatabaseHelper.CreateStoredProcedure();
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            // drop table
            DatabaseHelper.DropTestTable();
            
            // drop stored procedure
            DatabaseHelper.DropStoredProcedure();
        }

        private IDbConnection _realConnection;
        private IConnection _sut;

        public override void SetUp()
        {
            base.SetUp();

            var connectionStringBuilder = DatabaseHelper
                .GetConnectionStringBuilder();

            _realConnection = new NpgsqlConnection(connectionStringBuilder);
            _realConnection.Open();

            _sut = new Connection(_realConnection);
        }

        [TearDown]
        public void TearDown()
        {
            _realConnection.Execute(Scripts.Dml.DeleteAllTable);

            _sut.Dispose();
        }

        [Test]
        public async Task ExecuteCommand_InsertsRowsInDatabase_WhenInsertStatementIsPassed()
        {
            // arrange
            var expected = new Test {Id = 1, Name = "dude"};

            // act
            _sut
                .ExecuteCommand(Scripts.Dml.InsertIntoTable, expected)
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

        [Test]
        public async Task ExecuteCommand_UpdateRowsInDatabase_WhenUpdateStatementIsPassed()
        {
            // arrange
            var initial = new Test { Id = 1, Name = "dude" };
            _realConnection
                .Execute(
                    Scripts.Dml.InsertIntoTable,
                    initial);

            var expected = new Test { Id = 1, Name = "another dude" };

            // act
            _sut
                .ExecuteCommand(Scripts.Dml.UpdateTable, expected)
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

        [Test]
        public async Task ExecuteCommand_DeleteRowsInDatabase_WhenDeleteStatementIsPassed()
        {
            // arrange
            var toDelete = new Test { Id = 1, Name = "dude" };
            var toStay = new Test { Id = 2, Name = "other dude" };
            _realConnection
                .Execute(
                    Scripts.Dml.InsertIntoTable,
                    toDelete);
            _realConnection
                .Execute(
                    Scripts.Dml.InsertIntoTable,
                    toStay);

            // act
            _sut
                .ExecuteCommand(Scripts.Dml.DeleteTable, new { Id = 1 })
                .Wait();

            // assert
            var actual = (await _realConnection
                .QueryAsync<Test>("SELECT * FROM test WHERE Id = @Id", new { Id = 1 })
                .ConfigureAwait(false))
                .ToList();

            actual
                .Count()
                .Should()
                .Be(0);
        }

        [Test]
        public async Task ExecuteQuerySingleAsStoredProcedure_WillGetSingleRowUsingStoredProcedure()
        {
            // arrange
            var row = new Test
            {
                Id = 2,
                Name = "other dude"
            };
            
            _realConnection.Execute(Scripts.Dml.InsertIntoTable, row);
            
            // act
            var actual = await _sut
                .ExecuteQuerySingleAsStoredProcedure<Test>(Scripts.Dml.StoredProcedureName, null)
                .ConfigureAwait(false);

            // assert
            actual
                .ShouldBeEquivalentTo(row);
        }

        [Test]
        public async Task ExecuteQueryMultipleAsStoredProcedure_WillGetAllRowsUsingStoredProcedure()
        {
            // arrange
            var row1 = new Test
            {
                Id = 1,
                Name = "dude"
            };
            var row2 = new Test
            {
                Id = 2,
                Name = "other dude"
            };

            _realConnection.Execute(Scripts.Dml.InsertIntoTable, row1);
            _realConnection.Execute(Scripts.Dml.InsertIntoTable, row2);

            var expected = new List<Test>
            {
                row1,
                row2
            };

            // act
            var actual = await _sut
                .ExecuteQueryMultipleAsStoredProcedure<Test>(Scripts.Dml.StoredProcedureName, null)
                .ConfigureAwait(false);

            // assert
            actual
                .ShouldAllBeEquivalentTo(expected);
        }
    }
}