import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

export default defineConfig({
  plugins: [react()],
  server: {
    port: 5173,
    proxy: {
      '/api': {
       target: 'https://localhost:59241',
       changeOrigin: true,
       secure: false,
        changeOrigin: true
      }
    }
  }
})
