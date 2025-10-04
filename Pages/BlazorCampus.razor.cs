using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using Blazor.Extensions.Canvas.Canvas2D;
using Blazor.Extensions;
using System.Drawing;

namespace BlazorCanvas2026.Pages
{
    public partial class BlazorCampus : ComponentBase, IAsyncDisposable
    {
        [Inject]
        public IJSRuntime JSRuntime { get; set; } = default!;

        protected BECanvasComponent? BECanvasReference;
        private Canvas2DContext? Ctx;
        
        private bool _isDrawing = false;
        private string _selectedColor = "#2196F3";
        private int _brushSize = 5;
        private string _selectedTool = "brush";
        private List<DrawnPath> _drawnPaths = new();
        private List<DrawnShape> _shapes = new();
        private bool _showGrid = true;
        private float _canvasWidth = 800;
        private float _canvasHeight = 600;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                // Following the working pattern from FoundryBlazor
                Ctx = await BECanvasReference!.CreateCanvas2DAsync();
                await InitializeCanvas();
            }
        }

        private async Task InitializeCanvas()
        {
            if (Ctx == null) return;

            // Set background
            await Ctx.SetFillStyleAsync("#f8f9fa");
            await Ctx.FillRectAsync(0, 0, _canvasWidth, _canvasHeight);
            
            if (_showGrid)
            {
                await DrawGrid();
            }
        }

        private async Task DrawGrid()
        {
            if (Ctx == null) return;

            await Ctx.SetStrokeStyleAsync("#e9ecef");
            await Ctx.SetLineWidthAsync(0.5f);

            // Draw vertical lines
            for (int x = 0; x <= _canvasWidth; x += 20)
            {
                await Ctx.BeginPathAsync();
                await Ctx.MoveToAsync(x, 0);
                await Ctx.LineToAsync(x, _canvasHeight);
                await Ctx.StrokeAsync();
            }

            // Draw horizontal lines
            for (int y = 0; y <= _canvasHeight; y += 20)
            {
                await Ctx.BeginPathAsync();
                await Ctx.MoveToAsync(0, y);
                await Ctx.LineToAsync(_canvasWidth, y);
                await Ctx.StrokeAsync();
            }
        }

        // Mouse event handlers for Canvas2DContext
        private async Task OnCanvasMouseDown(MouseEventArgs e)
        {
            if (Ctx == null) return;
            
            // Use offset coordinates directly from the mouse event
            var x = e.OffsetX;
            var y = e.OffsetY;

            _isDrawing = true;

            switch (_selectedTool)
            {
                case "brush":
                    _drawnPaths.Add(new DrawnPath
                    {
                        Points = new List<PointF> { new PointF((float)x, (float)y) },
                        Color = _selectedColor,
                        Width = _brushSize
                    });
                    break;
                case "rectangle":
                    await DrawRectangle((float)x, (float)y, 100, 60);
                    break;
                case "circle":
                    await DrawCircle((float)x, (float)y, 50);
                    break;
                case "star":
                    await DrawStar((float)x, (float)y, 40);
                    break;
            }
        }

        private async Task OnCanvasMouseMove(MouseEventArgs e)
        {
            if (!_isDrawing || Ctx == null || _selectedTool != "brush") return;

            // Use offset coordinates directly from the mouse event
            var x = e.OffsetX;
            var y = e.OffsetY;

            var currentPath = _drawnPaths.LastOrDefault();
            if (currentPath != null)
            {
                currentPath.Points.Add(new PointF((float)x, (float)y));
                await RedrawCanvas();
            }
        }

        private Task OnCanvasMouseUp(MouseEventArgs e)
        {
            _isDrawing = false;
            return Task.CompletedTask;
        }

        private async Task DrawRectangle(float x, float y, float width, float height)
        {
            if (Ctx == null) return;

            _shapes.Add(new DrawnShape
            {
                Type = ShapeType.Rectangle,
                X = x,
                Y = y,
                Width = width,
                Height = height,
                Color = _selectedColor
            });

            await Ctx.SetFillStyleAsync(_selectedColor);
            await Ctx.FillRectAsync(x, y, width, height);
            await Ctx.SetStrokeStyleAsync("#000000");
            await Ctx.SetLineWidthAsync(2);
            await Ctx.StrokeRectAsync(x, y, width, height);
        }

        private async Task DrawCircle(float x, float y, float radius)
        {
            if (Ctx == null) return;

            _shapes.Add(new DrawnShape
            {
                Type = ShapeType.Circle,
                X = x,
                Y = y,
                Radius = radius,
                Color = _selectedColor
            });

            await Ctx.SetFillStyleAsync(_selectedColor);
            await Ctx.BeginPathAsync();
            await Ctx.ArcAsync(x, y, radius, 0, 2 * Math.PI);
            await Ctx.FillAsync();
            await Ctx.SetStrokeStyleAsync("#000000");
            await Ctx.SetLineWidthAsync(2);
            await Ctx.StrokeAsync();
        }

        private async Task DrawStar(float centerX, float centerY, float radius)
        {
            if (Ctx == null) return;

            _shapes.Add(new DrawnShape
            {
                Type = ShapeType.Star,
                X = centerX,
                Y = centerY,
                Radius = radius,
                Color = _selectedColor
            });

            await Ctx.SetFillStyleAsync(_selectedColor);
            await Ctx.BeginPathAsync();

            for (int i = 0; i < 10; i++)
            {
                var angle = (i * Math.PI) / 5;
                var r = (i % 2 == 0) ? radius : radius * 0.5f;
                var x = centerX + (float)(r * Math.Cos(angle - Math.PI / 2));
                var y = centerY + (float)(r * Math.Sin(angle - Math.PI / 2));

                if (i == 0)
                    await Ctx.MoveToAsync(x, y);
                else
                    await Ctx.LineToAsync(x, y);
            }

            await Ctx.ClosePathAsync();
            await Ctx.FillAsync();
            await Ctx.SetStrokeStyleAsync("#000000");
            await Ctx.SetLineWidthAsync(2);
            await Ctx.StrokeAsync();
        }

        private async Task RedrawCanvas()
        {
            if (Ctx == null) return;

            // Clear canvas
            await Ctx.ClearRectAsync(0, 0, 800, 600);

            // Draw grid if enabled
            if (_showGrid)
            {
                await DrawGrid();
            }

            // Redraw all paths using Canvas2DContext
            foreach (var path in _drawnPaths)
            {
                if (path.Points.Count > 1)
                {
                    await Ctx.SetStrokeStyleAsync(path.Color);
                    await Ctx.SetLineWidthAsync(path.Width);
                    await Ctx.BeginPathAsync();
                    
                    var firstPoint = path.Points[0];
                    await Ctx.MoveToAsync(firstPoint.X, firstPoint.Y);
                    
                    for (int i = 1; i < path.Points.Count; i++)
                    {
                        await Ctx.LineToAsync(path.Points[i].X, path.Points[i].Y);
                    }
                    
                    await Ctx.StrokeAsync();
                }
            }

            // Redraw all shapes using their Canvas2DContext methods
            foreach (var shape in _shapes)
            {
                switch (shape.Type)
                {
                    case ShapeType.Rectangle:
                        await Ctx.SetFillStyleAsync(shape.Color);
                        await Ctx.FillRectAsync(shape.X, shape.Y, shape.Width, shape.Height);
                        await Ctx.SetStrokeStyleAsync("#000000");
                        await Ctx.SetLineWidthAsync(2);
                        await Ctx.StrokeRectAsync(shape.X, shape.Y, shape.Width, shape.Height);
                        break;
                    case ShapeType.Circle:
                        await Ctx.SetFillStyleAsync(shape.Color);
                        await Ctx.BeginPathAsync();
                        await Ctx.ArcAsync(shape.X, shape.Y, shape.Radius, 0, 2 * Math.PI);
                        await Ctx.FillAsync();
                        await Ctx.SetStrokeStyleAsync("#000000");
                        await Ctx.SetLineWidthAsync(2);
                        await Ctx.StrokeAsync();
                        break;
                    case ShapeType.Star:
                        await DrawStarShape(shape.X, shape.Y, shape.Radius, shape.Color);
                        break;
                }
            }
        }

        private async Task DrawStarShape(float centerX, float centerY, float radius, string color)
        {
            if (Ctx == null) return;

            await Ctx.SetFillStyleAsync(color);
            await Ctx.BeginPathAsync();

            for (int i = 0; i < 10; i++)
            {
                var angle = (i * Math.PI) / 5;
                var r = (i % 2 == 0) ? radius : radius * 0.5f;
                var x = centerX + (float)(r * Math.Cos(angle - Math.PI / 2));
                var y = centerY + (float)(r * Math.Sin(angle - Math.PI / 2));

                if (i == 0)
                    await Ctx.MoveToAsync(x, y);
                else
                    await Ctx.LineToAsync(x, y);
            }

            await Ctx.ClosePathAsync();
            await Ctx.FillAsync();
            await Ctx.SetStrokeStyleAsync("#000000");
            await Ctx.SetLineWidthAsync(2);
            await Ctx.StrokeAsync();
        }

        private async Task ClearCanvas()
        {
            _drawnPaths.Clear();
            _shapes.Clear();
            if (Ctx != null)
            {
                await Ctx.ClearRectAsync(0, 0, 800, 600);
                if (_showGrid)
                {
                    await DrawGrid();
                }
            }
        }

        private async Task ToggleGrid()
        {
            _showGrid = !_showGrid;
            await RedrawCanvas();
            StateHasChanged();
        }

        private async Task ToggleGridClick()
        {
            _showGrid = !_showGrid;
            await RedrawCanvas();
            StateHasChanged();
        }

        private async Task CreateCampusScene()
        {
            await ClearCanvas();
            await DrawCampusScene();
        }

        private async Task DrawCampusScene()
        {
            if (Ctx == null) return;

            // Clear canvas
            await Ctx.ClearRectAsync(0, 0, 800, 600);

            // Draw sky
            await Ctx.SetFillStyleAsync("#87CEEB");
            await Ctx.FillRectAsync(0, 0, 800, 300);

            // Draw ground
            await Ctx.SetFillStyleAsync("#228B22");
            await Ctx.FillRectAsync(0, 300, 800, 300);

            // Draw main building
            await Ctx.SetFillStyleAsync("#8B4513");
            await Ctx.FillRectAsync(300, 200, 200, 150);

            // Draw building roof
            await Ctx.SetFillStyleAsync("#DC143C");
            await Ctx.BeginPathAsync();
            await Ctx.MoveToAsync(300, 200);
            await Ctx.LineToAsync(400, 150);
            await Ctx.LineToAsync(500, 200);
            await Ctx.ClosePathAsync();
            await Ctx.FillAsync();

            // Draw windows
            await Ctx.SetFillStyleAsync("#87CEEB");
            await Ctx.FillRectAsync(320, 220, 30, 40);
            await Ctx.FillRectAsync(370, 220, 30, 40);
            await Ctx.FillRectAsync(420, 220, 30, 40);
            await Ctx.FillRectAsync(470, 220, 30, 40);

            // Draw door
            await Ctx.SetFillStyleAsync("#654321");
            await Ctx.FillRectAsync(385, 290, 30, 60);

            // Draw trees
            await DrawTree(150, 300);
            await DrawTree(650, 300);

            // Draw walkway
            await Ctx.SetFillStyleAsync("#696969");
            await Ctx.FillRectAsync(0, 340, 800, 20);

            // Draw campus name
            await Ctx.SetFillStyleAsync("#000000");
            await Ctx.SetFontAsync("bold 24px Arial");
            await Ctx.FillTextAsync("BLAZOR CAMPUS", 310, 100);
        }

        private async Task DrawTree(float x, float y)
        {
            if (Ctx == null) return;

            // Draw trunk
            await Ctx.SetFillStyleAsync("#8B4513");
            await Ctx.FillRectAsync(x - 5, y - 40, 10, 40);

            // Draw leaves
            await Ctx.SetFillStyleAsync("#228B22");
            await Ctx.BeginPathAsync();
            await Ctx.ArcAsync(x, y - 60, 25, 0, 2 * Math.PI);
            await Ctx.FillAsync();
        }

        public class DrawnPath
        {
            public List<PointF> Points { get; set; } = new();
            public string Color { get; set; } = "#000000";
            public int Width { get; set; } = 1;
        }

        public class DrawnShape
        {
            public ShapeType Type { get; set; }
            public float X { get; set; }
            public float Y { get; set; }
            public float Width { get; set; }
            public float Height { get; set; }
            public float Radius { get; set; }
            public string Color { get; set; } = "#000000";
        }

        public enum ShapeType
        {
            Rectangle,
            Circle,
            Star
        }

        public async ValueTask DisposeAsync()
        {
            // Canvas2DContext doesn't need explicit disposal
            await Task.CompletedTask;
        }
    }
}