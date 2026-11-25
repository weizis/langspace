BEGIN;
SET LOCAL search_path = public;
DO $$
BEGIN
    IF lower(current_database()) <> 'langspacedemo' THEN
        RAISE EXCEPTION 'Run only in LangSpaceDemo';
    END IF;
END $$;

UPDATE users
SET password_hash = 'AQAAAAIAAYagAAAAEBI0VniQq83vEjRWeJCrze/+8lfj6tILjJopIr1jJMX0I4qAYs1VSpVVpXr8giNBFQ=='
WHERE left(username, 5) = 'demo_'
AND email = username || '@example.invalid'
AND password_hash = 'DISABLED_PENDING_AUTH_REVIEW';

COMMIT;
