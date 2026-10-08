/**
 * Configuração da Chave PIX e QR Code para Apoio/Doação ao Cruzadas.online.
 * 
 * DADOS OFICIAIS ATUALIZADOS:
 * - Beneficiário: Diego Ferreira Moreno
 * - Chave PIX: 6cea112f-0b98-4d3f-a1bc-d798835d4453 (Chave Aleatória)
 * - Instituição: Mercado Pago
 * - QR Code: apps/web/public/pix-qrcode.png
 */

export interface PixDonationConfig {
  pixKey: string;
  keyType: 'E-mail' | 'Telefone' | 'CPF/CNPJ' | 'Chave Aleatória';
  beneficiaryName: string;
  city: string;
  pixCopiaECola: string;
  qrCodeImageUrl: string;
}

export const DONATION_CONFIG: PixDonationConfig = {
  pixKey: '6cea112f-0b98-4d3f-a1bc-d798835d4453',
  keyType: 'Chave Aleatória',
  beneficiaryName: 'Diego Ferreira Moreno',
  city: 'São Paulo',

  // Código Pix "Copia e Cola" Oficial (Padrão BR Code Banco Central)
  pixCopiaECola:
    '00020126580014br.gov.bcb.pix01366cea112f-0b98-4d3f-a1bc-d798835d44535204000053039865802BR5921DIEGO FERREIRA MORENO6009Sao Paulo62220518daqr2821837969163563045AE1',

  // Imagem oficial do QR Code
  qrCodeImageUrl: '/pix-qrcode.png',
};
