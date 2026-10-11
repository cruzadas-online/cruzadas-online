import React, { useState, useEffect } from 'react';
import { DONATION_CONFIG } from '../config/donation';

interface DonationModalProps {
  isOpen: boolean;
  onClose: () => void;
  isAutomaticPrompt?: boolean;
  hasDonated?: boolean;
  onMarkDonated?: () => void;
}

export const DonationModal: React.FC<DonationModalProps> = ({
  isOpen,
  onClose,
  isAutomaticPrompt = false,
  hasDonated = false,
  onMarkDonated,
}) => {
  const [copiedType, setCopiedType] = useState<'code' | 'key' | null>(null);
  const [markedDonatedLocally, setMarkedDonatedLocally] = useState(false);

  useEffect(() => {
    setMarkedDonatedLocally(false);
  }, [isOpen]);

  // Close on Escape key
  useEffect(() => {
    const handleKeyDown = (e: KeyboardEvent) => {
      if (e.key === 'Escape') {
        onClose();
      }
    };

    if (isOpen) {
      document.body.style.overflow = 'hidden';
      window.addEventListener('keydown', handleKeyDown);
    }

    return () => {
      document.body.style.overflow = '';
      window.removeEventListener('keydown', handleKeyDown);
    };
  }, [isOpen, onClose]);

  if (!isOpen) return null;

  const handleCopy = async (text: string, type: 'code' | 'key') => {
    try {
      if (navigator.clipboard && navigator.clipboard.writeText) {
        await navigator.clipboard.writeText(text);
      } else {
        // Fallback for older browsers
        const textarea = document.createElement('textarea');
        textarea.value = text;
        textarea.style.position = 'fixed';
        textarea.style.opacity = '0';
        document.body.appendChild(textarea);
        textarea.select();
        document.execCommand('copy');
        document.body.removeChild(textarea);
      }

      setCopiedType(type);
      setTimeout(() => setCopiedType(null), 3000);
    } catch {
      // If clipboard access is blocked
      setCopiedType(type);
      setTimeout(() => setCopiedType(null), 3000);
    }
  };

  const handleConfirmDonated = () => {
    setMarkedDonatedLocally(true);
    if (onMarkDonated) {
      onMarkDonated();
    }
  };

  const isUserDonor = hasDonated || markedDonatedLocally;

  return (
    <div
      className="donation-backdrop"
      onClick={onClose}
      role="presentation"
      aria-hidden={!isOpen}
    >
      <div
        className="donation-modal"
        role="dialog"
        aria-modal="true"
        aria-labelledby="donation-modal-title"
        onClick={(e) => e.stopPropagation()}
      >
        <button
          className="donation-close-btn"
          onClick={onClose}
          aria-label="Fechar janela de apoio"
        >
          ✕
        </button>

        <div className="donation-header">
          {isUserDonor ? (
            <span className="donation-badge donation-badge-donor">
              💛 Benfeitor do Cruzadas.online
            </span>
          ) : (
            <span className="donation-badge">✠ Apoio ao Apostolado</span>
          )}

          <h2 id="donation-modal-title" className="donation-title">
            {isUserDonor
              ? 'Nossa gratidão por manter o projeto vivo!'
              : isAutomaticPrompt
              ? 'Que bom ter você jogando conosco!'
              : 'Apoie o Cruzadas.online'}
          </h2>

          <p className="donation-subtitle">
            {isUserDonor ? (
              <>
                Sua generosidade permite manter servidores ativos e conteúdo católico autêntico
                e gratuito. Que Deus recompense o seu apoio generoso a esta missão!
              </>
            ) : isAutomaticPrompt ? (
              <>
                Notamos que você tem aproveitado nossos jogos! O <strong>Cruzadas.online</strong> é mantido
                100% por doações da comunidade, sem anúncios incômodos. Considere fazer uma contribuição via Pix para nos apoiar.
              </>
            ) : (
              <>
                Ajude a manter a plataforma no ar, 100% gratuita e sem anúncios invasivos para
                todos que buscam aprofundar seu conhecimento na fé e na cultura cristã.
              </>
            )}
          </p>
        </div>

        {/* Impact List */}
        <div className="donation-impact-grid">
          <div className="donation-impact-item">
            <span className="impact-icon">✝</span>
            <div>
              <strong>Servidores & Infra</strong>
              <p>Custos de nuvem, banco de dados e estabilidade contínua.</p>
            </div>
          </div>
          <div className="donation-impact-item">
            <span className="impact-icon">📖</span>
            <div>
              <strong>Novas Perguntas</strong>
              <p>Curadoria e referências do Catecismo e Sagradas Escrituras.</p>
            </div>
          </div>
          <div className="donation-impact-item">
            <span className="impact-icon">⚔</span>
            <div>
              <strong>Novos Jogos</strong>
              <p>Palavras cruzadas, caça-palavras e desafios bíblicos.</p>
            </div>
          </div>
        </div>

        {/* PIX Donation Box */}
        <div className="donation-pix-box">
          <div className="donation-qrcode-wrapper">
            <img
              src={DONATION_CONFIG.qrCodeImageUrl}
              alt="QR Code PIX para Doação ao Cruzadas.online"
              className="donation-qrcode-img"
            />
            <span className="qrcode-hint">Escaneie com a câmera ou app do seu banco</span>
          </div>

          <div className="donation-details">
            <div className="donation-meta-row">
              <span className="meta-label">Beneficiário:</span>
              <span className="meta-value">{DONATION_CONFIG.beneficiaryName}</span>
            </div>
            <div className="donation-meta-row">
              <span className="meta-label">Chave PIX ({DONATION_CONFIG.keyType}):</span>
              <span className="meta-value font-mono">{DONATION_CONFIG.pixKey}</span>
            </div>

            <div className="donation-actions-group">
              <button
                type="button"
                className={`btn btn-copy-pix ${copiedType === 'code' ? 'copied' : ''}`}
                onClick={() => handleCopy(DONATION_CONFIG.pixCopiaECola, 'code')}
              >
                {copiedType === 'code' ? (
                  <>✓ Código Pix Copiado!</>
                ) : (
                  <>📋 Copiar Código Pix (Copia e Cola)</>
                )}
              </button>

              <button
                type="button"
                className={`btn btn-copy-key ${copiedType === 'key' ? 'copied' : ''}`}
                onClick={() => handleCopy(DONATION_CONFIG.pixKey, 'key')}
              >
                {copiedType === 'key' ? (
                  <>✓ Chave Copiada!</>
                ) : (
                  <>Copiar apenas a Chave</>
                )}
              </button>
            </div>

            {copiedType && (
              <p className="donation-copy-feedback" role="status">
                Código copiado! Cole no aplicativo do seu banco na opção <strong>Pix Copia e Cola</strong>.
              </p>
            )}
          </div>
        </div>

        {/* Instructions */}
        <div className="donation-steps">
          <h4>Como doar em 3 passos simples:</h4>
          <ol>
            <li>Abra o aplicativo do seu banco ou carteira digital.</li>
            <li>Selecione <strong>Pix</strong> e escolha <strong>Ler QR Code</strong> ou <strong>Pix Copia e Cola</strong>.</li>
            <li>Defina o valor que desejar (R$ 2, R$ 5, R$ 10 ou mais) e confirme a contribuição.</li>
          </ol>
        </div>

        {/* Already Donated Button / Status */}
        <div className="donation-donor-action">
          {isUserDonor ? (
            <div className="donor-status-card" role="status">
              <span className="donor-check-icon">✓</span>
              <span>Você já marcou sua contribuição! Deus lhe pague pelo zelo e generosidade.</span>
            </div>
          ) : (
            <button
              type="button"
              className="btn btn-already-donated"
              onClick={handleConfirmDonated}
            >
              💛 Já realizei uma contribuição
            </button>
          )}
        </div>

        <div className="donation-footer">
          <p className="donation-gratitude">
            <em>“Cada um dê conforme determinou em seu coração, não com pesar ou por obrigação, pois Deus ama quem dá com alegria.” (2 Cor 9, 7)</em>
          </p>
          <button type="button" className="btn btn-secondary" onClick={onClose}>
            Fechar
          </button>
        </div>
      </div>
    </div>
  );
};
