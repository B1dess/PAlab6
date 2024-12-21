namespace lr6_game;

public class ScoreTable
{
    public List<ScoreRow> Rows = [];

    enum Combinations
    {
        Ones,
        Twos,
        Threes,
        Fours,
        Fives,
        Sixes,
        Straight,
        FullHouse,
        Four,
        General
    }

    public void Clear()
    {
        foreach (var row in Rows)
        {
            row.PlayerScore = 0;
            row.ComputerScore = 0;
        }
    }
    
    public void Initialize()
    {
        Rows =
        [
            new ScoreRow { Combination = "Одиниці", PlayerScore = 0, ComputerScore = 0 },
            new ScoreRow { Combination = "Двійки", PlayerScore = 0, ComputerScore = 0 },
            new ScoreRow { Combination = "Трійки", PlayerScore = 0, ComputerScore = 0 },
            new ScoreRow { Combination = "Четвірки", PlayerScore = 0, ComputerScore = 0 },
            new ScoreRow { Combination = "П'ятірки", PlayerScore = 0, ComputerScore = 0 },
            new ScoreRow { Combination = "Шестірки", PlayerScore = 0, ComputerScore = 0 },
            new ScoreRow { Combination = "Стріт", PlayerScore = 0, ComputerScore = 0 },
            new ScoreRow { Combination = "Фул Хаус", PlayerScore = 0, ComputerScore = 0 },
            new ScoreRow { Combination = "Чотири", PlayerScore = 0, ComputerScore = 0 },
            new ScoreRow { Combination = "Генерал", PlayerScore = 0, ComputerScore = 0 },
            new ScoreRow { Combination = "Загалом", PlayerScore = 0, ComputerScore = 0}
        ];
    }
}

