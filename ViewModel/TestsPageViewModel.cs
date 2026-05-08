using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using LangSpace.Models;
using LangSpace.Service;
using LangSpace.Views;

namespace LangSpace.ViewModel
{
    public class TestsPageViewModel : BaseViewModel
    {
        private readonly CardService cardService;
        private readonly StatisticsService statisticsService;
        private readonly int currentUserId;
        private readonly LevelTestService levelTestService;
        private string testType;
        private int grammarRuleId;

        private List<GrammarQuestion> grammarQuestions = new();
        private List<Card> cardQuestions = new();
        private int currentQuestionIndex;
        private int score;
        private bool testCompleted;
        private List<UserAnswer> userAnswers = new();

        private string title = "Тест";
        private string progressText = "";
        private string questionText = "";
        private string wordToTranslate = "";
        private string userAnswerText = "";
        private string optionA = "";
        private string optionB = "";
        private string optionC = "";
        private string optionD = "";
        private bool? rbA = false;
        private bool? rbB = false;
        private bool? rbC = false;
        private bool? rbD = false;
        private string nextButtonContent = "Далее";
        private string resultText = "";
        private string mistakesText = "";

        private Visibility selectionPanelVisible = Visibility.Collapsed;
        private Visibility progressVisible = Visibility.Collapsed;
        private Visibility grammarQuestionVisible = Visibility.Collapsed;
        private Visibility cardQuestionVisible = Visibility.Collapsed;
        private Visibility nextButtonVisible = Visibility.Collapsed;
        private Visibility cancelButtonVisible = Visibility.Visible;
        private Visibility restartButtonVisible = Visibility.Collapsed;
        private Visibility resultPanelVisible = Visibility.Collapsed;

        private List<string> themeFilters = new();
        private List<string> levelFilters = new();
        private string selectedTheme = "Все темы";
        private string selectedLevel = "Все уровни";

        private List<Theme> allThemes = new();
        private List<Level> allLevels = new();

        public class UserAnswer
        {
            public string RussianWord { get; set; } = "";
            public string CorrectEnglishWord { get; set; } = "";
            public string UserAnswerText { get; set; } = "";
        }
        public string Title
        {
            get { return title; }
            set { title = value; OnPropertyChanged("Title"); }
        }

        public string ProgressText
        {
            get { return progressText; }
            set { progressText = value; OnPropertyChanged("ProgressText"); }
        }

        public string QuestionText
        {
            get { return questionText; }
            set { questionText = value; OnPropertyChanged("QuestionText"); }
        }

        public string WordToTranslate
        {
            get { return wordToTranslate; }
            set { wordToTranslate = value; OnPropertyChanged("WordToTranslate"); }
        }

        public string UserAnswerText
        {
            get { return userAnswerText; }
            set { userAnswerText = value; OnPropertyChanged("UserAnswerText"); }
        }

        public string OptionA
        {
            get { return optionA; }
            set { optionA = value; OnPropertyChanged("OptionA"); }
        }

        public string OptionB
        {
            get { return optionB; }
            set { optionB = value; OnPropertyChanged("OptionB"); }
        }

        public string OptionC
        {
            get { return optionC; }
            set { optionC = value; OnPropertyChanged("OptionC"); }
        }

        public string OptionD
        {
            get { return optionD; }
            set { optionD = value; OnPropertyChanged("OptionD"); }
        }

        public bool? RbA
        {
            get { return rbA; }
            set { rbA = value; OnPropertyChanged("RbA"); }
        }

        public bool? RbB
        {
            get { return rbB; }
            set { rbB = value; OnPropertyChanged("RbB"); }
        }

        public bool? RbC
        {
            get { return rbC; }
            set { rbC = value; OnPropertyChanged("RbC"); }
        }

        public bool? RbD
        {
            get { return rbD; }
            set { rbD = value; OnPropertyChanged("RbD"); }
        }

        public string NextButtonContent
        {
            get { return nextButtonContent; }
            set { nextButtonContent = value; OnPropertyChanged("NextButtonContent"); }
        }

        public string ResultText
        {
            get { return resultText; }
            set { resultText = value; OnPropertyChanged("ResultText"); }
        }

