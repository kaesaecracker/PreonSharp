import {defineConfig} from 'vite';
import react from '@vitejs/plugin-react';
import {VitePWA} from 'vite-plugin-pwa';

// https://vitejs.dev/config/
export default defineConfig({
  plugins: [
    react(),
    VitePWA({
      registerType: 'autoUpdate',
      devOptions: {enabled: true},
      injectRegister: 'auto',
      workbox: {
        globPatterns: ['**/*.{js,css,html,ico,png,svg}']
      },
      pwaAssets: {
        image: 'favicon.svg',
        config: false
      },
      manifest: {
        name: "Preon# Web ",
        theme_color: '#0294EE',
        background_color: '#000000'
      }
    })
  ],
});
