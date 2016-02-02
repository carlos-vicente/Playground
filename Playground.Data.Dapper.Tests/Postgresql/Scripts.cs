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
	AND    table_name = '{0}'
) THEN
	CREATE TABLE {0}(id int, name varchar(100));
ELSE
	DELETE FROM {0};
END IF;
END
$do$
";

    }
}