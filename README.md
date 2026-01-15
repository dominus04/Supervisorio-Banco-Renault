# Supervisório Banco Renault

## Sistema de Teste de Estanqueidade (Radiadores)
Foco: Qualidade, Rastreabilidade e Integração de Hardware.

Título: Industrial Leak Test Supervisor – C# / WPF

Descrição: > Sistema desenvolvido para automatizar o processo de teste de vazamento em radiadores, eliminando o erro humano e garantindo 100% de rastreabilidade. O software gerencia desde a entrada do serial da peça até a impressão da etiqueta de aprovação, integrando-se diretamente ao PLC da máquina.

### Principais Funcionalidades:

- **Controle de Rastreabilidade:** Armazenamento de dados de produção e verificação de duplicidade de seriais.

- **Gestão de Receitas:** Configuração dinâmica de parâmetros de teste para diferentes modelos de produtos.

- **Módulo de Impressão:** Geração e modelagem de etiquetas térmicas após validação técnica.

- **Monitoramento em Tempo Real:** Interface intuitiva para acompanhamento do status da máquina.

### Desafio Técnico Superado:

- **Produção Concorrente:** Implementação de monitoramento de estados simultâneos, permitindo que o sistema gerencie duas etapas produtivas distintas ao mesmo tempo, otimizando o cycle time da máquina.

- **Otimização da Lógica Bit-a-Bit:** Evolução do controle de estados via TCP/IP. Migração de um sistema de "set/reset" via software para uma lógica de monitoramento de ciclo baseada em eventos no PLC, eliminando falhas de sincronismo de rede e garantindo a integridade dos dados de produção.
