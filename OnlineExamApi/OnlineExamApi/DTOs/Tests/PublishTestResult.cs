namespace OnlineExamApi.DTOs.Tests;

//for response after publish the test
public class PublishTestResult
{
    public int TestId { get; set; }
    public string Name { get; set; } = null!;
    public bool IsPublished { get; set; }
    public DateTime PublishedAt { get; set; }
}
