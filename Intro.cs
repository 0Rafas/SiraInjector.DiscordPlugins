using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;

namespace LoveStreakPlugin
{
    public partial class Intro : Form
    {
        private List<LoveParticle> particles = new List<LoveParticle>();
        private Random random = new Random();
        private System.Windows.Forms.Timer animationTimer;
        private int particleCount = 25;
        private bool regionInitialized = false;
        private bool isAnimating = false;
        private string selectedPluginPath = "";

        public Intro()
        {
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.None;
            
            this.DoubleBuffered = true;

            guna2CustomGradientPanel1.Paint += Guna2CustomGradientPanel1_Paint;

            ImportButton.Click += ImportButton_Click;
            InjectButton.Click += InjectButton_Click;
            
            animationTimer = new System.Windows.Forms.Timer();
            animationTimer.Interval = 33; // ~30 FPS
            animationTimer.Tick += AnimationTimer_Tick;
            animationTimer.Start();

            InitializeParticles();
            
            InitializeRegion();
        }

        private void ImportButton_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Title = "Select Plugin File";
                openFileDialog.Filter = "JavaScript/TypeScript Files (*.js;*.ts)|*.js;*.ts|All Files (*.*)|*.*";
                openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    selectedPluginPath = openFileDialog.FileName;
                    PathLabel.Text = selectedPluginPath;
                }
            }
        }

        private void InjectButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(selectedPluginPath))
            {
                using (var dialog = new MessageDialog("Error", "Please select a plugin file first.", false))
                {
                    dialog.ShowDialog(this);
                }
                return;
            }

            // Execute injection
            var result = PluginInjector.InjectPlugin(selectedPluginPath);

            using (var dialog = new MessageDialog(
                result.Success ? "Injection Successful" : "Injection Failed",
                result.Message,
                result.Success))
            {
                dialog.ShowDialog(this);
            }

            if (result.Success)
            {
                selectedPluginPath = "";
                PathLabel.Text = "C:/";
            }
        }

        private void Guna2CustomGradientPanel1_Paint(object sender, PaintEventArgs e)
        {
            try
            {
                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                
                foreach (var particle in particles)
                {
                    particle.Draw(e.Graphics);
                }
            }
            catch { }
        }

        private void InitializeRegion()
        {
            if (regionInitialized)
                return;

            GraphicsPath path = new GraphicsPath();
            try
            {
                int notchSize = 10;

                path.AddLine(notchSize, 0, this.Width - notchSize, 0);
                path.AddLine(this.Width - notchSize, 0, this.Width, notchSize);
                
                path.AddLine(this.Width, notchSize, this.Width, this.Height - notchSize);
                
                path.AddLine(this.Width, this.Height - notchSize, this.Width - notchSize, this.Height);
                
                path.AddLine(this.Width - notchSize, this.Height, notchSize, this.Height);
                
                path.AddLine(notchSize, this.Height, 0, this.Height - notchSize);
                
                path.AddLine(0, this.Height - notchSize, 0, notchSize);
                
                path.AddLine(0, notchSize, notchSize, 0);
                
                path.CloseFigure();

                var oldRegion = this.Region;
                this.Region = new Region(path);
                oldRegion?.Dispose();
                
                regionInitialized = true;
            }
            finally
            {
                path.Dispose();
            }
        }

        private void InitializeParticles()
        {
            particles.Clear();
            Rectangle bounds = guna2CustomGradientPanel1.ClientRectangle;
            for (int i = 0; i < particleCount; i++)
            {
                particles.Add(new LoveParticle(random, bounds));
            }
        }

        private void AnimationTimer_Tick(object sender, EventArgs e)
        {
            if (isAnimating)
                return;

            isAnimating = true;
            try
            {
                Rectangle bounds = guna2CustomGradientPanel1.ClientRectangle;
                
                for (int i = particles.Count - 1; i >= 0; i--)
                {
                    particles[i].Update(bounds);
                    
                    if (!particles[i].IsAlive)
                    {
                        particles.RemoveAt(i);
                    }
                }

                while (particles.Count < particleCount)
                {
                    particles.Add(new LoveParticle(random, bounds));
                }

                if (guna2CustomGradientPanel1.IsHandleCreated && !guna2CustomGradientPanel1.IsDisposed)
                {
                    guna2CustomGradientPanel1.Invalidate();
                }
            }
            finally
            {
                isAnimating = false;
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
        }

        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        private void Intro_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        private void CloseButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void MinimizeButton_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }
    }
}
