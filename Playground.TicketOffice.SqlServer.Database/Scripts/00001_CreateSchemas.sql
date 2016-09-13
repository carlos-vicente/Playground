USE TicketMovieOffice;

IF SCHEMA_ID('MovieTheater') IS NULL
      EXECUTE('CREATE SCHEMA MovieTheater')