        public string MistakesText
        {
            get { return mistakesText; }
            set { mistakesText = value; OnPropertyChanged("MistakesText"); }
        }

        public Visibility SelectionPanelVisible
        {
            get { return selectionPanelVisible; }
            set { selectionPanelVisible = value; OnPropertyChanged("SelectionPanelVisible"); }
        }

        public Visibility ProgressVisible
        {
            get { return progressVisible; }
            set { progressVisible = value; OnPropertyChanged("ProgressVisible"); }
        }

        public Visibility GrammarQuestionVisible
        {
            get { return grammarQuestionVisible; }
            set { grammarQuestionVisible = value; OnPropertyChanged("GrammarQuestionVisible"); }
        }

        public Visibility CardQuestionVisible
        {
            get { return cardQuestionVisible; }
            set { cardQuestionVisible = value; OnPropertyChanged("CardQuestionVisible"); }
        }

        public Visibility NextButtonVisible
        {
            get { return nextButtonVisible; }
            set { nextButtonVisible = value; OnPropertyChanged("NextButtonVisible"); }
        }

        public Visibility CancelButtonVisible
        {
            get { return cancelButtonVisible; }
            set { cancelButtonVisible = value; OnPropertyChanged("CancelButtonVisible"); }
        }

        public Visibility RestartButtonVisible
        {
            get { return restartButtonVisible; }
            set { restartButtonVisible = value; OnPropertyChanged("RestartButtonVisible"); }
        }

        public Visibility ResultPanelVisible
        {
            get { return resultPanelVisible; }
            set { resultPanelVisible = value; OnPropertyChanged("ResultPanelVisible"); }
        }

        public List<string> ThemeFilters
        {
            get { return themeFilters; }
            set { themeFilters = value; OnPropertyChanged("ThemeFilters"); }
        }

        public List<string> LevelFilters
        {
            get { return levelFilters; }
            set { levelFilters = value; OnPropertyChanged("LevelFilters"); }
        }

        public string SelectedTheme
        {
            get { return selectedTheme; }
            set { selectedTheme = value; OnPropertyChanged("SelectedTheme"); }
        }

        public string SelectedLevel
        {
            get { return selectedLevel; }
            set { selectedLevel = value; OnPropertyChanged("SelectedLevel"); }
        }
        public TestsPageViewModel()
        {
            cardService = new CardService();
            statisticsService = new StatisticsService();
            currentUserId = LoginWindow.currentUser?.IdUser ?? 0;
            levelTestService = new LevelTestService();
            testType = "cards";
            Title = "Тест по карточкам";
            ShowSelectionPanel();
        }

        public TestsPageViewModel(int grammarRuleId)
        {
            cardService = new CardService();
            statisticsService = new StatisticsService();
            levelTestService = new LevelTestService();
            currentUserId = LoginWindow.currentUser?.IdUser ?? 0;
            testType = "grammar";
            this.grammarRuleId = grammarRuleId;
            Title = "Тест по грамматике";
            SelectionPanelVisible = Visibility.Collapsed;

            LoadGrammarQuestions();
        }
        public void ShowSelectionPanel()
        {
            SelectionPanelVisible = Visibility.Visible;
            ProgressVisible = Visibility.Collapsed;
            GrammarQuestionVisible = Visibility.Collapsed;
            CardQuestionVisible = Visibility.Collapsed;
            NextButtonVisible = Visibility.Collapsed;
            RestartButtonVisible = Visibility.Collapsed;
            CancelButtonVisible = Visibility.Visible;
            ResultPanelVisible = Visibility.Collapsed;

            LoadThemeFilters();
            LoadLevelFilters();
        }
        private void LoadLevelFilters()
        {
            // Загружаем все уровни из сервиса
            allLevels = cardService.GetAllLevels();

            // Получаем уровень пользователя
            string userLevel = levelTestService.GetUserLevel(currentUserId);
            if (string.IsNullOrEmpty(userLevel) || userLevel == "Уровень не установлен")
                userLevel = "A1";

            // Получаем доступные уровни
            var availableLevelNames = CardService.GetAvailableLevelNames(userLevel);

            // Фильтруем уровни для показа в комбобоксе
            var levels = allLevels
                .Where(l => availableLevelNames.Contains(l.LevelName))
                .Select(l => l.LevelName)
                .ToList();

            levels.Insert(0, "Все уровни");
            LevelFilters = levels;
            SelectedLevel = "Все уровни";
        }
        private void LoadThemeFilters()
        {
            allThemes = cardService.GetAllThemes();
            var themes = allThemes.Select(t => t.ThemeName).ToList();
            themes.Insert(0, "Все темы");
            ThemeFilters = themes;
            SelectedTheme = "Все темы";
        }

      

