# Particle.Maui
A .NET MAUI library to display particle effects (confetti, radial bursts, etc.). Works on Android, iOS, macOS, and Windows.

Originally created as [Particle.Forms](https://github.com/mariusmuntean/Particle.Forms) for Xamarin.Forms by Marius Muntean, then ported to .NET MAUI as [Particle.Maui](https://github.com/jbowmanp1107/Particle.Maui) by Jeff Bowman. This fork maintains the library for .NET 9/.NET 10 and SkiaSharp 3.x.

Get it from NuGet [![Nuget](https://img.shields.io/nuget/vpre/particle.maui)](https://www.nuget.org/packages/particle.maui/)

## Preview
<img src="Media/sample.gif" width="640px" />

## API Reference

The starting point is the class `ParticleView`.


| BindableProperty | Default | Description |
|------------------|---------|-------------|
| `IsActive` | `true` | Whether or not the control is displaying particles. Use this property to stop and restart the particles. |
| `IsRunning` | `true` | Whether or not the control is animating particles. Use this property to pause and resume the particles. |
| `HasFallingParticles` | `false` | Whether or not falling particles should be shown. |
| `FallingParticlesPerSecond` | `60` | Amount of new particles to be added every second when `HasFallingParticles` is true. |
| `AddParticlesOnTap` | `false` | Whether or not to add particles on tap. |
| `TapParticleCount` | `30` | Amount of particles to add on tap when `AddParticlesOnTap` is true. |
| `AddParticlesOnDrag` | `false` | Whether or not to add particles on drag. |
| `DragParticleCount` | `60` | Amount of particles to add on drag when `AddParticlesOnDrag` is true. |
| `DragParticleMoveType` | `ParticleMovetype.Fall` | Particle movement type while dragging. |
| `UseSKGLView` | `False` on all platforms except Android | Whether or not to use the hardware-accelerated view for drawing. |
| `ShowDebugInfo` | `False` | Whether or not to show debug information. |
| `DebugInfoColor` | `LawnGreen` | Color to use when displaying debug information. |
<br>

These properties aren't bindable.

| Property | Default | Description |
|------------------|-|---------|
| `TouchParticleGenerator` | `SimpleParticleGenerator` | A `ParticleBase` generator to be used when interacting with the `ParticleView` |
| `FallingParticleGenerator` | `FallingParticleGenerator` | A `ParticleBase` generator to be used when showing particles that fall from the top edge to the bottom |
| `CanvasSize` | <none>| Contains the current canvas size |
<br>

## Usage Sample

```xml
<?xml version="1.0" encoding="utf-8"?>

<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:particle="clr-namespace:Particle.Maui;assembly=Particle.Maui"
             x:Class="Particle.Maui.Sample.Demo2.Demo2"
             Title="Custom Particles">
    <ContentPage.Resources>
        <x:Array Type="{x:Type Color}" x:Key="ConfettiColors">
            <Color>#a864fd</Color>
            <Color>#29cdff</Color>
            <Color>#78ff44</Color>
            <Color>#ff718d</Color>
            <Color>#fdff6a</Color>
            <Color>#ffcbf2</Color>
        </x:Array>
    </ContentPage.Resources>

    <Grid VerticalOptions="FillAndExpand"
          BackgroundColor="White"
          Margin="0 ,0, 0, 5">


        <particle:ParticleView x:Name="MyParticleCanvas"
                            IsActive="True"
                            IsRunning="True"
                            HasFallingParticles="True"
                            FallingParticlesPerSecond="20"
                            Margin="0, 20"
                            VerticalOptions="FillAndExpand"
                            HorizontalOptions="FillAndExpand"
                            ParticleColors="{StaticResource ConfettiColors}" />
    </Grid>
</ContentPage>
```

### Tip
To conserve resources you can pause the particles when the `Page` is about to disappear and resume when the `Page` is about to appear.
```csharp
        protected override void OnAppearing()
        {
            base.OnAppearing();

            MyParticleCanvas.IsRunning = true;
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            MyParticleCanvas.IsRunning = false;
        }
```

## Changelog

### 2.0.0
- Multi-targets .NET 9 and .NET 10 with SkiaSharp 3.x
- Replaced deprecated `Device.BeginInvokeOnMainThread` with `MainThread.BeginInvokeOnMainThread`
- Replaced deprecated `Device.RuntimePlatform` with `DeviceInfo.Platform`
- Migrated SKMatrix44 APIs to SkiaSharp 3.x static factory methods
- Migrated debug text rendering to SKFont + SKPaint (SkiaSharp 3.x text API)
- Removed unused UpgradeAssistant analyzer package

## Support

If you find this project useful, consider supporting development:

[![Buy Me A Coffee](https://img.shields.io/badge/Buy%20Me%20A%20Coffee-mos379-yellow?logo=buymeacoffee)](https://buymeacoffee.com/mos379)

## License

MIT License - see [LICENSE](LICENSE) for details.

Copyright (c) 2020 Marius Muntean, 2022 Jeff Bowman, 2026 ITs4BM
