# Blazor.Extensions.Canvas (BECanvas) Implementation & Troubleshooting Guide

## Table of Contents
1. [Quick Setup Checklist](#quick-setup-checklist)
2. [Common Issues & Solutions](#common-issues--solutions)
3. [Implementation Patterns](#implementation-patterns)
4. [Debugging Techniques](#debugging-techniques)
5. [Advanced Scenarios](#advanced-scenarios)
6. [Performance Considerations](#performance-considerations)

---

## Quick Setup Checklist

### ✅ 1. Package Installation
```xml
<!-- In .csproj file -->
<PackageReference Include="Blazor.Extensions.Canvas" Version="1.1.1" />
```

### ✅ 2. JavaScript Reference (CRITICAL!)
```html
<!-- In _Host.cshtml or App.razor - MUST be included! -->
<script src="_content/Blazor.Extensions.Canvas/blazor.extensions.canvas.js"></script>
```

**⚠️ WARNING:** Missing this script reference is the #1 cause of "nothing renders" issues!

### ✅ 3. Using Directives
```csharp
using Blazor.Extensions;
using Blazor.Extensions.Canvas;
using Blazor.Extensions.Canvas.Canvas2D;
```

### ✅ 4. Component Structure
```html
<!-- Wrapper div for styling and events -->
<div style="border: 1px solid #000; background-color: #f0f0f0; display: inline-block;"
     @onmousedown="StartDrawing"
     @onmousemove="Draw"
     @onmouseup="StopDrawing">
    <!-- BECanvas component with ONLY Width, Height, and @ref -->
    <BECanvas @ref="canvasElement" Width="800" Height="600" />
</div>
```

### ✅ 5. Component Code-Behind Pattern
```csharp
public partial class MyCanvasComponent : ComponentBase, IAsyncDisposable
{
    protected BECanvasComponent? canvasElement;
    private Canvas2DContext? _context;
    private bool _isDisposed = false;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender && !_isDisposed)
        {
            try
            {
                _context = await canvasElement!.CreateCanvas2DAsync();
                await InitializeCanvas();
            }
            catch (Exception)
            {
                // Handle initialization errors gracefully
            }
        }
    }

    public ValueTask DisposeAsync()
    {
        _isDisposed = true;
        _context = null; // Context disposal handled by BECanvasComponent
        return ValueTask.CompletedTask;
    }
}
```

---

## Common Issues & Solutions

### 🚨 Issue #1: "Nothing Renders When Buttons Are Clicked"

**Symptoms:**
- Canvas appears but drawing methods don't work
- Console error: `Could not find 'BlazorExtensions.Canvas2d.add' ('BlazorExtensions' was undefined)`

**Root Cause:** Missing JavaScript file reference

**Solution:**
```html
<!-- Add to _Host.cshtml BEFORE closing </body> tag -->
<script src="_content/Blazor.Extensions.Canvas/blazor.extensions.canvas.js"></script>
```

**Verification:**
```csharp
// Add temporary debug logging
protected override async Task OnAfterRenderAsync(bool firstRender)
{
    if (firstRender && !_isDisposed)
    {
        try
        {
            Console.WriteLine("Attempting to create Canvas2D context...");
            _context = await canvasElement!.CreateCanvas2DAsync();
            Console.WriteLine($"Canvas2D context created: {_context != null}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
}
```

### 🚨 Issue #2: "BECanvas Component Not Found"

**Symptoms:**
- Build errors about BECanvas not being recognized
- Red squiggly lines under `<BECanvas>`

**Solution:**
```csharp
// Add these using directives at top of .razor file
@using Blazor.Extensions
@using Blazor.Extensions.Canvas
@using Blazor.Extensions.Canvas.Canvas2D
```

### 🚨 Issue #3: "Canvas Context is Null"

**Symptoms:**
- Canvas renders but methods fail silently
- Context initialization fails

**Debugging Steps:**
1. Check if `OnAfterRenderAsync` is being called
2. Verify JavaScript file is loading (check browser Network tab)
3. Ensure component reference is not null

**Solution Pattern:**
```csharp
private async Task DrawSomething()
{
    if (_context == null || _isDisposed) 
    {
        Console.WriteLine("Context not available");
        return;
    }
    
    try
    {
        // Your drawing code here
        await _context.SetFillStyleAsync("#ff0000");
        await _context.FillRectAsync(10, 10, 50, 50);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Drawing error: {ex.Message}");
    }
}
```

### 🚨 Issue #4: "Mouse Events Not Working"

**Symptoms:**
- Drawing methods work via buttons but not mouse interaction

**Root Cause:** Mouse events must be on wrapper div, not BECanvas component

**Correct Pattern:**
```html
<!-- ✅ CORRECT: Events on wrapper div -->
<div @onmousedown="StartDrawing" @onmousemove="Draw" @onmouseup="StopDrawing">
    <BECanvas @ref="canvasElement" Width="800" Height="600" />
</div>

<!-- ❌ WRONG: Events directly on BECanvas -->
<BECanvas @ref="canvasElement" Width="800" Height="600" 
          @onmousedown="StartDrawing" />
```

---

## Implementation Patterns

### 🎨 Basic Drawing Pattern
```csharp
private async Task DrawRectangle()
{
    if (_context == null || _isDisposed) return;
    
    try
    {
        await _context.SetStrokeStyleAsync("#000000");
        await _context.SetLineWidthAsync(2);
        await _context.StrokeRectAsync(50, 50, 100, 75);
    }
    catch (Exception)
    {
        // Handle gracefully
    }
}
```

### 🎨 Filled Shape Pattern
```csharp
private async Task DrawFilledCircle()
{
    if (_context == null || _isDisposed) return;
    
    try
    {
        await _context.SetFillStyleAsync("#ff0000");
        await _context.BeginPathAsync();
        await _context.ArcAsync(100, 100, 50, 0, 2 * Math.PI);
        await _context.FillAsync();
    }
    catch (Exception)
    {
        // Handle gracefully
    }
}
```

### 🎨 Animation Pattern
```csharp
private async Task RunAnimation()
{
    double x = 50, y = 50;
    double dx = 2, dy = 2;
    const double radius = 25;

    while (animationRunning && _context != null && !_isDisposed)
    {
        try
        {
            // Clear canvas
            await _context.ClearRectAsync(0, 0, 800, 600);
            await _context.SetFillStyleAsync("#f0f0f0");
            await _context.FillRectAsync(0, 0, 800, 600);

            // Draw animated object
            await _context.SetFillStyleAsync("#ff0000");
            await _context.BeginPathAsync();
            await _context.ArcAsync(x, y, radius, 0, 2 * Math.PI);
            await _context.FillAsync();

            // Update position with bounds checking
            x += dx;
            y += dy;
            if (x + radius > 800 || x - radius < 0) dx = -dx;
            if (y + radius > 600 || y - radius < 0) dy = -dy;

            await Task.Delay(16); // ~60 FPS
        }
        catch (Exception)
        {
            break; // Stop on error
        }
    }
}
```

### 🎨 Free-Hand Drawing Pattern
```csharp
private bool isDrawing = false;

private async Task StartDrawing(MouseEventArgs e)
{
    if (_context == null || _isDisposed) return;
    
    isDrawing = true;
    var x = e.OffsetX;
    var y = e.OffsetY;
    
    try
    {
        await _context.SetStrokeStyleAsync(strokeColor);
        await _context.SetLineWidthAsync(lineWidth);
        await _context.BeginPathAsync();
        await _context.MoveToAsync(x, y);
    }
    catch (Exception) { }
}

private async Task Draw(MouseEventArgs e)
{
    if (!isDrawing || _context == null || _isDisposed) return;

    var x = e.OffsetX;
    var y = e.OffsetY;
    
    try
    {
        await _context.LineToAsync(x, y);
        await _context.StrokeAsync();
    }
    catch (Exception) { }
}

private Task StopDrawing(MouseEventArgs e)
{
    isDrawing = false;
    return Task.CompletedTask;
}
```

---

## Debugging Techniques

### 🔍 1. JavaScript Console Verification
```javascript
// Check in browser console if BlazorExtensions is loaded
console.log(typeof BlazorExtensions); // Should not be "undefined"
```

### 🔍 2. Network Tab Verification
- Open browser Developer Tools
- Go to Network tab
- Refresh page
- Look for `blazor.extensions.canvas.js` - should return 200 OK

### 🔍 3. Step-by-Step Context Creation Test
```csharp
private async Task TestCanvasStep()
{
    Console.WriteLine("Step 1: Checking canvas element...");
    if (canvasElement == null)
    {
        Console.WriteLine("❌ Canvas element is null");
        return;
    }
    Console.WriteLine("✅ Canvas element exists");

    Console.WriteLine("Step 2: Creating context...");
    try
    {
        _context = await canvasElement.CreateCanvas2DAsync();
        Console.WriteLine($"✅ Context created: {_context != null}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Context creation failed: {ex.Message}");
        return;
    }

    Console.WriteLine("Step 3: Testing simple draw...");
    try
    {
        await _context.SetFillStyleAsync("#ff0000");
        await _context.FillRectAsync(10, 10, 50, 50);
        Console.WriteLine("✅ Simple draw successful");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Draw failed: {ex.Message}");
    }
}
```

### 🔍 4. Build Verification Checklist
```bash
# 1. Clean build
dotnet clean
dotnet build

# 2. Check for package restoration
dotnet restore

# 3. Verify package is installed
dotnet list package | grep -i canvas
```

---

## Advanced Scenarios

### 🚀 1. Multiple Canvas Components
```csharp
// Each canvas needs its own context
protected BECanvasComponent? canvas1;
protected BECanvasComponent? canvas2;
private Canvas2DContext? context1;
private Canvas2DContext? context2;

protected override async Task OnAfterRenderAsync(bool firstRender)
{
    if (firstRender)
    {
        context1 = await canvas1!.CreateCanvas2DAsync();
        context2 = await canvas2!.CreateCanvas2DAsync();
    }
}
```

### 🚀 2. Dynamic Canvas Sizing
```csharp
private int canvasWidth = 800;
private int canvasHeight = 600;

private async Task ResizeCanvas(int newWidth, int newHeight)
{
    canvasWidth = newWidth;
    canvasHeight = newHeight;
    StateHasChanged();
    
    // Reinitialize after resize
    await Task.Delay(100); // Allow DOM to update
    if (_context != null)
    {
        await InitializeCanvas();
    }
}
```

### 🚀 3. Canvas Data Export
```csharp
// Note: Data export requires additional JavaScript interop
[Inject] public IJSRuntime JSRuntime { get; set; }

private async Task ExportCanvasAsImage()
{
    try
    {
        var dataUrl = await JSRuntime.InvokeAsync<string>(
            "eval", 
            $"document.querySelector('canvas').toDataURL('image/png')"
        );
        // Process dataUrl as needed
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Export failed: {ex.Message}");
    }
}
```

---

## Performance Considerations

### ⚡ 1. Batch Drawing Operations
```csharp
// ✅ GOOD: Batch operations
private async Task DrawMultipleShapes()
{
    if (_context == null) return;
    
    await _context.BeginPathAsync();
    // Draw multiple shapes before calling stroke/fill
    for (int i = 0; i < 10; i++)
    {
        await _context.RectAsync(i * 50, 50, 40, 40);
    }
    await _context.StrokeAsync(); // Single stroke call
}

// ❌ BAD: Individual operations
private async Task DrawMultipleShapesSlow()
{
    for (int i = 0; i < 10; i++)
    {
        await _context.StrokeRectAsync(i * 50, 50, 40, 40); // Individual calls
    }
}
```

### ⚡ 2. Animation Frame Management
```csharp
private CancellationTokenSource? animationCancellation;

private async Task StartOptimizedAnimation()
{
    animationCancellation = new CancellationTokenSource();
    
    try
    {
        while (!animationCancellation.Token.IsCancellationRequested)
        {
            await RenderFrame();
            await Task.Delay(16, animationCancellation.Token); // ~60 FPS
        }
    }
    catch (OperationCanceledException)
    {
        // Expected when animation is stopped
    }
}

private void StopAnimation()
{
    animationCancellation?.Cancel();
}
```

### ⚡ 3. Memory Management
```csharp
public async ValueTask DisposeAsync()
{
    _isDisposed = true;
    animationCancellation?.Cancel();
    animationCancellation?.Dispose();
    
    // Context disposal is handled by BECanvasComponent
    _context = null;
}
```

---

## Quick Reference: Component Properties

### BECanvas Component
```html
<!-- ONLY these properties are supported -->
<BECanvas @ref="canvasRef" 
          Width="800" 
          Height="600" />

<!-- ❌ These will NOT work -->
<BECanvas style="border: 1px solid red"    <!-- Use wrapper div -->
          id="myCanvas"                      <!-- Use wrapper div --> 
          @onclick="HandleClick" />          <!-- Use wrapper div -->
```

### Canvas2DContext Key Methods
```csharp
// Drawing
await context.StrokeRectAsync(x, y, width, height);
await context.FillRectAsync(x, y, width, height);
await context.ArcAsync(x, y, radius, startAngle, endAngle);
await context.BeginPathAsync();
await context.MoveToAsync(x, y);
await context.LineToAsync(x, y);
await context.StrokeAsync();
await context.FillAsync();

// Styling
await context.SetStrokeStyleAsync("#000000");
await context.SetFillStyleAsync("#ff0000");
await context.SetLineWidthAsync(2);

// Canvas Management
await context.ClearRectAsync(x, y, width, height);
await context.SaveAsync();
await context.RestoreAsync();
```

---

## Troubleshooting Flowchart

```
Canvas Not Working?
├── Can you see the canvas element? 
│   ├── No → Check component markup and build errors
│   └── Yes ↓
├── Are there console errors about 'BlazorExtensions'?
│   ├── Yes → Add JavaScript reference to _Host.cshtml
│   └── No ↓
├── Does context creation succeed?
│   ├── No → Check OnAfterRenderAsync implementation
│   └── Yes ↓
├── Do button clicks work but mouse events don't?
│   ├── Yes → Move mouse events to wrapper div
│   └── No ↓
└── Add debug logging and check browser network tab
```

---

## Version Compatibility Notes

- **Blazor.Extensions.Canvas 1.1.1**: Tested with .NET 9.0
- **JavaScript file path**: `_content/Blazor.Extensions.Canvas/blazor.extensions.canvas.js`
- **Server-side Blazor**: Fully supported
- **WebAssembly**: Should work but requires additional testing

---

*This guide was created based on successful implementation and debugging of BECanvas in a .NET 9 Blazor Server application. Keep this document updated as you encounter new scenarios and solutions.*