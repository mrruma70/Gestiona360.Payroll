using MudBlazor;

namespace Gestiona360.Payroll.Frontend.Theme
{
    public static class PayrollTheme
    {
        public static MudTheme Default => new MudTheme
        {
            PaletteDark = new PaletteDark
            {
                Primary = "#FFD700",        // Amarillo oro
                Secondary = "#FFC107",
                Background = "#1a1a1a",
                Surface = "#2d2d2d",
                TextPrimary = "#e0e0e0",
                TextSecondary = "#aaaaaa",
                AppbarBackground = "#1a1a1a",
                DrawerBackground = "#1a1a1a",
                DrawerText = "#e0e0e0",
                DrawerIcon = "#FFD700"
            },
            PaletteLight = new PaletteLight
            {
                Primary = "#FFD700",
                Secondary = "#FFC107",
                AppbarBackground = "#1a1a1a",
                AppbarText = "#FFD700",
                DrawerBackground = "#1a1a1a",
                DrawerText = "#e0e0e0"
            }
            // La tipografía se omite para evitar errores de versión
        };
    }
}