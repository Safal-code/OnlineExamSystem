using Microsoft.EntityFrameworkCore;
using OnlineExamApi.Data;
using OnlineExamApi.DTOs.Tests;
using OnlineExamApi.Models;

namespace OnlineExamApi.Services;

public class TestService
{
    private readonly ExamDbContext _dbContext;

    public TestService(ExamDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<CreateTestResult> Create(
     CreateTestRequest request,
     int userId)
    {
        var test = new Test
        {
            Name = request.Name,
            Subject = request.Subject,
            Description = request.Description,
            DurationMinutes = request.DurationMinutes,
            PassingPercentage = request.PassingPercentage,
            IsPublished = false,
            CreatedBy = userId,
            CreatedAt = DateTime.UtcNow
        };

        _dbContext.Tests.Add(test);

        await _dbContext.SaveChangesAsync();

        return new CreateTestResult
        {
            TestId = test.TestId,
            Name = test.Name,
            Subject = test.Subject,
            Description = test.Description,
            DurationMinutes = test.DurationMinutes,
            PassingPercentage = test.PassingPercentage,
            IsPublished = test.IsPublished,
            CreatedBy = test.CreatedBy,
            CreatedAt = test.CreatedAt,
            PublishedAt = test.PublishedAt
        };
    }

    public async Task<List<TestListResult>> GetAll()
    {
        return await _dbContext.Tests
            .Select(t => new TestListResult
            {
                TestId = t.TestId,
                Name = t.Name,
                Subject = t.Subject,
                Description = t.Description,
                DurationMinutes = t.DurationMinutes,
                PassingPercentage = t.PassingPercentage,
                IsPublished = t.IsPublished,
                CreatedBy = t.CreatedBy,
                CreatedAt = t.CreatedAt,
                PublishedAt = t.PublishedAt,
                UsersAppeared = t.TestAttempts
                    .Select(a => a.UserId)
                    .Distinct()
                    .Count()
            })
            .ToListAsync();
    }

    public async Task<PublishTestResult?> Publish(int testId)
    {
        var test = await _dbContext.Tests
            .AsSplitQuery()
            .Include(t => t.Questions)
            .ThenInclude(q => q.Options)
            .FirstOrDefaultAsync(t => t.TestId == testId);

        if (test == null)
        {
            return null;
        }

        if (test.IsPublished)
        {
            return null;
        }

        if (test.Questions.Count == 0)
        {
            throw new InvalidOperationException("Test must have at least one question.");
        }

        foreach (var question in test.Questions)
        {
            if (question.Options.Count < 2)
            {
                throw new InvalidOperationException("Every question must have at least 2 options.");
            }

            var correctOptions = question.Options
                .Count(o => o.IsCorrect);

            if (correctOptions != 1)
            {
                throw new InvalidOperationException("Every question must have exactly one correct option.");
            }
        }

        test.IsPublished = true;
        test.PublishedAt = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync();

        return new PublishTestResult
        {
            TestId = test.TestId,
            Name = test.Name,
            IsPublished = test.IsPublished,
            PublishedAt = test.PublishedAt!.Value  //null forgiving operator
        };
    }

    public async Task<List<PublishedTestResult>> GetPublished()
    {
        return await _dbContext.Tests
            .Where(t => t.IsPublished)
            .Select(t => new PublishedTestResult
            {
                TestId = t.TestId,
                Name = t.Name,
                Subject = t.Subject,
                Description = t.Description,
                DurationMinutes = t.DurationMinutes,
                PassingPercentage = t.PassingPercentage
            })
            .ToListAsync();
    }

    //delete function for the test
    public async Task<bool> Delete(int testId)
    {
        var test = await _dbContext.Tests
            .FirstOrDefaultAsync(t => t.TestId == testId);
 
        if (test == null)
        {

            return false;
        }

        _dbContext.Tests.Remove(test);

        await _dbContext.SaveChangesAsync();

        return true;
    }

    //leaderboard method for admin
    public async Task<TestLeaderboardResult?> GetLeaderboard(int testId)
    {
        var test = await _dbContext.Tests
            .Include(t => t.Questions)
            .FirstOrDefaultAsync(t => t.TestId == testId);

        if (test == null)
        {
            return null;
        }

        var attempts = await _dbContext.TestAttempts
            .Include(a => a.User)
            .Include(a => a.TestAnswers)
            .Where(a =>
                a.TestId == testId &&
                a.CompletedAt != null)
            .OrderBy(a => a.StartedAt)
            .ToListAsync();

        //getting only first attempt per user
        var firstAttempts = attempts
            .GroupBy(a => a.UserId)
            .Select(g => g.First())         //selecting first attempt of each user
            .OrderByDescending(a => a.Score ?? 0)
            .ThenBy(a => a.CompletedAt)
            .ToList();

        var results = firstAttempts
            .Select((attempt, index) => new LeaderboardUserResult
            {
                Rank = index + 1,

                UserId = attempt.UserId,

                UserName = attempt.User.UserName,

                Name = attempt.User.Name,

                CorrectAnswers = attempt.TestAnswers
                    .Count(a => a.IsCorrect),

                TotalQuestions = attempt.TotalQuestions,

                Score = attempt.Score ?? 0,

                IsPassed = attempt.IsPassed ?? false,

                SubmittedAt = attempt.CompletedAt!.Value
            })
            .ToList();

        return new TestLeaderboardResult
        {
            TestId = test.TestId,

            Name = test.Name,

            Subject = test.Subject,

            DurationMinutes = test.DurationMinutes,

            PassingPercentage = test.PassingPercentage,

            TotalQuestions = test.Questions.Count,

            UsersAppeared = results.Count,

            Results = results
        };
    }
}
