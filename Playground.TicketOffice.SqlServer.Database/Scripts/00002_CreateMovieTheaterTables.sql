USE TicketMovieOffice;

IF (NOT EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'MovieTheater' 
                 AND  TABLE_NAME = 'Theater'))
BEGIN
    CREATE TABLE [MovieTheater].[Theater](
		Id UNIQUEIDENTIFIER PRIMARY KEY,
		Name varchar(150));
END