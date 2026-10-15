using System;
using SkiaSharp;

namespace Particle.Maui.Particles
{
    /// <summary>
    /// A star that flies from one point to another along an arc and then reports that it
    /// arrived. Unlike the other particles it is not thrown and left to physics: it has a
    /// destination, which is what makes it read as something being collected — a reward
    /// travelling to the score, an item to the inventory.
    /// </summary>
    public class FlyingStarParticle : ParticleBase
    {
        private readonly SKPoint _origin;
        private readonly SKPoint _destination;
        private readonly SKPoint _controlPoint;
        private readonly float _lifetimeMillis;
        private readonly float _startSize;
        private readonly SKPaint _paint;
        private readonly SKPaint _glowPaint;

        private long _firstUpdateMillis;
        private float _progress;
        private bool _arrivedRaised;

        /// <summary>Raised once, on the frame the star reaches its destination.</summary>
        public event EventHandler Arrived;

        public FlyingStarParticle(SKColor color, SKPoint origin, SKPoint destination, float size, float lifetimeMillis, float arcHeightFactor)
            : base(new SKPoint3(0, 0, 220f), 0f, 0f, SKPoint3.Empty, origin, new SKSize(size, size))
        {
            _origin = origin;
            _destination = destination;
            _lifetimeMillis = lifetimeMillis <= 0 ? 1f : lifetimeMillis;
            _startSize = size;

            // Control point at the midpoint, lifted perpendicular to the flight, so the
            // star lobs rather than sliding in a straight line.
            var mid = new SKPoint((origin.X + destination.X) / 2f, (origin.Y + destination.Y) / 2f);
            var dx = destination.X - origin.X;
            var dy = destination.Y - origin.Y;
            var distance = (float) Math.Sqrt(dx * dx + dy * dy);
            _controlPoint = new SKPoint(mid.X, mid.Y - distance * arcHeightFactor);

            _paint = new SKPaint { Color = color, IsAntialias = true, Style = SKPaintStyle.Fill };
            _glowPaint = new SKPaint
            {
                Color = color.WithAlpha(90),
                IsAntialias = true,
                Style = SKPaintStyle.Fill,
                MaskFilter = SKMaskFilter.CreateBlur(SKBlurStyle.Normal, size * 0.6f),
            };
        }

        public override bool IsFinished => _progress >= 1f;

        public override void Update(long absoluteElapsedMillis, SKSize scale)
        {
            if (_firstUpdateMillis == 0)
            {
                _firstUpdateMillis = absoluteElapsedMillis;
            }

            var elapsed = absoluteElapsedMillis - _firstUpdateMillis;
            _progress = Math.Clamp(elapsed / _lifetimeMillis, 0f, 1f);

            // Quadratic bezier from origin through the lifted control point to the target
            var t = _progress;
            var oneMinusT = 1f - t;
            Position = new SKPoint(
                oneMinusT * oneMinusT * _origin.X + 2f * oneMinusT * t * _controlPoint.X + t * t * _destination.X,
                oneMinusT * oneMinusT * _origin.Y + 2f * oneMinusT * t * _controlPoint.Y + t * t * _destination.Y);

            // Shrink a little on approach so it settles into the target rather than
            // arriving at full size, and spin throughout.
            var shrink = 1f - t * 0.3f;
            Size = new SKSize(_startSize * shrink, _startSize * shrink);
            Orientation = new SKPoint3(0, 0, elapsed * 0.001f * RotationSpeed.Z);

            TransformationMatrix = SKMatrix.CreateTranslation(-Position.X, -Position.Y)
                .PostConcat(SKMatrix.CreateScale(scale.Width, scale.Height))
                .PostConcat(SKMatrix.CreateRotationDegrees(Orientation.Z))
                .PostConcat(SKMatrix.CreateTranslation(Position.X, Position.Y));

            if (_progress >= 1f && !_arrivedRaised)
            {
                _arrivedRaised = true;
                Arrived?.Invoke(this, EventArgs.Empty);
            }
        }

        protected override void Draw(SKCanvas canvas)
        {
            // Hold full strength until the last moment, then fade out over the final
            // stretch so it dissolves into the destination instead of stopping dead.
            const float fadeFrom = 0.85f;
            var opacity = _progress > fadeFrom ? 1f - (_progress - fadeFrom) / (1f - fadeFrom) : 1f;
            var alpha = (byte) Math.Clamp(opacity * 255f, 0f, 255f);

            _glowPaint.Color = _glowPaint.Color.WithAlpha((byte) (alpha * 0.35f));
            canvas.DrawCircle(Position.X, Position.Y, Size.Width, _glowPaint);

            _paint.Color = _paint.Color.WithAlpha(alpha);
            using var path = CreateStarPath(Position, Size.Width, Size.Width * 0.45f);
            canvas.DrawPath(path, _paint);
        }

        private static SKPath CreateStarPath(SKPoint centre, float outerRadius, float innerRadius)
        {
            var path = new SKPath();
            const int points = 5;

            for (var i = 0; i < points * 2; i++)
            {
                var radius = i % 2 == 0 ? outerRadius : innerRadius;
                var angle = Math.PI / points * i - Math.PI / 2;
                var point = new SKPoint(
                    centre.X + (float) (Math.Cos(angle) * radius),
                    centre.Y + (float) (Math.Sin(angle) * radius));

                if (i == 0) path.MoveTo(point);
                else path.LineTo(point);
            }

            path.Close();
            return path;
        }
    }
}
