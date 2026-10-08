import React from 'react';
import type { GameItem, QuizGroup } from '../types';

interface GameCatalogProps {
  games: GameItem[];
  groups: QuizGroup[];
  selectedGroupSlug: string | null;
  onSelectGroup: (groupSlug: string | null) => void;
  onSelectGame: (game: GameItem) => void;
  onStartRandomQuiz: () => void;
  isLoading: boolean;
  isStartingRandom?: boolean;
}

export const GameCatalog: React.FC<GameCatalogProps> = ({
  games,
  groups,
  selectedGroupSlug,
  onSelectGroup,
  onSelectGame,
  onStartRandomQuiz,
  isLoading,
  isStartingRandom = false,
}) => {
  const getIconForGroup = (icon: string) => {
    switch (icon) {
      case 'church':
        return '⛪';
      case 'book':
        return '📖';
      case 'shield':
        return '🛡️';
      case 'flame':
        return '🔥';
      default:
        return '✠';
    }
  };

  const getDifficultyClass = (level?: string) => {
    switch (level?.toLowerCase()) {
      case 'iniciante':
        return 'iniciante';
      case 'intermediário':
      case 'intermediario':
        return 'intermediario';
      case 'avançado':
      case 'avancado':
        return 'avancado';
      default:
        return 'iniciante';
    }
  };

  return (
    <div>
      {/* Hero */}
      <section className="hero" aria-labelledby="hero-heading">
        <div className="hero-badge" aria-hidden="true">
          <span>✠</span> MVP v0.1 — Formação Intelectual e Cultura Cristã
        </div>
        <h1 id="hero-heading" className="hero-title">
          Conhecimento, Tradição e Fé <span>em Forma de Jogo</span>
        </h1>
        <p className="hero-description">
          Aprofunde seus conhecimentos sobre as Sagradas Escrituras, a vida dos santos,
          os sacramentos, a liturgia e a história da Igreja através de desafios católicos interativos.
        </p>

        {/* Quick Play Hero Banner */}
        <div className="hero-quick-play">
          <div className="hero-quick-play-info">
            <h2>
              <span>🎲</span> Partida Rápida Desafio
            </h2>
            <p>
              Inicie imediatamente um quiz sorteado aleatoriamente entre todas as categorias para testar seus conhecimentos.
            </p>
          </div>
          <button
            onClick={onStartRandomQuiz}
            disabled={isStartingRandom || isLoading}
            className="hero-quick-play-btn"
            aria-label="Jogar Partida Rápida com Quiz Aleatório"
          >
            {isStartingRandom ? 'Sorteando Desafio...' : '🎲 Jogar Partida Rápida ➔'}
          </button>
        </div>
      </section>

      {/* Catalog Section */}
      <section aria-labelledby="catalog-heading">
        <div className="section-header">
          <h2 id="catalog-heading" className="section-title">Catálogo por Grupos Temáticos</h2>
          <p className="section-subtitle">Escolha um tema ou navegue por todas as opções disponíveis.</p>
        </div>

        {/* Category Filters */}
        <div className="group-filter-bar" role="tablist" aria-label="Filtrar jogos por categoria">
          <button
            role="tab"
            aria-selected={selectedGroupSlug === null}
            onClick={() => onSelectGroup(null)}
            className={`group-filter-chip ${selectedGroupSlug === null ? 'active' : ''}`}
          >
            ✠ Todos os Jogos
          </button>

          {groups.map((group) => (
            <button
              key={group.id}
              role="tab"
              aria-selected={selectedGroupSlug === group.slug}
              onClick={() => onSelectGroup(group.slug)}
              className={`group-filter-chip ${selectedGroupSlug === group.slug ? 'active' : ''}`}
            >
              <span aria-hidden="true">{getIconForGroup(group.icon)}</span>
              {group.name}
            </button>
          ))}
        </div>

        {isLoading ? (
          <div className="state-box" aria-live="polite">
            <div className="spinner" />
            <p>Carregando desafios disponíveis...</p>
          </div>
        ) : games.length === 0 ? (
          <div className="state-box">
            <p>Nenhum jogo encontrado para a categoria selecionada.</p>
            <button
              onClick={() => onSelectGroup(null)}
              className="btn btn-secondary"
              style={{ marginTop: '1rem' }}
            >
              Ver Todos os Jogos
            </button>
          </div>
        ) : (
          <div className="games-grid">
            {games.map((game) => {
              const isAvailable = game.isAvailable;
              return (
                <article
                  key={game.id}
                  className={`game-card ${isAvailable ? 'available' : 'disabled'}`}
                >
                  <div className="game-card-header">
                    <span className="game-category">{game.category}</span>
                    <div style={{ display: 'flex', gap: '0.4rem', alignItems: 'center' }}>
                      {game.difficultyLevel && (
                        <span className={`badge-difficulty ${getDifficultyClass(game.difficultyLevel)}`}>
                          {game.difficultyLevel}
                        </span>
                      )}
                      <span
                        className={`badge-status ${isAvailable ? 'active' : 'coming'}`}
                      >
                        {game.status}
                      </span>
                    </div>
                  </div>

                  <h3 className="game-title">{game.title}</h3>
                  <p className="game-description">{game.description}</p>

                  <div className="game-card-footer">
                    {isAvailable ? (
                      <button
                        onClick={() => onSelectGame(game)}
                        className="btn btn-primary"
                        aria-label={`Iniciar partida de ${game.title}`}
                      >
                        Iniciar Partida ➔
                      </button>
                    ) : (
                      <button
                        disabled
                        className="btn btn-secondary"
                        aria-disabled="true"
                      >
                        Em Desenvolvimento
                      </button>
                    )}
                  </div>
                </article>
              );
            })}
          </div>
        )}
      </section>
    </div>
  );
};
