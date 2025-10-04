using Microsoft.AspNetCore.Components;
using FoundryBlazor.Solutions;

namespace BlazorCanvas2026.Pages
{
    public partial class SimpleCanvas2D : ComponentBase
    {
        [Inject] 
        public IWorkspace? Workspace { get; set; }
        
        [Inject]
        public IFoundryService? FoundryService { get; set; }
        
        private string lastAction = "";
        private string serviceInfo = "";

        private void DoSomething()
        {
            lastAction = $"Button pushed at {DateTime.Now:HH:mm:ss}";
            StateHasChanged();
            
            // Test if we can interact with the workspace when Canvas2DComponent is working
        }

        private void TestServices()
        {
            try
            {
                var workspace = Workspace?.GetDrawing();
                var foundry = FoundryService?.Drawing();
                
                serviceInfo = $"Services available: Workspace={workspace != null}, FoundryService={foundry != null}";
            }
            catch (Exception ex)
            {
                serviceInfo = $"Service error: {ex.Message}";
            }
            
            StateHasChanged();
        }
    }
}