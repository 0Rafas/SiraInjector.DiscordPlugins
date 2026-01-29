using System;
using System.Drawing;

namespace LoveStreakPlugin
{
    public class LoveParticle
    {
        private PointF position;
        private PointF velocity;
        private float radius;
        private Color color;
        private Random random;
        private float opacity;
        private float lifeTime;
        private float maxLifeTime;

        public LoveParticle(Random rand, Rectangle bounds)
        {
            random = rand;
            radius = (float)(random.NextDouble() * 3 + 2);
            
            Color[] colors = new Color[]
            {
                Color.FromArgb(255, 105, 180),  // Hot Pink
                Color.FromArgb(220, 20, 60),   // Crimson
                Color.FromArgb(199, 21, 133),  // Medium Violet Red
                Color.FromArgb(255, 20, 147),  // Deep Pink
                Color.FromArgb(240, 128, 128), // Light Coral
                Color.FromArgb(255, 182, 193), // Light Pink
                Color.FromArgb(255, 192, 203)  // Pink
            };
            
            color = colors[random.Next(colors.Length)];
            
            position = new PointF(
                random.Next(bounds.Width), 
                random.Next(bounds.Height)
            );

            double angle = random.NextDouble() * 2 * Math.PI;
            float speed = (float)(random.NextDouble() * 0.8f + 0.3f);
            velocity = new PointF(
                (float)(Math.Cos(angle) * speed),
                (float)(Math.Sin(angle) * speed * 0.3f - 0.2f)
            );

            opacity = 200;
            maxLifeTime = (float)(random.NextDouble() * 5 + 4);
            lifeTime = 0;
        }

        public void Update(Rectangle bounds)
        {
            position.X += velocity.X;
            position.Y += velocity.Y;

            velocity.Y -= 0.01f;

            if (position.X < 0 || position.X > bounds.Width)
                velocity.X = -velocity.X * 0.95f;
            if (position.Y < 0 || position.Y > bounds.Height)
                velocity.Y = -velocity.Y * 0.95f;

            position.X = Math.Max(0, Math.Min(bounds.Width, position.X));
            position.Y = Math.Max(0, Math.Min(bounds.Height, position.Y));

            lifeTime += 0.016f;
            float lifeRatio = lifeTime / maxLifeTime;
            
            opacity = 200 * (1 - (lifeRatio * lifeRatio * 0.5f));
            
            radius = radius * (1 - lifeRatio * 0.08f);
        }

        public void Draw(Graphics g)
        {
            if (opacity <= 3)
                return;

            try
            {
                using (SolidBrush brush = new SolidBrush(Color.FromArgb((int)opacity, color)))
                {
                    g.FillEllipse(brush, position.X - radius, position.Y - radius, radius * 2, radius * 2);
                }
            }
            catch { }
        }

        public bool IsAlive => lifeTime < maxLifeTime;
    }
}
