#nullable enable
using Microsoft.Maui;
using Microsoft.Maui.Controls;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]

namespace Particle.Maui.Sample
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
        }
    }
}