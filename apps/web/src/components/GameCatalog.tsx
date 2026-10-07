import React from 'react';
import type { GameItem } from '../types';

interface GameCatalogProps {
  games: GameItem[];
  onSelectGame: (game: GameItem) => void;
  isLoading: boolean;
}

export const GameCatalog: React.FC<GameCatalogProps> = ({
  games,
  onSelectGame,
  isLoading,
}) => {
  return (
    <div>
      {/* Hero */}
      <section className="hero" aria-labelledby="hero-heading">
        <div className="hero-badge" aria-hidden="true">
          <span>✠</span> MVP v0.1 — Edição de Lançamento
        </div>
        <h1 id="hero-heading" className="hero-title">
          Conhecimento, Tradição e Fé <span>em Forma de Jogo</span>
        </h1>
        <p className="hero-description">
          Aprofunde seus conhecimentos sobre a Sagrada Escritura, a vida dos santos,
          a liturgia e os fundamentos da fé católica através de desafios interativos.
        </p>
      </section>

      {/* Catalog */}
      <section aria-labelledby="catalog-heading">
        <div className="section-header">
          <h2 id="catalog-heading" className="section-title">Catálogo de Jogos</h2>
          <p className="section-subtitle">Escolha um desafio e inicie sua jornada.</p>
        </div>

        {isLoading ? (
          <div className="state-box" aria-live="polite">
            <div className="spinner" />
            <p>Carregando catálogo de jogos...</p>
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
                    <span
                      className={`badge-status ${isAvailable ? 'active' : 'coming'}`}
                    >
                      {game.status}
                    </span>
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
