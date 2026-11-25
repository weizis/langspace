BEGIN;
SET LOCAL search_path = public;

ALTER TABLE card ADD CONSTRAINT card_private_owner_check
    CHECK (NOT is_private OR id_user IS NOT NULL);
ALTER TABLE test_result ADD CONSTRAINT test_result_score_check
    CHECK (score >= 0 AND total >= 0 AND score <= total);
ALTER TABLE note ALTER COLUMN date_created SET DEFAULT CURRENT_DATE;

DROP TRIGGER IF EXISTS trigger_check_grammar_rule_exists ON grammar_question;
DROP TRIGGER IF EXISTS trigger_check_level_for_card ON card;
DROP TRIGGER IF EXISTS trigger_check_user_for_card ON card;
DROP TRIGGER IF EXISTS trigger_check_private_card_owner ON card;
DROP TRIGGER IF EXISTS trigger_validate_test_score ON test_result;
DROP TRIGGER IF EXISTS trigger_note_date ON note;
DROP TRIGGER IF EXISTS trigger_prevent_language_deletion ON languages;

DROP FUNCTION IF EXISTS check_grammar_rule_exists();
DROP FUNCTION IF EXISTS check_level_exists();
DROP FUNCTION IF EXISTS check_user_exists_for_card();
DROP FUNCTION IF EXISTS check_private_card_owner();
DROP FUNCTION IF EXISTS validate_test_score();
DROP FUNCTION IF EXISTS set_creation_date();
DROP FUNCTION IF EXISTS prevent_language_deletion();
DROP FUNCTION IF EXISTS log_card_deletion();

COMMIT;
