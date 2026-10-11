import { describe, it, expect, beforeEach } from 'vitest';
import {
  getPlayTrackerData,
  trackGameStarted,
  trackGameCompleted,
  markUserDonated,
  recordPromptShown,
  shouldPromptDonation,
} from './playTracker';

describe('playTracker utility', () => {
  beforeEach(() => {
    localStorage.clear();
  });

  it('inicia com contagens zeradas e sem doação', () => {
    const data = getPlayTrackerData();
    expect(data.startedCount).toBe(0);
    expect(data.completedCount).toBe(0);
    expect(data.hasDonated).toBe(false);
    expect(data.lastPromptAtPlayCount).toBe(0);
    expect(shouldPromptDonation()).toBe(false);
  });

  it('incrementa partidas iniciadas', () => {
    trackGameStarted();
    trackGameStarted();
    const data = getPlayTrackerData();
    expect(data.startedCount).toBe(2);
    expect(data.completedCount).toBe(0);
    expect(shouldPromptDonation()).toBe(false);
  });

  it('dispara prompt na 3ª partida jogada pela primeira vez', () => {
    trackGameStarted();
    trackGameStarted();
    expect(shouldPromptDonation()).toBe(false);

    trackGameStarted(); // 3ª partida
    expect(shouldPromptDonation()).toBe(true);

    // Registra que exibiu o prompt
    recordPromptShown();
    expect(shouldPromptDonation()).toBe(false);
  });

  it('após o primeiro prompt, aguarda 5 novas partidas para disparar novamente', () => {
    // 3 partidas iniciais
    trackGameStarted();
    trackGameStarted();
    trackGameStarted();
    recordPromptShown(); // prompt aos 3

    // 4ª, 5ª, 6ª, 7ª partidas
    trackGameStarted();
    trackGameStarted();
    trackGameStarted();
    trackGameStarted();
    expect(shouldPromptDonation()).toBe(false);

    // 8ª partida (5 partidas após a 3ª)
    trackGameStarted();
    expect(shouldPromptDonation()).toBe(true);
  });

  it('quando o usuário marca que doou, estende o intervalo para 15 partidas e marca hasDonated', () => {
    trackGameStarted();
    trackGameStarted();
    trackGameStarted();

    const donorData = markUserDonated();
    expect(donorData.hasDonated).toBe(true);
    expect(donorData.donatedAt).toBeDefined();
    expect(shouldPromptDonation()).toBe(false);

    // Joga mais 14 partidas (total 17)
    for (let i = 0; i < 14; i++) {
      trackGameStarted();
    }
    expect(shouldPromptDonation()).toBe(false);

    // 15ª partida após doação (total 18)
    trackGameStarted();
    expect(shouldPromptDonation()).toBe(true);
  });

  it('incrementa contagem de partidas completas', () => {
    trackGameStarted();
    trackGameCompleted();
    const data = getPlayTrackerData();
    expect(data.startedCount).toBe(1);
    expect(data.completedCount).toBe(1);
  });
});
