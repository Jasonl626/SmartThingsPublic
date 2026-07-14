# HA0004 Koch Age Oven — Runtime Build Runbook (Improved Data Logging)

How to produce the new `.mer` runtime from this restored project in
FactoryTalk View Studio ME, with the improved data logging applied.
Target result: `HA0004_Koch_Age_Oven_v800_<yyyymmdd>.mer`.

## Prerequisites

- FactoryTalk View Studio **Machine Edition 8.0 or later** (the runtime
  must be created at a version the PanelView Plus firmware supports;
  the current app is v8.00).
- This project folder (`HA0004_Koch_Age_Oven/`) copied onto the Studio
  machine, or opened via `HA0004_Koch_Age_Oven.med`.
- **Before anything else:** confirm the new PLC tags exist in the
  ControlLogix program (Studio 5000). The original 8 log tags are
  verbatim from the old app; the additions below were inferred from
  the ME parameter files and MUST be verified/corrected:
  `*.HI_LIMIT.PV/.Tripped`, `ZONE*.{Comb_Air,ReCirc}_Fan.Running`,
  `EXHAUST.Fan.Running`, `SKID_PART_COUNT`, `OVEN_STATE`,
  `SYSTEM_FAULT_ACTIVE`.

## One ME constraint to decide up front

FactoryTalk View ME supports **one data log model per runtime**, with a
**single sample rate** for all tags. The two-logger design (10 s process
+ 1 s flame) is an Optix feature, not an ME one. Recommended compromise:

> **One model, periodic, 2 s.** 20 tags at 2 s ≈ 864k samples/day —
> well within ME limits, fine on an SD card, and fast enough to catch
> flame-signal instability.

If storage is tight, fall back to 5 s and accept coarser flame detail.

## Steps in View Studio ME

1. **Open the project** — File → Open, select `HA0004_Koch_Age_Oven`.
2. **Edit the data log model** — Explorer → Data Log → Data Log Models →
   `Age Oven flame intensity` (keep the name; the Startup settings and
   trend displays already reference it).
   - **Setup tab:** storage location → External Storage Card
     (files survive power cycles and can be pulled for analysis);
     set max log files ≥ 30 so history isn't overwritten weekly.
   - **Log Triggers tab:** Periodic, **2 seconds**.
   - **Tags in Model tab:** keep the existing 8 and add:
     ```
     {[Aging_Furnace]HMI_COMM_CONFIG_MB_Z1_HI_LIMIT.PV}
     {[Aging_Furnace]HMI_COMM_CONFIG_MB_Z1_HI_LIMIT.Tripped}
     {[Aging_Furnace]HMI_COMM_CONFIG_MB_Z2_HI_LIMIT.PV}
     {[Aging_Furnace]HMI_COMM_CONFIG_MB_Z2_HI_LIMIT.Tripped}
     {[Aging_Furnace]ZONE1.Comb_Air_Fan.Running}
     {[Aging_Furnace]ZONE1.ReCirc_Fan.Running}
     {[Aging_Furnace]ZONE2.Comb_Air_Fan.Running}
     {[Aging_Furnace]ZONE2.ReCirc_Fan.Running}
     {[Aging_Furnace]EXHAUST.Fan.Running}
     {[Aging_Furnace]SKID_PART_COUNT}
     {[Aging_Furnace]OVEN_STATE}
     {[Aging_Furnace]SYSTEM_FAULT_ACTIVE}
     ```
     (Correct any path the tag browser rejects — the browser reading
     the live shortcut is the source of truth.)
3. **Confirm startup logging** — Explorer → Startup: "Data logging"
   checked with model `Age Oven flame intensity` (already true today).
4. **Optional but recommended** — add the new pens to the existing
   trend displays (`Age Oven flame intensity_04H/08H/12H/24H`) or add a
   second trend page for hi-limit PVs; use the interactive demo in
   `../HA0004_Koch_Age_Oven_ME_Demo/` as the layout reference.
5. **Test** — Application → Test Application. Watch the diagnostics
   list for tag-read errors on the new tags; fix any path typos.
6. **Create the runtime** — Application → Create Runtime:
   - File name: `HA0004_Koch_Age_Oven_v800_<yyyymmdd>.mer`
   - Runtime version: **8.00** (match panel firmware)
   - Conversion: **"Always allow conversion"** — this is the only
     reason we were able to restore the current app; keep it enabled.
7. **Deploy** — Tools → Transfer Utility → download to the PanelView
   Plus over Ethernet (or copy via SD), set as the startup application,
   and restart the terminal.
8. **Verify on the panel** — after 10 minutes of running, check the
   storage card for a growing log set, then pull a copy and confirm all
   20 columns are populated.

## Getting data out (the "improved" part in practice)

ME log sets are proprietary `.dat` files. To hand them to QA:
copy the log-file folder off the storage card and convert with
Rockwell's FactoryTalk View File Viewer utility (exports CSV). The CSV
column layout will match the demo's "Export ProcessLog CSV" output.

## After the build

Commit the new `.mer` (and any changed project files) back to this
repo on the working branch so the runtime and its source stay paired.
