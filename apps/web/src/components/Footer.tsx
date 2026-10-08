import React from 'react';

interface FooterProps {
  onOpenDonation?: () => void;
}

export const Footer: React.FC<FooterProps> = ({ onOpenDonation }) => {
  return (
    <footer className="site-footer" role="contentinfo">
      <div className="container footer-inner">
        <p className="footer-motto">“Conhecereis a verdade, e a verdade vos libertará.” (Jo 8, 32)</p>
        <p>© {new Date().getFullYear()} Cruzadas.online — Plataforma de formação intelectual, cultura e tradição cristã.</p>
        {onOpenDonation && (
          <div className="footer-support-row">
            <button
              onClick={onOpenDonation}
              className="footer-support-link"
              title="Apoie o Cruzadas.online com PIX"
            >
              ♥ Apoie este apostolado com uma contribuição via PIX
            </button>
          </div>
        )}
      </div>
    </footer>
  );
};
