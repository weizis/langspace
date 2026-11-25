BEGIN;
SET LOCAL search_path = public;

CREATE OR REPLACE FUNCTION public.calculate_test_streak(p_user_id integer)
RETURNS integer LANGUAGE sql STABLE AS $$
    WITH RECURSIVE activity AS (
        SELECT DISTINCT date::date AS day
        FROM public.test_result
        WHERE id_user = p_user_id AND date::date <= CURRENT_DATE
    ), streak(day) AS (
        SELECT max(day) FROM activity
        HAVING max(day) >= CURRENT_DATE - 1
        UNION ALL
        SELECT a.day FROM activity a JOIN streak s ON a.day = s.day - 1
    )
    SELECT count(*)::integer FROM streak;
$$;

CREATE OR REPLACE FUNCTION public.update_goal_streak()
RETURNS trigger LANGUAGE plpgsql AS $$
BEGIN
    IF TG_OP <> 'INSERT' THEN
        UPDATE public.goal SET days_streak = public.calculate_test_streak(OLD.id_user)
        WHERE id_user = OLD.id_user;
    END IF;
    IF TG_OP <> 'DELETE' THEN
        UPDATE public.goal SET days_streak = public.calculate_test_streak(NEW.id_user)
        WHERE id_user = NEW.id_user;
    END IF;
    RETURN NULL;
END;
$$;

DROP TRIGGER IF EXISTS trigger_update_streak_on_test ON public.test_result;
CREATE TRIGGER trigger_update_streak_on_test
AFTER INSERT OR UPDATE OR DELETE ON public.test_result
FOR EACH ROW EXECUTE FUNCTION public.update_goal_streak();

UPDATE public.goal SET days_streak = public.calculate_test_streak(id_user);

CREATE OR REPLACE FUNCTION public.auto_assign_default_language()
RETURNS trigger LANGUAGE plpgsql AS $$
BEGIN
    INSERT INTO public.user_language (id_user, id_language, id_level)
    SELECT NEW.id_user, l.id_language, v.id_level
    FROM public.languages l CROSS JOIN public.levels v
    WHERE l.language_name = 'English' AND v.level_name = 'A1'
    ON CONFLICT (id_user, id_language) DO NOTHING;
    RETURN NEW;
END;
$$;

COMMIT;
