/**
 * Gerenciamento do histórico e contagem de partidas do usuário
 * para exibição contextual do modal de doação/apoio.
 *
 * Utiliza localStorage no navegador para total privacidade (sem necessidade
 * de gravar IPs de visitantes nem violar LGPD), com persistência confiável.
 */

const STORAGE_KEY = 'cruzadas_play_tracker';

export interface PlayTrackerData {
  /** Quantidade de partidas iniciadas (parcial ou completa) */
  startedCount: number;
  /** Quantidade de partidas finalizadas */
  completedCount: number;
  /** Se o usuário já marcou que contribuiu */
  hasDonated: boolean;
  /** Data/hora ISO de quando marcou a contribuição */
  donatedAt?: string;
  /** Quantas partidas ele jogou até a última vez que o modal foi exibido automaticamente */
  lastPromptAtPlayCount: number;
}

const DEFAULT_DATA: PlayTrackerData = {
  startedCount: 0,
  completedCount: 0,
  hasDonated: false,
  lastPromptAtPlayCount: 0,
};

/**
 * Lê os dados atuais do tracker no localStorage
 */
export function getPlayTrackerData(): PlayTrackerData {
  try {
    const raw = localStorage.getItem(STORAGE_KEY);
    if (!raw) return { ...DEFAULT_DATA };
    const parsed = JSON.parse(raw);
    return {
      startedCount: typeof parsed.startedCount === 'number' ? parsed.startedCount : 0,
      completedCount: typeof parsed.completedCount === 'number' ? parsed.completedCount : 0,
      hasDonated: Boolean(parsed.hasDonated),
      donatedAt: parsed.donatedAt,
      lastPromptAtPlayCount: typeof parsed.lastPromptAtPlayCount === 'number' ? parsed.lastPromptAtPlayCount : 0,
    };
  } catch {
    return { ...DEFAULT_DATA };
  }
}

/**
 * Salva os dados no localStorage
 */
function savePlayTrackerData(data: PlayTrackerData): void {
  try {
    localStorage.setItem(STORAGE_KEY, JSON.stringify(data));
  } catch {
    // Ignora se o localStorage estiver bloqueado pelo navegador
  }
}

/**
 * Registra o início de uma partida (seja via catálogo ou partida rápida).
 * Conta tanto partidas parciais quanto completas.
 */
export function trackGameStarted(): PlayTrackerData {
  const current = getPlayTrackerData();
  const updated: PlayTrackerData = {
    ...current,
    startedCount: current.startedCount + 1,
  };
  savePlayTrackerData(updated);
  return updated;
}

/**
 * Registra a conclusão com sucesso de uma partida.
 */
export function trackGameCompleted(): PlayTrackerData {
  const current = getPlayTrackerData();
  const updated: PlayTrackerData = {
    ...current,
    completedCount: current.completedCount + 1,
  };
  savePlayTrackerData(updated);
  return updated;
}

/**
 * Marca que o usuário realizou uma contribuição.
 * Isso alonga o prazo para qualquer nova exibição do modal e ativa mensagem personalizada de gratidão.
 */
export function markUserDonated(): PlayTrackerData {
  const current = getPlayTrackerData();
  const updated: PlayTrackerData = {
    ...current,
    hasDonated: true,
    donatedAt: new Date().toISOString(),
    // Reseta o marco de exibição para a contagem atual de partidas
    lastPromptAtPlayCount: current.startedCount,
  };
  savePlayTrackerData(updated);
  return updated;
}

/**
 * Registra que o prompt automático de doação foi exibido.
 */
export function recordPromptShown(): PlayTrackerData {
  const current = getPlayTrackerData();
  const updated: PlayTrackerData = {
    ...current,
    lastPromptAtPlayCount: current.startedCount,
  };
  savePlayTrackerData(updated);
  return updated;
}

/**
 * Avalia se o modal de doação deve ser aberto automaticamente.
 * 
 * Regra:
 * - O melhor momento para o usuário é após concluir uma partida (ou desistir),
 *   pois o fluxo cognitivo de jogar terminou e ele já experimentou valor real.
 * - Usuário não doador: aparece pela primeira vez após 3 partidas jogadas,
 *   e depois a cada 5 partidas (ex: 3ª, 8ª, 13ª...).
 * - Usuário que já doou: prazo estendido para cada 15 partidas (ex: 15 partidas após doar),
 *   com mensagem personalizada de agradecimento pelo apoio contínuo.
 */
export function shouldPromptDonation(): boolean {
  const data = getPlayTrackerData();

  // Precisa ter jogado pelo menos 3 vezes no total
  if (data.startedCount < 3) {
    return false;
  }

  // Se já doou, intervalo estendido: a cada 15 partidas desde a última doação / prompt
  if (data.hasDonated) {
    const playsSinceLastPrompt = data.startedCount - data.lastPromptAtPlayCount;
    return playsSinceLastPrompt >= 15;
  }

  // Primeira vez: exatamente na 3ª partida jogada
  if (data.lastPromptAtPlayCount === 0) {
    return data.startedCount >= 3;
  }

  // Próximas vezes: a cada 5 partidas jogadas
  const playsSinceLastPrompt = data.startedCount - data.lastPromptAtPlayCount;
  return playsSinceLastPrompt >= 5;
}
