using System.Runtime.CompilerServices;

namespace BlazorMandelbrot
{
    public class Mandelbrot
    {
        private readonly double _yMin = -1.5;
        private readonly double _yMax = 1.5;
        private readonly double _xMin = -2.0;
        private readonly double _xMax = 2.0;
        private readonly int[] _colours;
        private readonly int _black = 255 << 24 |    // alpha
                                0 << 16 |    // blue
                                0 << 8 |    // green
                                0;          // red

        private double _zoom = 0.01;
        private int _maxIterations = 200;

        //Tante Renate
        private readonly double _px = -0.7746806106269039;
        private readonly double _py = -0.1374168856037867;

        public Mandelbrot()
        {
            _colours = new int[1000];
            for (int i = 0; i < _colours.Length; i++)
            {
                var colourIndex = (double)i / _colours.Length;
                var hue = Math.Pow(colourIndex, 0.25);
                _colours[i] = ColorFromHSLA(hue, 0.9, 0.6);
            }
        }

        public void Reset()
        {
            _zoom = 0.01;
            _maxIterations = 200;
        }

        public void RenderImage(ArraySegment<int> data, int width, int height, bool highResolution = false)
        {
            _zoom *= 0.9;
            _maxIterations += 10;

            var RX1 = _px - _zoom / 2;
            var RY1 = _py - _zoom / 2;
            var dRx = _px + _zoom / 2 - RX1;
            var dRy = _py + _zoom / 2 - RY1;

            var xyPixelStep = highResolution ? 1 : 4;
            var xStep = (_xMax - _xMin) / width * xyPixelStep;
            var yStep = (_yMax - _yMin) / height * xyPixelStep;

            var yPix = 0;
            for (var y = _yMin; y < _yMax; y += yStep)
            {
                var y0 = y * dRy + RY1;
                var xPix = 0;
                for (var x = _xMin; x < _xMax; x += xStep)
                {
                    var x0 = x * dRx + RX1;
                    var x1 = x0;
                    var y1 = y0;

                    int n = 0;
                    while (++n < _maxIterations)
                    {
                        double x2 = x1 * x1;
                        double y2 = y1 * y1;
                        y1 = 2 * x1 * y1 + y0;
                        x1 = x2 - y2 + x0;

                        if (x2 + y2 >= 4)
                        {
                            break;
                        }
                    }

                    var color = n < _colours.Length ? _colours[n] : _black;

                    for (int pX = 0; pX < xyPixelStep; pX++)
                    {
                        for (int pY = 0; pY < xyPixelStep; pY++)
                        {
                            var p = (yPix + pY) * width + xPix + pX;
                            if (p < data.Count)
                            {
                                data[p] = color;
                            }
                        }
                    }

                    xPix += xyPixelStep;
                }
                yPix += xyPixelStep;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int ColorFromHSLA(double H, double S, double L)
        {
            double v;
            double r, g, b;

            r = L;
            g = L;
            b = L;

            v = L <= 0.5 ? L * (1.0 + S) : L + S - L * S;

            if (v > 0)
            {
                double m;
                double sv;
                int sextant;
                double fract, vsf, mid1, mid2;

                m = L + L - v;
                sv = (v - m) / v;
                H *= 6.0;
                sextant = (int)H;
                fract = H - sextant;
                vsf = v * sv * fract;
                mid1 = m + vsf;
                mid2 = v - vsf;

                switch (sextant)
                {
                    case 0:
                        r = v;
                        g = mid1;
                        b = m;
                        break;

                    case 1:
                        r = mid2;
                        g = v;
                        b = m;
                        break;

                    case 2:
                        r = m;
                        g = v;
                        b = mid1;
                        break;

                    case 3:
                        r = m;
                        g = mid2;
                        b = v;
                        break;

                    case 4:
                        r = mid1;
                        g = m;
                        b = v;
                        break;

                    case 5:
                        r = v;
                        g = m;
                        b = mid2;
                        break;
                }
            }

            return 255 << 24 |                // alpha
                    (int)(b * 255) << 16 |    // blue
                    (int)(g * 255) << 8 |    // green
                    (int)(r * 255);             // red
        }
    }
}
