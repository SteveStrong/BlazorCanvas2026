using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace BlazorCanvas2026.Pages
{
    public partial class Canvas : ComponentBase
    {
        [Inject]
        public IJSRuntime JSRuntime { get; set; } = default!;

        private ElementReference canvasElement;
        private bool isDrawing = false;
        private string strokeColor = "#000000";
        private int lineWidth = 2;
        private bool animationRunning = false;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await JSRuntime.InvokeVoidAsync("initializeCanvas", canvasElement);
            }
        }

        private async Task StartDrawing(MouseEventArgs e)
        {
            isDrawing = true;
            var rect = await JSRuntime.InvokeAsync<ClientRect>("getCanvasRect", canvasElement);
            var x = e.ClientX - rect.Left;
            var y = e.ClientY - rect.Top;
            await JSRuntime.InvokeVoidAsync("startPath", canvasElement, x, y, strokeColor, lineWidth);
        }

        private async Task Draw(MouseEventArgs e)
        {
            if (isDrawing)
            {
                var rect = await JSRuntime.InvokeAsync<ClientRect>("getCanvasRect", canvasElement);
                var x = e.ClientX - rect.Left;
                var y = e.ClientY - rect.Top;
                await JSRuntime.InvokeVoidAsync("drawLine", canvasElement, x, y);
            }
        }

        private Task StopDrawing()
        {
            isDrawing = false;
            return Task.CompletedTask;
        }

        private async Task ClearCanvas()
        {
            await JSRuntime.InvokeVoidAsync("clearCanvas", canvasElement);
        }

        private async Task DrawRectangle()
        {
            await JSRuntime.InvokeVoidAsync("drawRectangle", canvasElement, 100, 100, 200, 150, strokeColor, lineWidth);
        }

        private async Task DrawCircle()
        {
            await JSRuntime.InvokeVoidAsync("drawCircle", canvasElement, 400, 200, 75, strokeColor, lineWidth);
        }

        private async Task DrawFilledRectangle()
        {
            await JSRuntime.InvokeVoidAsync("drawFilledRectangle", canvasElement, 150, 150, 200, 150, strokeColor);
        }

        private async Task DrawFilledCircle()
        {
            await JSRuntime.InvokeVoidAsync("drawFilledCircle", canvasElement, 450, 250, 75, strokeColor);
        }

        private async Task StartAnimation()
        {
            if (!animationRunning)
            {
                animationRunning = true;
                await JSRuntime.InvokeVoidAsync("startAnimation", canvasElement);
                StateHasChanged();
            }
        }

        private async Task StopAnimation()
        {
            if (animationRunning)
            {
                animationRunning = false;
                await JSRuntime.InvokeVoidAsync("stopAnimation");
                StateHasChanged();
            }
        }

        public class ClientRect
        {
            public double Left { get; set; }
            public double Top { get; set; }
            public double Right { get; set; }
            public double Bottom { get; set; }
            public double Width { get; set; }
            public double Height { get; set; }
        }
    }
}