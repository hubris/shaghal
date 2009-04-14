using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Shaghal
{
    public partial class VolumeRaycasterSettings : Form
    {
        public VolumeRaycasterSettings(VolumeRaycaster vrc)
        {
            InitializeComponent();
            _vrc = vrc;
        }

        private VolumeRaycaster _vrc;

        private void numericUpDownNumSteps_ValueChanged(object sender, EventArgs e)
        {
            _vrc.StepSize = 1.0f/(float)numericUpDownNumSteps.Value;
        }

        private void checkBoxPreInt_CheckedChanged(object sender, EventArgs e)
        {
            _vrc.PreIntegration = checkBoxPreInt.Checked;
        }
    }
}
