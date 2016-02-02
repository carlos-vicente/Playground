namespace Playground.Data.Dapper.Tests.Postgresql
{
    public class Scripts
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

        public const string InsertIntoTable = "INSERT INTO test(id, name) VALUES (@id, @name);";
    }
}