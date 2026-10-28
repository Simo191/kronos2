-- =====================================================================
-- KRONOS — Déploiement du 27/09/2026 — VÉRIFICATION AVANT MIGRATION
-- Base : gestion_prestataires (PROD) — lecture seule, ne modifie rien
-- =====================================================================

-- 1) Dernière migration appliquée en prod.
--    ATTENDU : 20260611233857_AddEntreprises en première ligne.
--    Si ce n'est pas le cas : NE PAS continuer (la prod n'est pas au niveau attendu).
SELECT "MigrationId", "ProductVersion"
FROM "__EFMigrationsHistory"
ORDER BY "MigrationId" DESC
LIMIT 3;

-- 2) La nouvelle migration ne doit pas encore être appliquée.
--    ATTENDU : 0
SELECT COUNT(*) AS suivi_commandes_deja_appliquee
FROM "__EFMigrationsHistory"
WHERE "MigrationId" = '20260925233241_AddSuiviCommandes';

-- 3) Photo "avant" (à comparer avec 02_verification_apres.sql).
SELECT
  (SELECT COUNT(*) FROM contrats)             AS nb_contrats,
  (SELECT COUNT(*) FROM users)                AS nb_users,
  (SELECT COUNT(*) FROM applications_planning) AS nb_applications,
  (SELECT COUNT(*) FROM application_servers)   AS nb_serveurs;
