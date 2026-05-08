using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using LangSpace.Service;
using LangSpace.Models;

namespace LangSpace.Views
{
    public partial class LevelTestWindow : Window
    {
        private LevelTestService testService;
        private List<TestQuestion> questions;
        private int currentQuestionIndex;
        private int score;
        private bool testCompleted;
        public LevelTestWindow()
        {
            InitializeComponent();
            testService = new LevelTestService();
            LoadTest();
        }
        private void LoadTest()
        {
            questions = testService.GetRandomQuestions();
            currentQuestionIndex = 0;
            score = 0;
            testCompleted = false;
            ShowQuestion();
        }
        private void ShowQuestion()
        {
            if (currentQuestionIndex >= questions.Count)
            {
                FinishTest();
                return;
            }
            var question = questions[currentQuestionIndex];
            txtProgress.Text = $"Вопрос {currentQuestionIndex + 1} из {questions.Count}";
            txtQuestion.Text = question.QuestionText;
            txtOptionA.Text = $"A) {question.OptionA}";
            txtOptionB.Text = $"B) {question.OptionB}";
            txtOptionC.Text = $"C) {question.OptionC}";
            txtOptionD.Text = $"D) {question.OptionD}";

            rbA.IsChecked = false;
            rbB.IsChecked = false;
            rbC.IsChecked = false;
            rbD.IsChecked = false;
            if (currentQuestionIndex == questions.Count - 1)
            {
                btnNext.Content = "Завершить";
            }
            else
            {
                btnNext.Content = "Далее";
            }
        }
        private void btnNext_Click(object sender, EventArgs e)
        {
            char? selectedOption = null;
            if (rbA.IsChecked == true) selectedOption = 'A';
            else if (rbB.IsChecked == true) selectedOption = 'B';
            else if (rbC.IsChecked == true) selectedOption = 'C';
            else if (rbD.IsChecked == true) selectedOption = 'D';
            if (selectedOption == null)
            {
                MessageBox.Show("Пожалуйста, выберите вариант ответа.");
                return;
            }
            var currentQuestion = questions[currentQuestionIndex];
            if (selectedOption == currentQuestion.CorrectOption)
            {
                score++;
            }
            currentQuestionIndex++;
            if (currentQuestionIndex < questions.Count)
            {
                ShowQuestion();
            }
            else
            {
                FinishTest();
            }
        }
        private void FinishTest()
        {
            if (testCompleted) return;
            testCompleted = true;
            string level = testService.GetLevelByScore(score);
            if (LoginWindow.currentUser != null)
            {
                testService.SaveUserLevel(LoginWindow.currentUser.IdUser, level);
            }
            MessageBox.Show($"Тест завершен!\n\nВаш результат: {score} из {questions.Count}\nОпределенный уровень: {level}");
            
            this.Close();
        }
        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Вы уверены, что хотите выйти? Ваш прогресс не будет сохранен.", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                
                this.Close();
            }
        }
    }
}
