# Iterations-Analyse #4: Service-Locator & Node-Suche zentralisieren

Erstellt: 2026-06-21
Repository: https://github.com/FienonTV/twcs
Branch: fix/iteration-4-service-locator
Build: 0 Warnungen, 0 Fehler

---

## 1. Zusammenfassung

Iteration 3 hat Konventionen durchgesetzt und Null-Safety verbessert. Iteration 4 konzentriert sich auf Singleton-Lookups und FindChild.

---

## 2. Statistiken

| Metrik | Anzahl |
|---|---|
| C#-Dateien | 58 |
| Singleton-Pfad-Lookups | 8 |
| `FindChild` | 18 |
| `GetNodeOrNull` | 26 |

---

## 3. P1 — Singleton-Lookups

- `InventoryMenu.cs`, `Player.cs`, `FollowPlayerBehavior.cs`
- `PlayerStateMachine.cs`, `ToolStateMachine.cs`, `WorldInitialization.cs`
- `ResourcePaths.InventoryMenuAutoload`

Lösung: zentraler Service-Locator.

## 4. P2 — FindChild

Item, Sword, HealItemEffectResource, AnimationController, HealthBarDisplay, Character, Player, PlayerInteractionComponents, SmallTree.

Lösung: Export-Referenzen wo sinnvoll.

## 5. Fazit

Nächste Schritte: ServiceLocator einführen, Singleton-Lookups ersetzen, FindChild reduzieren.
