using Microsoft.AspNetCore.Components;
using FoundryBlazor.Solutions;

namespace BlazorCanvas2026.Pages
{
    public partial class Canvas2D : ComponentBase
    {
        [Inject] public IWorkspace? Workspace { get; set; }
        [Inject] public IFoundryService? FoundryService { get; set; }

        // Canvas properties
        private int CanvasWidth { get; set; } = 800;
        private int CanvasHeight { get; set; } = 600;
        private string SceneName { get; set; } = "Canvas2DDemo";
        
        // Status info
        private string serviceInfo = "";

        private void TestServices()
        {
            try
            {
                var workspace = Workspace?.GetDrawing();
                var foundry = FoundryService?.Drawing();
                
                serviceInfo = $"Services: Workspace={workspace != null}, FoundryService={foundry != null}";
                StateHasChanged();
            }
            catch (Exception ex)
            {
                serviceInfo = $"Service test error: {ex.Message}";
                StateHasChanged();
            }
        }
    }
}