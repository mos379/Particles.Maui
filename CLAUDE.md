# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

`Particle.Maui` is a cross-platform .NET MAUI library for displaying particle effects (confetti, etc.). Originally created as [Particle.Forms](https://github.com/mariusmuntean/Particle.Forms) (Xamarin) by Marius Muntean, ported to MAUI by Jeff Bowman ([Particle.Maui](https://github.com/jbowmanp1107/Particle.Maui)), and now maintained at [mos379/Particles.Maui](https://github.com/mos379/Particles.Maui) for .NET 9/.NET 10. Distributed as the NuGet package `particles.maui`.

## Build & Run

There are no build scripts — use standard .NET CLI or open the solution in Visual Studio.

```bash
# Build the library (also generates the NuGet package automatically)
dotnet build Src/Particle.Maui/Particle.Maui.csproj

# Build in Release (generates .nupkg in bin/Release/)
dotnet build Src/Particle.Maui/Particle.Maui.csproj -c Release

# Run the sample app (requires a target platform)
# Open Sample/Particle.Maui.Sample/Particle.Maui.Sample.sln in Visual Studio
# and select a target device/platform
```

There are no test projects. The sample app in `Sample/` serves as the integration test.

## Architecture

### Core Rendering Pipeline

`ParticleView` (`Src/Particle.Maui/ParticleView.cs`) is the central class — a MAUI `ContentView` that hosts an `SKCanvasView` (SkiaSharp) and drives a 16ms animation loop (~60 FPS). It:
- Manages the particle collection (thread-safe with `_particleLock`)
- Calls `IParticleGenerator` implementations to spawn particles
- Delegates to `RateBasedParticleRequester` for smooth rate-controlled spawning
- Handles touch input (tap and drag) to trigger particle bursts

### Extension Points

The library is designed to be extended at two seams:

1. **Custom Particles** — Extend `ParticleBase` and implement `Draw(SKCanvas canvas)`. The base class handles position, velocity, rotation, opacity, and size lifecycle; subclasses only need to render their shape. See `RectParticle`, `EllipseParticle`, and the `Demo2/` logo particles for examples.

2. **Custom Generators** — Implement `IParticleGenerator` to control what particles are created and with what properties. Assign to `ParticleView.TouchParticleGenerator` or `ParticleView.FallingParticleGenerator`. Built-in generators: `SimpleParticleGenerator` (360° radiate), `FallingParticleGenerator` (downward 45–135°), `RandomParticleGenerator` (wraps another generator with random color/type selection).

### Key Bindable Properties

Defined in `ParticleViewProperties.cs` — all are bindable and XAML-compatible:

| Property | Default | Purpose |
|---|---|---|
| `IsActive` | `true` | Shows/hides the particle view |
| `IsRunning` | `true` | Pauses/resumes the animation loop |
| `HasFallingParticles` | `false` | Enables continuous falling particles |
| `FallingParticlesPerSecond` | `60` | Rate of falling particles |
| `AddParticlesOnTap` | `false` | Burst on tap |
| `TapParticleCount` | `30` | Particles per tap |
| `AddParticlesOnDrag` | `false` | Emit while dragging |
| `DragParticleCount` | `60` | Particles per drag event |
| `DragParticleMoveType` | `Fall` | `Fall` or `Radiate` |
| `UseSKGLView` | `false` (true on Android) | Use GPU-accelerated GL view |
| `ShowDebugInfo` | `false` | Overlay FPS/particle count |

Non-bindable: `TouchParticleGenerator`, `FallingParticleGenerator`, `CanvasSize`.

### Platform Targets

Multi-targets .NET 9 and .NET 10:
- `net9.0-android`, `net10.0-android`
- `net9.0-ios`, `net10.0-ios` (14.2+)
- `net9.0-maccatalyst`, `net10.0-maccatalyst` (14.0+)
- `net9.0-windows10.0.19041.0`, `net10.0-windows10.0.19041.0` (Windows 10+)

`Microsoft.Maui.Controls` is conditionally referenced: v9.0.100 for net9.0, v10.0.20 for net10.0.

### Dependencies

- `Microsoft.Maui.Controls` v10.0.20
- `SkiaSharp.Views.Maui.Controls` v3.119.2
- `SkiaSharp.Views.Maui.Core` v3.119.2

### NuGet Package

Building the library project automatically generates the `.nupkg` (`GeneratePackageOnBuild=true`). Current version: 2.0.0. Package ID: `Particles.Maui`.

## Performance Notes

- Call `IsRunning = false` when a page is not visible, and `true` when it reappears — this stops the animation loop.
- Rotation matrices are cached in `ParticleBase` to reduce per-frame allocation.
- `SKGLView` (`UseSKGLView=true`) provides better GPU performance, especially on Android.
