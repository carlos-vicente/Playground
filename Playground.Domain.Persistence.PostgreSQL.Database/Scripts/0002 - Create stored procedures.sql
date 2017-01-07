CREATE OR REPLACE FUNCTION es.get_events_for_stream(streamId UUID)
RETURNS TABLE ("EventId" bigint, "TypeName" text, "OccurredOn" timestamp without time zone, "BatchId" uuid, "EventBody" json) AS $$
BEGIN
	RETURN QUERY SELECT E."EventId", E."TypeName", E."OccurredOn", E."BatchId", E."EventBody"
		FROM es."Events" as E
		WHERE E."EventStreamId" = streamId;
END;
$$ LANGUAGE plpgsql;




CREATE OR REPLACE FUNCTION es.get_selected_events_for_stream(streamId UUID, fromEventId bigint)
RETURNS TABLE ("EventId" bigint, "TypeName" text, "OccurredOn" timestamp without time zone, "BatchId" uuid, "EventBody" json) AS $$
BEGIN
	RETURN QUERY SELECT E."EventId", E."TypeName", E."OccurredOn", E."BatchId", E."EventBody"
		FROM es."Events" as E
		WHERE E."EventStreamId" = streamId AND E."EventId" >= fromEventId;
END;
$$ LANGUAGE plpgsql;



CREATE OR REPLACE FUNCTION es.create_event_stream(streamid uuid, streamName text, createdOn timestamp)
  RETURNS void AS $$
BEGIN
	IF(SELECT COUNT(*) 
		FROM es."EventStreams"
		WHERE "EventStreamId" = streamId) > 0 THEN
		RAISE EXCEPTION 'Cannot create an event stream for (%) because it already exists', streamId;
	END IF;

	INSERT INTO es."EventStreams" ("EventStreamId", "EventStreamName", "CreatedOn")
	values (streamId, streamName, createdOn);
END;
$$ LANGUAGE plpgsql;



CREATE OR REPLACE FUNCTION es.check_stream_exists(streamId UUID)
RETURNS BOOLEAN AS $$
BEGIN
	RETURN (SELECT COUNT(*) 
		FROM es."EventStreams"
		WHERE "EventStreamId" = streamId) > 0;
END;
$$ LANGUAGE plpgsql;


CREATE OR REPLACE FUNCTION es.get_last_event_for_stream(streamId UUID)
RETURNS TABLE ("EventId" bigint, "TypeName" text, "OccurredOn" timestamp without time zone, "BatchId" uuid, "EventBody" json) AS $$
BEGIN
	RETURN QUERY SELECT E."EventId", E."TypeName", E."OccurredOn", E."BatchId", E."EventBody"
		FROM es."Events" as E
		WHERE E."EventStreamId" = streamId
		ORDER BY E."OccurredOn" DESC, E."EventId" DESC
		LIMIT 1;
END;
$$ LANGUAGE plpgsql;


CREATE OR REPLACE FUNCTION es.save_events_in_stream(events ES.event[], streamId uuid)
RETURNS void AS $$
DECLARE size int;
BEGIN
	IF(SELECT COUNT(*) 
		FROM es."EventStreams"
		WHERE "EventStreamId" = streamId) = 0 THEN
		RAISE EXCEPTION 'Stream (%) does not exist', streamId;
	END IF;
	
	size := array_length(events, 1);

	FOR index IN 1..size LOOP
		INSERT INTO es."Events" ("EventStreamId", "EventId", "TypeName", "OccurredOn", "BatchId", "EventBody")
		VALUES (streamId, events[index].EventId, events[index].TypeName, events[index].OccurredOn, events[index].BatchId, events[index].EventBody);
	END LOOP;
END;
$$ LANGUAGE plpgsql;



CREATE OR REPLACE FUNCTION es.upsert_snapshot(streamId uuid, version bigint, takenOn timestamp without time zone, data json)
RETURNS void AS $$
BEGIN
	IF(SELECT COUNT(*) 
		FROM es."Snapshots"
		WHERE "EventStreamId" = streamId) = 0 THEN		
		UPDATE es."Snapshots"
		SET "Version" = version, "TakenOn" = takenOn, "Data" = data;
	ELSE
		INSERT INTO es."Snapshots" ("EventStreamId", "Version", "TakenOn", "Data")
		VALUES (streamId, version, takenOn, data);
	END IF;
END;
$$ LANGUAGE plpgsql;



CREATE OR REPLACE FUNCTION es.get_snapshot_for_stream(streamId UUID)
RETURNS TABLE ("Version" bigint, "TakenOn" timestamp without time zone, "Data" json) AS $$
BEGIN
	RETURN QUERY SELECT "Version", "Data"
		FROM es."Snaphots" as S
		WHERE S."EventStreamId" = streamId;
END;
$$ LANGUAGE plpgsql;
























