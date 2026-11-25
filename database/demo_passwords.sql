BEGIN;
SET LOCAL search_path = public;
DO $$
BEGIN
    IF lower(current_database()) <> 'langspacedemo' THEN
        RAISE EXCEPTION 'Run only in LangSpaceDemo';
    END IF;
END $$;

UPDATE users
SET password_hash = 'AQAAAAIAAYagAAAAELWW82JN2iifu7mQ2FueVHECo/MisjE039HYJwqDmZgoGfd9VpUJroMB+xS91qaTOQ=='
WHERE username IN ('demo_admin','demo_tutor','demo_student','demo_student2','demo_student3','demo_student4','demo_student5','demo_student6','demo_student7','demo_student8')
AND email = username || '@example.invalid'
AND password_hash = 'DISABLED_PENDING_AUTH_REVIEW';

COMMIT;
