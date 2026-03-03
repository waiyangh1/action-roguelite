# Player

## Components (required on Player GameObject)

| Component | Notes |
|---|---|
| `Rigidbody2D` | Gravity Scale = 0, Freeze Z Rotation |
| `Animator` | Controller: `Player.controller` |
| `SpriteRenderer` | Used by GhostTrail to copy sprites |
| `PlayerController` | Main MonoBehaviour |
| `GhostTrail` | Ghost effect for dash |

No `PlayerInput` component — input is managed entirely in code.

---

## State Machine

```
StateMachine<TOwner>          (generic base, Assets/Scripts/StateMachine.cs)
    └── PlayerStateMachine    (Assets/Scripts/Player/PlayerStateMachine.cs)
            ├── PlayerIdleState
            ├── PlayerRunState
            └── PlayerDashState
```

States inherit from `PlayerBaseState : IState`. They access the player via `Player` (`StateMachine.Owner`).

State flow:

```
Idle ──[any input]──► Run ──[no input]──► Idle
 │                      │
 └──[Dash]──► Dash ◄────┘
              │
              └──[duration ends]──► Run or Idle
```

---

## Movement

**Script:** [PlayerRunState.cs](../Assets/Scripts/Player/States/PlayerRunState.cs)

- Input: `Controls.Player.Move` (Vector2, WASD / left stick)
- Direction is normalized before applying velocity — diagonals are not faster
- Velocity applied in `FixedUpdate` via `Rigidbody2D.linearVelocity`
- `LastMoveDir` is updated each frame while moving — persists into Idle for correct facing

**Serialized fields on PlayerController:**

| Field | Default | Description |
|---|---|---|
| `moveSpeed` | `5` | Units per second |

**Animator parameters set by RunState:**

| Parameter | Value |
|---|---|
| `MoveX` | `normalized.x` |
| `MoveY` | `normalized.y` |
| `MoveMagnitude` | `1` |
| `LastMoveX` | `normalized.x` |
| `LastMoveY` | `normalized.y` |

**Animator parameters set by IdleState (on enter):**

| Parameter | Value |
|---|---|
| `MoveX` | `0` |
| `MoveY` | `0` |
| `MoveMagnitude` | `0` |

---

## Dash

**Scripts:**
- [PlayerDashState.cs](../Assets/Scripts/Player/States/PlayerDashState.cs)
- [GhostTrail.cs](../Assets/Scripts/Player/GhostTrail.cs)

**Input:** `Controls.Player.Sprint` (default: Left Shift / gamepad South)

**Behavior:**
1. Can be triggered from Idle or Run
2. Dashes in the direction of `LastMoveDir` (last non-zero input direction)
3. Fixed velocity for `dashDuration` seconds — no steering during dash
4. Cooldown starts immediately on dash enter; cannot dash again until it expires
5. On exit: velocity is zeroed, transitions to Run or Idle based on current input

**Serialized fields on PlayerController:**

| Field | Default | Description |
|---|---|---|
| `dashSpeed` | `18` | Units per second during dash |
| `dashDuration` | `0.18` | Duration of dash in seconds |
| `dashCooldown` | `1.0` | Cooldown after dash in seconds |

### Ghost Trail

**Script:** [GhostTrail.cs](../Assets/Scripts/Player/GhostTrail.cs)

A `MonoBehaviour` that spawns fading sprite copies of the player at regular intervals while the dash is active.

- `Play()` — called by `PlayerDashState.Enter()`, starts the spawn coroutine
- `Stop()` — called by `PlayerDashState.Exit()`, stops spawning
- Each ghost is a new `GameObject` with a `SpriteRenderer` (copies current sprite, flip, sorting layer)
- Ghosts fade from `ghostColor.alpha` → 0 over `ghostLifetime` seconds, then self-destroy

**Serialized fields on GhostTrail:**

| Field | Default | Description |
|---|---|---|
| `interval` | `0.04` | Seconds between ghost spawns |
| `ghostLifetime` | `0.25` | Seconds for ghost to fully fade |
| `ghostColor` | `(0.4, 0.8, 1.0, 0.55)` | Tint and starting alpha of ghost |

---

## Input Summary

| Action | Binding | Controls action |
|---|---|---|
| Move | WASD / Left Stick | `Player.Move` |
| Dash | Left Shift / Gamepad South | `Player.Sprint` |

Input asset: `Assets/Controls.inputactions`
