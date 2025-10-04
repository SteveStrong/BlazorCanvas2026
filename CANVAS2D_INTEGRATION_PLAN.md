# Canvas2DComponent Integration Plan

## 🎯 Goal
Integrate FoundryBlazor Canvas2DComponent following the same layout patterns as `/canvas` and `/blazorcanvas` pages.

## 📐 Layout Pattern Analysis

### Existing Pattern Structure:
```html
<div class="canvas-layout">
    <div class="canvas-container">
        <!-- Canvas Component -->
    </div>
    <div class="canvas-controls">
        <div class="control-section">
            <h5>Section Title</h5>
            <div class="control-group">
                <!-- Controls -->
            </div>
        </div>
    </div>
</div>
```

## 🔧 Required Dependencies

### 1. Service Registration (Program.cs)
```csharp
// Add back FoundryBlazor services
var envConfig = new EnvConfig("./.env");
builder.Services.AddFoundryBlazorServices(envConfig);
```

### 2. Package References (csproj)
```xml
<PackageReference Include="Radzen.Blazor" Version="7.3.5" />
```

### 3. JavaScript References (_Host.cshtml)
```html
<script src="_content/FoundryBlazor/js/app-lib.js"></script>
```

### 4. Using Statements
```csharp
@using FoundryBlazor.Shared
@using FoundryBlazor.Solutions
```

## 📄 Proposed Canvas2D.razor

```aspnetcorerazor
@page "/canvas2d"
@using FoundryBlazor.Shared
@using FoundryBlazor.Solutions

<PageTitle>Canvas2D</PageTitle>

<h1>FoundryBlazor Canvas2D</h1>

<div class="canvas-layout">
    <div class="canvas-container">
        <div style="border: 1px solid #000; background-color: #f0f0f0; display: inline-block;">
            <Canvas2DComponent 
                SceneName="@SceneName" 
                CanvasWidth="@CanvasWidth" 
                CanvasHeight="@CanvasHeight" />
        </div>
    </div>

    <div class="canvas-controls">
        <div class="control-section">
            <h5>Canvas Settings</h5>
            <div class="control-group">
                <label class="mb-2">
                    <strong>Width:</strong>
                    <input type="range" min="400" max="1600" @bind="CanvasWidth" @bind:event="oninput" class="form-range">
                    <span class="badge bg-secondary">@CanvasWidth</span>
                </label>
                <label class="mb-2">
                    <strong>Height:</strong>
                    <input type="range" min="300" max="1200" @bind="CanvasHeight" @bind:event="oninput" class="form-range">
                    <span class="badge bg-secondary">@CanvasHeight</span>
                </label>
                <label class="mb-2">
                    <strong>Scene Name:</strong>
                    <input type="text" @bind="SceneName" class="form-control" placeholder="Enter scene name">
                </label>
            </div>
        </div>

        <div class="control-section">
            <h5>Workspace Tools</h5>
            <div class="control-group">
                <button class="btn btn-primary mb-2" @onclick="ClearWorkspace">Clear Workspace</button>
                <button class="btn btn-secondary mb-2" @onclick="AddTestShape">Add Test Shape</button>
                <button class="btn btn-info mb-2" @onclick="TestServices">Test Services</button>
            </div>
        </div>

        <div class="control-section">
            <h5>Service Status</h5>
            <div class="control-group">
                @if (!string.IsNullOrEmpty(serviceInfo))
                {
                    <div class="alert alert-info">
                        <small>@serviceInfo</small>
                    </div>
                }
            </div>
        </div>
    </div>
</div>
```

## 📄 Proposed Canvas2D.razor.cs

```csharp
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

        private void ClearWorkspace()
        {
            try
            {
                var drawing = Workspace?.GetDrawing();
                drawing?.ClearAll();
                serviceInfo = "Workspace cleared successfully";
                StateHasChanged();
            }
            catch (Exception ex)
            {
                serviceInfo = $"Clear error: {ex.Message}";
                StateHasChanged();
            }
        }

        private void AddTestShape()
        {
            try
            {
                var drawing = Workspace?.GetDrawing();
                if (drawing != null)
                {
                    // Add a simple test shape
                    serviceInfo = "Test shape added";
                }
                else
                {
                    serviceInfo = "No drawing context available";
                }
                StateHasChanged();
            }
            catch (Exception ex)
            {
                serviceInfo = $"Add shape error: {ex.Message}";
                StateHasChanged();
            }
        }

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
```

## 🚀 Implementation Steps

### Phase 1: Minimal Setup
1. Add FoundryBlazor service registration back to Program.cs
2. Add Radzen.Blazor package reference
3. Add FoundryBlazor JavaScript reference
4. Create basic Canvas2D.razor page

### Phase 2: Layout Integration
1. Follow exact canvas-layout structure from working pages
2. Implement consistent control sections
3. Add responsive canvas sizing controls
4. Test basic rendering

### Phase 3: Functionality
1. Add workspace interaction methods
2. Implement service status indicators
3. Add error handling and user feedback
4. Test with different canvas sizes

### Phase 4: Navigation
1. Add to navigation menu
2. Update routing
3. Test full integration

## 🎨 CSS Classes to Reuse
- `.canvas-layout` - Main layout container
- `.canvas-container` - Canvas wrapper
- `.canvas-controls` - Controls sidebar
- `.control-section` - Individual control groups
- `.control-group` - Control items wrapper

## ✅ Success Criteria
- Canvas2DComponent renders without errors
- Layout matches existing canvas pages
- Controls work and provide feedback
- Services are properly injected
- Navigation integration complete
- Consistent styling and behavior

---
*Ready to implement when FoundryBlazor dependencies are restored*