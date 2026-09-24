# TeamCraft — Handoff per agent

Sistema di talent matching: collega dipendenti, competenze e progetti; genera proposte di team ottimali tramite algoritmo di matching.

## Stack tecnico

**Backend**: .NET 8, EF Core 8, SQL Server (LocalDB in locale). Clean Architecture a 4 progetti: `TeamCraft.Domain`, `.Application`, `.Infrastructure`, `.API`, più `TeamCraft.Tests` (xUnit). Migrato da .NET 7 a .NET 8 il 2026-09-18 in vista del deploy su Azure (.NET 7 è EOL, non più selezionabile come stack per nuovi Web App).
**Librerie**: MediatR (Command/Query + Domain Event in-memory), MassTransit (Integration Event, oggi In-Memory Transport, nessun broker reale collegato), Moq (test).
**Frontend**: Angular 21, standalone components, Signals, PrimeNG, Reactive Forms. Font Inter, palette Apple-style (`#0071e3` accent).
**Deploy (tutto live, account Azure personale di Luca, RG `teamcraft-rg`)**: API su App Service (F1, Linux, .NET 8, **Sweden Central**) `teamcraft-api-haawg0cfgzdpdxgn`, Azure SQL Basic, frontend su **Static Web Apps Free** `red-stone-04625aa0f.1.azurestaticapps.net` via **GitHub Actions** (push su `main` ripubblica). Repo: `github.com/lucagallina17/teamcraft`. Configurazione via env var dell'App Service, mai nel repo: `ConnectionStrings__DefaultConnection`, `Cors__AllowedOrigins__0` (origin della SWA, senza slash finale). Backend si pubblica ancora **a mano da Visual Studio → Publish** (il drag&drop Kudu/QuickDeploy è inutilizzabile per binari già compilati). Le regioni "West Europe" risultano bloccate per nuovi clienti su questo account (`RequestDisallowedByAzure`). Dettagli in `log/2026-09-18-sessione.md` e `log/2026-09-21-sessione.md`.

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
- **Autenticazione JWT** (2026-09-23): `ITokenService`/`TokenService` con lo stesso pattern interfaccia-in-Application/implementazione-in-Infrastructure dei repository (generare un token è un dettaglio tecnico, non business logic). `AuthService.RegisterAsync` collega l'account cercando l'`Employee` per **email** (non più per Id scelto da una tendina): evita che un utente colleghi per errore o apposta l'account a un'altra persona. Ruolo di `UserAccount` sempre `"Employee"` fisso lato server, mai scelto dal client (vedi Problemi già risolti). Nessun endpoint ha ancora `[Authorize]`.

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
5. **Mai lasciare che un campo controllato dal client determini un dato rilevante per l'autorizzazione o l'identità** — trovati due bug reali di questo tipo nella registrazione (ruolo scelto liberamente dal client = privilege escalation; `EmployeeId` scelto da una tendina = furto d'identità di un altro dipendente). Ogni volta che un endpoint pubblico scrive `Role`, `EmployeeId`/`UserId`, o campi simili, chiedersi esplicitamente "chi decide questo valore, il server o chi chiama?".

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
- **Repo pubblico: scansionare prima del primo push** (`git add -n .`, `git grep` per email/GUID/password). I profili di publish di Visual Studio (`Properties/PublishProfiles/*.pubxml`, `ServiceDependencies/*/profile.arm.json`) contengono gli ID delle subscription Azure: ora ignorati in `.gitignore`. `git filter-branch` cancella dal disco i file de-tracciati → salvare prima quelli locali che servono (es. il `.pubxml` attuale).
- **Regioni Azure**: sul nuovo account "West Europe" è bloccata per nuovi clienti (`RequestDisallowedByAzure`); usare Sweden Central (o altra regione che accetta). SQL admin login non può coincidere con l'admin Entra.
- **`PasswordHasher<T>` non disponibile in una class library** (`Microsoft.NET.Sdk`, non `Sdk.Web`) senza riferimento esplicito → pacchetto `Microsoft.Extensions.Identity.Core` (non `Microsoft.AspNetCore.Identity`, nome fuorviante). Stesso motivo per cui `IConfiguration.GetValue<T>()` serve `Microsoft.Extensions.Configuration.Binder` a parte da `IConfiguration` stesso.
- **`[ApiController]` + `return null` su un metodo `ActionResult<T>`** → 200 OK con corpo vuoto, non un errore. Per un fallimento va sempre restituito esplicitamente `Unauthorized()`/`BadRequest()`/ecc.
- **Reactive Forms Angular**: `getRawValue()` tipizza i campi `string | null` anche con `fb.group()` e valore iniziale non nullo (per via di `reset()`) — dopo un controllo esplicito `if (form.invalid) return`, serve `!` (non-null assertion) sui campi, non è un errore del codice.

## Stato attuale

- Migrazione `Team` → `TeamAggregate` **completata** in tutto il progetto (Service, Repository, Controller, DB). Nessuna coesistenza residua.
- Catena end-to-end funzionante: `TeamsController` → `IMediator.Send(ActivateTeamCommand)` → `TeamAggregate.Activate()` → `Publish(TeamActivatedEvent)` → `ActivateProjectOnTeamActivatedHandler` aggiorna `Project`.
- `TeamReviewAggregate` (feature recensioni fine-progetto) implementata end-to-end: Aggregate, repository, Command/Handler con verifica cross-aggregate, EF Core config, migration applicata.
- **Autenticazione JWT: implementata e verificata end-to-end** (2026-09-23) — login, registrazione per email propria, interceptor e route guard sul frontend. `[Authorize]` non ancora applicato a nessun endpoint (l'API resta di fatto pubblica). `CreatedBy` su `Project` resta un placeholder (GUID a zero) finché non si collega all'utente autenticato.
- MassTransit: solo In-Memory Transport, mai collegato a un vero broker (RabbitMQ/Kafka).
- Test: xUnit su `TeamAggregate`, `TeamReviewAggregate`, `ActivateTeamCommandHandler`, `SubmitTeamReviewCommandHandler` (con Moq, incl. doppio mock coordinato). 9/9 verdi dopo upgrade a .NET 8.
- **Deploy completo e verificato** (2026-09-21): backend + DB + frontend online e collegati (CORS verificato con `curl`). Frontend: `environment.ts` = produzione (URL API Azure), `environment.development.ts` = locale (`localhost:7026`), swap via `fileReplacements`; `public/staticwebapp.config.json` fa il fallback SPA.
- Git: repo **pubblico** (`github.com/lucagallina17/teamcraft`), 5 commit su `main`, storia riscritta il 2026-09-21 per togliere i profili di publish con gli ID subscription; locale = `origin/main`, pulito. Restano pubblici di proposito `CLAUDE.md`, agente, memoria, `log/`, `ai_output_audit.md` (vetrina del metodo di lavoro con l'AI). `README.md` di root in inglese. Azure committa il workflow SWA su GitHub → `git pull` prima di un push se il locale è indietro.

## Modalità di collaborazione (preferenza di Luca)

**Default per qualunque feature non banale, infrastruttura O codice (.NET/Angular)**: guidare passo passo e lasciare che sia Luca a scrivere codice/comandi (spiegare il concetto, dare l'input un pezzo alla volta, aspettare l'esito). Confermato esplicitamente il 2026-09-22 anche per il codice applicativo (non solo Azure/Git/CI-CD come nella formulazione iniziale) — non assumere di poter scrivere la feature al posto suo senza chiederlo prima. Non committare/pushare né toccare risorse cloud in autonomia; **mai force-push su `main`**: fornire il comando a Luca. Per i comandi bash da incollare: se compaiono caratteri strani è il bracketed paste (`bind 'set enable-bracketed-paste off'`).

**Corretto il 2026-09-24, niente eccezioni per "codice meccanico"**: in precedenza si pensava che DTO banali/using potessero restare come snippet completo da incollare — sbagliato, corretto con insistenza da Luca. **Dare sempre indizi (file da guardare come pattern, campi/percorso concettuale mancante), mai codice pronto**, indipendentemente da quanto un pezzo sembri semplice — non è Claude che deve giudicare cosa è "abbastanza banale" da meritare la scorciatoia. Inoltre: dare sempre del **tu**, mai "voi".

Eccezione confermata il 2026-09-23: se Luca dice esplicitamente "pensaci tu"/"scrivo io" su una parte specifica, quella parte può essere scritta da Claude (anche via subagent `dotnet-dev`) — ma **sempre rivedendo di persona il codice del subagent riga per riga prima di riportarlo come ok**, non fidandosi solo del suo resoconto. Vale anche il contrario: nel codice scritto da Luca durante la stessa feature sono emersi comunque bug reali (due di logica, due di sicurezza — vedi Pattern ricorrenti punto 5) trovati solo in revisione — la revisione va fatta sempre, indipendentemente da chi scrive.

## Prossimi passi

1. **`[Authorize]` sugli endpoint**: oggi nessuno ne ha uno, l'API resta pubblica nonostante login/registrazione funzionino. È il pezzo che manca per rendere utile la JWT appena costruita.
2. **Committare tutto il lavoro della sessione JWT** (backend + frontend, ancora non committato) e cancellare le risorse del vecchio account Azure (RG, SQL, App Service) per non consumare credito.
3. Valutare il rename di `UserAccount.Role` (collide concettualmente con `ProjectRole`, già esistente per i team) quando si costruisce un vero sistema di ruoli applicativi.
4. CI/CD anche per il backend (oggi publish manuale da Visual Studio).
5. Rivalutare il target framework prima di novembre 2026 (fine supporto .NET 8 LTS) — probabile .NET 10.
6. Valutare se estendere DDD a `Project` (oggi anemico) — `project.Status = ...` scritto direttamente in `ActivateProjectOnTeamActivatedHandler`, nessuna protezione di dominio.
7. Pulizia: cartella residua `src/backend/TeamCraft/TeamCraft.API/` (vecchio scaffold, fuori dalla `.sln`); `src/ai_output_audit.md` dice ancora ".NET 7". Opzionale: GitHub Support per rimuovere le cached views dei vecchi commit (raggiungibili solo via hash, rischio basso).
8. Notifiche/toast: verificare copertura errori su tutti gli endpoint nuovi (TeamReview). Budget bundle Angular in warning (702 kB vs 500 kB).
