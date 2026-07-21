# HA0004 Koch Age Oven — PlantPAx-Style Display Redesign Spec

Companion to `AgeOven_PlantPAx_Mockup.html` (the clickable design).
This maps the 34 legacy FT View ME displays onto a new high-performance
display set and names the library objects to build them with in Studio.

## Style rules (ISA-101 / PlantPAx high-performance)

- Gray canvas; equipment drawn in muted grays. **Color appears only for
  abnormal**: red = urgent alarm/trip, yellow = warning. Running
  equipment = white fill; stopped = dark gray. No green "everything OK"
  paint, no photo-realistic 3D gradients from the legacy screens.
- Every process value lives in a white value box; interactive elements
  get the blue touch border; abnormal values get a blinking red border.
- Persistent chrome: top banner (app · screen title · newest unack
  alarm · clock), bottom navigation bar, alarm count on the ALARMS key.
- Detail lives in **faceplates** (popups), not dedicated screens.

## New screen set (5 + faceplates, replacing 34)

| New screen | Replaces (legacy) |
|---|---|
| **MAIN** — P&ID overview: zones, burners, fans, loops, hi-limits, state, part count | FurnaceMain, StatusSafetyOverview, Age Oven Message Display, S500_Servo_STATUS |
| **TRENDS** — temps + output + 1 s flame signal | Age Oven flame intensity_04H/08H/12H/24H, Flame_Intensity_Trend_Select |
| **DATA LOG** — logger status, latest samples, export | (new — the improved-data-logging screen) |
| **ALARMS** — alarm summary w/ severity + ack | Faults_Screen, Flame_Safety_Fault/Normal msg displays |
| **MAINTENANCE** — device grid → faceplates | Maintenance_Overview, Maintenance_MotorControl, Maintenance_Flame_Safety_Overview, Maintenance_MB_Comm |
| *Faceplates* (popups) | Eng_PID_Screen, Eng_Motor_Para, Eng_Various_Parameters, Maintenance_PLC_* I/O screens (fold into per-device diagnostics tabs), Pop_Up_Confirm_* |
| *Keep as-is initially* | Operator/Eng/Quality profile + test screens (Soak_Data, Quality_*, Eng_Profile_Config, Operator_Profile, Contact_Info) — migrate in phase 2 with the same style rules |

## Library objects (Rockwell Automation Library of Process Objects — ME faceplates)

| Device | Object / faceplate |
|---|---|
| Loop 1 / Loop 2 (nanodac via PLC) | `P_PIDE` — PV/SP/OUT, Auto-Manual, SP ramp |
| 5 fans | `P_Motor` — run status, start/stop, run hours, interlocks |
| Hi-limit instruments | `P_AIn` + `P_Intlk` (trip = interlock to burner group) |
| Flame safety | `P_DIn` / custom faceplate showing FS signal strength (analog) |
| Alarms | `P_Alarm` conventions: severity by shape + color + text |

If the Process Objects Library isn't in use on this PLC, build the same
faceplates as plain ME displays with parameter files — the existing
`PAR/` files already parameterize per-device screens, so the pattern
carries straight over.

## Build order in View Studio

1. Import the Process Library ME faceplates (or create the 3 faceplate
   displays with parameter files).
2. Build MAIN from the mockup's P&ID layout; wire value boxes to the
   same `[Aging_Furnace]` tags listed in the runtime runbook.
3. Rebuild the trend displays against the upgraded data log model.
4. Alarm summary display using the ME alarm setup (import existing
   trigger tags from `M_Alarms/MachineAlarms.mal`).
5. Startup macro unchanged; replace display references as screens land.
6. Follow `../HA0004_Koch_Age_Oven/RUNTIME_BUILD_RUNBOOK.md` to compile
   and deploy the runtime.
