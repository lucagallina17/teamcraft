# dotnet-app/ — Contesto di progetto

## ⚠️ Promemoria di fine sessione
Quando la conversazione sembra concludersi (l'utente saluta, dice che ha
finito, o passano diversi minuti senza nuove richieste), ricordami
esplicitamente di digitare `/save-session` per salvare i punti salienti
prima di chiudere. Non salvare automaticamente: aspetta che io lo richieda
o confermi.

## Chi sono
Luca, Senior Front-End Developer, sta imparando ASP.NET Core / .NET backend
costruendo questo progetto reale. Obiettivo: deploy su Azure App Service,
uso di Entity Framework Core per il database, portfolio dimostrabile per
posizionamento come full-stack .NET/Azure.

## Stack e convenzioni
- Backend: .NET 8, EF Core 8, SQL Server (LocalDB in locale). Clean Architecture a 4 progetti: `TeamCraft.Domain`, `.Application`, `.Infrastructure`, `.API`, più `TeamCraft.Tests` (xUnit). Migrato da .NET 7 il 2026-09-18 perché .NET 7 è EOL e non più selezionabile come stack su Azure App Service.
- Librerie: MediatR (Command/Query + Domain Event in-memory), MassTransit (Integration Event, oggi In-Memory Transport, nessun broker reale collegato), Moq (test).
- Frontend: Angular 21, standalone components, Signals, PrimeNG, Reactive Forms. Font Inter, palette Apple-style (`#0071e3` accent).
- Deploy: backend live su Azure (Free Trial) — App Service (tier F1, Linux, .NET 8) + Azure SQL Database (tier Basic), deploy da Visual Studio → Publish. Frontend in corso di pubblicazione su Azure Static Web Apps (Free) con GitHub Actions.

## Convenzioni di questo repository
- Codice sorgente: `src/`
- Documentazione di decisioni architetturali: `docs/`
- Log di sessione: `log/<data>-sessione.md`
- Prima di modifiche architetturali importanti, spiegare il trade-off e
  chiedere conferma esplicita
- Vedi `.claude/agent-memory/dotnet-dev/MEMORY.md` per lo stato di dettaglio
  aggiornato (decisioni prese, pattern ricorrenti, problemi già risolti)
