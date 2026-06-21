# CHANGELOG: Iteration 4 — Service-Locator

Branch: `fix/iteration-4-service-locator`
Ziel: Singleton-Lookups zentralisieren und FindChild reduzieren.

1. Services.cs eingeführt mit `Services.Get<T>`.
2. Alle `/root/...`-Lookups durch `Services.Get<T>` ersetzt.
3. FindChild-Aufrufe in Item, Sword, SmallTree, AnimationController, HealthBarDisplay, InventorySlotUI, PlayerInteractionComponents, Player, Character durch Export-Referenzen ersetzt.
4. Alte Protokolldateien entfernt.
5. Build: 0 Warnungen, 0 Fehler.
