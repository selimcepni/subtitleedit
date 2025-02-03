using Nikse.SubtitleEdit.Core.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using System.Text;
using Nikse.SubtitleEdit.Logic;

namespace Nikse.SubtitleEdit.Forms
{
    public sealed partial class Context : Form
    {
        private class ContextItem
        {
            public string Name { get; set; }
            public override string ToString() => Name;
        }

        private class ContextValue
        {
            public string Context { get; set; }
        }

        private readonly HttpClient _httpClient;
        private List<ContextItem> _contexts;
        private const string CONTEXTS_API_URL = "https://us-central1-peaceful-branch-448020-i9.cloudfunctions.net/tabiiprojectlist";
        private const string CONTEXT_VALUE_API_URL = "https://us-central1-peaceful-branch-448020-i9.cloudfunctions.net/tabiicontextvalue";
        private const string SAVE_CONTEXT_API_URL = "https://us-central1-peaceful-branch-448020-i9.cloudfunctions.net/tabiicontextsave";

        public Context()
        {
            UiUtil.PreInitialize(this);
            InitializeComponent();
            UiUtil.FixFonts(this);

            _httpClient = new HttpClient();
            
            comboBoxContexts.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            comboBoxContexts.AutoCompleteSource = AutoCompleteSource.ListItems;
            comboBoxContexts.SelectedIndexChanged += ComboBoxContexts_SelectedIndexChanged;
            
            _ = LoadContextsAsync();
        }

        private async void ComboBoxContexts_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxContexts.SelectedItem is ContextItem selectedContext)
            {
                await LoadContextValueAsync(selectedContext.Name);
            }
        }

        private async Task LoadContextsAsync()
        {
            try
            {
                var response = await _httpClient.GetStringAsync(CONTEXTS_API_URL);
                _contexts = JsonConvert.DeserializeObject<List<ContextItem>>(response);

                comboBoxContexts.BeginUpdate();
                comboBoxContexts.Items.Clear();
                comboBoxContexts.Items.AddRange(_contexts.ToArray());
                comboBoxContexts.EndUpdate();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading projects: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                _contexts = new List<ContextItem>();
            }
        }

        private async Task LoadContextValueAsync(string projectName)
        {
            try
            {
                buttonSend.Enabled = false;
                textBoxValue.Enabled = false;
                Cursor = Cursors.WaitCursor;

                var requestData = new { projectName };
                var jsonContent = JsonConvert.SerializeObject(requestData);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(CONTEXT_VALUE_API_URL, content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var contextValue = JsonConvert.DeserializeObject<ContextValue>(responseContent);
                    textBoxValue.Text = contextValue.Context;
                }
                else
                {
                    MessageBox.Show($"Error loading context: {responseContent}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading context: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                buttonSend.Enabled = true;
                textBoxValue.Enabled = true;
                Cursor = Cursors.Default;
            }
        }

        private async void buttonSend_Click(object sender, EventArgs e)
        {
            if (comboBoxContexts.SelectedItem == null)
            {
                MessageBox.Show("Please select a project.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                comboBoxContexts.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(textBoxValue.Text))
            {
                MessageBox.Show("Please enter a context.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBoxValue.Focus();
                return;
            }

            try
            {
                buttonSend.Enabled = false;
                Cursor = Cursors.WaitCursor;

                var requestData = new
                {
                    key = ((ContextItem)comboBoxContexts.SelectedItem).Name,
                    context = textBoxValue.Text.Trim()
                };

                var jsonContent = JsonConvert.SerializeObject(requestData);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(SAVE_CONTEXT_API_URL, content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Context saved successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    DialogResult = DialogResult.OK;
                    Close();
                }
                else
                {
                    MessageBox.Show($"Error saving context: {responseContent}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                buttonSend.Enabled = true;
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