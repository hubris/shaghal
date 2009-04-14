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
        private TabPage tabPage1;
        private Button buttonColor;
        private Microsoft.Xna.Framework.Graphics.Color[] _gradColors;
        private NumericUpDown numericUpDown1;
        private TrackBar trackBarAlpha;

        private Microsoft.Xna.Framework.Graphics.Color _matColor;

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

        public Microsoft.Xna.Framework.Graphics.Color MaterialColor
        {
            get { return _matColor; }
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
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
            this.trackBarAlpha = new System.Windows.Forms.TrackBar();
            this.buttonColor = new System.Windows.Forms.Button();
            this.tabControl1.SuspendLayout();
            this.tabGradient.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownMax)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarMax)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownMin)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarMin)).BeginInit();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarAlpha)).BeginInit();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabGradient);
            this.tabControl1.Controls.Add(this.tabPage1);
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
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.numericUpDown1);
            this.tabPage1.Controls.Add(this.trackBarAlpha);
            this.tabPage1.Controls.Add(this.buttonColor);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(199, 117);
            this.tabPage1.TabIndex = 1;
            this.tabPage1.Text = "Material";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // numericUpDown1
            // 
            this.numericUpDown1.Location = new System.Drawing.Point(131, 35);
            this.numericUpDown1.Name = "numericUpDown1";
            this.numericUpDown1.Size = new System.Drawing.Size(40, 20);
            this.numericUpDown1.TabIndex = 2;
            // 
            // trackBarAlpha
            // 
            this.trackBarAlpha.Location = new System.Drawing.Point(8, 35);
            this.trackBarAlpha.Maximum = 255;
            this.trackBarAlpha.Name = "trackBarAlpha";
            this.trackBarAlpha.Size = new System.Drawing.Size(117, 45);
            this.trackBarAlpha.TabIndex = 1;
            this.trackBarAlpha.TickFrequency = 16;
            this.trackBarAlpha.ValueChanged += new System.EventHandler(this.trackBarAlpha_ValueChanged);
            // 
            // buttonColor
            // 
            this.buttonColor.Location = new System.Drawing.Point(8, 6);
            this.buttonColor.Name = "buttonColor";
            this.buttonColor.Size = new System.Drawing.Size(75, 23);
            this.buttonColor.TabIndex = 0;
            this.buttonColor.Text = "Color";
            this.buttonColor.UseVisualStyleBackColor = true;
            this.buttonColor.MouseClick += new System.Windows.Forms.MouseEventHandler(this.buttonColor_MouseClick);
            // 
            // GradientEditor
            // 
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(210, 142);
            this.Controls.Add(this.tabControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
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
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarAlpha)).EndInit();
            this.ResumeLayout(false);

        }
        private void buttonColor_MouseClick(object sender, MouseEventArgs e)
        {
            ColorDialog diag = new ColorDialog();
            diag.ShowDialog();
            Color col = diag.Color;
            _matColor = new Microsoft.Xna.Framework.Graphics.Color(col.R, col.G, col.B, trackBarAlpha.Value);
        }

        private void trackBarAlpha_ValueChanged(object sender, EventArgs e)
        {

        }
    }
}
