import { describe, it, expect, vi, beforeEach } from 'vitest';
import { render, screen, fireEvent } from '@testing-library/react';
import { DonationModal } from '../components/DonationModal';
import { DONATION_CONFIG } from '../config/donation';

describe('DonationModal Component', () => {
  const onCloseMock = vi.fn();

  beforeEach(() => {
    vi.clearAllMocks();
  });

  it('não deve renderizar quando isOpen é false', () => {
    const { container } = render(<DonationModal isOpen={false} onClose={onCloseMock} />);
    expect(container).toBeEmptyDOMElement();
  });

  it('deve renderizar informações do PIX, QR Code e instruções quando isOpen é true', () => {
    render(<DonationModal isOpen={true} onClose={onCloseMock} />);

    expect(screen.getByRole('dialog')).toBeInTheDocument();
    expect(screen.getByText('Apoie o Cruzadas.online')).toBeInTheDocument();
    expect(screen.getByText(DONATION_CONFIG.beneficiaryName)).toBeInTheDocument();
    expect(screen.getByText(DONATION_CONFIG.pixKey)).toBeInTheDocument();

    const qrImage = screen.getByAltText('QR Code PIX para Doação ao Cruzadas.online');
    expect(qrImage).toBeInTheDocument();
    expect(qrImage).toHaveAttribute('src', DONATION_CONFIG.qrCodeImageUrl);

    expect(screen.getByText(/Como doar em 3 passos simples/i)).toBeInTheDocument();
  });

  it('deve chamar onClose ao clicar no botão fechar', () => {
    render(<DonationModal isOpen={true} onClose={onCloseMock} />);

    const closeBtn = screen.getByLabelText('Fechar janela de apoio');
    fireEvent.click(closeBtn);

    expect(onCloseMock).toHaveBeenCalledTimes(1);
  });

  it('deve permitir copiar o código PIX Copia e Cola', async () => {
    // Mock navigator.clipboard
    const writeTextMock = vi.fn().mockResolvedValue(undefined);
    Object.assign(navigator, {
      clipboard: {
        writeText: writeTextMock,
      },
    });

    render(<DonationModal isOpen={true} onClose={onCloseMock} />);

    const copyBtn = screen.getByText(/Copiar Código Pix/i);
    fireEvent.click(copyBtn);

    expect(writeTextMock).toHaveBeenCalledWith(DONATION_CONFIG.pixCopiaECola);
    expect(await screen.findByText(/Código Pix Copiado!/i)).toBeInTheDocument();
  });

  it('deve exibir botão para marcar contribuição e chamar callback onMarkDonated', () => {
    const onMarkDonatedMock = vi.fn();
    render(
      <DonationModal
        isOpen={true}
        onClose={onCloseMock}
        onMarkDonated={onMarkDonatedMock}
        hasDonated={false}
      />
    );

    const alreadyDonatedBtn = screen.getByText(/Já realizei uma contribuição/i);
    expect(alreadyDonatedBtn).toBeInTheDocument();

    fireEvent.click(alreadyDonatedBtn);
    expect(onMarkDonatedMock).toHaveBeenCalledTimes(1);
    expect(screen.getByText(/Você já marcou sua contribuição!/i)).toBeInTheDocument();
  });

  it('deve exibir mensagem de benfeitor e gratidão quando hasDonated é true', () => {
    render(
      <DonationModal
        isOpen={true}
        onClose={onCloseMock}
        hasDonated={true}
      />
    );

    expect(screen.getByText(/Benfeitor do Cruzadas.online/i)).toBeInTheDocument();
    expect(screen.getByText(/Nossa gratidão por manter o projeto vivo!/i)).toBeInTheDocument();
  });

  it('deve exibir mensagem de engajamento no prompt automático para não-doador', () => {
    render(
      <DonationModal
        isOpen={true}
        onClose={onCloseMock}
        isAutomaticPrompt={true}
        hasDonated={false}
      />
    );

    expect(screen.getByText(/Que bom ter você jogando conosco!/i)).toBeInTheDocument();
  });
});
