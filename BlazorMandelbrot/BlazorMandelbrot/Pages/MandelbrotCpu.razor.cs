using Aptacode.BlazorCanvas;
using Microsoft.AspNetCore.Components;

namespace BlazorMandelbrot.Pages
{
    public class MandelbrotCpuBase : ComponentBase
    {
        protected BlazorCanvas Canvas { get; set; } = default!;
        protected int Width => 400;
        protected int Height => 400;

        private readonly ArraySegment<int> _data;

        private readonly Mandelbrot _mandelbrot = new();

        public MandelbrotCpuBase()
        {
            _data = new int[Width * Height];
        }

        protected override async Task OnInitializedAsync()
        {
            using var timer = new PeriodicTimer(TimeSpan.FromMilliseconds(15));

            while (Canvas is not { Ready: true })
            {
                await Task.Delay(10);
            }

            Canvas.SetImageBuffer(_data);

            while (await timer.WaitForNextTickAsync())
            {
                _mandelbrot.RenderImage(_data, Width, Height, HighResolution);
                Canvas.DrawImageBuffer(0, 0, Width, Height);
                await InvokeAsync(StateHasChanged);
            }
        }

        public bool HighResolution { get; set; }
        public void MouseDown()
        {
            HighResolution = true;
        }

        public void MouseUp()
        {
            HighResolution = false;
        }
    }
}
