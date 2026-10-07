import type {
  GameItem,
  QuizDetail,
  StartAttemptResponse,
  QuizResult,
} from '../types';

const API_BASE_URL = (import.meta.env.VITE_API_URL || 'http://localhost:5185/api/v1').replace(/\/$/, '');

export class ApiError extends Error {
  public status: number;
  public details?: string;

  constructor(message: string, status: number, details?: string) {
    super(message);
    this.name = 'ApiError';
    this.status = status;
    this.details = details;
  }
}

async function handleResponse<T>(response: Response): Promise<T> {
  if (!response.ok) {
    let errorMessage = `Erro HTTP ${response.status}: ${response.statusText}`;
    let errorDetail: string | undefined;

    try {
      const data = await response.json();
      if (data?.title) {
        errorMessage = data.title;
      }
      if (data?.detail) {
        errorDetail = data.detail;
      }
    } catch {
      // Body not JSON
    }

    throw new ApiError(errorMessage, response.status, errorDetail);
  }

  return response.json() as Promise<T>;
}

export const api = {
  async getGamesCatalog(): Promise<GameItem[]> {
    const res = await fetch(`${API_BASE_URL}/games`, {
      headers: { Accept: 'application/json' },
    });
    return handleResponse<GameItem[]>(res);
  },

  async getQuizDetail(slug: string): Promise<QuizDetail> {
    const res = await fetch(`${API_BASE_URL}/quizzes/${encodeURIComponent(slug)}`, {
      headers: { Accept: 'application/json' },
    });
    return handleResponse<QuizDetail>(res);
  },

  async startAttempt(slug: string): Promise<StartAttemptResponse> {
    const res = await fetch(`${API_BASE_URL}/quizzes/${encodeURIComponent(slug)}/attempts`, {
      method: 'POST',
      headers: {
        Accept: 'application/json',
        'Content-Type': 'application/json',
      },
    });
    return handleResponse<StartAttemptResponse>(res);
  },

  async completeAttempt(
    slug: string,
    attemptId: string,
    answers: Record<string, string>
  ): Promise<QuizResult> {
    const res = await fetch(
      `${API_BASE_URL}/quizzes/${encodeURIComponent(slug)}/attempts/${encodeURIComponent(attemptId)}/complete`,
      {
        method: 'POST',
        headers: {
          Accept: 'application/json',
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({ answers }),
      }
    );
    return handleResponse<QuizResult>(res);
  },
};
