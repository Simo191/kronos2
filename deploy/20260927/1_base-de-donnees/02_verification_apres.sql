-- =====================================================================
-- KRONOS — Déploiement du 27/09/2026 — VÉRIFICATION APRÈS MIGRATION
-- Lecture seule
-- =====================================================================

-- 1) La migration est enregistrée.  ATTENDU : 1
SELECT COUNT(*) AS migration_ok
FROM "__EFMigrationsHistory"
WHERE "MigrationId" = '20260925233241_AddSuiviCommandes';

-- 2) Les 5 nouvelles colonnes existent.  ATTENDU : 5 lignes
SELECT column_name, data_type, column_default
FROM information_schema.columns
WHERE table_name = 'contrats'
  AND column_name IN ('numero_commande', 'reference_ctr', 'tranche', 'application_id', 'commentaire')
ORDER BY column_name;

-- 3) La nouvelle table existe.  ATTENDU : 1
SELECT COUNT(*) AS table_repartitions_ok
FROM information_schema.tables
WHERE table_name = 'contrat_repartitions_annuelles';

-- 4) Tous les contrats existants sont en tranche "Ferme".
SELECT tranche, COUNT(*) AS nb FROM contrats GROUP BY tranche;

-- 5) Mêmes chiffres qu'avant (comparer avec 00_verification_avant.sql).
SELECT
  (SELECT COUNT(*) FROM contrats)             AS nb_contrats,
  (SELECT COUNT(*) FROM users)                AS nb_users,
  (SELECT COUNT(*) FROM applications_planning) AS nb_applications,
  (SELECT COUNT(*) FROM application_servers)   AS nb_serveurs;
