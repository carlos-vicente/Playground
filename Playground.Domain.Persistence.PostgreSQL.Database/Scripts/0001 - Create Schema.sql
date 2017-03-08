CREATE SCHEMA IF NOT EXISTS es;
CREATE SCHEMA IF NOT EXISTS esgeneric;

CREATE TABLE es."EventStreams"
(
  "EventStreamId" uuid NOT NULL,
  "EventStreamName" text NOT NULL,
  "CreatedOn" timestamp without time zone NOT NULL,
  CONSTRAINT "EventStream_PK" PRIMARY KEY ("EventStreamId")
);

CREATE TABLE esgeneric."EventStreams"
(
  "EventStreamId" varchar(100) NOT NULL,
  "EventStreamName" varchar(100) NOT NULL,
  "CreatedOn" timestamp without time zone NOT NULL,
  CONSTRAINT "EventStream_PK" PRIMARY KEY ("EventStreamId")
);

CREATE TABLE es."Events"
(
  "EventStreamId" uuid NOT NULL,
  "EventId" bigint NOT NULL,
  "TypeName" text NOT NULL,
  "OccurredOn" timestamp without time zone NOT NULL,
  "BatchId" uuid NOT NULL,
  "EventBody" json NOT NULL,
  CONSTRAINT "EventPK" PRIMARY KEY ("EventStreamId", "EventId"),
  CONSTRAINT "Events_EventStreams_FK" FOREIGN KEY ("EventStreamId")
      REFERENCES es."EventStreams" ("EventStreamId") MATCH SIMPLE
      ON UPDATE NO ACTION ON DELETE NO ACTION
);

CREATE TABLE esgeneric."Events"
(
  "EventStreamId" varchar(100) NOT NULL,
  "EventId" bigint NOT NULL,
  "TypeName" text NOT NULL,
  "OccurredOn" timestamp without time zone NOT NULL,
  "BatchId" uuid NOT NULL,
  "EventBody" json NOT NULL,
  CONSTRAINT "EventPK" PRIMARY KEY ("EventStreamId", "EventId"),
  CONSTRAINT "Events_EventStreams_FK" FOREIGN KEY ("EventStreamId")
      REFERENCES esgeneric."EventStreams" ("EventStreamId") MATCH SIMPLE
      ON UPDATE NO ACTION ON DELETE NO ACTION
);

CREATE TABLE es."Snapshots"
(
	"EventStreamId" uuid NOT NULL,
	"Version" bigint NOT NULL,
	"TakenOn" timestamp without time zone NOT NULL,
	"Data" json NOT NULL,
	CONSTRAINT "Snapshots_PK" PRIMARY KEY ("EventStreamId", "Version"),
	CONSTRAINT "Snapshots_EventStreams_FK" FOREIGN KEY ("EventStreamId")
      REFERENCES es."EventStreams" ("EventStreamId") MATCH SIMPLE
      ON UPDATE NO ACTION ON DELETE NO ACTION
);

CREATE TABLE esgeneric."Snapshots"
(
	"EventStreamId" varchar(100) NOT NULL,
	"Version" bigint NOT NULL,
	"TakenOn" timestamp without time zone NOT NULL,
	"Data" json NOT NULL,
	CONSTRAINT "Snapshots_PK" PRIMARY KEY ("EventStreamId", "Version"),
	CONSTRAINT "Snapshots_EventStreams_FK" FOREIGN KEY ("EventStreamId")
      REFERENCES esgeneric."EventStreams" ("EventStreamId") MATCH SIMPLE
      ON UPDATE NO ACTION ON DELETE NO ACTION
);

DROP TYPE IF EXISTS es.event;
CREATE TYPE es.event AS (
	EventId bigint, 
	TypeName text, 
	OccurredOn timestamp without time zone,
	BatchId uuid,
	EventBody json
);

DROP TYPE IF EXISTS esgeneric.event;
CREATE TYPE esgeneric.event AS (
	EventId bigint, 
	TypeName text, 
	OccurredOn timestamp without time zone,
	BatchId uuid,
	EventBody json
);