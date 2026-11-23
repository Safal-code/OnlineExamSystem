namespace OnlineExamApi.DTOs.Exam;

//response dto for user analytics after submitting the exam
public class UserAnalyticsResult
{
    public int TestAttemptId { get; set; }

    public int TestId { get; set; }

    public string TestName { get; set; } = null!;

    public string Subject { get; set; } = null!;

    public int Score { get; set; }

    public int TotalQuestions { get; set; }

    public int CorrectAnswers { get; set; }

    public int IncorrectAnswers { get; set; }

    public bool IsPassed { get; set; }

    public List<AnalyticsQuestionResult> Questions { get; set; } = new();
}

public class AnalyticsQuestionResult
{
    public int QuestionId { get; set; }

    public int QuestionNumber { get; set; }

    public string QuestionText { get; set; } = null!;

    public bool IsCorrect { get; set; }

    public List<AnalyticsOptionResult> Options { get; set; } = new();
}

public class AnalyticsOptionResult
{
    public int OptionId { get; set; }

    public string OptionText { get; set; } = null!;

    public bool IsCorrect { get; set; }

    public bool IsSelected { get; set; }
}
