# Performance Benchmark Report

Generated: July 29, 2026

## Financial Reporting Platform

| Benchmark | Measured | Target | Status |
|-----------|----------|--------|--------|
| Dashboard Load | **26ms** | <300ms | ✓ Pass |
| Report Generation (Revenue) | **481ms** | <1s | ✓ Pass |
| Analytics Query (Revenue Trends) | **156ms** | <1s | ✓ Pass |
| Export to Excel | **132ms** | <5s | ✓ Pass |
| 5 Reports Combined | **80ms** | <5s | ✓ Pass |

## Payment Gateway Platform

Benchmarks are measured as part of unit tests; all 102 tests pass with total duration of ~641ms.

## Test Summary

| Project | Total | Passed | Failed | Skipped | Duration |
|---------|-------|--------|--------|---------|----------|
| Financial Reporting | 89 | 89 | 0 | 0 | 685ms |
| Payment Gateway | 102 | 102 | 0 | 0 | 641ms |
| **Combined** | **191** | **191** | **0** | **0** | **1,326ms** |

## AI & Intelligence Platform

Integration tests run against a real in-memory host backed by a throwaway `postgres:16-alpine` container (schema built from `GenerateCreateScript` + seed inserts stripped + reference data re-seeded). Duration below is the full HTTP integration run.

| Project | Total | Passed | Failed | Skipped | Duration |
|---------|-------|--------|--------|---------|----------|
| AI Integration Tests | 73 | 73 | 0 | 0 | ~18s |
| AI.Application | 234 | 234 | 0 | 0 | ~1s |
| AI.Domain | 156 | 156 | 0 | 0 | ~3s |
| AI.Infrastructure | 105 | 105 | 0 | 0 | ~9s |
| **AI Combined** | **568** | **568** | **0** | **0** | — |

All performance targets are met with significant headroom.
