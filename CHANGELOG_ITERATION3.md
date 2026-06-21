# CHANGELOG: Iteration 3 — Konventionen, Safety, Cleanup

Branch: `fix/iteration-3-cleanup`
Ziel: C#-Konventionen durchsetzen, öffentliche API bereinigen, Null-Safety verbessern, tote Dateien entfernen.

---

## 1. Öffentliche Member PascalCase ohne `_`
- `Item.cs`: `_ItemData` -> `ItemData`
- `Chest.cs`: `_InteractionLabel` -> `InteractionLabel`
- `HandItem.cs`: `_HitBoxComponent` -> `HitBoxComponent`, `_Damage` -> `Damage`, `_HandItemCategory` -> `HandItemCategory`
- `ItemDataResource.cs`: `_Description` -> `Description`, `_Texture` -> `Texture`, `_MaxStackSize` -> `MaxStackSize`, `_Name` -> `Name`
- `AnimationController.cs`: `_AnimationPlayer` -> `AnimationPlayer`, `_EffectPlayer` -> `EffectPlayer`
- `inventory_menu.cs`: `_IsOpen` -> `IsOpen`
- `SlotDataResource.cs`: `_ItemData` -> `ItemData`, `_Quantity` -> `Quantity`
- `InventoryDataResource.cs`: `_Slots` -> `Slots`
- `Player.cs`: `_INVENTORY_DATA` -> `InventoryData`
- `CharacterMovementComponent.cs`: `_Character` -> `Character`, `_StartPosition` -> `StartPosition`, `_CurrentPosition` -> `CurrentPosition`
- `WalkState.cs`: `_MovementBehavior` -> `MovementBehavior`
- `CharacterStateMachine.cs`: `_CurrentDirection` -> `CurrentDirection`
- `WorldInitialization.cs`: `_PlayerSpawnPosition` -> `PlayerSpawnPosition`, `_PlayerParentPath` -> `PlayerParentPath`
- `PlayerMovementComponent.cs`: `_MovingSpeed` -> `MovingSpeed`
- `HealthComponent.cs`: `_MaxHealth`, `_CurrentHealth`, `_HealthChanged`, `_MaxHealthChanged`, `_HealthEmpty` konsequent PascalCase ohne `_`

## 2. Klassennamen korrigiert
- `data_types` -> `DataTypes`
- `inventory_menu` -> `InventoryMenu`

## 3. HealthComponent-Signale ohne `_`
- `_HealthChanged` -> `HealthChanged`
- `_MaxHealthChanged` -> `MaxHealthChanged`
- `_HealthEmpty` -> `HealthEmpty`

## 4. Null-Safety verbessert
- `CharacterStateMachine.ResolveAnimationController()` und `ResolveAnimationPlayer()` prüfen `Owner` auf null.
- `WalkState._Ready()` prüft `Owner` auf null.
- `UseToolState._Ready()` prüft `Owner` auf null.
- `Player.InventoryData` ist jetzt Lazy-Property mit `ResourceLoader.TryLoad` und Logger-Fehler.

## 5. FindChild-Reduktion
- `Player.cs`: HealthComponent, MovementComponent, AttackComponent, PlayerInteractionComponents, InputHandler, GameManager als Export-Referenzen eingeführt.
- `Character.cs`: HealthComponent als Export-Referenz.
- `AttackComponent`: HitBoxTimer als Export-Referenz.
- `HitBoxComponent`: CollisionShape2D als Export-Referenz.
- `HurtBoxComponent`: CooldownTimer als Export-Referenz.
- `HealthBarDisplay`: Hurtbar, Healthbar, VisibleTimer als Export-Referenzen.
- `AnimationController`: AnimationPlayer, EffectPlayer, HurtEffectTimer als Export-Referenzen.

## 6. Tote Dateien entfernt
- `Characters/Character Components/CharacterMovementComponent.cs` gelöscht (wurde von keiner Szene referenziert).
- `Characters/Player/PlayerOnlyComponents/CharacterMovementComponent`-Verzeichnis geprüft.

## 7. Verbleibende GD.Print/GD.PrintErr
- Keine verbleibenden `GD.Print` / `GD.PrintErr` außer in `Logger.cs` selbst.

## 8. Aufräumarbeiten
- Alte Protokoll-/Analyse-Dateien aus vorherigen Branches entfernt.
- Neue Protokolle: `CHANGELOG_ITERATION3.md`, `SUMMARY_ITERATION3.md`, `ITERATION3_ANALYSIS_2026-06-21.md`.

## Build-Status
- `dotnet build 'TwoWorlds CSharp.csproj'`
- 0 Warnungen, 0 Fehler.
