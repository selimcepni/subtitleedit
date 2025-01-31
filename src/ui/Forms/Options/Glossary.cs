using Nikse.SubtitleEdit.Core.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using Nikse.SubtitleEdit.Logic;
using System.Drawing;
using System.Text;

namespace Nikse.SubtitleEdit.Forms
{
    public sealed partial class Glossary : Form
    {
        private class User
        {
            public string Name { get; set; }
            public override string ToString() => Name;
        }

        private class Project
        {
            public string Name { get; set; }
            public override string ToString() => Name;
        }

        private class Term
        {
            public string Name { get; set; }
            public override string ToString() => Name;
        }

        private List<User> _users;
        private List<Project> _projects;
        private List<Term> _terms;
        private readonly HttpClient _httpClient;
        private const string USER_API_URL = "https://us-central1-peaceful-branch-448020-i9.cloudfunctions.net/tabiiuserlist";
        private const string PROJECT_API_URL = "https://us-central1-peaceful-branch-448020-i9.cloudfunctions.net/tabiiprojectlist";
        private const string TERM_API_URL = "https://us-central1-peaceful-branch-448020-i9.cloudfunctions.net/tabiitermlist";
        private ListBox _termSuggestionList;

        public Glossary()
        {
            UiUtil.PreInitialize(this);
            InitializeComponent();
            UiUtil.FixFonts(this);

            InitializeTermSuggestionList();

            _httpClient = new HttpClient();
            LoadLanguages();
            _ = LoadTermsAsync();
            _ = LoadUsersAsync();
            _ = LoadProjectsAsync();

            textBoxTerm.TextChanged += TextBoxTerm_TextChanged;
            Text = "Glossary";
        }

        private void InitializeTermSuggestionList()
        {
            _termSuggestionList = new ListBox
            {
                Visible = false,
                Location = new Point(textBoxTerm.Left, textBoxTerm.Bottom + 1),
                Width = textBoxTerm.Width,
                Height = 120,
                BorderStyle = BorderStyle.FixedSingle,
                Font = textBoxTerm.Font,
                BackColor = SystemColors.Window,
                ForeColor = SystemColors.WindowText,
                SelectionMode = SelectionMode.One
            };

            _termSuggestionList.MouseMove += (s, e) =>
            {
                int index = _termSuggestionList.IndexFromPoint(e.Location);
                if (index != ListBox.NoMatches)
                {
                    _termSuggestionList.SelectedIndex = index;
                }
            };

            Controls.Add(_termSuggestionList);
            _termSuggestionList.BringToFront();
            _termSuggestionList.Click += TermSuggestionList_Click;
        }

        private void TextBoxTerm_TextChanged(object sender, EventArgs e)
        {
            var searchText = textBoxTerm.Text.Trim().ToLower();
            if (string.IsNullOrEmpty(searchText) || _terms == null)
            {
                _termSuggestionList.Visible = false;
                return;
            }

            var suggestions = _terms
                .Where(t => t.Name.ToLower().Contains(searchText))
                .Take(5)
                .Select(t => t.Name)
                .ToArray();

            _termSuggestionList.BeginUpdate();
            _termSuggestionList.Items.Clear();
            _termSuggestionList.Items.AddRange(suggestions);
            _termSuggestionList.EndUpdate();
            _termSuggestionList.Visible = suggestions.Length > 0;
            _termSuggestionList.Height = Math.Min(120, suggestions.Length * 20 + 4);

            _termSuggestionList.Location = new Point(
                textBoxTerm.Left,
                textBoxTerm.Bottom + 1);
            _termSuggestionList.BringToFront();
        }

        private void TermSuggestionList_Click(object sender, EventArgs e)
        {
            if (_termSuggestionList.SelectedItem != null)
            {
                textBoxTerm.Text = _termSuggestionList.SelectedItem.ToString();
                _termSuggestionList.Visible = false;
                textBoxTerm.Focus();
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            _termSuggestionList?.Dispose();
            base.OnFormClosing(e);
        }

        private void LoadLanguages()
        {
            var languages = new[]
            {              
                new { Name = "English", Code = "en" },
                new { Name = "Spanish", Code = "es" },
                new { Name = "Urdu", Code = "ur" },
                new { Name = "Arabic", Code = "ar" }
            };

            comboBoxTargetLanguage.BeginUpdate();
            comboBoxTargetLanguage.Items.Clear();
            comboBoxTargetLanguage.Items.AddRange(languages.Select(l => l.Name).ToArray());
            comboBoxTargetLanguage.DropDownStyle = ComboBoxStyle.DropDown;
            comboBoxTargetLanguage.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            comboBoxTargetLanguage.AutoCompleteSource = AutoCompleteSource.ListItems;
            if (comboBoxTargetLanguage.Items.Count > 0)
            {
                comboBoxTargetLanguage.SelectedIndex = 0;
            }
            comboBoxTargetLanguage.EndUpdate();
        }

        private async Task LoadUsersAsync()
        {
            try
            {
                var response = await _httpClient.GetStringAsync(USER_API_URL);
                _users = JsonConvert.DeserializeObject<List<User>>(response);

                comboBoxUser.BeginUpdate();
                comboBoxUser.Items.Clear();
                comboBoxUser.Items.AddRange(_users.ToArray());
                comboBoxUser.EndUpdate();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading users: {ex.Message}");
                _users = new List<User>();
            }
        }

        private async Task LoadProjectsAsync()
        {
            try
            {
                var response = await _httpClient.GetStringAsync(PROJECT_API_URL);
                _projects = JsonConvert.DeserializeObject<List<Project>>(response);

                comboBoxProject.BeginUpdate();
                comboBoxProject.Items.Clear();
                comboBoxProject.Items.AddRange(_projects.ToArray());
                comboBoxProject.EndUpdate();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading projects: {ex.Message}");
                _projects = new List<Project>();
            }
        }

        private async Task LoadTermsAsync()
        {
            try
            {
                var response = await _httpClient.GetStringAsync(TERM_API_URL);
                _terms = JsonConvert.DeserializeObject<List<Term>>(response);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading terms: {ex.Message}");
                _terms = new List<Term>();
            }
        }

        private void buttonSend_Click(object sender, EventArgs e)
        {
            // Send functionality will be implemented later
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