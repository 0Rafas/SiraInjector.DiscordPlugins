using System;
using System.Drawing;
using System.Windows.Forms;

namespace LoveStreakPlugin
{
    public class MessageDialog : Form
    {
        private Label messageLabel;
        private Button okButton;
        private Panel headerPanel;
        private Label titleLabel;
        private bool isSuccess;

        public MessageDialog(string title, string message, bool success = true)
        {
            isSuccess = success;
            InitializeComponent(title, message);
        }

        private void InitializeComponent(string title, string message)
        {
            this.Text = title;
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.CenterParent;
            this.Size = new Size(450, 280);
            this.DoubleBuffered = true;
            this.BackColor = Color.White;

            headerPanel = new Panel();
            headerPanel.BackColor = isSuccess ? Color.FromArgb(76, 175, 80) : Color.FromArgb(244, 67, 54);
            headerPanel.Height = 60;
            headerPanel.Dock = DockStyle.Top;

            titleLabel = new Label();
            titleLabel.Text = isSuccess ? "? Success" : "? Error";
            titleLabel.ForeColor = Color.White;
            titleLabel.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            titleLabel.TextAlign = ContentAlignment.MiddleCenter;
            titleLabel.Dock = DockStyle.Fill;
            headerPanel.Controls.Add(titleLabel);
            messageLabel = new Label();
            messageLabel.Text = message;
            messageLabel.ForeColor = Color.FromArgb(33, 33, 33);
            messageLabel.Font = new Font("Segoe UI", 10);
            messageLabel.AutoSize = false;
            messageLabel.TextAlign = ContentAlignment.TopLeft;
            messageLabel.Margin = new Padding(20);
            messageLabel.Bounds = new Rectangle(20, 70, 410, 150);
            okButton = new Button();
            okButton.Text = "OK";
            okButton.BackColor = isSuccess ? Color.FromArgb(76, 175, 80) : Color.FromArgb(244, 67, 54);
            okButton.ForeColor = Color.White;
            okButton.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            okButton.FlatStyle = FlatStyle.Flat;
            okButton.FlatAppearance.BorderSize = 0;
            okButton.Bounds = new Rectangle(175, 235, 100, 35);
            okButton.Click += (s, e) => this.Close();
            okButton.Cursor = Cursors.Hand;

            this.Controls.Add(messageLabel);
            this.Controls.Add(okButton);
            this.Controls.Add(headerPanel);
        }
    }
}
