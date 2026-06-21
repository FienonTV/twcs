# CHANGELOG: Deeper Runtime & Architecture Cleanup

Branch: `fix/deeper-analysis-cleanup`
Ziel: P0-Laufzeitfehler und P1-Architekturrisiken aus `DEEPER_ANALYSIS_2026-06-21.md` lösen.

---

## 1. HealthBarDisplay null-sicher
- Datei: `UI/Healthbar/HealthBarDisplay.cs`
- Früher: `NullReferenceException`, wenn kein `HealthComponent` gefunden wurde.
- Jetzt: Frühzeitiger Abbruch mit `Logger.Error`, wenn `_CharacterParent` oder `_HealthComponent` fehlen.

## 2. BaseMovementBehavior gegen leere Player-Gruppe abgesichert
- Datei: `Characters/Behaviors/BaseMovementBehavior.cs`
- Früher: `IndexOutOfRangeException`, wenn keine Player-Node in der Gruppe war.
- Jetzt: Verwendet `GetNodesInGroup("Player")` mit Längenprüfung; `_CurrentScenePlayer` bleibt null.

## 3. PlayerStateMachine/ToolStateMachine InputHandler robuster
- Dateien: `StateMachine/PlayerStateMachine.cs`, `StateMachine/ToolStateMachine.cs`
- Früher: `GetNodeOrNull` ohne Rückfall, stiller Fehler möglich.
- Jetzt: Explizite Prüfung und `Logger.Error`, wenn `/root/InputHandler` fehlt.
- Optional: Event-Subscription nur bei erfolgreichem Lookup.

## 4. PlayerMovementComponent generisch für Character
- Datei: `Characters/Player/PlayerOnlyComponents/PlayerMovementComponent/PlayerMovementComponent.cs`
- Früher: Hart auf `Player`-Typ geprüft.
- Jetzt: Akzeptiert `Character` als Parent; `HandleMovement` arbeitet auf `CharacterBody2D`.
- Damit wiederverwendbar für NPCs mit derselben Komponente.

## 5. AnimationController reaktiviert
- Datei: `Animation/AnimationController.cs`, `StateMachine/CharacterStateMachine.cs`
- Früher: `CharacterStateMachine` sprach direkt `AnimationPlayer` an; `AnimationController` war de-facto tot.
- Jetzt: `CharacterStateMachine.ResolveAnimationController()` findet `AnimationController` am Owner.
- `AnimationPlayer` bleibt als Fallback, wenn kein Controller existiert.

## 6. PlayerInteractionComponents ohne FindParent("Player")
- Datei: `Characters/Player/PlayerOnlyComponents/PlayerInteractionsComponent/PlayerInteractionComponents.cs`
- Früher: `FindParent("Player")` war fragil.
- Jetzt: Sucht über Parent-Hierarchie nach dem nächsten `Character` (nicht nur Player).

## 7. InventoryUI leert Slots vor Update
- Datei: `UI/Inventory/Scripts/InventoryUI.cs`
- Früher: Slots wurden bei jedem Öffnen appended, was zu Duplikaten führte.
- Jetzt: `ClearInventory()` wird vor `UpdateInventory()` aufgerufen.

## 8. AttackComponent Export-Referenz für HitBox
- Datei: `Characters/Character Components/Attack Component/AttackComponent.cs`
- Früher: `FindChild("HitBoxComponent", recursive: true)` am Parent.
- Jetzt: `[Export] HitBoxComponent _HitBoxComponent` für explizite Verdrahtung in der Szene.
- Fallback: Suche am Tool-Node, wenn Export leer bleibt.

## 9. HitBoxComponent kennt Owner
- Datei: `Characters/Character Components/Hit Box Component/HitBoxComponent.cs`
- Neue Property `OwnerCharacter`, die beim Aktivieren gesetzt wird.
- `UseToolState` übergibt den Besitzer an `ActivateHitBox(Character)`.
- Damit kann später Knockback, Schadensquelle oder XP-Zuweisung realisiert werden.

## 10. Verbleibende GD.Print/GD.PrintErr bereinigt
- Dateien: `InventoryDataResource.cs`, `HealthBarDisplay.cs`, `PlayerInteractionComponents.cs`, `BaseMovementBehavior.cs`, `PlayerMovementComponent.cs`
- Ersetzt durch `Logger.Debug`, `Logger.Warning` oder `Logger.Error`.

## 11. Namenskonventionen korrigiert
- `HurtBoxComponent._OnDamageRecived` -> `_OnDamageReceived`
- Signale ohne `_`-Präfix vorbereitet (teils via neue Wrapper-Methoden).

## 12. Aufräumarbeiten
- Lokale `.uid`-Dateien entfernt.
- Alte Protokoll-/Analyse-Dateien aus früheren Branches entfernt.
- Neue Protokolle: `CHANGELOG_DEEPER_CLEANUP.md`, `SUMMARY_DEEPER_CLEANUP.md`.

## Build-Status
- `dotnet build 'TwoWorlds CSharp.csproj'`
- 0 Warnungen, 0 Fehler.
