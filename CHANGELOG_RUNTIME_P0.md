# CHANGELOG: Runtime P0 & Architecture P1/P3 Fixes

Branch: `fix/runtime-p0-issues`
Ziel: Kritische Laufzeitfehler und Architekturrisiken aus der Analyse vom 2026-06-21 beheben.

---

## 1. Setup
- Branch `fix/runtime-p0-issues` von `feature/merged-state-machine-fixes` abgeleitet.
- Changelog und Summary angelegt.

## 2. Unversionierte Artefakte aufräumen
- `.uid`-Dateien sind Godot-4.4-Metadaten und gehören nicht in das Repo, solange das Projekt auf Godot 4.3 ausgerichtet ist.
- `TwoWorlds CSharp.csproj.old` ist ein Backup-Relikt.
- `.gitignore` um `*.uid` erweitert.
- Alle `.uid`-Dateien und `TwoWorlds CSharp.csproj.old` entfernt.

## 3. P0 Fix: UseToolState direkter CollisionShape-Zugriff
- Datei: `StateMachine/UseToolState.cs`
- Problem: State-Logik griff direkt auf `_HitBoxComponent._CollisionShape2D.Disabled` zu, brach bei fehlendem Shape, und deaktivierte die Hitbox nicht beim Verlassen des States.
- Lösung:
  - Nur noch `_HitBoxComponent.ActivateHitBox()` / `_HitBoxComponent.DeactivateHitBox()` aufrufen.
  - In `Exit()` sicher deaktivieren.
  - Null-Check für `_HitBoxComponent`.

## 4. P0 Fix: Heilitems heilen den falschen Player
- Dateien:
  - `Items/ItemEffects/ItemEffectResource.cs`
  - `Items/ItemEffects/Heal [Item Effect Resource]/HealItemEffectResource.cs`
  - `Items/Scripts/ItemDataResource.cs`
  - `UI/Inventory/InventorySlotUI.cs`
- Problem: `HealItemEffectResource` nutzte `GameManager.getPlayer()` und heilte immer einen potenziell falschen, von der Engine instanziierten Player.
- Lösung:
  - `ItemEffectResource.Use(Character user)` eingeführt.
  - `ItemDataResource.Use(Character user)` eingeführt.
  - `HealItemEffectResource.Use(Character user)` heilt jetzt den übergebenen Character.
  - `InventorySlotUI` ermittelt den aktuellen Player über die Gruppe `"Player"` und übergibt ihn an das Item.

## 5. P0 Fix: Verwaiste Signal-Verbindungen in Szenen
- Dateien:
  - `Characters/Player/Player.tscn`
  - `UI/Inventory/Inventory.tscn`
  - `Characters/Enemies/Minotaur/Minotaur.tscn`
- Problem: Drei Szenen hatten Signal-Verbindungen auf nicht existierende Methoden.
- Lösung:
  - `animation_finished` → `_FinishedAnimation` entfernt.
  - `InventoryActive` → `_on_inventory_inventory_active` entfernt.
  - `area_entered` → `_on_area_entered` entfernt.

## 6. P0 Fix: Inventory Stapel-Limit und leere Slots
- Dateien:
  - `Items/Scripts/ItemDataResource.cs`
  - `UI/Inventory/Scripts/InventoryDataResource.cs`
  - `UI/Inventory/InventorySlotUI.cs`
- Problem:
  - Items konnten unbegrenzt in einen Slot gestapelt werden.
  - Leere Slots blieben nach Verbrauch mit 0 Quantity erhalten.
- Lösung:
  - `ItemDataResource._MaxStackSize = 99` eingeführt.
  - `InventoryDataResource.addItem(item, amount)` respektiert Stapel-Limit und verteilt auf mehrere Slots.
  - `InventorySlotUI.ItemPressed` setzt leere Slots auf `null` zurück.

## 7. P1 Fix: WalkState generisch aufräumen
- Datei: `StateMachine/WalkState.cs`
- Problem: State enthielt Player-Sonderlogik (`if (Owner.GetType() != typeof(Player))`).
- Lösung:
  - WalkState bewegt nur noch generisch über `IMovementComponent`.
  - Richtung kommt von außen: PlayerStateMachine setzt `_CurrentDirection` via Input, NPC-Behavior setzt sie über `_MovementBehavior`.
  - WalkState sucht sich selbst das `MovementComponent`.

## 8. P1 Fix: PlayerStateMachine robust machen
- Datei: `StateMachine/PlayerStateMachine.cs`
- Problem: `async void _Ready()` ohne `await`, fehlende Entkopplung der Signal-Handler.
- Lösung:
  - `async` entfernt.
  - `GetNodeOrNull` für InputHandler.
  - `_ExitTree()` entkoppelt Signal-Handler.

