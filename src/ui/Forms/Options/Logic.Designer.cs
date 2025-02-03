using System.Windows.Forms;
namespace Nikse.SubtitleEdit.Forms
{
    partial class LogicForm
    {
        private System.ComponentModel.IContainer components = null;

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.labelRules = new System.Windows.Forms.Label();
            this.labelLanguage = new System.Windows.Forms.Label();
            this.comboBoxLanguages = new System.Windows.Forms.ComboBox();
            this.textBoxRules = new Nikse.SubtitleEdit.Controls.NikseTextBox();
            this.labelNewRule = new System.Windows.Forms.Label();
            this.textBoxNewRule = new Nikse.SubtitleEdit.Controls.NikseTextBox();
            this.buttonAddRule = new System.Windows.Forms.Button();
            this.SuspendLayout();

            // labelRules
            this.labelRules.AutoSize = true;
            this.labelRules.Location = new System.Drawing.Point(12, 15);
            this.labelRules.Name = "labelRules";
            this.labelRules.Size = new System.Drawing.Size(34, 13);
            this.labelRules.Text = "Rules";

            // labelLanguage
            this.labelLanguage.AutoSize = true;
            this.labelLanguage.Location = new System.Drawing.Point(12, 40);
            this.labelLanguage.Name = "labelLanguage";
            this.labelLanguage.Size = new System.Drawing.Size(55, 13);
            this.labelLanguage.Text = "Language";

            // comboBoxLanguages
            this.comboBoxLanguages.FormattingEnabled = true;
            this.comboBoxLanguages.Location = new System.Drawing.Point(15, 56);
            this.comboBoxLanguages.Name = "comboBoxLanguages";
            this.comboBoxLanguages.Size = new System.Drawing.Size(250, 21);
            this.comboBoxLanguages.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDown;

            // textBoxRules
            this.textBoxRules.Location = new System.Drawing.Point(15, 90);
            this.textBoxRules.Name = "textBoxRules";
            this.textBoxRules.Size = new System.Drawing.Size(250, 100);
            this.textBoxRules.Multiline = true;
            this.textBoxRules.ReadOnly = true;
            this.textBoxRules.ScrollBars = ScrollBars.Vertical;

            // labelNewRule
            this.labelNewRule.AutoSize = true;
            this.labelNewRule.Location = new System.Drawing.Point(12, 200);
            this.labelNewRule.Name = "labelNewRule";
            this.labelNewRule.Size = new System.Drawing.Size(54, 13);
            this.labelNewRule.Text = "New Rule";

            // textBoxNewRule
            this.textBoxNewRule.Location = new System.Drawing.Point(15, 216);
            this.textBoxNewRule.Name = "textBoxNewRule";
            this.textBoxNewRule.Size = new System.Drawing.Size(250, 50);
            this.textBoxNewRule.Multiline = true;
            this.textBoxNewRule.ScrollBars = ScrollBars.Vertical;

            // buttonAddRule
            this.buttonAddRule.Location = new System.Drawing.Point(190, 275);
            this.buttonAddRule.Name = "buttonAddRule";
            this.buttonAddRule.Size = new System.Drawing.Size(75, 23);
            this.buttonAddRule.Text = "Add Rule";
            this.buttonAddRule.Click += new System.EventHandler(this.buttonAddRule_Click);

            // LogicForm
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 315);
            this.Controls.Add(this.labelRules);
            this.Controls.Add(this.labelLanguage);
            this.Controls.Add(this.comboBoxLanguages);
            this.Controls.Add(this.textBoxRules);
            this.Controls.Add(this.labelNewRule);
            this.Controls.Add(this.textBoxNewRule);
            this.Controls.Add(this.buttonAddRule);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "LogicForm";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Logic";
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label labelRules;
        private System.Windows.Forms.Label labelLanguage;
        private System.Windows.Forms.ComboBox comboBoxLanguages;
        private Nikse.SubtitleEdit.Controls.NikseTextBox textBoxRules;
        private System.Windows.Forms.Label labelNewRule;
        private Nikse.SubtitleEdit.Controls.NikseTextBox textBoxNewRule;
        private System.Windows.Forms.Button buttonAddRule;
    }
} 