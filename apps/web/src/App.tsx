import React, { useState, useEffect, useCallback } from 'react';
import { Header } from './components/Header';
import { Footer } from './components/Footer';
import { GameCatalog } from './components/GameCatalog';
import { QuizPlay } from './components/QuizPlay';
import { QuizResultView } from './components/QuizResultView';
import { api, ApiError } from './api/client';
import type { GameItem, StartAttemptResponse, QuizResult } from './types';

type ViewMode = 'catalog' | 'loading_quiz' | 'playing' | 'submitting' | 'result' | 'error';

export const App: React.FC = () => {
  const [viewMode, setViewMode] = useState<ViewMode>('catalog');
  const [games, setGames] = useState<GameItem[]>([]);
  const [isLoadingCatalog, setIsLoadingCatalog] = useState(true);
  const [errorMessage, setErrorMessage] = useState<string | null>(null);

  // Active quiz session state
  const [activeQuizSlug, setActiveQuizSlug] = useState<string>('fundamentos-da-fe');
  const [activeAttempt, setActiveAttempt] = useState<StartAttemptResponse | null>(null);
  const [quizResult, setQuizResult] = useState<QuizResult | null>(null);

  const loadCatalog = useCallback(async () => {
    setIsLoadingCatalog(true);
    setErrorMessage(null);
    try {
      const data = await api.getGamesCatalog();
      setGames(data);
    } catch (err) {
      const message = err instanceof ApiError
        ? `${err.message}${err.details ? `: ${err.details}` : ''}`
        : 'Não foi possível carregar os jogos. Verifique a conexão com a API.';
      setErrorMessage(message);
    } finally {
      setIsLoadingCatalog(false);
    }
  }, []);

  useEffect(() => {
    let ignore = false;
    api.getGamesCatalog()
      .then((data) => {
        if (!ignore) {
          setGames(data);
          setErrorMessage(null);
        }
      })
      .catch((err) => {
        if (!ignore) {
          const message = err instanceof ApiError
            ? `${err.message}${err.details ? `: ${err.details}` : ''}`
            : 'Não foi possível carregar os jogos. Verifique a conexão com a API.';
          setErrorMessage(message);
        }
      })
      .finally(() => {
        if (!ignore) {
          setIsLoadingCatalog(false);
        }
      });

    return () => {
      ignore = true;
    };
  }, []);

  const handleStartQuiz = async (game: GameItem) => {
    setViewMode('loading_quiz');
    setErrorMessage(null);
    setActiveQuizSlug(game.slug);

    try {
      const attemptData = await api.startAttempt(game.slug);
      setActiveAttempt(attemptData);
      setViewMode('playing');
    } catch (err) {
      const msg = err instanceof ApiError
        ? `${err.message}${err.details ? ` (${err.details})` : ''}`
        : 'Falha ao iniciar a partida. Tente novamente.';
      setErrorMessage(msg);
      setViewMode('error');
    }
  };

  const handleCompleteQuiz = async (answers: Record<string, string>) => {
    if (!activeAttempt) return;

    setViewMode('submitting');
    setErrorMessage(null);

    try {
      const resultData = await api.completeAttempt(activeQuizSlug, activeAttempt.attemptId, answers);
      setQuizResult(resultData);
      setViewMode('result');
    } catch (err) {
      const msg = err instanceof ApiError
        ? `${err.message}${err.details ? ` (${err.details})` : ''}`
        : 'Falha ao enviar respostas para o servidor.';
      setErrorMessage(msg);
      setViewMode('error');
    }
  };

  const handlePlayAgain = async () => {
    setViewMode('loading_quiz');
    setErrorMessage(null);

    try {
      const attemptData = await api.startAttempt(activeQuizSlug);
      setActiveAttempt(attemptData);
      setQuizResult(null);
      setViewMode('playing');
    } catch (err) {
      const msg = err instanceof ApiError ? err.message : 'Falha ao iniciar nova partida.';
      setErrorMessage(msg);
      setViewMode('error');
    }
  };

  const handleGoHome = () => {
    setActiveAttempt(null);
    setQuizResult(null);
    setErrorMessage(null);
    setViewMode('catalog');
  };

  return (
    <div className="app-shell">
      <Header onGoHome={handleGoHome} />

      <main className="main-content" id="main-content">
        <div className="container">
          {viewMode === 'catalog' && (
            <GameCatalog
              games={games}
              onSelectGame={handleStartQuiz}
              isLoading={isLoadingCatalog}
            />
          )}

          {viewMode === 'loading_quiz' && (
            <div className="state-box" aria-live="polite">
              <div className="spinner" />
              <h2 style={{ fontFamily: 'var(--font-serif)', marginBottom: '0.5rem' }}>
                Preparando Partida...
              </h2>
              <p style={{ color: 'var(--color-text-muted)' }}>
                Sorteando perguntas e organizando o desafio.
              </p>
            </div>
          )}

          {(viewMode === 'playing' || viewMode === 'submitting') && activeAttempt && (
            <QuizPlay
              quizTitle={activeAttempt.quizTitle}
              totalQuestions={activeAttempt.totalQuestions}
              questions={activeAttempt.questions}
              onComplete={handleCompleteQuiz}
              onQuit={handleGoHome}
              isSubmitting={viewMode === 'submitting'}
            />
          )}

          {viewMode === 'result' && quizResult && (
            <QuizResultView
              result={quizResult}
              onPlayAgain={handlePlayAgain}
              onBackToCatalog={handleGoHome}
            />
          )}

          {viewMode === 'error' && (
            <div className="state-box" role="alert">
              <h2 className="error-title">Ops! Algo deu errado</h2>
              <p className="error-text">{errorMessage}</p>
              <button
                onClick={() => {
                  handleGoHome();
                  void loadCatalog();
                }}
                className="btn btn-primary"
                style={{ maxWidth: '240px', margin: '0 auto' }}
              >
                Voltar à Página Inicial
              </button>
            </div>
          )}
        </div>
      </main>

      <Footer />
    </div>
  );
};

export default App;
