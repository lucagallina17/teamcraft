---
description: Salva i punti salienti della sessione corrente nel log e aggiorna la memoria essenziale di dotnet-dev
---

Fai queste due cose in ordine, in questo repository:

1. Scrivi un riassunto conciso dei punti salienti di questa sessione in un
   nuovo file `log/<data-di-oggi>-sessione.md` (formato YYYY-MM-DD). Includi:
   cosa è stato implementato o modificato, decisioni architetturali prese e
   perché, problemi risolti, problemi ancora aperti.

2. Aggiorna `.claude/agent-memory/dotnet-dev/MEMORY.md` con SOLO lo stato
   essenziale attuale (non duplicare il dettaglio già scritto nel log):
   architettura corrente, convenzioni adottate, pattern ricorrenti nel
   codice, prossimo passo consigliato. Se MEMORY.md si avvicina al limite di
   200 righe, accorcia le parti meno rilevanti invece di continuare ad
   aggiungere.

Conferma a fine operazione cosa hai scritto e dove.
