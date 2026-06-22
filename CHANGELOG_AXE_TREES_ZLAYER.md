# CHANGELOG_AXE_TREES_ZLAYER.md

## Branch
`fix/axe-trees-zlayer`

## Auslöser
Play-Test zeigte zwei Probleme:
1. Die Axt trifft Bäume nicht / kann diese nicht abbauen.
2. Der Charakter wird immer vor den Bäumen gerendert, nie dahinter (falscher Z-Layer).

## Analyse
Die Test-Szene `Scenes/Testing/Test_Scene_Objects_Trees.tscn` platzierte Bäume als **Atlas-Tiles** im `Trees`-TileMapLayer. Atlas-Tiles haben kein `HurtBoxComponent`, `HealthComponent` oder korrektes Y-Sort-Verhalten, weswegen die Axt nichts treffen konnte und der Z-Layer nicht funktionierte. Die korrekt aufgebauten Baum-Szenen `small_tree.tscn` / `large_tree.tscn` waren nur im TileSet als Szenen-Quelle hinterlegt, wurden aber nicht verwendet.

## Änderungen

### Problem 1: Axt trifft Bäume nicht
- `Characters/Character Components/Hurt Box Component/HurtBoxComponent.cs`
  - Entfernt: falsche Prüfung `hitBoxComponent.Owner is HandItem` (`Owner` ist in Godot nicht der Szenenparent).
  - Verwendet jetzt direkt `hitBoxComponent.Tool.HandItemCategory`.
  - Fügt einen Cooldown-Schutz hinzu (`_CanGetDamage`), damit ein Baum nicht pro Frame mehrfach getroffen wird.
  - Startet den vorhandenen `CooldownTimer` beim Treffer.
  - `EffectiveItems` ist jetzt `public`, damit Export-Werte aus Szenen korrekt zugewiesen werden.
- `StateMachine/UseToolState.cs`
  - Fallback hinzugefügt, um `HitBoxComponent` auch im Parent-Node des StateMachine-Owners zu suchen, falls die direkte Kind-Suche fehlschlägt.
- `Scenes/Objects/Trees/small_tree.tscn`
  - `_EffectiveItems = [1]` korrigiert zu `EffectiveItems = [1]` (C#-Export-Property).
- `Scenes/Objects/Trees/large_tree.tscn`
  - `_EffectiveItems = [1]` korrigiert zu `EffectiveItems = [1]`.
- `Scenes/Testing/TreeTileReplacer.cs` (neu)
  - Ersetzt Atlas-Bäume in der Test-Szene zur Laufzeit durch echte `small_tree.tscn` / `large_tree.tscn`-Instanzen.
  - Instanziiert Baum-Szenen an den Weltpositionen der Atlas-Tiles, aktiviert `YSortEnabled`, und leert danach den TileMapLayer.
- `Scenes/Testing/Test_Scene_Objects_Trees.tscn`
  - `TreeTileReplacer`-Node hinzugefügt, der `Trees`-Layer referenziert.

### Problem 2: Z-Layer / Y-Sort
- `Scenes/Testing/WorldInitialization.cs`
  - `ZIndex` des Players von `1` auf `0` gesetzt (ansonsten war er immer über Objekten).
  - `ZAsRelative = true` und `YSortEnabled = true` aktiviert, damit der Player korrekt nach Y-Position sortiert wird.
- `Scenes/Testing/Test_Scene_Objects_Trees.tscn`
  - `y_sort_enabled = true` auf Root-Node und `GameTileMap` aktiviert, damit Bäume und Player korrekt nach Y-Position sortiert werden.
- `Scenes/Objects/Trees/small_tree.tscn`
  - `z_index = 1` und `y_sort_enabled = true` auf der Root-Node aktiviert.
  - `y_sort_enabled = true` auf `StaticBody2D` aktiviert.
- `Scenes/Objects/Trees/large_tree.tscn`
  - `z_index = 1` und `y_sort_enabled = true` auf der Root-Node aktiviert.
  - `y_sort_enabled = true` auf `StaticBody2D` aktiviert.

## Build
`dotnet build 'TwoWorlds CSharp.csproj' -v quiet`
- 0 Warnungen
- 0 Fehler

## Verifizierte Dateien
- `Characters/Character Components/Hurt Box Component/HurtBoxComponent.cs`
- `Characters/Character Components/Hurt Box Component/Hurt_Box_Component.tscn`
- `Scenes/Objects/Trees/small_tree.tscn`
- `Scenes/Objects/Trees/large_tree.tscn`
- `Scenes/Testing/WorldInitialization.cs`
- `Scenes/Testing/Test_Scene_Objects_Trees.tscn`
- `Scenes/Testing/TreeTileReplacer.cs`
- `StateMachine/UseToolState.cs`
- `Tools/Axe/axe.tscn`
- `Tools/HandItem.cs`
- `Tools/DataTypes.cs`
