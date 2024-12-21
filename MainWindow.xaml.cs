using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace lr6_game  
{
    public partial class MainWindow : Window
    {
        private Random random = new Random();
        private int[] dice = new int[5];
        private const int maxAttempts = 2;
        private int attemptsLeft = maxAttempts; 
        private int currentRound = 1;
        private const int maxRounds = 10;
        private string currentPlayer;
        private List<string> players = ["Користувач", "Комп'ютер"];
        private ScoreTable scoreTable = new ScoreTable();
        private string selectedCombination;
        

        public MainWindow()
        {
            InitializeComponent();
            scoreTable.Initialize();
            InitializeGame();
            InitializeScoreTable();
        }

        private void InitializeGame()
        {
            scoreTable.Clear();
            currentPlayer = players[0];
            currentRound = 1;
            Title = $"Генерал. Раунд {currentRound}";
            attemptsLeft = maxAttempts;
            InitializeDice();
            UpdateScoreTable();
        }

        private void InitializeDice()
        {
            DiceArea.Items.Clear();
            for (int i = 0; i < dice.Length; i++)
            {
                int num = random.Next(1,7);
                dice[i] = num;
                Button diceButton = new Button
                {
                    Content = num.ToString(),
                    FontSize = 24,
                    Width = 100,
                    Height = 100,
                    Margin = new Thickness(5, 0, 5, 0),
                    Tag = i,
                    Background = Brushes.LightGray
                };
                diceButton.Click += DiceButton_Click;
                DiceArea.Items.Add(diceButton);
            }
            UpdateCombinationSelector();
        }
        
        private void InitializeScoreTable()
        {
            scoreTable.Initialize();
            ScoreGrid.ItemsSource = scoreTable.Rows;
        }

        private void DiceButton_Click(object sender, RoutedEventArgs e)
        {
            Button clickedButton = sender as Button;
            if (clickedButton != null)
            {
                clickedButton.Background = clickedButton.Background == Brushes.LightGray ? Brushes.White : Brushes.LightGray;
            }

            foreach (Button button in DiceArea.Items)
            {
                if (button.Background == Brushes.White)
                {
                    RollDiceButton.IsEnabled = true;
                    return;
                }
            }

            RollDiceButton.IsEnabled = false;
        }

        private void RollDiceButton_Click(object sender, RoutedEventArgs e)
        {
            if (attemptsLeft <= 0)
            {
                MessageBox.Show("Ви вичерпали всі спроби на цей раунд.");
                return;
            }

            RollDice();
            attemptsLeft--;
            UpdateCombinationSelector();
        }
        
        private void RollDice()
        {
            for (int i = 0; i < dice.Length; i++)
            {
                Button diceButton = DiceArea.Items[i] as Button;
                if (diceButton != null && diceButton.Background == Brushes.White)
                {
                    dice[i] = random.Next(1, 7);
                    diceButton.Content = dice[i].ToString();
                }
            }
        }

        
        private void UpdateCombinationSelector()
        {
            var availableCombinations = GetAvailableCombinations();

            CombinationSelector.Items.Clear();

            foreach (var combination in availableCombinations)
            {
                CombinationSelector.Items.Add(new ComboBoxItem { Content = combination });
            }

            if (availableCombinations.Count > 0)
            {
                CombinationSelector.SelectedIndex = 0;
            }
        }
        
        private void EndTurnButton_Click(object sender, RoutedEventArgs e)
        {
            if (SetScore() == 65)
            {
                MessageBox.Show($"Гра завершена великим генералом гравцем {currentPlayer}!");
                InitializeGame();
                return;
            }
            currentPlayer = GetNextPlayer();
            if (currentPlayer == players[0])
            {
                currentRound++;
                Title = $"Генерал. Раунд {currentRound}";
            }

            if (currentRound > maxRounds)
            {
                EndGame();
                return;
            }
            
            Title = $"Генерал. Раунд {currentRound}";

            attemptsLeft = maxAttempts;
            InitializeDice();
            UpdateScoreTable();

            if (currentPlayer == "Комп'ютер")
            {
                ComputerTurn();
            }
        }

        private int SetScore()
        {
            var score = CalculateScore(selectedCombination);

            if (score == 0)
            {
                return score;
            }

            if (score == 65)
            {
                return score;
            }
            
            switch (currentPlayer)
            {
                case "Користувач":
                    scoreTable.Rows.Where(x => x.Combination == selectedCombination).ToArray()[0].PlayerScore = score;
                    break;
                case "Комп'ютер":
                    scoreTable.Rows.Where(x => x.Combination == selectedCombination).ToArray()[0].ComputerScore = score;
                    break;
            }

            return score;
        }

        private int CalculateScore(string combination)
        {
            int score = 0;
            switch (combination)
            {
                case "Одиниці":
                    score += 1 * dice.Count(x => x == 1);
                    break;
                case "Двійки":
                    score += 2 * dice.Count(x => x == 2);
                    break;
                case "Трійки":
                    score += 3 * dice.Count(x => x == 3);
                    break;
                case "Четвірки":
                    score += 4 * dice.Count(x => x == 4);
                    break;
                case "П'ятірки":
                    score += 5 * dice.Count(x => x == 5);
                    break;
                case "Шестірки":
                    score += 6 * dice.Count(x => x == 6);
                    break;
                case "Стріт":
                    score += 20;
                    break;
                case "Фул Хаус":
                    score += 30;
                    break;
                case "Чотири":
                    score += 40;
                    break;
                case "Генерал":
                    score += 60;
                    break;
            }

            if (attemptsLeft >= 2 && combination is "Стріт" or "Фул Хаус" or "Чотири" or "Генерал")
            {
                score += 5;
            }

            return score;
        }
        
        private void CombinationSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem selectedItem = CombinationSelector.SelectedItem as ComboBoxItem;
            if (selectedItem != null)
            {
                selectedCombination = selectedItem.Content.ToString();
            }
        }

        private List<string> GetAvailableCombinations()
        {
            var availableCombinations = new List<string>();

            switch (currentPlayer)
            {
                case "Користувач":
                    if (dice.Contains(1)
                        && scoreTable.Rows.Where(x => x.Combination == "Одиниці").ToArray()[0].PlayerScore == 0)
                    {
                        availableCombinations.Add($"Одиниці");
                    }
            
                    if (dice.Contains(2)
                        && scoreTable.Rows.Where(x => x.Combination == "Двійки").ToArray()[0].PlayerScore == 0)
                    {
                        availableCombinations.Add($"Двійки");
                    }
            
                    if (dice.Contains(3)
                        && scoreTable.Rows.Where(x => x.Combination == "Трійки").ToArray()[0].PlayerScore == 0)
                    {
                        availableCombinations.Add($"Трійки");
                    }
            
                    if (dice.Contains(4)
                        && scoreTable.Rows.Where(x => x.Combination == "Четвірки").ToArray()[0].PlayerScore == 0)
                    {
                        availableCombinations.Add($"Четвірки");
                    }
            
                    if (dice.Contains(5)
                        && scoreTable.Rows.Where(x => x.Combination == "П'ятірки").ToArray()[0].PlayerScore == 0)
                    {
                        availableCombinations.Add($"П'ятірки");
                    }
            
                    if (dice.Contains(6)
                        && scoreTable.Rows.Where(x => x.Combination == "Шестірки").ToArray()[0].PlayerScore == 0)
                    {
                        availableCombinations.Add($"Шестірки");
                    }
            
                    if(IsStraight()
                       && scoreTable.Rows.Where(x => x.Combination == "Стріт").ToArray()[0].PlayerScore == 0)
                    {
                        availableCombinations.Add("Стріт");
                    }
            
                    if (IsFullHouse()
                        && scoreTable.Rows.Where(x => x.Combination == "Фул Хаус").ToArray()[0].PlayerScore == 0)
                    {
                        availableCombinations.Add("Фул Хаус");
                    }
            
                    if (IsFourOfAKind()
                        && scoreTable.Rows.Where(x => x.Combination == "Чотири").ToArray()[0].PlayerScore == 0)
                    {
                        availableCombinations.Add("Чотири");
                    }
            
                    if (IsGeneral()
                        && scoreTable.Rows.Where(x => x.Combination == "Генерал").ToArray()[0].PlayerScore == 0)
                    {
                        availableCombinations.Add("Генерал");
                    }
                    break;
                case "Комп'ютер":
                    if (dice.Contains(1)
                        && scoreTable.Rows.Where(x => x.Combination == "Одиниці").ToArray()[0].ComputerScore == 0)
                    {
                        availableCombinations.Add($"Одиниці");
                    }
            
                    if (dice.Contains(2)
                        && scoreTable.Rows.Where(x => x.Combination == "Двійки").ToArray()[0].ComputerScore == 0)
                    {
                        availableCombinations.Add($"Двійки");
                    }
            
                    if (dice.Contains(3)
                        && scoreTable.Rows.Where(x => x.Combination == "Трійки").ToArray()[0].ComputerScore == 0)
                    {
                        availableCombinations.Add($"Трійки");
                    }
            
                    if (dice.Contains(4)
                        && scoreTable.Rows.Where(x => x.Combination == "Четвірки").ToArray()[0].ComputerScore == 0)
                    {
                        availableCombinations.Add($"Четвірки");
                    }
            
                    if (dice.Contains(5)
                        && scoreTable.Rows.Where(x => x.Combination == "П'ятірки").ToArray()[0].ComputerScore == 0)
                    {
                        availableCombinations.Add($"П'ятірки");
                    }
            
                    if (dice.Contains(6)
                        && scoreTable.Rows.Where(x => x.Combination == "Шестірки").ToArray()[0].ComputerScore == 0)
                    {
                        availableCombinations.Add($"Шестірки");
                    }
            
                    if(IsStraight()
                       && scoreTable.Rows.Where(x => x.Combination == "Стріт").ToArray()[0].ComputerScore == 0)
                    {
                        availableCombinations.Add("Стріт");
                    }
            
                    if (IsFullHouse()
                        && scoreTable.Rows.Where(x => x.Combination == "Фул Хаус").ToArray()[0].ComputerScore == 0)
                    {
                        availableCombinations.Add("Фул Хаус");
                    }
            
                    if (IsFourOfAKind()
                        && scoreTable.Rows.Where(x => x.Combination == "Чотири").ToArray()[0].ComputerScore == 0)
                    {
                        availableCombinations.Add("Чотири");
                    }
            
                    if (IsGeneral()
                        && scoreTable.Rows.Where(x => x.Combination == "Генерал").ToArray()[0].ComputerScore == 0)
                    {
                        availableCombinations.Add("Генерал");
                    }
                    break;
            }
            return availableCombinations;
        }
        
        private void ComputerTurn()
        {
            InitializeDice();
            attemptsLeft = maxAttempts; // Дві спроби для комп'ютера

            while (attemptsLeft > 0)
            {
                // Вибір кубиків для перекидання на основі поточного стану
                var diceToReroll = DecideDiceToReroll();

                if (diceToReroll.Count == 0)
                {
                    // Якщо немає кубиків, які треба перекидати, завершуємо спробу
                    UpdateCombinationSelector();
                    break;
                }

                // Перекидання вибраних кубиків
                RollSpecificDice(diceToReroll);
                UpdateCombinationSelector();
                attemptsLeft--;
            }

            // Вибір найкращої комбінації та завершення ходу
            selectedCombination = ChooseBestCombination();

            if (selectedCombination != null)
            {
                if (SetScore() == 65)
                {
                    MessageBox.Show($"Гра завершена великим генералом гравцем {currentPlayer}!");
                    InitializeGame();
                    return;
                }
            }
            
            currentPlayer = GetNextPlayer();
            if (currentPlayer == players[0])
            {
                currentRound++;
            }

            if (currentRound > maxRounds)
            {
                EndGame();
                return;
            }
            
            Title = $"Генерал. Раунд {currentRound}";

            InitializeDice();
            UpdateScoreTable();
            attemptsLeft = maxAttempts;
            RollSpecificDice([0,1,2,3,4]);
        }

        private List<int> DecideDiceToReroll()
        {
            // Аналізуємо кубики для вибору найбільш перспективної комбінації
            var groupedDice = dice.GroupBy(x => x)
                                  .OrderByDescending(g => g.Count())
                                  .ThenByDescending(g => g.Key);

            var diceToReroll = new List<int>();

            // Приклад стратегії: залишаємо кубики, які утворюють множини або "стріт"
            if (groupedDice.First().Count() >= 3)
            {
                // Залишаємо всі кубики, які формують трійку
                int targetNumber = groupedDice.First().Key;
                for (int i = 0; i < dice.Length; i++)
                {
                    if (dice[i] != targetNumber)
                    {
                        diceToReroll.Add(i);
                    }
                }
            }
            else if (IsStraight())
            {
                // Якщо є "стріт", не перекидаємо жодних кубиків
                return diceToReroll;
            }
            else
            {
                // Інакше залишаємо найвищі значення (для загального рахунку)
                int threshold = 5; // Наприклад, залишаємо тільки кубики 5 і 6
                for (int i = 0; i < dice.Length; i++)
                {
                    if (dice[i] < threshold)
                    {
                        diceToReroll.Add(i);
                    }
                }
            }

            return diceToReroll;
        }

        private void RollSpecificDice(List<int> diceToReroll)
        {
            foreach (int index in diceToReroll)
            {
                dice[index] = random.Next(1, 7);
                Button diceButton = DiceArea.Items[index] as Button;
                if (diceButton != null)
                {
                    diceButton.Content = dice[index].ToString();
                }
            }
        }

        private string ChooseBestCombination()
        {
            // Вибір найкращої доступної комбінації
            var combinations = GetAvailableCombinations();

            if (combinations.Count == 0)
            {
                return null;
            }

            // Приклад стратегії: вибираємо комбінацію з максимальним потенційним рахунком
            if (IsGeneral() && combinations.Contains("Генерал")) return "Генерал";
            if (IsFourOfAKind() && combinations.Contains("Чотири")) return "Чотири";
            if (IsFullHouse() && combinations.Contains("Фул Хаус")) return "Фул Хаус";
            if (IsStraight() && combinations.Contains("Стріт")) return "Стріт";

            // Якщо жодна спеціальна комбінація не підходить, вибираємо за значеннями
            var scores = new Dictionary<string, int>
            {
                { "Одиниці", dice.Count(x => x == 1) },
                { "Двійки", dice.Count(x => x == 2) * 2 },
                { "Трійки", dice.Count(x => x == 3) * 3 },
                { "Четвірки", dice.Count(x => x == 4) * 4 },
                { "П'ятірки", dice.Count(x => x == 5) * 5 },
                { "Шестірки", dice.Count(x => x == 6) * 6 }
            };

            string bestCombination = scores.OrderByDescending(x => x.Value)
                                            .Where(x => combinations.Contains(x.Key))
                                            .Select(x => x.Key)
                                            .FirstOrDefault();

            return bestCombination ?? combinations.First();
        }
        
        private bool IsGeneral()
        {
            return dice.Distinct().Count() == 1;
        }

        private bool IsFourOfAKind()
        {
            return dice.GroupBy(x => x).Any(g => g.Count() == 4);
        }

        private bool IsFullHouse()
        {
            var groups = dice.GroupBy(x => x).ToList();
            return groups.Count == 2 && groups.Any(g => g.Count() == 3);
        }

        private bool IsStraight()
        {
            var sortedDice = dice.Order();
            return sortedDice.SequenceEqual(new int[] { 1, 2, 3, 4, 5 }) ||
                   sortedDice.SequenceEqual(new int[] { 2, 3, 4, 5, 6 }) ||
                   sortedDice.SequenceEqual(new int[] { 1, 1, 3, 4, 5 }) ||
                   sortedDice.SequenceEqual(new int[] { 1, 3, 4, 5, 6 });
        }

        private string GetNextPlayer()
        {
            int currentIndex = players.IndexOf(currentPlayer);
            return players[(currentIndex + 1) % players.Count];
        }

        private void UpdateScoreTable()
        {
            UpdateTotal();
            ScoreGrid.ItemsSource = null;
            ScoreGrid.ItemsSource = scoreTable.Rows;
        }

        private void UpdateTotal()
        {
            scoreTable.Rows.Where(x => x.Combination == "Загалом").ToArray()[0].PlayerScore =
                scoreTable.Rows.Take(scoreTable.Rows.Count - 1).Sum(row => row.PlayerScore);
            scoreTable.Rows.Where(x => x.Combination == "Загалом").ToArray()[0].ComputerScore = 
                scoreTable.Rows.Take(scoreTable.Rows.Count - 1).Sum(row => row.ComputerScore);
        }

        private void EndGame()
        {
            int playerTotal = scoreTable.Rows.Where(x => x.Combination == "Загалом").ToArray()[0].PlayerScore, 
                computerTotal = scoreTable.Rows.Where(x => x.Combination == "Загалом").ToArray()[0].ComputerScore;
            if (playerTotal == computerTotal)
            {
                MessageBox.Show($"Гра завершена нічиєю! Результат: {playerTotal} очок.");
            }

            MessageBox.Show(playerTotal > computerTotal
                ? $"Гра завершена! Переміг Користувач з результатом {playerTotal} очок."
                : $"Гра завершена! Переміг Комп'ютер з результатом {computerTotal} очок.");
            InitializeGame();
        }
    }
}