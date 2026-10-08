/**
 * Configuração da Chave PIX e QR Code para Apoio/Doação ao Cruzadas.online.
 * 
 * INSTRUÇÕES PARA ATUALIZAR COM OS DADOS REAIS:
 * 1. Chave PIX: Altere o campo 'pixKey' para o seu e-mail, telefone, CPF/CNPJ ou chave aleatória real.
 * 2. Nome do Beneficiário: Altere 'beneficiaryName' para o nome que aparece no seu banco.
 * 3. Cidade: Altere 'city' para a cidade da sua conta bancária.
 * 4. Código Copia e Cola: Abra o app do seu banco, gere um QR Code estático ou use sua chave Pix para gerar a string "Copia e Cola" (EMV BR Code) e cole em 'pixCopiaECola'.
 * 5. Imagem do QR Code: Salve a imagem do seu QR Code oficial em 'apps/web/public/pix-qrcode.png' (ou .svg) e aponte em 'qrCodeImageUrl'.
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
  // Substitua pela sua chave oficial quando desejar:
  pixKey: 'apoio@cruzadas.online',
  keyType: 'E-mail',
  beneficiaryName: 'Cruzadas.online - Apostolado e Cultura',
  city: 'Brasilia',

  // Código Pix "Copia e Cola" fictício de exemplo (Padrão BR Code Banco Central)
  // Substitua pela string gerada pelo seu banco:
  pixCopiaECola:
    '00020126580014br.gov.bcb.pix0136apoio@cruzadas.online5204000053039865802BR5925CRUZADAS ONLINE APOSTOLAD6008BRASILIA62070503***6304E8A2',

  // Caminho da imagem do QR Code (salva na pasta public/ do frontend)
  qrCodeImageUrl: '/pix-qrcode.svg',
};
