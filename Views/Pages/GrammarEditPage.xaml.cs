using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using LangSpace.Models;
using LangSpace.Service;

namespace LangSpace.Views.Pages
{
    public partial class GrammarEditPage : Page
    {
        private GrammarService _grammarService;
        private GrammarRule _editingRule;
        private bool _isEditMode;

        public GrammarEditPage()
        {
            InitializeComponent();
            _grammarService = new GrammarService();
            _isEditMode = false;
            tbTitle.Text = "Добавить правило";
            LoadLevels();
        }

        public GrammarEditPage(GrammarRule rule)
        {
            InitializeComponent();
            _grammarService = new GrammarService();
            _editingRule = rule;
            _isEditMode = true;
            tbTitle.Text = "Редактировать правило";
            LoadLevels();
            LoadRuleData();
        }

        private void LoadLevels()
        {
            var levels = _grammarService.GetAllLevels();
            cmbLevel.ItemsSource = levels;
            if (_editingRule != null)
            {
                cmbLevel.SelectedValue = _editingRule.IdLevel;
            }
        }

        private void LoadRuleData()
        {
            if (_editingRule == null) return;
            tbRuleName.Text = _editingRule.RuleName;
            tbRuleText.Text = _editingRule.RuleText;
            cmbLevel.SelectedValue = _editingRule.IdLevel;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(tbRuleName.Text))
            {
                MessageBox.Show("Введите название правила");
                return;
            }

            if (string.IsNullOrWhiteSpace(tbRuleText.Text))
            {
                MessageBox.Show("Введите текст правила");
                return;
            }

            if (cmbLevel.SelectedItem == null)
            {
                MessageBox.Show("Выберите уровень");
                return;
            }

            if (_isEditMode)
            {
                _editingRule.RuleName = tbRuleName.Text;
                _editingRule.RuleText = tbRuleText.Text;
                _editingRule.IdLevel = (int)cmbLevel.SelectedValue;

                if (_grammarService.UpdateRule(_editingRule))
                {
                    MessageBox.Show("Правило обновлено");
                    NavigationService.GoBack();
                }
            }
            else
            {
                var newRule = new GrammarRule
                {
                    RuleName = tbRuleName.Text,
                    RuleText = tbRuleText.Text,
                    IdLevel = (int)cmbLevel.SelectedValue,
                    IdLanguage = 1
                };

                if (_grammarService.CreateRule(newRule))
                {
                    MessageBox.Show("Правило добавлено");
                    NavigationService.GoBack();
                }
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void BackButton_Click(object sender, MouseButtonEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}