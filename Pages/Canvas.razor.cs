using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using System.Threading.Tasks;

namespace BlazorCanvas2026.Pages
{
    public partial class Canvas : ComponentBase
    {
        private ElementReference canvasElement;
        private bool isDrawing = false;
        private string strokeColor = "#000000";
        private int lineWidth = 2;
        private bool animationRunning = false;

        [Inject]
        protected IJSRuntime JSRuntime { get; set; } = default!;

        private async Task StartDrawing(MouseEventArgs e)
        {
            isDrawing = true;
            await JSRuntime.InvokeVoidAsync("canvasInterop.beginPath", canvasElement, e.OffsetX, e.OffsetY);
        }

        private async Task Draw(MouseEventArgs e)
        {
            if (isDrawing)
            {
                await JSRuntime.InvokeVoidAsync("canvasInterop.drawLine", canvasElement, e.OffsetX, e.OffsetY, strokeColor, lineWidth);
            }
        }

        private void StopDrawing(MouseEventArgs e)
        {
            isDrawing = false;
        }

        private async Task ClearCanvas()
        {
            await JSRuntime.InvokeVoidAsync("canvasInterop.clearCanvas", canvasElement);
        }

        private async Task DrawRectangle()
        {
            await JSRuntime.InvokeVoidAsync("canvasInterop.drawRectangle", canvasElement, 100, 100, 200, 150, strokeColor, lineWidth);
        }

        private async Task DrawCircle()
        {
            await JSRuntime.InvokeVoidAsync("canvasInterop.drawCircle", canvasElement, 400, 200, 50, strokeColor, lineWidth);
        }

        private async Task DrawFilledRectangle()
        {
            await JSRuntime.InvokeVoidAsync("canvasInterop.drawFilledRectangle", canvasElement, 300, 300, 150, 100, strokeColor);
        }

        private async Task DrawFilledCircle()
        {
            await JSRuntime.InvokeVoidAsync("canvasInterop.drawFilledCircle", canvasElement, 600, 400, 60, strokeColor);
        }

        private async Task StartAnimation()
        {
            animationRunning = true;
            await JSRuntime.InvokeVoidAsync("canvasInterop.startAnimation", canvasElement);
        }

        private async Task StopAnimation()
        {
            animationRunning = false;
            await JSRuntime.InvokeVoidAsync("canvasInterop.stopAnimation", canvasElement);
        }
    }
}