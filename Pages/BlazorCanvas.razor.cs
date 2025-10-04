using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using Blazor.Extensions.Canvas.Canvas2D;
using Blazor.Extensions;

namespace BlazorCanvas2026.Pages
{
    public partial class BlazorCanvas : ComponentBase, IAsyncDisposable
    {
        [Inject]
        public IJSRuntime JSRuntime { get; set; } = default!;

        protected BECanvasComponent? canvasElement;
        private Canvas2DContext? _context;
        private bool isDrawing = false;
        private string strokeColor = "#000000";
        private int lineWidth = 2;
        private bool animationRunning = false;
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

        private async Task InitializeCanvas()
        {
            if (_context == null || _isDisposed) return;

            try
            {
                // Set default canvas background
                await _context.SetFillStyleAsync("#f0f0f0");
                await _context.FillRectAsync(0, 0, 800, 600);
            }
            catch (Exception)
            {
                // Handle errors gracefully
            }
        }

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
            catch (Exception)
            {
                // Handle errors gracefully
            }
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
            catch (Exception)
            {
                // Handle errors gracefully
            }
        }

        private Task StopDrawing(MouseEventArgs e)
        {
            isDrawing = false;
            return Task.CompletedTask;
        }

        private async Task ClearCanvas()
        {
            if (_context == null || _isDisposed) return;
            
            try
            {
                await _context.ClearRectAsync(0, 0, 800, 600);
                await _context.SetFillStyleAsync("#f0f0f0");
                await _context.FillRectAsync(0, 0, 800, 600);
            }
            catch (Exception)
            {
                // Handle errors gracefully
            }
        }

        private async Task DrawRectangle()
        {
            if (_context == null || _isDisposed) return;
            
            try
            {
                await _context.SetStrokeStyleAsync(strokeColor);
                await _context.SetLineWidthAsync(lineWidth);
                await _context.StrokeRectAsync(100, 100, 200, 150);
            }
            catch (Exception)
            {
                // Handle errors gracefully
            }
        }

        private async Task DrawCircle()
        {
            if (_context == null || _isDisposed) return;
            
            try
            {
                await _context.SetStrokeStyleAsync(strokeColor);
                await _context.SetLineWidthAsync(lineWidth);
                await _context.BeginPathAsync();
                await _context.ArcAsync(400, 200, 75, 0, 2 * Math.PI);
                await _context.StrokeAsync();
            }
            catch (Exception)
            {
                // Handle errors gracefully
            }
        }

        private async Task DrawFilledRectangle()
        {
            if (_context == null || _isDisposed) return;
            
            try
            {
                await _context.SetFillStyleAsync(strokeColor);
                await _context.FillRectAsync(150, 150, 200, 150);
            }
            catch (Exception)
            {
                // Handle errors gracefully
            }
        }

        private async Task DrawFilledCircle()
        {
            if (_context == null || _isDisposed) return;
            
            try
            {
                await _context.SetFillStyleAsync(strokeColor);
                await _context.BeginPathAsync();
                await _context.ArcAsync(450, 250, 75, 0, 2 * Math.PI);
                await _context.FillAsync();
            }
            catch (Exception)
            {
                // Handle errors gracefully
            }
        }

        private async Task StartAnimation()
        {
            if (!animationRunning && _context != null && !_isDisposed)
            {
                animationRunning = true;
                await RunAnimation();
                StateHasChanged();
            }
        }

        private Task StopAnimation()
        {
            if (animationRunning)
            {
                animationRunning = false;
                StateHasChanged();
            }
            return Task.CompletedTask;
        }

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

                    // Draw bouncing ball
                    await _context.SetFillStyleAsync("#ff0000");
                    await _context.BeginPathAsync();
                    await _context.ArcAsync(x, y, radius, 0, 2 * Math.PI);
                    await _context.FillAsync();

                    // Update position
                    x += dx;
                    y += dy;

                    // Bounce off walls
                    if (x + radius > 800 || x - radius < 0) dx = -dx;
                    if (y + radius > 600 || y - radius < 0) dy = -dy;

                    // Small delay
                    await Task.Delay(16); // ~60 FPS
                }
                catch (Exception)
                {
                    // Handle animation errors gracefully
                    break;
                }
            }
        }

        public ValueTask DisposeAsync()
        {
            _isDisposed = true;
            animationRunning = false;
            
            // Canvas2DContext disposal is handled by the BECanvasComponent
            _context = null;
            
            return ValueTask.CompletedTask;
        }
    }
}