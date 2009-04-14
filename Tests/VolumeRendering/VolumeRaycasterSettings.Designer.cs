namespace Shaghal
{
    partial class VolumeRaycasterSettings
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.checkBoxPreInt = new System.Windows.Forms.CheckBox();
            this.numericUpDownNumSteps = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownNumSteps)).BeginInit();
            this.SuspendLayout();
            // 
            // checkBoxPreInt
            // 
            this.checkBoxPreInt.AutoSize = true;
            this.checkBoxPreInt.Location = new System.Drawing.Point(12, 12);
            this.checkBoxPreInt.Name = "checkBoxPreInt";
            this.checkBoxPreInt.Size = new System.Drawing.Size(91, 17);
            this.checkBoxPreInt.TabIndex = 0;
            this.checkBoxPreInt.Text = "Preintegration";
            this.checkBoxPreInt.UseVisualStyleBackColor = true;
            this.checkBoxPreInt.CheckedChanged += new System.EventHandler(this.checkBoxPreInt_CheckedChanged);
            // 
            // numericUpDownNumSteps
            // 
            this.numericUpDownNumSteps.Location = new System.Drawing.Point(12, 35);
            this.numericUpDownNumSteps.Maximum = new decimal(new int[] {
            5000,
            0,
            0,
            0});
            this.numericUpDownNumSteps.Minimum = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.numericUpDownNumSteps.Name = "numericUpDownNumSteps";
            this.numericUpDownNumSteps.Size = new System.Drawing.Size(120, 20);
            this.numericUpDownNumSteps.TabIndex = 2;
            this.numericUpDownNumSteps.Value = new decimal(new int[] {
            256,
            0,
            0,
            0});
            this.numericUpDownNumSteps.ValueChanged += new System.EventHandler(this.numericUpDownNumSteps_ValueChanged);
            // 
            // VolumeRaycasterSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(184, 90);
            this.Controls.Add(this.numericUpDownNumSteps);
            this.Controls.Add(this.checkBoxPreInt);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.Name = "VolumeRaycasterSettings";
            this.Text = "Raycaster Settings";
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownNumSteps)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox checkBoxPreInt;
        private System.Windows.Forms.NumericUpDown numericUpDownNumSteps;
    }
}