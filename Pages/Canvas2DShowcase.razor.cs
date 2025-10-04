using Microsoft.AspNetCore.Components;

namespace BlazorCanvas2026.Pages
{
    public partial class Canvas2DShowcase : ComponentBase
    {
        // Canvas parameters that users can adjust
        private int CanvasWidth { get; set; } = 800;
        private int CanvasHeight { get; set; } = 600;
        private string SceneName { get; set; } = "Canvas2DDemo";
        
        // TODO: When FoundryBlazor services are properly registered, 
        // we can inject them here to show advanced interactions:
        // [Inject] public IWorkspace? Workspace { get; set; }
        // [Inject] private ComponentBus? PubSub { get; set; }
        // [Inject] protected IFoundryService? FoundryService { get; set; }
    }
}