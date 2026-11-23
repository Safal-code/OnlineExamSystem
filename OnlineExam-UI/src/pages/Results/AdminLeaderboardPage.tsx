import { useEffect, useState } from "react";
import { Link, useParams } from "react-router-dom";

import { useAuth } from "../../hooks/useAuth";
import { getTestLeaderboard } from "../../services/testService";

import type { TestLeaderboard } from "../../types/result";

import Loader from "../../components/common/Loader";
import ErrorMessage from "../../components/common/ErrorMessage";

export default function AdminLeaderboardPage() {
  const { testId } = useParams<{ testId: string }>();
  const { user } = useAuth();

  const [leaderboard, setLeaderboard] =
    useState<TestLeaderboard | null>(null);

  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  useEffect(() => {
    async function loadLeaderboard() {
      if (!testId || !user) {
        return;
      }

      try {
        setLoading(true);
        setError("");

        const data = await getTestLeaderboard(
          Number(testId),
          user.userId
        );

        setLeaderboard(data);
      } catch (error) {
        setError(
          error instanceof Error
            ? error.message
            : "Failed to load leaderboard"
        );
      } finally {
        setLoading(false);
      }
    }

    loadLeaderboard();
  }, [testId, user]);

  if (loading) {
    return <Loader />;
  }

  if (error) {
    return <ErrorMessage message={error} />;
  }

  if (!leaderboard) {
    return <ErrorMessage message="Leaderboard not found." />;
  }

  return (
    <div className="leaderboard-page">
      <div className="leaderboard-header">
        <div>
          <h1>
            {leaderboard.name} - Results Leaderboard
          </h1>

          <p>
            Subject: {leaderboard.subject} | Duration:{" "}
            {leaderboard.durationMinutes} minutes | Passing:{" "}
            {leaderboard.passingPercentage}%
          </p>
        </div>

        <Link
          to={`/tests/${leaderboard.testId}`}
          className="btn"
        >
          Back to Test
        </Link>
      </div>

      <div className="leaderboard-summary">
        <div>
          <strong>
            {leaderboard.totalQuestions}
          </strong>
          <span>Total Questions</span>
        </div>

        <div>
          <strong>
            {leaderboard.usersAppeared}
          </strong>
          <span>Users Appeared</span>
        </div>
      </div>

      <div className="leaderboard-card">
        <div className="leaderboard-table header-row">
          <span>Rank</span>
          <span>User Name</span>
          <span>Correct</span>
          <span>Score (%)</span>
          <span>Submitted At</span>
        </div>

        {leaderboard.results.length === 0 ? (
          <p className="empty-message">
            No users have submitted this test yet.
          </p>
        ) : (
          leaderboard.results.map((result) => (
            <div
              className="leaderboard-table result-row"
              key={result.userId}
            >
              <span>{result.rank}</span>

              <span>
                <strong>{result.name}</strong>
                <small>{result.userName}</small>
              </span>

              <span>
                {result.correctAnswers} /{" "}
                {result.totalQuestions}
              </span>

              <span>
                {result.score}%
              </span>

              <span>
                {new Date(
                  result.submittedAt
                ).toLocaleString()}
              </span>
            </div>
          ))
        )}
      </div>
    </div>
  );
}