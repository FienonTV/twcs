# Finale Audit-Analyse #5: Export-Referenzen & Node-Suche

Erstellt: 2026-06-21
Repository: https://github.com/FienonTV/twcs
Branch: fix/iteration-5-final-audit
Build: 0 Warnungen, 0 Fehler

---

## 1. Zusammenfassung

Iteration 5 konzentriert sich auf die verbleibenden `FindChild`/`GetNodeOrNull`-Aufrufe, die durch `[Export]`-Referenzen ersetzt oder zumindest mit sicheren Fallbacks versehen werden sollten.

---

## 2. Statistiken

| Metrik | Anzahl |
|---|---|
| C#-Dateien | 58 |
| `GD.Print` | 2 (nur Logger) |
| `async void` | 0 |
| `FindChild` | 12 |
| `GetNodeOrNull` | 11 |
| Singleton-Pfad-Lookups | 1 (absichtlich in Services) |
| Öffentliche `_`-Felder | 0 |
| Klassen mit Kleinbuchstaben | 0 |

---

## 3. P1 — Verbleibende FindChild-Fallbacks

1. `Item.cs`: `CollectableComponent`, `Sprite2D`
2. `Tools/Sword/Sword.cs`: `HitBoxComponent`
3. `HealItemEffectResource.cs`: `HealthComponent`
4. `Animation/AnimationController.cs`: `AnimationPlayer`, `HurtEffectTimer`
5. `UI/Healthbar/HealthBarDisplay.cs`: `HealthComponent`
6. `Characters/Character.cs`: `HealthComponent`
7. `Characters/Player/Player.cs`: `HealthComponent`, `AttackComponent`, `Interaction Components`

## 4. P2 — Verbleibende GetNodeOrNull-Fallbacks

1. `Animation/AnimationController.cs`: `EffectPlayer`
2. `UI/Healthbar/HealthBarDisplay.cs`: `Hurtbar`, `Healthbar`, `VisibleTimer`
3. `PlayerInteractionComponents.cs`: `InteractLabel`, `InteractionAreaCollisionShape`
4. `HealthComponent.cs`: `DamageNumberComponent`
5. `AttackComponent.cs`: `HitBoxTimer`
6. `CharacterStateMachine.cs`: `AnimationController`, `AnimationPlayer`

## 5. Fazit

Nächste Schritte: Export-Attribute ergänzen, Fallbacks robuster machen, Build, Protokoll, Commit, Push.
