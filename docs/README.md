# Respawn Tools

Respawn Tools is a practical mod for Vintage Story, designed to improve the experience of death and revival in-game. 
It introduces two unique items: the Respawn Beacon, which allows players to respawn at a chosen location, and the
Respawn Tablet, a single-use item that saves players from otherwise fatal damage. These features offer server owners and players
greater flexibility and control over respawn mechanics, making gameplay more forgiving and strategic without compromising
challenge.

## Features

### Respawn Beacon

The Respawn Beacon is a special block that, when activated, allows any player within its range to respawn at its location upon death, rather than at their default spawn point.
This is ideal for community hubs, adventure maps, or challenging areas where returning to a specific spot is crucial. 
The beacon's settings can be customised, including its range, sound effects, and activation status, providing server administrators with full control over its behaviour.

| Beacon Setting | Default | Range | Description |
| --- | ---: | ---: | --- |
| Enabled | Off | On/Off | Whether this beacon can be selected as a respawn point. |
| Radius | 128 blocks | 0–1,280 blocks | Horizontal distance within which the beacon is eligible. |
| Ambient volume | 100% | 0–100% | Volume of the beacon's ambient sound while enabled. |
| Respawn volume | 100% | 0–100% | Volume of the sound played when a player respawns at it. |

### Respawn Tablet

A Respawn Tablet is a single-stack item that automatically saves its holder from death when it is in their main hand, off hand, backpack, or selected hotbar slot. 
Each activation consumes one charge; the tablet is consumed when it runs out.

## Server settings

Administrators can change these world settings through the mod's admin settings in the mod settings menu. 
Turning off crafting removes the recipe but leaves the item or block available in the Creative menu.

| Feature | Setting | Default | Description |
| --- | --- | ---: | --- |
| Respawn Beacon | Craftable | On | Whether players can craft Respawn Beacons. |
| Respawn Tablet | Craftable | On | Whether players can craft Respawn Tablets. |
| Respawn Tablet | Charges | 1 | Charges given to a tablet; configurable from 1 to 10. |

## Privileges

Privileges can be assigned or removed for server roles using the IDs below. 
Administrators are automatically granted each privilege. 
The two **use** privileges are also added to every server role by default; remove them from a role if its players should not use that feature.

| Feature | Privilege ID | Allows |
| --- | --- | --- |
| Respawn Beacon | `respawntools.beacons.place` | Place Respawn Beacon blocks. |
| Respawn Beacon | `respawntools.beacons.configure` | Sneak-right-click a beacon to change its settings. |
| Respawn Beacon | `respawntools.beacons.use` | See and select **Respawn at Beacon**, and respawn at an eligible beacon. |
| Respawn Beacon | `respawntools.beacons.break` | Break Respawn Beacon blocks. |
| Respawn Tablet | `respawntools.tablets.use` | Activate Respawn Tablets. |

In Creative mode, placing, configuring, and breaking beacons does not require the corresponding beacon privilege.
The beacon and tablet **use** privileges still control whether the player can use those respawn features.
