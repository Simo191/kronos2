-- =====================================================================
-- KRONOS — RETOUR ARRIÈRE de la migration "AddSuiviCommandes"
-- À n'utiliser QUE si l'on revient à l'ancienne version de l'application.
-- ATTENTION : supprime les données saisies dans les nouveaux champs
-- (n° commande, CTR, tranche, logiciel, commentaire, répartitions par année).
-- Ne touche pas aux autres données (contrats, users, etc.).
-- =====================================================================

START TRANSACTION;


DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260925233241_AddSuiviCommandes') THEN
    ALTER TABLE contrats DROP CONSTRAINT "FK_contrats_applications_planning_application_id";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260925233241_AddSuiviCommandes') THEN
    DROP TABLE contrat_repartitions_annuelles;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260925233241_AddSuiviCommandes') THEN
    DROP INDEX ix_contrats_application_id;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260925233241_AddSuiviCommandes') THEN
    ALTER TABLE contrats DROP COLUMN application_id;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260925233241_AddSuiviCommandes') THEN
    ALTER TABLE contrats DROP COLUMN commentaire;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260925233241_AddSuiviCommandes') THEN
    ALTER TABLE contrats DROP COLUMN numero_commande;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260925233241_AddSuiviCommandes') THEN
    ALTER TABLE contrats DROP COLUMN reference_ctr;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260925233241_AddSuiviCommandes') THEN
    ALTER TABLE contrats DROP COLUMN tranche;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260925233241_AddSuiviCommandes') THEN
    DELETE FROM "__EFMigrationsHistory"
    WHERE "MigrationId" = '20260925233241_AddSuiviCommandes';
    END IF;
END $EF$;
COMMIT;

