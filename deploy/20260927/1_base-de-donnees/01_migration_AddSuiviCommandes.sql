-- =====================================================================
-- KRONOS — Déploiement du 27/09/2026 — MIGRATION "AddSuiviCommandes"
-- Base : gestion_prestataires (PROD)
--
-- Ajoute (sans rien supprimer ni modifier d'existant) :
--   * contrats : numero_commande, reference_ctr, tranche (défaut 'Ferme'),
--                application_id (logiciel, FK applications_planning), commentaire
--   * table contrat_repartitions_annuelles (répartition des jours par année)
--
-- Script idempotent : peut être relancé sans risque (ne s'applique qu'une fois).
-- Ne touche PAS à la table users (aucun mot de passe modifié).
-- =====================================================================

-- Garde-fou : la prod doit être au niveau "AddEntreprises" (sinon on s'arrête)
DO $$
BEGIN
    IF NOT EXISTS (SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260611233857_AddEntreprises') THEN
        RAISE EXCEPTION 'ARRET : la migration 20260611233857_AddEntreprises est absente. Base non conforme, ne pas continuer.';
    END IF;
END $$;

START TRANSACTION;


DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260925233241_AddSuiviCommandes') THEN
    ALTER TABLE contrats ADD application_id uuid;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260925233241_AddSuiviCommandes') THEN
    ALTER TABLE contrats ADD commentaire character varying(1000);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260925233241_AddSuiviCommandes') THEN
    ALTER TABLE contrats ADD numero_commande character varying(100);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260925233241_AddSuiviCommandes') THEN
    ALTER TABLE contrats ADD reference_ctr character varying(100);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260925233241_AddSuiviCommandes') THEN
    ALTER TABLE contrats ADD tranche character varying(10) NOT NULL DEFAULT 'Ferme';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260925233241_AddSuiviCommandes') THEN
    CREATE TABLE contrat_repartitions_annuelles (
        id uuid NOT NULL,
        contrat_id uuid NOT NULL,
        annee integer NOT NULL,
        nombre_jours numeric(10,2) NOT NULL,
        CONSTRAINT "PK_contrat_repartitions_annuelles" PRIMARY KEY (id),
        CONSTRAINT "FK_contrat_repartitions_annuelles_contrats_contrat_id" FOREIGN KEY (contrat_id) REFERENCES contrats (id) ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260925233241_AddSuiviCommandes') THEN
    CREATE INDEX ix_contrats_application_id ON contrats (application_id);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260925233241_AddSuiviCommandes') THEN
    CREATE UNIQUE INDEX ux_contrat_repartitions_contrat_annee ON contrat_repartitions_annuelles (contrat_id, annee);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260925233241_AddSuiviCommandes') THEN
    ALTER TABLE contrats ADD CONSTRAINT "FK_contrats_applications_planning_application_id" FOREIGN KEY (application_id) REFERENCES applications_planning (id) ON DELETE SET NULL;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260925233241_AddSuiviCommandes') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20260925233241_AddSuiviCommandes', '8.0.22');
    END IF;
END $EF$;
COMMIT;

