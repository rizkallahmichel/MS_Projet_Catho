# CI/CD pipeline

## What it does
- Runs on push/pull_request to `main`/`master`, and via manual dispatch.
- Restores, builds, and tests the full solution (`MyApp.sln`) in Release mode.
- Publishes Release builds for:
  - `MyApp.ApiService` → `artifacts/api`
  - `MyApp.WebApp` → `artifacts/web`
- Uploads test results (`*.trx`) and publish artifacts to the workflow run.

## How to run it
- Automatic: push or open a PR against `main` or `master`.
- Manual: in GitHub → Actions → `CI-CD` workflow → **Run workflow** (choose branch).
- CLI (GitHub CLI): `gh workflow run CI-CD --ref <branch>`

## How to get artifacts
- In the GitHub Actions run, download `test-results` for TRX logs and `publish-artifacts` for the published binaries.

## Local parity
- Same steps locally:
  ```powershell
  dotnet restore MyApp.sln
  dotnet build MyApp.sln --configuration Release --no-restore
  dotnet test MyApp.sln --configuration Release --no-build --logger "trx;LogFileName=test-results.trx"
  dotnet publish MyApp.ApiService/MyApp.ApiService.csproj -c Release -o ./artifacts/api
  dotnet publish MyApp.WebApp/MyApp.WebApp.csproj -c Release -o ./artifacts/web
  ```

## Secrets & deployment targets
- No deploy target is configured yet; this pipeline stops at publishing artifacts.
- To add deployment (e.g., Azure App Service, containers, etc.), add a deploy step/job that consumes the `artifacts/*` outputs and uses platform secrets (kept in the repo’s secret store).