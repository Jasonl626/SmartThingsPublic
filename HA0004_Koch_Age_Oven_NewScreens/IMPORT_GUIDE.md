# HA0004 Koch Age Oven — New ME Screens: Import Guide

These are **real FactoryTalk View ME graphic-display XML files** in your
project's exact export dialect (schema `Gfx-ME16.xsd`, learned from your
own `Motor_11100.xml` export). They import through the same Graphics
Import Export Wizard you used to export — producing actual `.gfx`
displays in the `HA0004_Koch_Age_Oven` project.

## Files

| File | Display # | Contents |
|---|---|---|
| `AgeOven_MAIN.xml` | 10 | PlantPAx **graphical P&ID overview**: oven vessel with Zone 1/2, fans + ducts, conveyor with skids, burners with flame indicators, embedded per-zone readout panels (PV·SP·Output·Hi-Limit), hi-limit trip, system-fault + skid part count. Green = running/OK, red = fault/trip (matches your Motor screen's house style) |
| `AgeOven_PROCESS_VALUES.xml` | 11 | All 20 improved-logging tags: 12 numeric values + 8 boolean indicators |

Every element and attribute here was copied from your own export — only
`rectangle`, `text`, `numericDisplay`, `multistateIndicator`, and their
`connections`/`states` children are used. Both files are validated as
well-formed XML.

## Import steps (View Studio)

1. Open the `HA0004_Koch_Age_Oven` project.
2. Explorer → right-click **Displays** → **Import and Export…**
3. Choose **Import graphic information into displays** → Next.
4. Choose **Import a single display** (repeat per file), browse to
   `AgeOven_MAIN.xml`, and let it create a **new** display. Repeat for
   `AgeOven_PROCESS_VALUES.xml`.
5. Open each imported display to check it renders, then Save.

## Two values to confirm on first import (I flagged these because I
could not observe them in your export)

- **`displayType="replace"`** on both — the standard full-screen base
  display. Your sample was an `onTop` popup, so this one attribute is
  domain-standard rather than copied. If the wizard objects, set the
  display's type in Display Settings after import.
- **Canvas size `800 x 600`** — I sized these for a common PanelView
  Plus screen. If your panel differs, change width/height in Display
  Settings (or the `displaySettings` element) before importing.

## Tag paths — verify the ones marked NEW

The 8 original nanodac/flame tags are verbatim from the data log model
and are correct. These added paths are **inferred** from the ME
parameter files and must be confirmed against the ControlLogix program —
the import will still succeed, but the tags read error until the paths
match:

```
{[Aging_Furnace]HMI_COMM_CONFIG_MB_Z1_HI_LIMIT.PV}        (+ .Tripped)
{[Aging_Furnace]HMI_COMM_CONFIG_MB_Z2_HI_LIMIT.PV}        (+ .Tripped)
{[Aging_Furnace]ZONE1.Comb_Air_Fan.Running}   ZONE1.ReCirc_Fan.Running
{[Aging_Furnace]ZONE2.Comb_Air_Fan.Running}   ZONE2.ReCirc_Fan.Running
{[Aging_Furnace]EXHAUST.Fan.Running}
{[Aging_Furnace]SKID_PART_COUNT}   OVEN_STATE   SYSTEM_FAULT_ACTIVE
```

## Finishing in Studio (not in the XML)

- **Navigation**: drag the existing `NavBar` global object onto each new
  display (goto-display buttons are a Studio object I did not author
  blind).
- **Trends & alarm summary**: add the Trend and Alarm objects from
  Studio's toolbox — they aren't part of this XML vocabulary. Point the
  Trend at the upgraded data log model per the runtime runbook.
- Set one screen as the startup display (or wire NavBar) and rebuild the
  runtime per `../HA0004_Koch_Age_Oven/RUNTIME_BUILD_RUNBOOK.md`.

## Want the rest as importable XML too?

If you export one **trend** display and one **alarm** display the same
way you exported the motor screen, send them here and I'll generate the
TRENDS and ALARMS screens in that exact dialect as well.
