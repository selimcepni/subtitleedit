using System.Windows.Forms;
namespace Nikse.SubtitleEdit.Forms

{
    partial class Glossary
    {
        private System.ComponentModel.IContainer components = null;

        #region Windows Form Designer generated code
    
        private void InitializeComponent()
        {
            this.labelTerm = new System.Windows.Forms.Label();
            this.textBoxTerm = new Nikse.SubtitleEdit.Controls.NikseTextBox();
            this.labelTargetLanguage = new System.Windows.Forms.Label();
            this.comboBoxTargetLanguage = new System.Windows.Forms.ComboBox();
            this.labelTranslations = new System.Windows.Forms.Label();
            this.textBoxTranslations = new Nikse.SubtitleEdit.Controls.NikseTextBox();
            this.labelProject = new System.Windows.Forms.Label();
            this.comboBoxProject = new System.Windows.Forms.ComboBox();
            this.labelUser = new System.Windows.Forms.Label();
            this.comboBoxUser = new System.Windows.Forms.ComboBox();
            this.buttonSend = new System.Windows.Forms.Button();
            this.SuspendLayout();

            // labelTerm
            this.labelTerm.AutoSize = true;
            this.labelTerm.Location = new System.Drawing.Point(12, 15);
            this.labelTerm.Name = "labelTerm";
            this.labelTerm.Size = new System.Drawing.Size(31, 13);
            this.labelTerm.Text = "Term";

            // textBoxTerm
            this.textBoxTerm.Location = new System.Drawing.Point(15, 31);
            this.textBoxTerm.Name = "textBoxTerm";
            this.textBoxTerm.Size = new System.Drawing.Size(200, 20);

            // labelTargetLanguage
            this.labelTargetLanguage.AutoSize = true;
            this.labelTargetLanguage.Location = new System.Drawing.Point(12, 65);
            this.labelTargetLanguage.Name = "labelTargetLanguage";
            this.labelTargetLanguage.Size = new System.Drawing.Size(89, 13);
            this.labelTargetLanguage.Text = "Target Language";

            // comboBoxTargetLanguage
            this.comboBoxTargetLanguage = new System.Windows.Forms.ComboBox();
            this.comboBoxTargetLanguage.FormattingEnabled = true;
            this.comboBoxTargetLanguage.Location = new System.Drawing.Point(15, 81);
            this.comboBoxTargetLanguage.Name = "comboBoxTargetLanguage";
            this.comboBoxTargetLanguage.Size = new System.Drawing.Size(200, 21);
            this.comboBoxTargetLanguage.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;

            // labelTranslations
            this.labelTranslations.AutoSize = true;
            this.labelTranslations.Location = new System.Drawing.Point(12, 115);
            this.labelTranslations.Name = "labelTranslations";
            this.labelTranslations.Size = new System.Drawing.Size(63, 13);
            this.labelTranslations.Text = "Translations";

            // textBoxTranslations
            this.textBoxTranslations.Location = new System.Drawing.Point(15, 131);
            this.textBoxTranslations.Name = "textBoxTranslations";
            this.textBoxTranslations.Size = new System.Drawing.Size(200, 20);

            // labelProject
            this.labelProject.AutoSize = true;
            this.labelProject.Location = new System.Drawing.Point(12, 165);
            this.labelProject.Name = "labelProject";
            this.labelProject.Size = new System.Drawing.Size(40, 13);
            this.labelProject.Text = "Project";

            // comboBoxProject
            this.comboBoxProject.FormattingEnabled = true;
            this.comboBoxProject.Location = new System.Drawing.Point(15, 181);
            this.comboBoxProject.Name = "comboBoxProject";
            this.comboBoxProject.Size = new System.Drawing.Size(200, 21);
            this.comboBoxProject.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDown;
            this.comboBoxProject.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            this.comboBoxProject.AutoCompleteSource = AutoCompleteSource.ListItems;

            // labelUser
            this.labelUser.AutoSize = true;
            this.labelUser.Location = new System.Drawing.Point(12, 215);
            this.labelUser.Name = "labelUser";
            this.labelUser.Size = new System.Drawing.Size(29, 13);
            this.labelUser.Text = "User";

            // comboBoxUser
            this.comboBoxUser.FormattingEnabled = true;
            this.comboBoxUser.Location = new System.Drawing.Point(15, 231);
            this.comboBoxUser.Name = "comboBoxUser";
            this.comboBoxUser.Size = new System.Drawing.Size(200, 21);
            this.comboBoxUser.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDown;
            this.comboBoxUser.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            this.comboBoxUser.AutoCompleteSource = AutoCompleteSource.ListItems;

            // buttonSend
            this.buttonSend.Location = new System.Drawing.Point(230, 130);
            this.buttonSend.Name = "buttonSend";
            this.buttonSend.Size = new System.Drawing.Size(75, 23);
            this.buttonSend.Text = "Send";
            this.buttonSend.Click += new System.EventHandler(this.buttonSend_Click);

            // Glossary
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(320, 270);
            this.Controls.Add(this.labelTerm);
            this.Controls.Add(this.textBoxTerm);
            this.Controls.Add(this.labelTargetLanguage);
            this.Controls.Add(this.comboBoxTargetLanguage);
            this.Controls.Add(this.labelTranslations);
            this.Controls.Add(this.textBoxTranslations);
            this.Controls.Add(this.labelProject);
            this.Controls.Add(this.comboBoxProject);
            this.Controls.Add(this.labelUser);
            this.Controls.Add(this.comboBoxUser);
            this.Controls.Add(this.buttonSend);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Glossary";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Glossary";
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label labelTerm;
        private Nikse.SubtitleEdit.Controls.NikseTextBox textBoxTerm;
        private System.Windows.Forms.Label labelTargetLanguage;
        private System.Windows.Forms.ComboBox comboBoxTargetLanguage;
        private System.Windows.Forms.Label labelTranslations;
        private Nikse.SubtitleEdit.Controls.NikseTextBox textBoxTranslations;
        private System.Windows.Forms.Label labelProject;
        private System.Windows.Forms.ComboBox comboBoxProject;
        private System.Windows.Forms.Label labelUser;
        private System.Windows.Forms.ComboBox comboBoxUser;
        private System.Windows.Forms.Button buttonSend;
    }
}