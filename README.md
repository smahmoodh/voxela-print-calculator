# Voxela 3D Print Cost Calculator

A lightweight Persian (RTL) Windows desktop application for calculating FDM 3D printing costs.

## Features
- Manage filament/material profiles (add, edit, delete)
- Configure hourly print rate and profit percentage
- Calculate material, machine time, subtotal, profit, and final price using `decimal`
- Persist settings locally as JSON under `%LOCALAPPDATA%\Voxela3D\PrintCalculator\settings.json`
- Build a self-contained Windows x64 single-file executable with GitHub Actions

## Build
Every push to `master` runs the Windows build workflow. Download the published ZIP from the successful workflow run's **Artifacts** section.

The app targets .NET 10 WPF. No local SDK or IDE is needed to obtain the build artifact.
