namespace Playground.Data.Dapper.Tests.Postgresql
{
    public class Scripts
    {
        public static class Ddl
        {
            public const string CreateTable = @"DO
$do$
BEGIN
IF NOT EXISTS (
	SELECT 1
	FROM   information_schema.tables 
	WHERE  table_schema = 'public'
	AND    table_name = 'test'
) THEN
	CREATE TABLE test(id int, name varchar(100));
ELSE
	DELETE FROM test;
END IF;
END
$do$
";
            public const string DropTable = "DROP TABLE test;";

            public const string CreateProdecure = @"CREATE OR REPLACE FUNCTION get_test() 
RETURNS setof test AS $$
BEGIN
	RETURN QUERY SELECT * FROM test;
END;
$$ LANGUAGE plpgsql;";

            public const string DropProcedure = "DROP FUNCTION IF EXISTS get_test();";
        }

        public static class Dml
        {
            public const string InsertIntoTable = "INSERT INTO test(id, name) VALUES (@Id, @Name)";
            public const string UpdateTable = "UPDATE test SET Name = @Name WHERE Id = @Id";
            public const string DeleteAllTable = "DELETE FROM test";
            public const string DeleteTable = "DELETE FROM test WHERE Id = @Id";

            public const string StoredProcedureName = "get_test";
        }
    }
}