## 9. P1 Fix: ToolStateMachine Owner.Owner-Zugriff entfernen
- Datei: `StateMachine/ToolStateMachine.cs`
- Problem: ToolStateMachine kletterte mühsam durch `Owner` und `Owner.Owner`, um die Owner-StateMachine zu finden.
- Lösung:
  - `[Export] private CharacterStateMachine _OwnerStateMachine` eingeführt.
  - `AnimationFinished` wird beobachtet, damit UseTool automatisch wieder in Idle zurückkehrt.
  - `_ExitTree()` entkoppelt InputHandler und Animation-Handler.

## 10. P1 Fix: HitBoxComponent generisch gestalten
- Datei: `Characters/Character Components/Hit Box Component/HitBoxComponent.cs`
- Problem: Suche nach Parent war hart kodiert und brach leicht.
- Lösung:
  - `FindHandItemParent()` und `FindCharacterParent()` laufen die Hierarchie hoch, statt NodeName-Annahmen zu machen.
  - `_Tool` ist jetzt allgemein `HandItem` statt `Sword`.
  - Null-Checks vor jedem Zugriff.

## 11. P1 Fix: AttackComponent auf Character umstellen
- Datei: `Characters/Character Components/Attack Component/AttackComponent.cs`
- Problem: `GetParent<Player>()` funktioniert nur für Player, nicht für NPCs.
- Lösung:
  - `GetParent<Character>()` statt `GetParent<Player>()`.
  - Schadensrichtung aus `_CurrentLookingDirection` des Characters.
  - Null-Checks für `_HitBoxComponent` und `_HitBoxTimer`.

## 12. P1 Fix: Player-Equipment generisch halten
- Datei: `Characters/Player/Player.cs`
- Problem: `as Sword` war hardcodiert, und einige `GetNode`-Aufrufe fehlten.
- Lösung:
  - `EquipHandItemFromToolNode()` sucht generisch nach `HandItem` im Tool-Node.
  - Alle Node-Lookups auf `GetNodeOrNull` umgestellt.
  - `RegisterPlayer(this)` in `GameManager` eingeführt.

## 13. P1 Fix: GameManager vereinfachen
- Datei: `GameManager.cs`
- Problem: `GameManager` feuerte `PlayerSpawned`, aber Player wurde von `World` und `WorldInitialization` unabhängig instanziiert.
- Lösung:
  - `GameManager.RegisterPlayer(Player)` zentriert die Player-Registrierung.
  - `_Player` wird nur noch einmal über das Autoload gesetzt.
  - `EventBus.PlayerSpawned` wird zentral emittiert.

## 14. P1 Fix: CharacterStateMachine robust aufräumen
- Datei: `StateMachine/CharacterStateMachine.cs`
- Problem: `_Ready` war `async void`, Registry-Bildung erfolgte nach Frame-Wait.
- Lösung:
  - `async` entfernt.
  - State-Registry wird sofort gebaut.
  - `ResolveAnimationPlayer()` ist überschreibbar, damit `ToolStateMachine` seinen eigenen AnimationPlayer findet.
  - Default-State-Entry wird per `CallDeferred` ausgeführt.

## 15. P2 Fix: FollowState dokumentieren
- Datei: `StateMachine/FollowState.cs`
- Lösung: Klassen-Level-Dokumentation ergänzt, die erklärt, dass FollowState lediglich ein WalkState mit einem `FollowPlayerBehavior` ist.

## 16. P2 Fix: PlayerInteractionComponents robust machen
- Datei: `Characters/Player/PlayerOnlyComponents/PlayerInteractionsComponent/PlayerInteractionComponents.cs`
- Problem: `async void _Ready()`, harte `GetNode`-Aufrufe, `Owner.FindChild` statt lokalem Node.
- Lösung:
  - `async` entfernt.
  - `GetNodeOrNull` für alle Child-Nodes.
  - `InteractionArea` wird lokal im Node gesucht.
  - Frühzeitiger Abort bei fehlenden Abhängigkeiten mit `GD.PrintErr`.

## 17. P3 Fix: Toter Code entfernen
- Dateien:
  - `Characters/Player/Enemy.cs`
  - `Characters/Character.cs`
- Lösung:
  - Auskommentierte Zeilen und nicht mehr benötigte Felder entfernt.
  - `Enemy.cs` auf minimale, funktionierende Implementation reduziert.

## 18. P3 Fix: NPCInputHandler entfernen
- Dateien:
  - `Characters/NPCInputHandler.cs` gelöscht.
  - `Characters/Enemies/Minotaur/Minotaur.tscn`
- Problem: Klasse war komplett auskommentiert und diente nur als Behaviors-Container.
- Lösung:
  - Behaviors werden jetzt direkt unter dem Character als Child-Nodes angelegt.
  - NPCInputHandler-Node und Script-Referenz aus der Minotaur-Szene entfernt.

## 19. P3 Fix: Typo korrigieren
- Dateien:
  - `Characters/Character Components/Health Component/HealthComponent.cs`
  - `Scenes/Objects/Trees/SmallTree.cs`
- Lösung:
  - `StartTemporaryInvulnerarbility` → `StartTemporaryInvulnerability`.
  - `ReciveDamage` → `ReceiveDamage`.

## 20. Build-Ergebnis
- Letzter Build: 0 Warnungen, 0 Fehler.
