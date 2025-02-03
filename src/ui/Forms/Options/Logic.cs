using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using System.Text;
using Nikse.SubtitleEdit.Logic;

namespace Nikse.SubtitleEdit.Forms
{
    public sealed partial class LogicForm : Form
    {
        private class LanguageItem
        {
            public string Name { get; set; }
            public override string ToString() => Name;
        }

        private class RuleResponse
        {
            public string Rules { get; set; }
        }

        private readonly HttpClient _httpClient;
        private List<LanguageItem> _languages;
        private const string RULES_API_URL = "https://us-central1-peaceful-branch-448020-i9.cloudfunctions.net/tabiirulesvalue";
        private const string ADD_RULE_API_URL = "https://us-central1-peaceful-branch-448020-i9.cloudfunctions.net/tabiiruleadd";

        public LogicForm()
        {
            UiUtil.PreInitialize(this);
            InitializeComponent();
            UiUtil.FixFonts(this);

            _httpClient = new HttpClient();

            comboBoxLanguages.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            comboBoxLanguages.AutoCompleteSource = AutoCompleteSource.ListItems;
            comboBoxLanguages.SelectedIndexChanged += ComboBoxLanguages_SelectedIndexChanged;

            LoadLanguages();
        }

        private void LoadLanguages()
        {
            _languages = new List<LanguageItem>
            {
                new LanguageItem { Name = "English" },
                new LanguageItem { Name = "Turkish" },
                new LanguageItem { Name = "Spanish" },
                new LanguageItem { Name = "Arabic" },
                new LanguageItem { Name = "Urdu" },
            };

            comboBoxLanguages.BeginUpdate();
            comboBoxLanguages.Items.Clear();
            comboBoxLanguages.Items.AddRange(_languages.ToArray());
            comboBoxLanguages.EndUpdate();
        }

        private async void ComboBoxLanguages_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxLanguages.SelectedItem is LanguageItem selectedLanguage)
            {
                await LoadRulesAsync(selectedLanguage.Name);
            }
        }

        private async Task LoadRulesAsync(string language)
        {
            try
            {
                buttonAddRule.Enabled = false;
                textBoxRules.Enabled = false;
                Cursor = Cursors.WaitCursor;

                var requestData = new { language };
                var jsonContent = JsonConvert.SerializeObject(requestData);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(RULES_API_URL, content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var ruleResponse = JsonConvert.DeserializeObject<RuleResponse>(responseContent);
                    textBoxRules.Text = ruleResponse.Rules;
                }
                else
                {
                    MessageBox.Show($"Error loading rules: {responseContent}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading rules: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                buttonAddRule.Enabled = true;
                textBoxRules.Enabled = true;
                Cursor = Cursors.Default;
            }
        }

        private async void buttonAddRule_Click(object sender, EventArgs e)
        {
            if (comboBoxLanguages.SelectedItem == null)
            {
                MessageBox.Show("Please select a language.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                comboBoxLanguages.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(textBoxNewRule.Text))
            {
                MessageBox.Show("Please enter a new rule.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBoxNewRule.Focus();
                return;
            }

            try
            {
                buttonAddRule.Enabled = false;
                Cursor = Cursors.WaitCursor;

                var requestData = new
                {
                    language = ((LanguageItem)comboBoxLanguages.SelectedItem).Name,
                    rule = textBoxNewRule.Text.Trim()
                };

                var jsonContent = JsonConvert.SerializeObject(requestData);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(ADD_RULE_API_URL, content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Rule added successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    textBoxNewRule.Clear();
                    await LoadRulesAsync(((LanguageItem)comboBoxLanguages.SelectedItem).Name);
                }
                else
                {
                    MessageBox.Show($"Error adding rule: {responseContent}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                buttonAddRule.Enabled = true;
                Cursor = Cursors.Default;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _httpClient?.Dispose();
                components?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}