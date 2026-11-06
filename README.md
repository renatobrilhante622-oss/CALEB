# 🌈 CALEB BLOX WORLD

Three.js r170 + Vite, Express/Socket.io, PWA e projeto Android Capacitor. Os scripts Unity anteriores permanecem em `Assets/` como referência; a aplicação ativa é WebGL.

## Rodar

Node 22+ e npm:

```sh
npm ci
npm run assets
npm run build
npm start
```

Abra o endereço da máquina na porta 3000. O servidor imprime os endereços LAN. No PC: WASD, Espaço e arraste a câmera com o mouse. No celular: joystick esquerdo, arraste o cenário para girar a câmera e botão de pulo. Pise nos pads para interagir. Toque nos pets para carinho. A esteira fica à esquerda do ponto inicial: fique sobre ela para ganhar moedas.

Fluxo: esteira → ovo rosa (25 moedas) → pets → renda passiva → fusão (dois primeiros pets, 10 moedas) ou variante elemental (primeiro pet, 40 moedas) → arenas nos quatro cantos → ovo de boss → incubadora dourada. Os oito primeiros pets acompanham o avatar. O santuário tem 15 níveis; a esteira, seis. O parque tem checkpoints e recuperação sem perda; na cidade há direção e voo. Chapéus e asas no launcher são gratuitos. A engrenagem abre os presentes dos responsáveis após desafio aritmético; isso é uma barreira infantil, não autenticação de segurança.

## Offline e LAN

O build contém código, modelos e texturas locais. Nenhum CDN é necessário em runtime. O service worker precacheia os arquivos do build; é necessário concluir a primeira carga em contexto seguro (HTTPS ou localhost). Navegadores não permitem service worker em HTTP por IP LAN. O APK Capacitor inclui os arquivos no aparelho e funciona offline sem service worker.

Para LAN, rode o servidor em um computador na mesma rede e abra o **mesmo endereço** nos três aparelhos; permita TCP 3000 no firewall. Crie uma sala no primeiro e toque Entrar no outro. Há atualização de posição, emojis e dano compartilhado de boss. O servidor anuncia UDP 47777, mas browsers não escutam UDP: descoberta automática entre servidores/dispositivos nativos exigiria plugin Capacitor de discovery, ainda não implementado. A lista atual descobre salas dentro do servidor que já foi aberto. Não se promete conexão em dois segundos em redes com isolamento de clientes.

O APK atual é offline: conectar a um host LAN externo requer configurar a URL Socket.io e a política Android de tráfego local. Essa integração nativa ainda precisa ser feita e validada. Presentes entre jogadores, construção de bases compartilhadas, pets remotos e sincronização de inventário não estão implementados. O servidor aplica dano fixo e limite de frequência para bosses. Os saves individuais não são autoridade multiplayer. Não exponha o servidor familiar à internet.

Progresso é salvo em localStorage a cada cinco segundos e nas transações, com cópia anterior de recuperação. Conectar ao servidor cria também uma cópia em `.data/`; restauração dessa cópia ainda é manual. Não há credenciais nem serviços de nuvem. TTS depende das vozes instaladas no Android; os efeitos WebAudio são locais. O modo calmo começa ligado.

## Android

```sh
npm run android:sync
npx cap open android
```

O projeto nativo está em `android/`. No Android Studio, instale o SDK solicitado pelo Gradle, use JDK 21 e gere um APK de debug para testar; assinatura de release é feita pelo responsável. Fixe orientação Landscape se desejar nas configurações da Activity. Este ambiente gerou e sincronizou o projeto, **não produziu nem testou APK**.

## Validação

```sh
npm test
# Em outro terminal: npm start
node tests/lan.mjs
node tests/browser.mjs
```

O teste de navegador usa Chromium em `/usr/bin/chromium` e WebGL via SwiftShader; ajuste o caminho em outro SO. Testa carga, moedas, incubação, fusão e transição dos três mapas; salva captura em `Docs/gameplay.png`. Não equivale a teste de performance de GPU móvel ou Wi-Fi físico.

## Escopo visual e limitações

Avatar articulado, pets compostos arredondados, olhos, asas e coroas, veículos, árvores, incubadoras translúcidas, sombras, bloom, nuvens e partículas com pool. Existem 40 entradas de catálogo, com famílias visuais compartilhadas; 20 índices de chapéus e dez de asas combinam formas e cores, não são 30 modelos artísticos distintos. Os GLBs RobotExpressive e Parrot ficam no mundo como personagens decorativos animados. Créditos em `public/assets/ASSET-SOURCES.json` e `Docs/ASSETS.md`.

Esta versão tem um ciclo de jogo executável, mas **não corresponde ainda a uma produção AAA completa**. Faltam animação detalhada de incubação/fusão, seleção visual dos ingredientes, música, drop carregável de ovo, combate com investidas de pets, física de veículos, colisão de prédios, escorregadores funcionais e validação Android/LAN real. Não há microtransações, anúncios ou punição por queda. O percurso Obby usa plataformas de salto; pulos precisam ser testados com a criança e ajustados conforme conforto.
