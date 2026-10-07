export interface GameItem {
  id: string;
  title: string;
  slug: string;
  description: string;
  category: string;
  status: string;
  isAvailable: boolean;
}

export interface QuizDetail {
  id: string;
  title: string;
  slug: string;
  description: string;
  questionsPerAttempt: number;
}

export interface AnswerOption {
  id: string;
  text: string;
  order: number;
}

export interface QuizQuestion {
  id: string;
  text: string;
  order: number;
  options: AnswerOption[];
}

export interface StartAttemptResponse {
  attemptId: string;
  quizId: string;
  quizTitle: string;
  totalQuestions: number;
  questions: QuizQuestion[];
}

export interface SubmitAnswersRequest {
  answers: Record<string, string>;
}

export interface QuestionReview {
  questionId: string;
  questionText: string;
  selectedOptionId: string | null;
  selectedOptionText: string | null;
  correctOptionId: string;
  correctOptionText: string;
  isCorrect: boolean;
  explanation: string;
  sourceReference: string | null;
}

export interface QuizResult {
  attemptId: string;
  quizId: string;
  quizTitle: string;
  totalQuestions: number;
  correctAnswersCount: number;
  scorePercentage: number;
  startedAt: string;
  completedAt: string;
  questions: QuestionReview[];
}
