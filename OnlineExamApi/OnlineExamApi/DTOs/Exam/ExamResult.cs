namespace OnlineExamApi.DTOs.Exam;

//for exam result after submit exam
public class ExamResult
{
    public int TestAttemptId { get; set; }
    public int Score { get; set; }
    public int TotalQuestions { get; set; }
    public bool IsPassed { get; set; }
}
