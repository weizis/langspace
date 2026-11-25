BEGIN;
SET LOCAL search_path = public;
DO $$
BEGIN
    IF lower(current_database()) <> 'langspacedemo' THEN
        RAISE EXCEPTION 'Run only in LangSpaceDemo';
    END IF;
END $$;

UPDATE users
SET password_hash = 'AQAAAAIAAYagAAAAEEctz3MwxfptkGSHVDY/3gbfWfSDhIfqaL+WhXQUrf5phBmRjWHD0wfWiYgjHYBzyA=='
WHERE left(username, 5) = 'demo_'
AND email = username || '@example.invalid'
AND password_hash = 'DISABLED_PENDING_AUTH_REVIEW';

COMMIT;
