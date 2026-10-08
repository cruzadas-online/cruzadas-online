import React from 'react';

interface HeaderProps {
  onGoHome: () => void;
  onOpenDonation?: () => void;
}

export const Header: React.FC<HeaderProps> = ({ onGoHome, onOpenDonation }) => {
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
        <div className="header-actions">
          <span className="brand-tagline">Jogos de Fé e Cultura</span>
          {onOpenDonation && (
            <button
              onClick={onOpenDonation}
              className="btn-donation-header"
              title="Apoie o Cruzadas.online via PIX"
              aria-label="Apoiar o projeto Cruzadas.online com PIX"
            >
              <span className="donation-heart-icon" aria-hidden="true">♥</span>
              <span>Apoiar Projeto</span>
            </button>
          )}
        </div>
      </div>
    </header>
  );
};
