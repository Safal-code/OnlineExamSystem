namespace OnlineExamApi.DTOs.Exam;

public class SubmitExamRequest
{
    public List<SubmitAnswerRequest> Answers { get; set; } = new();
    public bool IsAutoSubmit { get; set; }
}

public class SubmitAnswerRequest
{
    public int QuestionId { get; set; }
    public int? SelectedOptionId { get; set; }
}
