import React from 'react';

export const Footer: React.FC = () => {
  return (
    <footer className="site-footer" role="contentinfo">
      <div className="container footer-inner">
        <p className="footer-motto">“Conhecereis a verdade, e a verdade vos libertará.” (Jo 8, 32)</p>
        <p>© {new Date().getFullYear()} Cruzadas.online — Plataforma de formação intelectual, cultura e tradição cristã.</p>
      </div>
    </footer>
  );
};
