# TeamCraft — Handoff per agent

Sistema di talent matching: collega dipendenti, competenze e progetti; genera proposte di team ottimali tramite algoritmo di matching.

## Stack tecnico

**Backend**: .NET 8, EF Core 8, SQL Server (LocalDB in locale). Clean Architecture a 4 progetti: `TeamCraft.Domain`, `.Application`, `.Infrastructure`, `.API`, più `TeamCraft.Tests` (xUnit). Migrato da .NET 7 a .NET 8 il 2026-09-18 in vista del deploy su Azure (.NET 7 è EOL, non più selezionabile come stack per nuovi Web App).
**Librerie**: MediatR (Command/Query + Domain Event in-memory), MassTransit (Integration Event, oggi In-Memory Transport, nessun broker reale collegato), Moq (test).
**Frontend**: Angular 21, standalone components, Signals, PrimeNG, Reactive Forms. Font Inter, palette Apple-style (`#0071e3` accent).
**Deploy**: backend **live su Azure** (Free Trial) — App Service `teamcraft-api-g5bfaecpdjevf3cf` (tier F1, Linux, .NET 8) + Azure SQL Database (tier Basic), connessione end-to-end funzionante (migration auto-applicate all'avvio). Deploy va fatto da **Visual Studio → Publish** (Azure App Service Linux) — il drag&drop Kudu/QuickDeploy nel Portale è inutilizzabile per binari già compilati (dettagli in `log/2026-09-18-sessione.md`). Frontend non ancora deployato.

## Regole di dipendenza (Clean Architecture) — non violare mai

```
API → Application → Domain
Infrastructure → Application → Domain
```

Domain non conosce nessun altro livello. Application può usare Domain liberamente. Eccezioni di dominio (`DomainRuleViolationException`, `EntityNotFoundException`, `EntityInUseException`) vivono in `Domain/Exceptions`, usate ovunque servano.

## Decisioni architetturali chiave

- **DDD parziale**: `TeamAggregate` e `TeamReviewAggregate` sono veri Aggregate Root (proprietà `private set`, regole di business incapsulate, costruttori che garantiscono stato valido). Il resto delle entity (`Employee`, `Project`, `Competency`, `ProjectRole`) resta **anemico** — migrazione a DDD non ancora estesa lì.
- **`ProjectRoleRequirementCompetency` collegata a `ProjectRoleRequirement`**, non a `ProjectRole` — permette competenze diverse per lo stesso ruolo su progetti diversi.
- **Due Aggregate non si referenziano mai direttamente** — solo tramite Id (es. `TeamReviewAggregate.TeamId`, mai un riferimento a `TeamAggregate`). Regole cross-aggregate vivono nel Command Handler, mai negli Aggregate.
- **Algoritmo di matching**: backtracking con potatura (non greedy — il greedy non garantiva l'ottimo globale, bug reale trovato in test). Considera competenze (peso, livello minimo, Required/Preferred) + bonus di affinità tra dipendenti (fattore 0.1x, minoritario). Esclude dipendenti già in team `Active` su altri progetti.
- **CQRS**: repository separati per Command (`ITeamAggregateRepository`, lavora su Aggregate) e Query (`ITeamReadRepository`, proiezione diretta con `.Select()` verso DTO, niente entity intermedia).
- **Domain Event vs Integration Event**: `TeamActivatedEvent` (Domain, in Domain/Events, via MediatR Publish, sincrono in-process) è distinto da `TeamActivatedIntegrationEvent` (Application/IntegrationEvents, via MassTransit, pensato per sistemi esterni). Il primo attiva `Project` in modo disaccoppiato tramite `ActivateProjectOnTeamActivatedHandler`.
- **Sync Team↔Project**: il progetto passa a `Active` SOLO quando il team passa a `Active` (non alla creazione/proposta). Nessuna sync automatica quando il team passa a `Closed` (scelta esplicita).
- **TeamStatus semplificato a 3 valori** (`Proposed`, `Active`, `Closed`) — `Approved` era ridondante con `Active`, rimosso dopo overengineering iniziale.
- Eliminazione fisica di un `Project` permessa solo se `Status == Draft`; oltre quello, solo `Cancelled` (via cambio stato, non delete).

## Convenzioni di codice

- DTO sempre con **enum tipizzati**, mai `string`, quando il campo corrisponde a un enum di dominio.
- Ogni DTO di riga in una collezione **deve esporre il proprio Id** (necessario per update/delete mirati) — errore ripetuto più volte, ora è una regola esplicita da applicare sempre.
- Naming repository: `I{Entity}Repository` in `Application/Repositories`, implementazione in `Infrastructure/Persistence/Repositories`. Niente eredità da `GenericRepository<T>` per repository "su misura" (Command/Query separati) — solo i metodi realmente serviti dallo use case.
- Backing field per collezioni di Aggregate: `private readonly List<T> _x`, esposto con `IReadOnlyList<T> X => _x.ToList()`. In EF Core config: `builder.Navigation(...).HasField("_x").UsePropertyAccessMode(PropertyAccessMode.Field)`.
- Costruttori di entity interne ad un Aggregate (es. `TeamMember`) sono `internal` — costruibili solo dall'Aggregate Root. Metodo `CreateLegacy` in `TeamMember` è un compromesso temporaneo per codice non ancora migrato (commentato con TODO).
- Relazioni EF Core tra due entità: **mai lasciare `WithMany()` vuoto se l'altra entità ha già una collection di navigazione** — causa colonne fantasma tipo `XxxId1` e conflitti di FK (vedi sezione problemi).
- Frontend: stili di sistema condivisi (`.page`, `.icon-btn`, `.empty-state`, bottoni pill) centralizzati in `styles.scss` globale, non duplicati per componente. `takeUntilDestroyed(this.destroyRef)` sempre come **ultimo operatore** della pipe RxJS, mai a metà catena (specialmente con `concatMap`).
- Toast/notifiche centralizzate via `NotificationService` (wrapper su `MessageService` PrimeNG) — mai `alert()` nativo.

## Pattern ricorrenti da applicare sempre

1. Ogni nuova regola di stato introdotta va verificata contro **tutte** le azioni CRUD esistenti sulla stessa entità (storico bug: introdotta una regola su un bottone, dimenticata sul suo gemello in un'altra pagina/lista).
2. Azioni distruttive o con effetti collaterali a cascata (es. cambio stato che attiva un'altra entità) richiedono conferma esplicita (`ConfirmService`), non solo le eliminazioni.
3. Prima di introdurre un enum con più di 2-3 valori, verificare che ogni stato rappresenti davvero un momento distinto nel processo reale (evita over-engineering, vedi `TeamStatus.Approved` rimosso).
4. Query che alimentano decisioni con pareggi possibili (es. algoritmo di matching) devono avere un **tie-break deterministico** esplicito — mai fidarsi dell'ordine implicito del database.

## Problemi già risolti (non ripetere)

- **EF Core versione errata**: la major di EF Core deve sempre corrispondere esplicitamente al TargetFramework (oggi .NET 8 → EF Core 8.x.x), mai lasciare che NuGet installi l'ultima assoluta (potrebbe richiedere una TFM più recente di quella del progetto).
- **Backing field non configurato** → EF Core non sa scrivere su collection con solo getter: serve `HasField` + `UsePropertyAccessMode(Field)` esplicito in ogni Aggregate con collection privata.
- **`WithMany()` vuoto con collection reale dall'altro lato** → genera FK duplicate (`XxxId1`) e conflitti di cascata. Sempre specificare esplicitamente: `.WithMany(p => p.Teams)`.
- **Due entità (`Team` anemico + `TeamAggregate`) mappate in EF Core contemporaneamente** → conflitti di relazione ambigui. La migrazione a un Aggregate va fatta per intero, mai a metà (coesistenza temporanea è instabile a livello di schema DB, non solo di codice).
- **Snapshot EF Core disallineato dal DB fisico** dopo una migration fallita a metà → una migration generata risulta "vuota" pur avendo residui reali nel DB; risolvibile con intervento SQL diretto (`DROP TABLE` manuale) quando EF Core "non vede" la discrepanza.
- **MediatR `IMediator` vs MassTransit `MassTransit.Mediator.IMediator`**: stesso nome, namespace diverso, autocomplete IDE può importare quello sbagliato → errori di compilazione fuorvianti (`CS0815` su `var`). Sempre verificare l'`using`.
- Per usare MediatR `Send`/`Publish`, Command/Event devono implementare esplicitamente `IRequest<T>`/`INotification` — non basta che l'Handler abbia la firma giusta "per coincidenza". Handler: firma esatta `Handle(T request, CancellationToken ct)`.
- **MassTransit 9.x richiede licenza a pagamento** anche per In-Memory Transport (crash all'avvio con `ConfigurationException`) — restare su **8.5.10** (ultima major open-source) finché non si valuta l'acquisto di una licenza.
- **Visual Studio non compila `net8.0`** (NETSDK1045) se la versione dell'IDE è troppo vecchia, anche con l'SDK giusto installato a livello di sistema (VS usa un toolset MSBuild interno proprio) — serve VS ≥ 17.8.

## Stato attuale

- Migrazione `Team` → `TeamAggregate` **completata** in tutto il progetto (Service, Repository, Controller, DB). Nessuna coesistenza residua.
- Catena end-to-end funzionante: `TeamsController` → `IMediator.Send(ActivateTeamCommand)` → `TeamAggregate.Activate()` → `Publish(TeamActivatedEvent)` → `ActivateProjectOnTeamActivatedHandler` aggiorna `Project`.
- `TeamReviewAggregate` (feature recensioni fine-progetto) implementata end-to-end: Aggregate, repository, Command/Handler con verifica cross-aggregate, EF Core config, migration applicata.
- Autenticazione JWT: **non implementata**. `CreatedBy` su `Project` è un placeholder.
- MassTransit: solo In-Memory Transport, mai collegato a un vero broker (RabbitMQ/Kafka).
- Test: xUnit su `TeamAggregate`, `TeamReviewAggregate`, `ActivateTeamCommandHandler`, `SubmitTeamReviewCommandHandler` (con Moq, incl. doppio mock coordinato). 9/9 verdi dopo upgrade a .NET 8.
- **Deploy Azure backend: completato e verificato** (2026-09-18) — vedi riga Deploy sopra e `log/2026-09-18-sessione.md` per il dettaglio del percorso (parecchi ostacoli di tooling Azure/VS, tutti risolti).
- **Nessun commit Git nel repository** (`git log` → "does not have any commits yet") nonostante tutto il lavoro fatto finora — da commitare.

## Prossimi passi

1. **Committare il lavoro fatto finora** (repo attualmente a zero commit).
2. Deploy del frontend Angular (probabile Azure Static Web Apps) + aggiornare `Cors:AllowedOrigins` con l'origin di produzione.
3. Autenticazione JWT (mai iniziata).
4. Rivalutare il target framework prima di novembre 2026 (fine supporto .NET 8 LTS) — probabile prossimo step .NET 10.
5. Valutare se estendere DDD a `Project` (oggi anemico) — `project.Status = ...` scritto direttamente in `ActivateProjectOnTeamActivatedHandler`, nessuna protezione di dominio.
6. Sistema di notifiche/toast: completo lato frontend, verificare copertura errori su tutti gli endpoint nuovi (TeamReview).
7. Pulizia: rimuovere la cartella residua `src/backend/TeamCraft/TeamCraft.API/` (vecchio scaffold, non referenziata dalla `.sln`).
