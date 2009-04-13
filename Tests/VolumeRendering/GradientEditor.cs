using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Shaghal;

namespace VolumeRendering
{
    class GradientEditor : Form
    {
        private TabControl tabControl1;
        private TrackBar trackBarMin;
        private NumericUpDown numericUpDownMin;
        private Label label1;
        private NumericUpDown numericUpDownMax;
        private Label label2;
        private TrackBar trackBarMax;
        private TabPage tabGradient;

        private Gradient _gradient;
        private Microsoft.Xna.Framework.Graphics.Color[] _gradColors;

        public Microsoft.Xna.Framework.Graphics.Color[] GradColors
        {
            get { return _gradColors; }
            set { _gradColors = value; }
        }

        public Gradient Gradient
        {
            set 
            { 
                _gradient = value;                
            }
        }

        public GradientEditor()
            : base()
        {
            InitializeComponent();
            numericUpDownMin.DataBindings.Add("Value", trackBarMin, "Value");
            trackBarMin.DataBindings.Add("Value", numericUpDownMin, "Value");
            numericUpDownMax.DataBindings.Add("Value", trackBarMax, "Value");
            //trackBarMax.DataBindings.Add("Value", numericUpDownMax, "Value");

            this.trackBarMax.Scroll += new System.EventHandler(this.trackBarMax_Scroll);
            this.trackBarMin.Scroll += new System.EventHandler(this.trackBarMin_Scroll);
        }

        private void trackBarMax_Scroll(object sender, System.EventArgs e)
        {
            if (trackBarMax.Value < trackBarMin.Value)
                trackBarMax.Value = trackBarMin.Value;
            numericUpDownMax.Value = trackBarMax.Value;
            _gradient.Max = trackBarMax.Value/255.0f;
            _gradColors = _gradient.GetColors(256);
        }

        private void trackBarMin_Scroll(object sender, System.EventArgs e)
        {
            if (trackBarMax.Value < trackBarMin.Value)
                trackBarMin.Value = trackBarMax.Value;
            numericUpDownMin.Value = trackBarMin.Value;
            _gradient.Min = trackBarMin.Value / 255.0f;
            _gradColors = _gradient.GetColors(256);
        }

        private void InitializeComponent()
        {
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabGradient = new System.Windows.Forms.TabPage();
            this.numericUpDownMax = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.trackBarMax = new System.Windows.Forms.TrackBar();
            this.numericUpDownMin = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.trackBarMin = new System.Windows.Forms.TrackBar();
            this.tabControl1.SuspendLayout();
            this.tabGradient.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownMax)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarMax)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownMin)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarMin)).BeginInit();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabGradient);
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(207, 143);
            this.tabControl1.SizeMode = System.Windows.Forms.TabSizeMode.FillToRight;
            this.tabControl1.TabIndex = 0;
            // 
            // tabGradient
            // 
            this.tabGradient.Controls.Add(this.numericUpDownMax);
            this.tabGradient.Controls.Add(this.label2);
            this.tabGradient.Controls.Add(this.trackBarMax);
            this.tabGradient.Controls.Add(this.numericUpDownMin);
            this.tabGradient.Controls.Add(this.label1);
            this.tabGradient.Controls.Add(this.trackBarMin);
            this.tabGradient.Location = new System.Drawing.Point(4, 22);
            this.tabGradient.Name = "tabGradient";
            this.tabGradient.Padding = new System.Windows.Forms.Padding(3);
            this.tabGradient.Size = new System.Drawing.Size(199, 117);
            this.tabGradient.TabIndex = 0;
            this.tabGradient.Text = "Gradient";
            this.tabGradient.UseVisualStyleBackColor = true;
            // 
            // numericUpDownMax
            // 
            this.numericUpDownMax.AutoSize = true;
            this.numericUpDownMax.Location = new System.Drawing.Point(146, 57);
            this.numericUpDownMax.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.numericUpDownMax.Name = "numericUpDownMax";
            this.numericUpDownMax.Size = new System.Drawing.Size(41, 20);
            this.numericUpDownMax.TabIndex = 4;
            this.numericUpDownMax.Value = new decimal(new int[] {
            255,
            0,
            0,
            0});
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(4, 59);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(27, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Max";
            // 
            // trackBarMax
            // 
            this.trackBarMax.Location = new System.Drawing.Point(34, 57);
            this.trackBarMax.Maximum = 255;
            this.trackBarMax.Name = "trackBarMax";
            this.trackBarMax.Size = new System.Drawing.Size(106, 45);
            this.trackBarMax.TabIndex = 3;
            this.trackBarMax.TickStyle = System.Windows.Forms.TickStyle.None;
            this.trackBarMax.Value = 255;
            // 
            // numericUpDownMin
            // 
            this.numericUpDownMin.AutoSize = true;
            this.numericUpDownMin.Location = new System.Drawing.Point(146, 6);
            this.numericUpDownMin.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.numericUpDownMin.Name = "numericUpDownMin";
            this.numericUpDownMin.Size = new System.Drawing.Size(41, 20);
            this.numericUpDownMin.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(4, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(24, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Min";
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // trackBarMin
            // 
            this.trackBarMin.Location = new System.Drawing.Point(34, 6);
            this.trackBarMin.Maximum = 255;
            this.trackBarMin.Name = "trackBarMin";
            this.trackBarMin.Size = new System.Drawing.Size(106, 45);
            this.trackBarMin.TabIndex = 0;
            this.trackBarMin.TickStyle = System.Windows.Forms.TickStyle.None;
            // 
            // GradientEditor
            // 
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(210, 142);
            this.Controls.Add(this.tabControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "GradientEditor";
            this.Text = "Gradient Editor";
            this.tabControl1.ResumeLayout(false);
            this.tabGradient.ResumeLayout(false);
            this.tabGradient.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownMax)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarMax)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownMin)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarMin)).EndInit();
            this.ResumeLayout(false);

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
