# PERFORMANCE

Status: **Adopted** - Owner: Chief Software Architect

Performance expectations for every feature.
Detail: `docs/mobile/08-Platform/05-Performance-Optimization.md`.

## 1. Lazy loading

- Lists render lazily (`ListView.builder`, `SliverList`, pagination). Never
  build unbounded `Column`s.
- Route pages are lazy route targets in go_router.
- Defer expensive initialization (DB, config) behind the startup/splash flow
  and provider lifecycle.

## 2. Pagination

- Consume list APIs with the pagination scheme from the OpenAPI spec; load
  more on scroll; show end-of-list state. Never fetch everything up front.

## 3. Caching

- Cache immutable, stable data (reference lists, lookups) in memory or Drift
  where freshness allows; invalidate on relevant mutations.
- Network responses that back offline-first reads live in Drift (see
  `DATABASE.md`); keep a small, bounded cache (no unbounded maps).

## 4. Memory

- Avoid retaining large objects in providers longer than needed; dispose
  controllers/subscriptions.
- Keep widget trees const where possible; avoid unnecessary allocations in
  `build` and `itemBuilder`.
- Watch for leaks in stream subscriptions and `ScrollController`s.

## 5. Animation optimization

- Prefer implicit animations (`AnimatedX`) over `AnimationController` where
  possible.
- Wrap heavy subtrees in `RepaintBoundary`; animate opacity/transform
  (GPU-friendly) not layout.
- Respect `MediaQuery.disableAnimations` / reduced-motion settings.

## 6. Widget rebuild optimization

- `const` constructors everywhere possible.
- Selectors / `select` on providers to limit rebuild scope.
- Stable widget keys for list items; avoid recreating callbacks inline where
  it causes rebuild churn.
- No network/DB work in `build`; keep `build` cheap and pure.

## 7. Verification

- Performance-sensitive screens are reviewed per `REVIEW_CHECKLIST.md`
  (rebuild analysis, list performance, memory).
- Do not ship micro-optimizations blindly - measure first where it matters.

## 8. Reference

- `docs/mobile/08-Platform/05-Performance-Optimization.md`
- `UI_GUIDELINES.md` Section 6
