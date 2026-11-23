namespace OnlineExamApi.DTOs.Tests;

//Leaderboard response dto  for admin panel's leaderboard page
public class TestLeaderboardResult
{
    public int TestId { get; set; }
    public string Name { get; set; } = null!;
    public string Subject { get; set; } = null!;
    public int DurationMinutes { get; set; }
    public int PassingPercentage { get; set; }
    public int TotalQuestions { get; set; }
    public int UsersAppeared { get; set; }

    public List<LeaderboardUserResult> Results { get; set; } = new();
}

public class LeaderboardUserResult
{
    public int Rank { get; set; }

    public int UserId { get; set; }

    public string UserName { get; set; } = null!;

    public string Name { get; set; } = null!;

    public int CorrectAnswers { get; set; }

    public int TotalQuestions { get; set; }

    public int Score { get; set; }

    public bool IsPassed { get; set; }

    public DateTime SubmittedAt { get; set; }
}

