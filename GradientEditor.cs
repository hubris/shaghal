using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace VolumeRendering
{
    class GradientEditor : Form
    {

        public GradientEditor()
            : base()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // GradientEditor
            // 
            this.ClientSize = new System.Drawing.Size(284, 264);
            this.Name = "GradientEditor";
            this.ResumeLayout(false);

        }
    }
}