        public bool CanStartCardTest()
        {
            return !string.IsNullOrEmpty(SelectedTheme) && !string.IsNullOrEmpty(SelectedLevel);
        }

        public void StartCardTest()
        {
            // Получаем все карточки пользователя (учитывая его уровень)
            var allUserCards = cardService.GetAllCardsByUserLevel(currentUserId);

            // Фильтруем по теме
            if (SelectedTheme != "Все темы")
            {
                var theme = allThemes.FirstOrDefault(t => t.ThemeName == SelectedTheme);
                if (theme != null)
                {
                    allUserCards = allUserCards.Where(c => c.IdTheme == theme.IdTheme).ToList();
                }
            }

            // Фильтруем по уровню
            if (SelectedLevel != "Все уровни")
            {
                allUserCards = allUserCards.Where(c => c.IdLevelNavigation.LevelName == SelectedLevel).ToList();
            }

            cardQuestions = allUserCards.ToList();

            if (cardQuestions.Count == 0)
            {
                MessageBox.Show("Нет карточек для выбранных параметров");
                return;
            }

            Random rnd = new Random();
            cardQuestions = cardQuestions.OrderBy(x => rnd.Next()).ToList();

            currentQuestionIndex = 0;
            score = 0;
            testCompleted = false;
            userAnswers = new List<UserAnswer>();

            SelectionPanelVisible = Visibility.Collapsed;
            ProgressVisible = Visibility.Visible;
            CardQuestionVisible = Visibility.Visible;
            NextButtonVisible = Visibility.Visible;
            CancelButtonVisible = Visibility.Visible;
            RestartButtonVisible = Visibility.Collapsed;
            ResultPanelVisible = Visibility.Collapsed;

            ShowCardQuestion();
        }

        public void LoadGrammarQuestions()
        {
            using (var context = new LangSpaceContext())
            {
                grammarQuestions = context.GrammarQuestions
                    .Where(q => q.IdGrammarRule == grammarRuleId)
                    .ToList();
            }

            if (grammarQuestions.Count == 0)
            {
                MessageBox.Show("Нет вопросов для этого правила");
                return;
            }

            currentQuestionIndex = 0;
            score = 0;
            testCompleted = false;

            ProgressVisible = Visibility.Visible;
            GrammarQuestionVisible = Visibility.Visible;
            NextButtonVisible = Visibility.Visible;
            CancelButtonVisible = Visibility.Visible;

            ShowGrammarQuestion();
        }

        public void ShowCardQuestion()
        {
            if (currentQuestionIndex >= cardQuestions.Count)
            {
                FinishTest();
                return;
            }

            var card = cardQuestions[currentQuestionIndex];
            ProgressText = $"Вопрос {currentQuestionIndex + 1} из {cardQuestions.Count}";
            WordToTranslate = card.BackText;
            UserAnswerText = "";
            NextButtonContent = currentQuestionIndex == cardQuestions.Count - 1 ? "Завершить" : "Далее";
        }

        public void ShowGrammarQuestion()
        {
            if (currentQuestionIndex >= grammarQuestions.Count)
            {
                FinishTest();
                return;
            }

            var question = grammarQuestions[currentQuestionIndex];
            ProgressText = $"Вопрос {currentQuestionIndex + 1} из {grammarQuestions.Count}";
            QuestionText = question.QuestionText;
            OptionA = $"A) {question.Option1}";
            OptionB = $"B) {question.Option2}";
            OptionC = $"C) {question.Option3}";
            OptionD = $"D) {question.Option4}";

            RbA = RbB = RbC = RbD = false;
            NextButtonContent = currentQuestionIndex == grammarQuestions.Count - 1 ? "Завершить" : "Далее";
        }

