import {defineConfig} from 'vite';
export default defineConfig({build:{rollupOptions:{output:{manualChunks:{three:['three'],post:['three/addons/postprocessing/EffectComposer.js','three/addons/postprocessing/UnrealBloomPass.js'],network:['socket.io-client']}}}}});