CREATE OR REPLACE FUNCTION esgeneric.get_events_for_stream(streamId varchar(100))
RETURNS TABLE ("EventId" bigint, "TypeName" text, "OccurredOn" timestamp without time zone, "BatchId" uuid, "EventBody" json) AS $$
BEGIN
	RETURN QUERY SELECT E."EventId", E."TypeName", E."OccurredOn", E."BatchId", E."EventBody"
		FROM esgeneric."Events" as E
		WHERE E."EventStreamId" = streamId;
END;
$$ LANGUAGE plpgsql;




CREATE OR REPLACE FUNCTION esgeneric.get_selected_events_for_stream(streamId varchar(100), fromEventId bigint)
RETURNS TABLE ("EventId" bigint, "TypeName" text, "OccurredOn" timestamp without time zone, "BatchId" uuid, "EventBody" json) AS $$
BEGIN
	RETURN QUERY SELECT E."EventId", E."TypeName", E."OccurredOn", E."BatchId", E."EventBody"
		FROM esgeneric."Events" as E
		WHERE E."EventStreamId" = streamId AND E."EventId" >= fromEventId;
END;
$$ LANGUAGE plpgsql;



CREATE OR REPLACE FUNCTION esgeneric.create_event_stream(streamid varchar(100), streamName text, createdOn timestamp)
  RETURNS void AS $$
BEGIN
	IF(SELECT COUNT(*) 
		FROM esgeneric."EventStreams"
		WHERE "EventStreamId" = streamId) > 0 THEN
		RAISE EXCEPTION 'Cannot create an event stream for (%) because it already exists', streamId;
	END IF;

	INSERT INTO esgeneric."EventStreams" ("EventStreamId", "EventStreamName", "CreatedOn")
	values (streamId, streamName, createdOn);
END;
$$ LANGUAGE plpgsql;



CREATE OR REPLACE FUNCTION esgeneric.check_stream_exists(streamId varchar(100))
RETURNS BOOLEAN AS $$
BEGIN
	RETURN (SELECT COUNT(*) 
		FROM esgeneric."EventStreams"
		WHERE "EventStreamId" = streamId) > 0;
END;
$$ LANGUAGE plpgsql;


CREATE OR REPLACE FUNCTION esgeneric.get_last_event_for_stream(streamId varchar(100))
RETURNS TABLE ("EventId" bigint, "TypeName" text, "OccurredOn" timestamp without time zone, "BatchId" uuid, "EventBody" json) AS $$
BEGIN
	RETURN QUERY SELECT E."EventId", E."TypeName", E."OccurredOn", E."BatchId", E."EventBody"
		FROM esgeneric."Events" as E
		WHERE E."EventStreamId" = streamId
		ORDER BY E."OccurredOn" DESC, E."EventId" DESC
		LIMIT 1;
END;
$$ LANGUAGE plpgsql;


CREATE OR REPLACE FUNCTION esgeneric.save_events_in_stream(events ESGeneric.event[], streamId varchar(100))
RETURNS void AS $$
DECLARE size int;
BEGIN
	IF(SELECT COUNT(*) 
		FROM esgeneric."EventStreams"
		WHERE "EventStreamId" = streamId) = 0 THEN
		RAISE EXCEPTION 'Stream (%) does not exist', streamId;
	END IF;
	
	size := array_length(events, 1);

	FOR index IN 1..size LOOP
		INSERT INTO esgeneric."Events" ("EventStreamId", "EventId", "TypeName", "OccurredOn", "BatchId", "EventBody")
		VALUES (streamId, events[index].EventId, events[index].TypeName, events[index].OccurredOn, events[index].BatchId, events[index].EventBody);
	END LOOP;
END;
$$ LANGUAGE plpgsql;



CREATE OR REPLACE FUNCTION esgeneric.upsert_snapshot(streamId varchar(100), version bigint, takenOn timestamp without time zone, data json)
RETURNS void AS $$
BEGIN
	IF(SELECT COUNT(*) 
		FROM esgeneric."Snapshots"
		WHERE "EventStreamId" = streamId) = 0 THEN		
		UPDATE esgeneric."Snapshots"
		SET "Version" = version, "TakenOn" = takenOn, "Data" = data;
	ELSE
		INSERT INTO esgeneric."Snapshots" ("EventStreamId", "Version", "TakenOn", "Data")
		VALUES (streamId, version, takenOn, data);
	END IF;
END;
$$ LANGUAGE plpgsql;



CREATE OR REPLACE FUNCTION esgeneric.get_snapshot_for_stream(streamId varchar(100))
RETURNS TABLE ("Version" bigint, "TakenOn" timestamp without time zone, "Data" json) AS $$
BEGIN
	RETURN QUERY SELECT "Version", "Data"
		FROM esgeneric."Snaphots" as S
		WHERE S."EventStreamId" = streamId;
END;
$$ LANGUAGE plpgsql;