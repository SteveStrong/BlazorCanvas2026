using Microsoft.AspNetCore.Components;

namespace BlazorCanvas2026.Services
{
    // Minimal stub implementations to get Canvas2DComponent working
    
    public interface IWorkspace 
    {
        object? GetDrawing();
        object? GetSelectionService();
        void PreRender(int tick);
        void PostRender(int tick);
        void RenderWatermark(object ctx, int tick);
    }
    
    public interface IFoundryService
    {
        object Toast();
        object Drawing();
        object Selection();
        object AnimationBus();
    }
    
    // Simple stub implementations
    public class StubWorkspace : IWorkspace
    {
        public object? GetDrawing() => new StubDrawing();
        public object? GetSelectionService() => new object();
        public void PreRender(int tick) { }
        public void PostRender(int tick) { }
        public void RenderWatermark(object ctx, int tick) { }
    }
    
    public class StubFoundryService : IFoundryService
    {
        public object Toast() => new object();
        public object Drawing() => new StubDrawing();
        public object Selection() => new object();
        public object AnimationBus() => new StubComponentBus();
    }
    
    public class StubDrawing
    {
        public void SetCanvasSizeInPixels(int width, int height) { }
        public void ClearAll() { }
        public bool IsFrameRefreshPaused() => false;
        public bool SetCurrentlyRendering(bool value, int tick) => false;
        public async Task RenderDrawing(object ctx, int tick, double fps) { await Task.CompletedTask; }
    }
    
    public class StubComponentBus
    {
        public void SubscribeTo<T>(Action<T> handler) { }
        public void UnSubscribeFrom<T>(Action<T> handler) { }
    }
}