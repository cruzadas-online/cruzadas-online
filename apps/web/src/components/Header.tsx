import React from 'react';

interface HeaderProps {
  onGoHome: () => void;
}

export const Header: React.FC<HeaderProps> = ({ onGoHome }) => {
  return (
    <header className="site-header" role="banner">
      <div className="container header-inner">
        <button
          onClick={onGoHome}
          className="brand-link"
          aria-label="Cruzadas.online — Ir para a página inicial"
        >
          <span className="brand-emblem" aria-hidden="true">✠</span>
          <span>Cruzadas<span style={{ color: 'var(--color-gold-light)' }}>.online</span></span>
        </button>
        <span className="brand-tagline">Jogos de Fé e Cultura</span>
      </div>
    </header>
  );
};
