# TeamCraft — Handoff per agent

Sistema di talent matching: collega dipendenti, competenze e progetti; genera proposte di team ottimali tramite algoritmo di matching.

## Stack tecnico

**Backend**: .NET 8, EF Core 8, SQL Server (LocalDB in locale). Clean Architecture a 4 progetti: `TeamCraft.Domain`, `.Application`, `.Infrastructure`, `.API`, più `TeamCraft.Tests` (xUnit). Migrato da .NET 7 a .NET 8 il 2026-09-18 (EOL, non più selezionabile su Azure App Service).
**Librerie**: MediatR (Command/Query + Domain Event in-memory), MassTransit **8.5.10** (la 9.x richiede licenza a pagamento anche per In-Memory Transport, non collegato a un broker reale), Moq (test).
**Frontend**: Angular 21, standalone components, Signals, PrimeNG, Reactive Forms. Font Inter, palette Apple-style (`#0071e3` accent).
**Deploy (account Azure personale di Luca, RG `teamcraft-rg`)**: API su App Service (F1, Linux, .NET 8, Sweden Central) `teamcraft-api-haawg0cfgzdpdxgn`, Azure SQL Basic, frontend su Static Web Apps Free `red-stone-04625aa0f.1.azurestaticapps.net` via GitHub Actions (push su `main` ripubblica, `app_location: src/frontend/teamcraft-fe`). Repo pubblico: `github.com/lucagallina17/teamcraft`. Config via env var dell'App Service, mai nel repo: `ConnectionStrings__DefaultConnection`, `Cors__AllowedOrigins__0`, `Jwt__Key`. Backend si pubblica a mano da Visual Studio → Publish (il drag&drop Kudu/QuickDeploy è inutilizzabile per binari già compilati). Regioni "West Europe" bloccate per nuovi clienti su questo account. Dettagli in `log/2026-09-18/21/23/24-sessione.md`.

## Regole di dipendenza (Clean Architecture) — non violare mai

```
API → Application → Domain
Infrastructure → Application → Domain
```

Domain non conosce nessun altro livello. Eccezioni di dominio (`DomainRuleViolationException`, `EntityNotFoundException`, `EntityInUseException`) in `Domain/Exceptions`.

## Decisioni architetturali chiave

- **DDD parziale**: `TeamAggregate` e `TeamReviewAggregate` sono veri Aggregate Root. Il resto (`Employee`, `Project`, `Competency`, `ProjectRole`) resta anemico.
- **`ProjectRoleRequirementCompetency` collegata a `ProjectRoleRequirement`**, non a `ProjectRole` — competenze diverse per lo stesso ruolo su progetti diversi.
- **Aggregate si referenziano solo per Id**, mai direttamente. Regole cross-aggregate nel Command Handler, mai negli Aggregate.
- **Algoritmo di matching**: backtracking con potatura (non greedy). Competenze (peso, livello minimo, Required/Preferred) + bonus affinità (0.1x). Esclude dipendenti già in team `Active` su altri progetti.
- **CQRS**: repository Command (`ITeamAggregateRepository`) vs Query (`ITeamReadRepository`, `.Select()` diretto a DTO).
- **Domain Event vs Integration Event**: `TeamActivatedEvent` (MediatR, sincrono) attiva `Project` via `ActivateProjectOnTeamActivatedHandler`; `TeamActivatedIntegrationEvent` (MassTransit) per sistemi esterni.
- **Sync Team↔Project**: progetto passa ad `Active` solo quando il team lo fa; nessuna sync al `Closed`.
- **TeamStatus a 3 valori** (`Proposed`/`Active`/`Closed`) — `Approved` rimosso, era ridondante.
- `Project` eliminabile fisicamente solo se `Status == Draft`.
- **Autenticazione JWT** (2026-09-23/24, completa): `ITokenService`/`TokenService` in Infrastructure (interfaccia in Application, stesso pattern dei repository). `AuthService.RegisterAsync` collega l'account cercando l'`Employee` per **email** (mai un Id scelto da tendina). Ruolo `UserAccount` sempre `"Employee"` fisso lato server. `[Authorize]` su tutti i controller tranne `AuthController`, messo per-action non per-classe (una nuova action futura resta pubblica se dimenticato). `MapInboundClaims = false` per evitare rimappatura silenziosa dei nomi claim in lettura.

