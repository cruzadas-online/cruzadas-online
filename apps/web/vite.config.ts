/// <reference types="vitest/config" />
import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

// https://vite.dev/config/
export default defineConfig({
  plugins: [react()],
  test: {
    globals: true,
    environment: 'jsdom',
    setupFiles: './src/test/setup.ts',
  },
  server: {
    host: true,
    port: 5175,
    proxy: {
      '/api': {
        target: 'http://localhost:5185',
        changeOrigin: true,
      },
    },
  },
})
