# CHANGELOG: Iteration 5 — Final Audit: Export-Referenzen & Node-Suche

Branch: `fix/iteration-5-final-audit`
Ziel: Verbleibende `FindChild`/`GetNodeOrNull`-Aufrufe durch Export-Attribute ersetzen oder mit sicheren Fallbacks versehen.

---

## 1. FindChild-Aufrufe durch Export ersetzt
- `Item.cs`: `CollectableComponent`, `Sprite2D`
- `Tools/Sword/Sword.cs`: `HitBoxComponent`
- `HealItemEffectResource.cs`: `HealthComponent`-Lookup entfernt; nutzt jetzt `Character.HealthComponent`
- `Animation/AnimationController.cs`: `AnimationPlayer`, `HurtEffectTimer`
- `UI/Healthbar/HealthBarDisplay.cs`: `HealthComponent` als Export
- `Characters/Character.cs`: `HealthComponent`/`HurtBoxComponent` als Export, FindChild nur als Fallback
- `Characters/Player/Player.cs`: `HealthComponent`, `AttackComponent`, `Interaction Components` bereits teilweise Export, restliche FindChild-Fälle bereinigt

## 2. GetNodeOrNull-Fallbacks bereinigt
- `UI/Healthbar/HealthBarDisplay.cs`: `Hurtbar`, `Healthbar`, `VisibleTimer` als Export
- `PlayerInteractionComponents.cs`: `InteractLabel`, `InteractionAreaCollisionShape` als Export
- `HealthComponent.cs`: `DamageNumberComponent` als Export
- `AttackComponent.cs`: `HitBoxTimer` als Export
- `CharacterStateMachine.cs`: `AnimationController`, `AnimationPlayer` als Export, FindChild nur als Fallback
- `Animation/AnimationController.cs`: `EffectPlayer` als Export

## 3. Aufräumarbeiten
- Alte Protokolldateien aus vorherigen Branches entfernt.
- Neue Protokolle: `CHANGELOG_ITERATION5.md`, `SUMMARY_ITERATION5.md`, `ITERATION5_ANALYSIS_2026-06-21.md`.

## Build-Status
- 0 Warnungen, 0 Fehler.
