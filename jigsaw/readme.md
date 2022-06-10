# jigzaw puzzle

8 jun 12:05 -> planejamento - 5h restantes

## workflow:
 - protótipo peças quadradas - 30min
 - adicionar imagem - 10min
 - forma das peças - 50min
 - efeito sonoros - 30min
 - musica simples - 30min
 - tela inicial (iniciar, configurações volume, sair) - 1h
 - pagina no itch - 30min

## protótipo peças quadradas:
  - grid de quads que guarda posição
  - movimentar peças da grid com o mouse
  - se peça estiver próxima da posição original quando solta, snap

## adicionar imagem
  - adicionar propriedade de coordenada de textura na grid de quads
  - adicionar imagem referenciada

## forma das peças
  - shader que cropa a imagem em cada lado baseada em um identificador ( borda, junta interna, junta externa, nada )

## efeito sonoros
 - efeitos para botoes
 - efeito para snap
 - efeitos para grab

## musica simples
 - piano relaxante de fundo
 - forma ABA
 - voz principal + acompanhamento simples
 - ternário

## tela inicial
 - logo no topo
 - opções centralizadas na parte inferior
  - iniciar
  - opções
   - label volume
    - musica
    - sfx
   - label graficos
    - paleta?
  - sair

## fim: 8 jun 12:30 - 4h35 min restantes

### 8 jun 17:12 -> prototipo peças quadradas
- iniciar projeto: fim: 8 jun 18:10 - 3h37 min restantes
- grid de quads que guarda posição - 18:21
- movimentar peças da grid com o mouse - 18:38
- se peça estiver próxima da posição original quando solta, snap - 18:43
fim 18:43

![First day](https://media.discordapp.net/attachments/338374677116354561/984472879112540260/unknown.png?width=785&height=473)

### 9 jun 12:12 -> adicionar imagem
- adicionar propriedade de coordenada de textura na grid de quads - 12:44
- adicionar imagem referenciada - 12:44
add bug: ultima peça clickada tem que ter prioridade na visualização, ser desenhada por ultimo

![Second day](https://media.discordapp.net/attachments/338374677116354561/984486504585887844/unknown.png)

### 9 jun 19:01 -> forma das peças
- ultima peça clickada tem que ter prioridade na visualização, ser desenhada por ultimo - 19:13
- peças das laterais livre de juntas - 20:20
- shader que cropa a imagem em cada lado baseada em um identificador ( borda, junta interna, junta externa, nada ) - 20:20
- algoritmo de juntas - < 21:25
- peças ocupam casa toda - incompleto - tentado novamente: 22:52
fim 22:04 - novo fim: 22:52

observações:
- adicionar fundo
- adicionar bordas arredondadas nas peças dos cantos

![Third day](https://media.discordapp.net/attachments/338374677116354561/984823442379522078/unknown.png?width=790&height=473)

### 10 jun 11:18 observações
- arredondar juntas - 11:38
- adicionar bordas arredondadas nas peças dos cantos - 11:51
- adicionar fundo - 12:13