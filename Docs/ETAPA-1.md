# Montagem da Etapa 1

## Projeto e cena

1. No Unity Hub, instale Unity 6 LTS 6000.0 com Android Build Support, SDK/NDK e OpenJDK. Crie um projeto 3D Built-In ou Universal 3D fora deste checkout. Copie `Assets/BloxWorld` para o `Assets` desse projeto. Use controle de versão também para o projeto Unity quando decidir sua localização definitiva.
2. Crie uma cena `Bootstrap`. Adicione um GameObject raiz `AppServices` com `SaveManager` e `GameManager` no MESMO objeto. Ambos persistem entre cenas; use somente um conjunto de serviços. A ordem de execução declarada carrega o JSON antes de inicializar o jogo.
3. Em `Assets`, crie uma pasta `PetDefinitions`. Pelo menu **Create > BloxWorld > Pet**, crie Cachorro (`dog`), Gato (`cat`) e Dragão (`dragon`). Os IDs devem ser únicos e estáveis. Configure cor, ícone opcional, renda, ataque e elemento. Arraste os três assets para `Pet Catalog` no GameManager. ScriptableObjects são definições compartilhadas; nunca grave progresso neles.
4. Crie um chão: Cube em `(0,-0.25,0)`, escala `(15,0.5,15)`. Crie `Player` em `(0,0,0)` e um filho `Visual` com `BlockAvatarGenerator`. O avatar mede aproximadamente 2,4 unidades e olha para +Z. A geração é visual; locomoção, animação, câmera seguidora e touch não pertencem à Etapa 1.
5. Crie um Material com shader Standard (Built-In) ou Universal Render Pipeline/Lit (URP) e atribua ao campo `Material Template`. Isso também mantém a referência ao shader no build Android. O fallback de shader serve ao protótipo no Editor. Para trails translúcidos, use um material transparente compatível; o material opaco padrão não fornece fade por alpha.
6. Coloque a câmera em `(4,3.5,6)`, apontando para `(0,1.2,0)`, e adicione uma Directional Light. Salve a cena e inclua-a em Build Profiles. Entre em Play: o avatar deve aparecer sem depender de modelos, texturas ou fontes externas.

## Arquitetura e contratos

- `SaveData`: DTO versionado com moedas `long`, níveis, limite de 3–8 pets equipados, pets individuais identificados por GUID, ascendência de fusão, ovos, construções, roupas e modo calmo. Campos de etapas futuras armazenam estado, sem implementar suas mecânicas antecipadamente.
- `PetData`: definição estática ScriptableObject, referenciada no JSON por ID. IDs ausentes no catálogo permanecem no save; `GetPet` retorna null. Resolver conteúdo removido exige migração deliberada.
- `SaveManager`: JSON UTF-8 em `Application.persistentDataPath/bloxworld.json`. Autosave a cada cinco segundos de tempo real, também em pausa, perda de foco e saída. Escreve `.tmp`, força flush e substitui o arquivo, guardando `.bak`. Um arquivo inválido é preservado com sufixo `.corrupt-*`; o backup é tentado em seguida. Versões futuras bloqueiam escrita para evitar downgrade destrutivo. Se o arquivamento falhar, escrita também é bloqueada. Não há reset automático destrutivo.
- `GameManager`: singleton persistente, catálogo e operações `AddCoins`, `TryPurchaseBuilding`, `GrantPet`, `SetEquipped`, `SetCalmMode` e `SaveAvatar`. Cada operação aceita atualiza a memória e tenta salvar imediatamente. Retorno de sucesso indica operação aceita; `PersistenceCompleted(bool)` informa se ela chegou ao disco. `SaveFailed` informa falha de I/O. A memória permanece disponível e o autosave tenta novamente. Valores de preço virão de configurações confiáveis nas próximas etapas.
- `BlockAvatarGenerator`: cubos para corpo, rosto, chapéu e asas, trail opcional; usa materiais próprios e remove recursos ao reconstruir. O modo calmo começa ligado e suprime o trail. Não há flashes, sons agudos ou punições nesta etapa. Atualizações do estado refletem a aparência no avatar. A otimização para atualizar apenas mudanças visuais pode ser feita quando houver ganho frequente de moedas.

`State` é exposto para integração; prefira os métodos do manager para alterações. Alterações diretas precisam de `SaveNow()` e não disparam eventos. Chamadas de Unity/serviços devem ocorrer na thread principal. A persistência é local, sem envio de dados. Os dados ainda não são autoridade multiplayer. Nenhum método de presente foi ligado a uma interface infantil; proteção parental será montada com a interface, sem prometer segurança através de um PIN fixo no código.

## Verificação funcional no Editor

As verificações abaixo são manuais e ainda precisam ser executadas em Unity. Este ambiente não possui Unity nem compilador C#.

1. Play: confira o avatar e Console sem erros. Se houver erro de catálogo, corrija IDs vazios ou duplicados antes de continuar.
2. Com debugger C# anexado ao Editor, pause e avalie `BloxWorld.GameManager.Instance.AddCoins(100)`; deve retornar true. Avalie `TryPurchaseBuilding("house-1", 30)`; deve retornar true e deixar 70 moedas. Repita a compra; deve retornar false e manter 70. Preços negativos e saldo insuficiente também devem falhar sem debitar.
3. Avalie `GrantPet("dog")` quatro vezes. Os quatro registros devem ter GUIDs distintos; apenas os três primeiros devem estar equipados. `GrantPet("missing")` deve retornar null. Pare e reinicie Play: saldo, construção e pets devem permanecer.
4. Avalie `SaveAvatar(new BloxWorld.AvatarData { hat = true, wings = true, trail = true })`. Confirme chapéu e asas. Trail só deve surgir ao desativar modo calmo e mover o Player. Reative modo calmo; trail deve desaparecer. Reinicie Play para verificar aparência persistida.
5. Feche Play. Faça uma cópia externa do diretório de save. Corrompa somente o arquivo principal com JSON inválido. Entre em Play: deve carregar `.bak` e preservar o principal inválido em `.corrupt-*`. O backup representa a gravação anterior e pode ter até uma transação a menos. Restaure sua cópia depois do teste.
6. Teste um arquivo com `version: 2`: deve registrar aviso e impedir escrita. Teste também falha de permissão de escrita e observe `SaveFailed`/Console. Não confunda atualização em memória com persistência concluída.
7. No Android físico, faça um build Development, habilite orientação Landscape e confira visualização, material, retomada após ir para background e conservação de dados ao reabrir. Remover o app apaga o save local. Interrupção forçada pode perder alterações ainda não persistidas; valide substituição de arquivo no backend Android escolhido antes de distribuir.

## Limites e próxima etapa

Não há esteira, compra por proximidade, combate, incubação, launcher ou LAN nesta entrega. Não há dependências online em runtime. A próxima etapa adicionará esteira, moedas visuais com pooling e pads. Não foram gerados APK, cena binária ou resultados de testes Unity neste ambiente.
