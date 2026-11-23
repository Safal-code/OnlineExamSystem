export interface ExamResult {
  testAttemptId: number;
  score: number;
  totalQuestions: number;
  isPassed: boolean;
}

//added for userAnalytics and admin leaderboard
export interface LeaderboardUserResult {
  rank: number;
  userId: number;
  userName: string;
  name: string;
  correctAnswers: number;
  totalQuestions: number;
  score: number;
  isPassed: boolean;
  submittedAt: string;
}

export interface TestLeaderboard {
  testId: number;
  name: string;
  subject: string;
  durationMinutes: number;
  passingPercentage: number;
  totalQuestions: number;
  usersAppeared: number;
  results: LeaderboardUserResult[];
}

export interface AnalyticsOptionResult {
  optionId: number;
  optionText: string;
  isCorrect: boolean;
  isSelected: boolean;
}

export interface AnalyticsQuestionResult {
  questionId: number;
  questionNumber: number;
  questionText: string;
  isCorrect: boolean;
  options: AnalyticsOptionResult[];
}

export interface UserAnalyticsResult {
  testAttemptId: number;
  testId: number;
  testName: string;
  subject: string;
  score: number;
  totalQuestions: number;
  correctAnswers: number;
  incorrectAnswers: number;
  isPassed: boolean;
  questions: AnalyticsQuestionResult[];
}