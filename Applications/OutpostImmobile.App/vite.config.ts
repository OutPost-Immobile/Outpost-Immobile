import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'
import electron from 'vite-plugin-electron'

export default defineConfig({
  plugins: [
    react(),
    electron({
      // Entry point for the Electron main process
      entry: 'main.ts',
    }),
  ],
})