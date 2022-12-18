using Aptacode.BlazorCanvas;
using Microsoft.AspNetCore.Components;

namespace BlazorMandelbrot.Pages
{
    public class MandelbrotCpuBase : ComponentBase
    {
        protected BlazorCanvas Canvas { get; set; } = default!;
        protected int Width => 400;
        protected int Height => 400;

        private ArraySegment<int> _data;

        private readonly Mandelbrot _mandelbrot = new();

        protected override async Task OnInitializedAsync()
        {

            while (Canvas is not { Ready: true })
            {
                await Task.Delay(10);
            }

            _data = new int[Width * Height];

            Canvas.SetImageBuffer(_data);

            using var timer = new PeriodicTimer(TimeSpan.FromMilliseconds(15));
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
