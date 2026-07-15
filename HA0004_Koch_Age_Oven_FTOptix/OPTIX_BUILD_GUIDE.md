# HA0004 Koch Age Oven — FactoryTalk Optix Build Guide

Click-path guide to build the full Optix project in FactoryTalk Optix
Studio: comms, tags, the two data loggers, alarms, and the PlantPAx-style
display set. The visual design of record is
`../HA0004_Koch_Age_Oven_ME_Demo/AgeOven_PlantPAx_Mockup.html` — open it
side-by-side and copy what you see.

## 0. Create the project

New project → template **"HMI - basic"** (or empty) → name
`HA0004_Koch_Age_Oven`. Pick the target now (OptixPanel / PC runtime /
PanelView 5510 w. Optix firmware) so the resolution matches — the mockup
is laid out for 1120×760 landscape.

## 1. Communication

Project view → **CommDrivers** → right-click → *New* →
**RAEtherNet/IP Driver** → under it *New Station*:
- Name: `Aging_Furnace`  ·  Address: *real PLC IP*  ·  Slot: 0
- (mirrors the FT View ME shortcut `[Aging_Furnace]`)

## 2. Tags

With the PLC reachable: right-click the station → **Online tag import**
and pull the tags listed in `TagImport/AgeOvenDataLog_Tags.csv`
(browse to the UDT members — the browser is the source of truth for the
paths flagged `NEW - verify`). Drag them into **Model** folders named
per `ProjectFiles/ModelTags.xml`: `ControlLoops`, `FlameSafety`,
`HiLimits`, `Motors`, `Production`.

## 3. Data storage + loggers (the improved logging)

1. **DataStores** → *New* → **Embedded Database** → name `AgeOvenLogs`.
2. **Loggers** → *New* → **DataLogger** → `AgeOvenProcessLog`
   - Store: `AgeOvenLogs` · Table: `ProcessLog`
   - Sampling: Periodic, **10 s** · log timestamp + quality
   - Add all 20 Model variables per `ProjectFiles/DataLogger.xml`
3. Second DataLogger → `FlameSafetyFastLog`
   - Store: `AgeOvenLogs` · Table: `FlameSafetyFast`
   - Sampling: Periodic, **1 s**
   - Variables: Z1/Z2 FS signal strength + both Working_Outputs
4. Retention: add the `DeleteRecordsOlderThan`-style purge (Optix ≥1.4:
   DataLogger "Log duration" property) — 365 d process / 30 d fast.

## 4. Alarms

**Alarms** folder → per `ProjectFiles/Alarms.xml`:
- `Z1_HiLimit_Trip` — **DigitalAlarm** on `Model/HiLimits/Z1_HiLimit_Tripped`,
  severity 900, message "Zone 1 hi-limit TRIPPED — burners locked out"
- `Z2_HiLimit_Trip` — same on Z2, severity 900
- `Z1_Flame_Low`, `Z2_Flame_Low` — **ExclusiveLevelAlarm** on FS signal,
  LowLow 20 (severity 700) / Low 35 (severity 500)
- `Active_Fault` — DigitalAlarm on `Model/Production/Active_Fault`, severity 800
Add an **AlarmGrid** to the ALARMS screen and the standard banner to the
header (Optix template "Alarm banner").

## 5. Screens (PlantPAx style — copy the mockup)

Style constants first (**Project → Styles**): canvas `#C6C8C9`, chrome
`#4A4E52`, equipment `#93989C`/`#5A5E62`, box white w/ `#6D7276` border,
interactive border `#2A78D6`, alarm red `#D03B3B`, warning `#FAB219`.
Color only for abnormal; running = white fill, stopped = dark gray.

| Screen | Optix pieces |
|---|---|
| **MainLayout** | Vertical layout: header panel (app name, screen title, alarm banner, DateTime label) · content panel · nav bar (5 buttons + alarm count label bound to `Retain::AlarmCount`) |
| **MAIN** | P&ID per mockup: Rectangles/Polygons for oven + ducts, 5 fan symbols (Ellipse + Polygon blades; fill bound to `.Running`), 2 burner symbols w/ flame Image visible-bound to FS>20, value boxes = Label + border, click → open faceplate Dialog |
| **TRENDS** | 3 **Trend** objects fed by the two loggers (temps+SP / outputs / FS fast), pens in this order & color: L1 `#2A78D6`, L2 `#1BAF7A`, Z1 `#EDA100`, Z2 `#008300`; SP pens dashed |
| **DATA LOG** | 2 status panels (row counts via NetLogic or DataGrid count) · **DataGrid** on `ProcessLog` table, newest first · "Export CSV" button → `CsvExportLogic.ExportProcessLog` |
| **ALARMS** | **AlarmGrid** + ack buttons (built-in) |
| **MAINTENANCE** | Grid of device cards → same faceplate dialogs |
| **Faceplates** (Dialog types) | `FP_Loop` (PV label, SP spinbox ±, OUT LinearGauge, Auto/Man buttons), `FP_Motor` (status, run hours, START/STOP), `FP_Flame` (FS gauge + state) — mirror the mockup's faceplates |

## 6. NetLogic

Add `NetLogic/CsvExportLogic.cs` (in this folder) as a Runtime NetLogic
under the DATA LOG screen. It queries `AgeOvenLogs` and writes
`AgeOvenProcessLog_<date>.csv` to `%PROJECTDIR%/CSVExport/`. Verify the
`Store.Query` signature against your Optix version's docs (1.4+).

## 7. Users & deploy

- Users: `Operator` (view, ack), `Maintenance` (+faceplate commands),
  `Engineer` (+SP changes) — map faceplate buttons' "Allowed users".
- **Save and build** → fix any binding errors → **Run in emulator** and
  compare against the mockup scenario-by-scenario → deploy to target.

## Acceptance checklist

- [ ] Both loggers writing (check row growth in DataStores panel)
- [ ] Trip test: force `Z1_HiLimit_Tripped` in the PLC → banner red,
      MAIN boxes blink, alarm in grid, burners show LOCKOUT
- [ ] CSV export produces the 20-column file
- [ ] All 20 tags Good quality on the DATA LOG screen
- [ ] Faceplate commands blocked for Operator role
