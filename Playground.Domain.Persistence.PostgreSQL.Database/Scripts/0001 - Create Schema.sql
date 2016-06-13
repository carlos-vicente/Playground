CREATE TABLE public."EventStreams"
(
  "EventStreamId" uuid NOT NULL,
  "CreatedOn" timestamp without time zone NOT NULL,
  CONSTRAINT "EventStream_PK" PRIMARY KEY ("EventStreamId")
);


CREATE TABLE public."Events"
(
  "EventStreamId" uuid NOT NULL,
  "EventId" bigint NOT NULL,
  "TypeName" text NOT NULL,
  "OccurredOn" timestamp without time zone NOT NULL,
  "EventBody" json NOT NULL,
  CONSTRAINT "EventPK" PRIMARY KEY ("EventStreamId", "EventId"),
  CONSTRAINT "Events_EventStreams_FK" FOREIGN KEY ("EventStreamId")
      REFERENCES public."EventStreams" ("EventStreamId") MATCH SIMPLE
      ON UPDATE NO ACTION ON DELETE NO ACTION
);

CREATE TYPE event AS (
	EventId bigint, 
	TypeName text, 
	OccurredOn timestamp without time zone, 
	EventBody json
);