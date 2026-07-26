# Home Pentest Lab Safety Checklist

## Before powering on

- Use only intentionally vulnerable, authorized training images.
- Remove bridged adapters from vulnerable targets.
- Use a dedicated host-only or internal network.

## Verify isolation

- Document the expected lab subnet.
- Confirm the target cannot reach the physical LAN.
- Confirm the target cannot reach the public internet unless an exercise explicitly requires it.

## Limit host integration

- Disable shared folders.
- Disable bidirectional clipboard and drag-and-drop.
- Use fictional accounts and synthetic test data.

## Recovery

- Create a clean named snapshot.
- Record VM versions and adapter settings.
- Restore the baseline after exercises.