## Convenzioni di codice

- DTO sempre con **enum tipizzati**, mai `string`.
- Ogni DTO di riga in una collezione **espone il proprio Id**.
- Naming repository: `I{Entity}Repository` in `Application/Repositories`, impl in `Infrastructure/Persistence/Repositories`.
- Backing field per collezioni di Aggregate: `private readonly List<T> _x` + `HasField`/`UsePropertyAccessMode(Field)` in EF config.
- Costruttori di entity interne a un Aggregate sono `internal`.
- EF Core: mai lasciare `WithMany()` vuoto se l'altra entità ha già una nav collection (FK fantasma).
- Frontend: stili condivisi (`.page`, `.icon-btn`, ecc.) in `styles.scss` globale. `takeUntilDestroyed(this.destroyRef)` sempre ultimo operatore RxJS. Notifiche via `NotificationService`, mai `alert()`.

## Pattern ricorrenti da applicare sempre

1. Ogni nuova regola di stato va verificata contro **tutte** le azioni CRUD della stessa entità.
2. Azioni con effetti collaterali a cascata richiedono conferma esplicita (`ConfirmService`), non solo le eliminazioni.
3. Prima di un enum >2-3 valori, verificare che ogni stato sia davvero distinto (evita overengineering).
4. Query con pareggi possibili devono avere un **tie-break deterministico** esplicito.
5. **Aggregate/repository/test non garantiscono che una feature sia raggiungibile**: `TeamReviewAggregate` sembrava "fatta" ma non aveva `IRequest<T>`/endpoint/repository reale — i test la mockavano, mascherando il buco. Verificare sempre che sia chiamabile da HTTP, non fermarsi a "i test passano".
6. **Mai lasciare che un campo controllato dal client determini autorizzazione/identità** (trovati: ruolo auto-assegnato = privilege escalation; `EmployeeId` da tendina = furto d'identità). Chiedersi sempre "chi decide questo valore, server o chiamante?".

## Problemi già risolti (non ripetere)

- EF Core: major sempre allineata al TargetFramework, mai lasciare che NuGet installi l'ultima assoluta.
- Backing field non configurato → EF Core non scrive su collection con solo getter.
- `WithMany()` vuoto con nav collection dall'altro lato → FK duplicate.
- MediatR `IMediator` vs `MassTransit.Mediator.IMediator`: stesso nome, namespace diverso, autocomplete può importare quello sbagliato.
- Command/Event devono implementare esplicitamente `IRequest<T>`/`INotification`, non basta la firma dell'Handler.
- Visual Studio non compila `net8.0` (NETSDK1045) se l'IDE è troppo vecchio (proprio toolset MSBuild interno) — serve VS ≥ 17.8.
- Repo pubblico: scansionare prima del primo push (`git grep` email/GUID/password). `.pubxml`/`ServiceDependencies` contengono ID subscription Azure, ora in `.gitignore`. `git filter-branch` cancella dal disco i file de-tracciati.
- Regioni Azure: "West Europe" bloccata su questo account, usare Sweden Central. SQL admin login non può coincidere con l'admin Entra.
- `PasswordHasher<T>` in una class library serve `Microsoft.Extensions.Identity.Core` (non `Microsoft.AspNetCore.Identity`). `IConfiguration.GetValue<T>()` serve `Microsoft.Extensions.Configuration.Binder` a parte.
- `[ApiController]` + `return null` su `ActionResult<T>` → 200 OK vuoto, non un errore. Serve `Unauthorized()`/`BadRequest()` espliciti.
- Reactive Forms: `getRawValue()` tipizza `string | null` anche con valore iniziale non nullo — dopo `if (form.invalid) return`, serve `!`.
- PrimeNG `p-rating` non ha input `value` bindabile, solo `formControlName`. Per sola lettura: stelle disegnate a mano con PrimeIcons.
- **`Jwt:Key` (o altro secret solo locale) mancante su Azure** → 500 su *tutte* le richieste, anche pubbliche (crash di `UseAuthentication` prima ancora di controllare permessi). Se un endpoint senza `[Authorize]` dà 500, sospettare sempre la config JWT. Dopo l'app setting serve un **Restart esplicito** da Overview, il salvataggio da solo non sempre basta.
- Query editor SQL su Azure Portal richiede il proprio IP in whitelist sul firewall del SQL Server (pulsante diretto nel Portale).

## Stato attuale

- Migrazione `Team` → `TeamAggregate` completata ovunque, nessuna coesistenza residua.
- `TeamReviewAggregate`: **ora davvero raggiungibile end-to-end** (2026-09-24) — `POST /api/projects/{projectId}/teams/{id}/review`, blocco duplicati, `TeamDto.TeamReview` la espone, UI su `team-detail` (form o sola lettura), visibile solo a team `Closed`.
- **Autenticazione JWT completa incluso `[Authorize]`** (2026-09-23/24) — login, registrazione per email propria, interceptor con redirect a `/login` su 401, route guard FE. `CreatedBy` su `Project` preso dal claim `sub`, non più placeholder.
- Test: xUnit su `TeamAggregate`, `TeamReviewAggregate`, `ActivateTeamCommandHandler`, `SubmitTeamReviewCommandHandler`. 9/9 verdi.
- Git: storia riscritta il 2026-09-21 (rimossi ID subscription). Tutto il lavoro JWT + recensione committato e pushato (2026-09-24), inclusi i file rimasti fuori dallo split iniziale (`TeamsController.cs`, `styles.scss`). Locale allineato a `origin/main`.
- **Deploy Azure aggiornato al 2026-09-24**: backend ripubblicato con JWT + `[Authorize]` + recensione. Il primo deploy dava 500 su tutto perché `Jwt__Key` non era mai stato messo nelle App Settings Azure — risolto. **Bootstrap completato**: `[Authorize]` su `EmployeesController` impediva di creare il primo `Employee` via API senza essere già autenticati; risolto inserendo il primo `Employee` a mano via Query editor SQL, poi registrazione normale dal sito (endpoint di registrazione resta pubblico apposta).

## Modalità di collaborazione (preferenza di Luca)

**Default per qualunque feature non banale, codice compreso (.NET/Angular), non solo Azure/Git/infrastruttura**: guidare passo passo, dare indizi (pattern/file da guardare, concetto mancante) e lasciare che sia Luca a scrivere codice/comandi — **mai codice pronto da incollare, nemmeno se sembra meccanico** (corretto con insistenza il 2026-09-24: niente eccezioni per "tanto è banale", non è Claude a giudicare cosa merita la scorciatoia). Dare sempre del **tu**, mai "voi". Non committare/pushare né toccare risorse cloud in autonomia; mai force-push su `main`, fornire il comando a Luca.

**Eccezione**: se Luca dice esplicitamente "pensaci tu"/"scrivo io" su una parte specifica, quella parte la scrive Claude (anche via subagent `dotnet-dev`) — ma sempre rivedendo il codice del subagent riga per riga prima di darlo per buono. La revisione va comunque sempre fatta anche sul codice scritto da Luca: emersi bug reali di logica e di sicurezza trovati solo in revisione.

Bracketed paste nel terminale bash: `bind 'set enable-bracketed-paste off'`.

## Prossimi passi

1. Cancellare le risorse del vecchio account Azure (RG, SQL, App Service) per non consumare credito.
2. Valutare il rename di `UserAccount.Role` (collide con `ProjectRole`) quando si costruisce un vero sistema di ruoli applicativi.
3. CI/CD anche per il backend (oggi publish manuale da Visual Studio).
4. Rivalutare il target framework prima di novembre 2026 (fine supporto .NET 8 LTS) — probabile .NET 10.
5. Valutare se estendere DDD a `Project` (oggi anemico, nessuna protezione di dominio sullo `Status`).
6. Pulizia: cartella scaffold residua `TeamCraft.API/`; `ai_output_audit.md` dice ancora ".NET 7". Opzionale: rimuovere le cached views dei vecchi commit Git.
7. Notifiche/toast: copertura errori sugli endpoint TeamReview. Budget bundle Angular in warning (702 kB vs 500 kB).