        public bool CanGoNext()
        {
            if (testType == "grammar")
                return RbA == true || RbB == true || RbC == true || RbD == true;
            else
                return !string.IsNullOrWhiteSpace(UserAnswerText);
        }

        public void NextQuestion()
        {
            if (testType == "grammar")
            {
                char? selectedOption = null;
                if (RbA == true) selectedOption = 'A';
                else if (RbB == true) selectedOption = 'B';
                else if (RbC == true) selectedOption = 'C';
                else if (RbD == true) selectedOption = 'D';

                var currentQuestion = grammarQuestions[currentQuestionIndex];
                if (selectedOption.ToString() == currentQuestion.CorrectAnswer)
                {
                    score++;
                }
            }
            else
            {
                string userAnswer = UserAnswerText.Trim();
                var card = cardQuestions[currentQuestionIndex];

                if (string.Equals(userAnswer, card.FrontText, StringComparison.CurrentCultureIgnoreCase))
                {
                    score++;
                }

                userAnswers.Add(new UserAnswer
                {
                    RussianWord = card.BackText,
                    CorrectEnglishWord = card.FrontText,
                    UserAnswerText = string.IsNullOrEmpty(userAnswer) ? "(не введено)" : userAnswer
                });
            }

            currentQuestionIndex++;

            if (testType == "grammar")
            {
                if (currentQuestionIndex < grammarQuestions.Count)
                    ShowGrammarQuestion();
                else
                    FinishTest();
            }
            else
            {
                if (currentQuestionIndex < cardQuestions.Count)
                    ShowCardQuestion();
                else
                    FinishTest();
            }
        }

        public void FinishTest()
        {
            if (testCompleted) return;
            testCompleted = true;

            string testTypeStr = testType == "grammar" ? "grammar" : "card";
            int totalQuestions = testType == "grammar" ? grammarQuestions.Count : cardQuestions.Count;
            statisticsService.SaveTestResult(currentUserId, testTypeStr, score, totalQuestions);

            ProgressVisible = Visibility.Collapsed;
            GrammarQuestionVisible = Visibility.Collapsed;
            CardQuestionVisible = Visibility.Collapsed;
            NextButtonVisible = Visibility.Collapsed;
            CancelButtonVisible = Visibility.Collapsed;
            RestartButtonVisible = Visibility.Visible;
            ResultPanelVisible = Visibility.Visible;

            int total = testType == "grammar" ? grammarQuestions.Count : cardQuestions.Count;
            double percent = (double)score / total * 100;
            ResultText = $"Результат: {score} из {total} ({percent:F1}%)";

            if (testType == "cards" && userAnswers != null)
            {
                string answersText = "\nПравильные ответы:\n\n";
                foreach (var answer in userAnswers)
                {
                    answersText += $"{answer.RussianWord} - {answer.CorrectEnglishWord}\n";
                    if (!answer.UserAnswerText.Equals(answer.CorrectEnglishWord, StringComparison.CurrentCultureIgnoreCase))
                    {
                        answersText += $"   Ваш ответ: {answer.UserAnswerText}\n";
                    }
                    answersText += "\n";
                }
                MistakesText = answersText;
            }
            else
            {
                if (score == total)
                    MistakesText = "Отлично! Все ответы правильные!";
                else
                    MistakesText = $"Вы ответили правильно на {score} из {total} вопросов.";
            }
        }

        public void RestartTest()
        {
            if (testType == "grammar")
            {
                currentQuestionIndex = 0;
                score = 0;
                testCompleted = false;

                ResultPanelVisible = Visibility.Collapsed;
                RestartButtonVisible = Visibility.Collapsed;
                CancelButtonVisible = Visibility.Visible;
                GrammarQuestionVisible = Visibility.Visible;
                ProgressVisible = Visibility.Visible;
                NextButtonVisible = Visibility.Visible;

                ShowGrammarQuestion();
            }
            else
            {
                ResultPanelVisible = Visibility.Collapsed;
                RestartButtonVisible = Visibility.Collapsed;
                ShowSelectionPanel();
            }
        }

        public bool HasActiveTest()
        {
            return (testType == "grammar" && currentQuestionIndex > 0 && !testCompleted) ||
                   (testType == "cards" && currentQuestionIndex > 0 && !testCompleted);
        }
    }
}