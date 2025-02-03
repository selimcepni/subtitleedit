using System.Windows.Forms;
namespace Nikse.SubtitleEdit.Forms
{
    partial class Context
    {
        private System.ComponentModel.IContainer components = null;

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.labelContexts = new System.Windows.Forms.Label();
            this.comboBoxContexts = new System.Windows.Forms.ComboBox();
            this.labelValue = new System.Windows.Forms.Label();
            this.textBoxValue = new Nikse.SubtitleEdit.Controls.NikseTextBox();
            this.buttonSend = new System.Windows.Forms.Button();
            this.SuspendLayout();

            // labelContexts
            this.labelContexts.AutoSize = true;
            this.labelContexts.Location = new System.Drawing.Point(12, 15);
            this.labelContexts.Name = "labelContexts";
            this.labelContexts.Size = new System.Drawing.Size(49, 13);
            this.labelContexts.Text = "Projects";

            // comboBoxContexts
            this.comboBoxContexts.FormattingEnabled = true;
            this.comboBoxContexts.Location = new System.Drawing.Point(15, 31);
            this.comboBoxContexts.Name = "comboBoxContexts";
            this.comboBoxContexts.Size = new System.Drawing.Size(250, 21);
            this.comboBoxContexts.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDown;

            // labelValue
            this.labelValue.AutoSize = true;
            this.labelValue.Location = new System.Drawing.Point(12, 65);
            this.labelValue.Name = "labelValue";
            this.labelValue.Size = new System.Drawing.Size(34, 13);
            this.labelValue.Text = "Context";

            // textBoxValue
            this.textBoxValue.Location = new System.Drawing.Point(15, 81);
            this.textBoxValue.Name = "textBoxValue";
            this.textBoxValue.Size = new System.Drawing.Size(250, 100);
            this.textBoxValue.Multiline = true;
            this.textBoxValue.ScrollBars = ScrollBars.Vertical;

            // buttonSend
            this.buttonSend.Location = new System.Drawing.Point(190, 190);
            this.buttonSend.Name = "buttonSend";
            this.buttonSend.Size = new System.Drawing.Size(75, 23);
            this.buttonSend.Text = "Send";
            this.buttonSend.Click += new System.EventHandler(this.buttonSend_Click);

            // Context
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 231);
            this.Controls.Add(this.labelContexts);
            this.Controls.Add(this.comboBoxContexts);
            this.Controls.Add(this.labelValue);
            this.Controls.Add(this.textBoxValue);
            this.Controls.Add(this.buttonSend);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Context";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Context";
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label labelContexts;
        private System.Windows.Forms.ComboBox comboBoxContexts;
        private System.Windows.Forms.Label labelValue;
        private Nikse.SubtitleEdit.Controls.NikseTextBox textBoxValue;
        private System.Windows.Forms.Button buttonSend;
    }
} 