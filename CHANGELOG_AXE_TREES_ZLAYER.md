# CHANGELOG_AXE_TREES_ZLAYER.md

## Branch
`fix/axe-trees-zlayer`

## Auslöser
Play-Test zeigte zwei Probleme:
1. Die Axt trifft Bäume nicht / kann diese nicht abbauen.
2. Der Charakter wird immer vor den Bäumen gerendert, nie dahinter (falscher Z-Layer).

## Änderungen

### Problem 1: Axt trifft Bäume nicht
- `Characters/Character Components/Hurt Box Component/HurtBoxComponent.cs`
  - Entfernt: falsche Prüfung `hitBoxComponent.Owner is HandItem` (`Owner` ist in Godot nicht der Szenenparent).
  - Verwendet jetzt direkt `hitBoxComponent.Tool.HandItemCategory`.
  - Fügt einen Cooldown-Schutz hinzu (`_CanGetDamage`), damit ein Baum nicht pro Frame mehrfach getroffen wird.
  - Startet den vorhandenen `CooldownTimer` beim Treffer.
  - `EffectiveItems` ist jetzt `public`, damit Export-Werte aus Szenen korrekt zugewiesen werden.
- `Scenes/Objects/Trees/small_tree.tscn`
  - `_EffectiveItems = [1]` korrigiert zu `EffectiveItems = [1]` (C#-Export-Property).
- `Scenes/Objects/Trees/large_tree.tscn`
  - `_EffectiveItems = [1]` korrigiert zu `EffectiveItems = [1]`.

### Problem 2: Z-Layer / Y-Sort
- `Scenes/Testing/WorldInitialization.cs`
  - `ZIndex` des Players von `1` auf `0` gesetzt (ansonsten war er immer über Objekten).
  - `ZAsRelative = true` und `YSortEnabled = true` aktiviert, damit der Player korrekt nach Y-Position sortiert wird.
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
- `Tools/Axe/axe.tscn`
- `Tools/HandItem.cs`
- `Tools/DataTypes.cs`
