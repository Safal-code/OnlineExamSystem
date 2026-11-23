import { useEffect, useState } from "react";
import { Link, useParams } from "react-router-dom";

import { useAuth } from "../../hooks/useAuth";
import { getExamAnalytics } from "../../services/examService";

import type { UserAnalyticsResult } from "../../types/result";

import Loader from "../../components/common/Loader";
import ErrorMessage from "../../components/common/ErrorMessage";

export default function UserAnalyticsPage() {
  const { attemptId } =
    useParams<{ attemptId: string }>();

  const { user } = useAuth();

  const [analytics, setAnalytics] =
    useState<UserAnalyticsResult | null>(null);

  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  useEffect(() => {
    async function loadAnalytics() {
      if (!attemptId || !user) {
        return;
      }

      try {
        setLoading(true);
        setError("");

        const data = await getExamAnalytics(
          Number(attemptId),
          user.userId
        );

        setAnalytics(data);
      } catch (error) {
        setError(
          error instanceof Error
            ? error.message
            : "Failed to load analytics"
        );
      } finally {
        setLoading(false);
      }
    }

    loadAnalytics();
  }, [attemptId, user]);

  if (loading) {
    return <Loader />;
  }

  if (error) {
    return <ErrorMessage message={error} />;
  }

  if (!analytics) {
    return <ErrorMessage message="Analytics not found." />;
  }

  return (
    <div className="analytics-page">
      <div className="analytics-header">
        <div>
          <h1>
            {analytics.testName} - Detailed Analytics
          </h1>

          <p>
            Subject: {analytics.subject}
          </p>
        </div>

        <Link
          to={`/result/${analytics.testAttemptId}`}
          className="btn"
        >
          Back to Result
        </Link>
      </div>

      <div className="performance-card">
        <h2>Your Performance</h2>

        <div className="performance-grid">
          <div>
            <strong>
              {analytics.totalQuestions}
            </strong>
            <span>Total Questions</span>
          </div>

          <div>
            <strong>
              {analytics.correctAnswers}
            </strong>
            <span>Correct Answers</span>
          </div>

          <div>
            <strong>
              {analytics.incorrectAnswers}
            </strong>
            <span>Incorrect Answers</span>
          </div>

          <div>
            <strong>
              {analytics.score}%
            </strong>
            <span>Total Score</span>
          </div>

          <div>
            <strong
              className={
                analytics.isPassed
                  ? "status-pass"
                  : "status-fail"
              }
            >
              {analytics.isPassed
                ? "PASS"
                : "FAIL"}
            </strong>

            <span>Status</span>
          </div>
        </div>
      </div>

      <div className="analytics-questions">
        {analytics.questions.map((question) => (
          <div
            className={`analytics-question ${
              question.isCorrect
                ? "question-correct"
                : "question-wrong"
            }`}
            key={question.questionId}
          >
            <div className="question-heading">
              <h2>
                {question.questionNumber}.{" "}
                {question.questionText}
              </h2>

              <span>
                {question.isCorrect
                  ? "✓ Correct"
                  : "✗ Wrong"}
              </span>
            </div>

            <div className="analytics-options">
              {question.options.map((option) => (
                <div
                  className={`analytics-option ${
                    option.isCorrect
                      ? "option-correct"
                      : ""
                  } ${
                    option.isSelected &&
                    !option.isCorrect
                      ? "option-wrong"
                      : ""
                  }`}
                  key={option.optionId}
                >
                  <span>
                    • {option.optionText}
                  </span>

                  <span>
                    {option.isSelected &&
                      !option.isCorrect && (
                        <strong className="wrong-label">
                          Your Answer
                        </strong>
                      )}

                    {option.isSelected &&
                      option.isCorrect && (
                        <strong className="correct-label">
                          Your Answer ✓
                        </strong>
                      )}

                    {!option.isSelected &&
                      option.isCorrect && (
                        <strong className="correct-label">
                          Correct Answer ✓
                        </strong>
                      )}
                  </span>
                </div>
              ))}
            </div>
          </div>
        ))}
      </div>
    </div>
  );
}