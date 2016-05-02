CREATE TABLE public."EventStreams"
(
  "EventStreamId" uuid NOT NULL,
  CONSTRAINT "EventStream_PK" PRIMARY KEY ("EventStreamId")
)
WITH (
  OIDS=FALSE
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
)
WITH (
  OIDS=FALSE
);





CREATE OR REPLACE FUNCTION get_events_for_stream(streamId UUID)
RETURNS TABLE ("EventId" bigint, "TypeName" text, "OccurredOn" timestamp without time zone, "EventBody" json) AS $$
BEGIN
	RETURN QUERY SELECT E."EventId", E."TypeName", E."OccurredOn", E."EventBody"
		FROM public."Events" as E
		WHERE E."EventStreamId" = streamId;
END;
$$ LANGUAGE plpgsql;



CREATE OR REPLACE FUNCTION public.create_event_stream(streamid uuid)
  RETURNS void AS $$
BEGIN
	IF(SELECT COUNT(*) 
		FROM public."EventStreams"
		WHERE "EventStreamId" = streamId) > 0 THEN
		RAISE EXCEPTION 'Cannot create an event stream for (%) because it already exists', streamId;
	END IF;

	INSERT INTO public."EventStreams" ("EventStreamId")
	values (streamId);
END;
$$ LANGUAGE plpgsql;



CREATE OR REPLACE FUNCTION check_stream_exists(streamId UUID)
RETURNS BOOLEAN AS $$
BEGIN
	RETURN (SELECT COUNT(*) 
		FROM public."EventStreams"
		WHERE "EventStreamId" = streamId) > 0;
END;
$$ LANGUAGE plpgsql;


CREATE OR REPLACE FUNCTION get_last_event_for_stream(streamId UUID)
RETURNS TABLE ("EventId" bigint, "TypeName" text, "OccurredOn" timestamp without time zone, "EventBody" json) AS $$
BEGIN
	RETURN QUERY SELECT E."EventId", E."TypeName", E."OccurredOn", E."EventBody"
		FROM public."Events" as E
		WHERE E."EventStreamId" = streamId
		ORDER BY E."OccurredOn" DESC
		LIMIT 1;
END;
$$ LANGUAGE plpgsql;