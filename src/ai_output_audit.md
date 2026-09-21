# AI Output Audit — ProjectRoster
## Documento di verifica degli output AI durante lo sviluppo

**Progetto:** ProjectRoster  
**Stack:** .NET 7 + Angular + EF Core + SQL Server  
**Obiettivo:** Documentare errori, pattern sbagliati e inconsistenze negli output AI durante lo sviluppo, come esercizio di AI Verification.
**Modello AI:** Sonnet 4.6 + Sonnet 5

---

## Struttura di ogni entry

| Campo | Descrizione |
|---|---|
| **ID** | Identificatore progressivo |
| **Data** | Quando è stato rilevato |
| **Categoria** | Tipo di errore (vedi legenda) |
| **Contesto** | Cosa si stava chiedendo all'AI |
| **Output errato** | Cosa ha prodotto l'AI |
| **Perché è sbagliato** | Analisi critica |
| **Correzione** | Come è stato risolto |
| **Pattern ricorrente** | Si/No — se si ripete va monitorato |
| **Gravità** | Alta / Media / Bassa |

---

## Legenda categorie

| Codice | Categoria | Descrizione |
|---|---|---|
| INC | Inconsistenza | L'AI contraddice decisioni prese in precedenza |
| ASS | Assunzione non dichiarata | L'AI decide qualcosa senza comunicarlo |
| CMP | Codice non compilabile | Errori sintattici o di dipendenze |
| OVE | Over-engineering | Soluzione troppo complessa per il problema |
| UND | Under-engineering | Soluzione troppo semplice che crea problemi dopo |
| CTX | Contesto perso | L'AI dimentica decisioni prese in precedenza |
| PAT | Pattern sbagliato | Soluzione funzionante ma architetturalmente errata |
| VER | Versione errata | L'AI suggerisce versioni incompatibili di librerie |
| INC | Incompleto | L'AI produce output parziale senza segnalarlo |

---

## Entry

---

### [001] Versione EF Core incompatibile
| Campo | Dettaglio |
|---|---|
| **Data** | Sessione corrente |
| **Categoria** | VER — Versione errata |
| **Contesto** | Installazione pacchetti NuGet EF Core su progetto .NET 7 |
| **Output errato** | L'AI ha suggerito di installare i pacchetti EF Core senza specificare la versione, portando all'installazione della versione 10.0.9 |
| **Perché è sbagliato** | EF Core 10 richiede .NET 10. Il progetto usa .NET 7, quindi i pacchetti devono essere alla versione 7.x.x |
| **Correzione** | Disinstallati i pacchetti e reinstallati specificando la versione 7.0.20 |
| **Pattern ricorrente** | Da monitorare — quando si lavora con versioni non LTS il rischio di mismatch è alto |
| **Gravità** | Alta — bloccava la compilazione |

---

### [002] Campo Source in EmployeeAffinity non necessario
| Campo | Dettaglio |
|---|---|
| **Data** | Sessione corrente |
| **Categoria** | ASS — Assunzione non dichiarata |
| **Contesto** | Definizione della entity EmployeeAffinity |
| **Output errato** | L'AI ha inserito un campo Source in EmployeeAffinity senza motivarlo esplicitamente |
| **Perché è sbagliato** | Nel dominio definito, l'affinità viene gestita esclusivamente dall'HR manualmente. Non esistono sorgenti multiple da distinguere. Il campo era ridondante |
| **Correzione** | Campo Source rimosso dalla entity dopo domanda critica dell'utente |
| **Pattern ricorrente** | Si — l'AI tende ad aggiungere campi "per completezza" senza verificare se hanno senso nel dominio specifico |
| **Gravità** | Bassa — non bloccante ma introduce rumore nel modello |

---

### [003] ProjectRoleCompetency legata al ruolo globale invece che al progetto
| Campo | Dettaglio |
|---|---|
| **Data** | Sessione corrente |
| **Categoria** | PAT — Pattern sbagliato |
| **Contesto** | Definizione del modello di dominio iniziale |
| **Output errato** | Il documento originale collegava ProjectRoleCompetency direttamente a ProjectRole, rendendo i requisiti di competenza globali per ogni ruolo |
| **Perché è sbagliato** | Le competenze richieste per un ruolo cambiano per progetto. Un Tech Lead su un e-commerce e uno su un sistema IoT hanno requisiti diversi. La struttura originale non permetteva questa flessibilità |
| **Correzione** | Introdotta ProjectRoleRequirementCompetency collegata a ProjectRoleRequirement, spostando i requisiti al livello del progetto specifico |
| **Pattern ricorrente** | Si — l'AI tende a semplificare le relazioni molti-a-molti contestuali, perdendo flessibilità |
| **Gravità** | Alta — avrebbe richiesto un refactoring importante in produzione |

### [004] Inconsistenza nel nome del metodo GetByIdWithCompetenc*Async
| Campo | Dettaglio |
|---|---|
| **Data** | Sessione corrente |
| **Categoria** | INC — Inconsistenza |
| **Contesto** | Definizione di IEmployeeRepository e utilizzo in EmployeeService |
| **Output errato** | L'AI ha usato `GetByIdWithCompetencyAsync` (singolare) in un passaggio e `GetByIdWithCompetenciesAsync` (plurale) in un altro |
| **Perché è sbagliato** | Il metodo carica la collezione di tutte le competenze — il nome corretto è al plurale. L'inconsistenza causa errori di compilazione |
| **Correzione** | Allineato il nome a `GetByIdWithCompetenciesAsync` in IEmployeeRepository.cs e EmployeeService.cs |
| **Pattern ricorrente** | Si |
| **Gravità** | Media |

### [005] Inconsistenza navigation property CompetencyAssessments in Employee
| Campo | Dettaglio |
|---|---|
| **Data** | Sessione corrente |
| **Categoria** | INC — Inconsistenza |
| **Contesto** | Utilizzo della navigation property di Employee in EmployeeService |
| **Output errato** | L'AI ha usato `employee.CompetencyAssessments` in EmployeeService ma la property era stata definita con nome diverso nella entity Employee |
| **Perché è sbagliato** | Il nome usato nel service non corrisponde a quello definito nella entity — causa errore di compilazione |
| **Correzione** | Allineato il nome della navigation property in Employee.cs e aggiornato il riferimento in EmployeeService.cs |
| **Pattern ricorrente** | Si — l'AI non tiene traccia dei nomi esatti definiti dall'utente nelle sessioni precedenti |
| **Gravità** | Media — causa errore di compilazione |

### [006] Uso di API deprecata provideAnimationsAsync
| Campo | Dettaglio |
|---|---|
| **Data** | Sessione corrente |
| **Categoria** | ASS — Assunzione non dichiarata |
| **Contesto** | Configurazione PrimeNG in app.config.ts |
| **Output errato** | L'AI ha suggerito `provideAnimationsAsync` che risulta deprecato in Angular 21 |
| **Perché è sbagliato** | La funzione è marcata come deprecated — il compilatore mostra un warning e in future versioni potrebbe essere rimossa |
| **Correzione** | Sostituito con `provideAnimations()` da `@angular/platform-browser/animations` |
| **Pattern ricorrente** | Si — l'AI può suggerire API deprecate specialmente quando le versioni cambiano rapidamente |
| **Gravità** | Bassa — non bloccante ma da correggere per mantenere il codice aggiornato |

### [007] Sostituzione con API ancora deprecata
| Campo | Dettaglio |
|---|---|
| **Data** | Sessione corrente |
| **Categoria** | INC — Inconsistenza |
| **Contesto** | Correzione di provideAnimationsAsync in app.config.ts |
| **Output errato** | L'AI ha sostituito provideAnimationsAsync con provideAnimations che è anch'essa deprecata |
| **Perché è sbagliato** | Entrambe le funzioni sono deprecate in Angular 21 — la correzione non ha risolto il problema |
| **Correzione** | Rimossa completamente la riga — in Angular 21 le animazioni sono abilitate di default |
| **Pattern ricorrente** | Si — l'AI tende a correggere con soluzioni parziali senza verificare se anche la sostituzione è deprecata |
| **Gravità** | Media — genera confusione e warning multipli |

### [008] CompetencyDto usa string invece di enum per Type
| Campo | Dettaglio |
|---|---|
| **Data** | Sessione corrente |
| **Categoria** | ASS — Assunzione non dichiarata |
| **Contesto** | Definizione di CompetencyDto in Application |
| **Output errato** | L'AI ha definito Type come string nel DTO invece di usare l'enum CompetencyType già definito in Domain |
| **Perché è sbagliato** | Usare string perde i benefici dell'enum — type safety, leggibilità e coerenza con il modello di dominio |
| **Correzione** | Sostituito string con CompetencyType nel CompetencyDto e rimosso .ToString() nel mapping del service |
| **Pattern ricorrente** | Si — l'AI tende a usare stringhe nei DTO per semplicità perdendo la type safety |
| **Gravità** | Bassa — funziona comunque ma è una cattiva pratica |

### [009] Nome classe componente errato nel routing
| Campo | Dettaglio |
|---|---|
| **Data** | Sessione corrente |
| **Categoria** | INC — Inconsistenza |
| **Contesto** | Configurazione delle route in app.routes.ts |
| **Output errato** | L'AI ha usato il suffisso Component nel nome della classe es. EmployeeListComponent |
| **Perché è sbagliato** | Angular CLI 21 genera i componenti senza il suffisso Component nel nome della classe — il nome corretto è EmployeeList |
| **Correzione** | Aggiornati tutti i then(m => m.XXX) rimuovendo il suffisso Component |
| **Pattern ricorrente** | Si — l'AI assume convenzioni di versioni precedenti di Angular |
| **Gravità** | Media — causa errore di compilazione |

### [010] Configurazione mancante Zone.js
| Campo | Dettaglio |
|---|---|
| **Data** | Sessione corrente |
| **Categoria** | ASS — Assunzione non dichiarata |
| **Contesto** | Configurazione iniziale del progetto Angular |
| **Output errato** | L'AI non ha incluso l'import di Zone.js nel setup iniziale |
| **Perché è sbagliato** | Angular 21 richiede Zone.js esplicitamente in questa configurazione |
| **Correzione** | Installato zone.js e aggiunto import in main.ts |
| **Pattern ricorrente** | Si — l'AI dimentica configurazioni di bootstrap nelle versioni recenti di Angular |
| **Gravità** | Alta — blocca completamente l'avvio dell'applicazione |

### [011] Dati caricati in ngOnInit invece di usare un Resolver
| Campo | Dettaglio |
|---|---|
| **Data** | Sessione corrente |
| **Categoria** | PAT — Pattern sbagliato |
| **Contesto** | Caricamento dati in EmployeeListComponent |
| **Output errato** | L'AI ha implementato il caricamento dati in ngOnInit con gestione manuale del loading state |
| **Perché è sbagliato** | Per dati essenziali al render della pagina il pattern corretto è il Resolver — i dati sono disponibili prima che il componente venga creato, il router gestisce il loading e gli errori possono bloccare la navigazione |
| **Correzione** | Creato employeeListResolver, aggiornata la route con resolve, rimosso ngOnInit e loading signal dal componente |
| **Pattern ricorrente** | Si — l'AI tende a caricare i dati nel componente invece di usare il resolver anche quando i dati sono essenziali per il render |
| **Gravità** | Media — funziona ma è un pattern subottimale che peggiora UX e manutenibilità |

### [012] Subscribe annidati in onDelete — memory leak
| Campo | Dettaglio |
|---|---|
| **Data** | Sessione corrente |
| **Categoria** | PAT — Pattern sbagliato |
| **Contesto** | Metodo onDelete in EmployeeList |
| **Output errato** | L'AI ha usato due subscribe annidati per gestire delete seguita da getAll |
| **Perché è sbagliato** | I subscribe annidati creano memory leak — le sottoscrizioni interne non vengono chiuse correttamente |
| **Correzione** | Sostituito con pipe(concatMap()) e un solo subscribe finale |
| **Pattern ricorrente** | Si — l'AI tende a annidare subscribe invece di concatenare Observable con operatori RxJS |
| **Gravità** | Alta — causa memory leak in produzione |

### [013] Configurazione CORS mancante
| Campo | Dettaglio |
|---|---|
| **Data** | Sessione corrente |
| **Categoria** | ASS — Assunzione non dichiarata |
| **Contesto** | Setup iniziale del backend, prima di collegare il frontend Angular |
| **Output errato** | L'AI non ha configurato CORS nel Program.cs durante il setup iniziale dell'API |
| **Perché è sbagliato** | Senza CORS il browser blocca automaticamente tutte le richieste da Angular (porta 4200) verso l'API .NET (porta 5001/7xxx) — errore prevedibile fin dall'inizio |
| **Correzione** | Aggiunta configurazione AddCors e UseCors con policy che permette richieste da localhost:4200 |
| **Pattern ricorrente** | Si — l'AI dimentica configurazioni infrastrutturali necessarie quando si integrano frontend e backend separati |
| **Gravità** | Alta — blocca completamente la comunicazione tra frontend e backend |

### [014] Mancata implementazione conferma per azione distruttiva
| Campo | Dettaglio |
|---|---|
| **Data** | Sessione corrente |
| **Categoria** | ASS — Assunzione non dichiarata |
| **Contesto** | Implementazione di onDelete in EmployeeList |
| **Output errato** | L'AI ha implementato la cancellazione del dipendente come azione diretta, senza alcuna richiesta di conferma all'utente |
| **Perché è sbagliato** | Eliminare un dipendente è un'azione distruttiva e irreversibile — una best practice UX/business fondamentale è richiedere sempre una conferma esplicita prima di azioni di questo tipo, per evitare cancellazioni accidentali. L'AI non ha sollevato il problema autonomamente, è stato l'utente a doverlo richiedere |
| **Correzione** | Introdotto ConfirmDialog di PrimeNG con ConfirmService riutilizzabile, integrato in onDelete con await prima di procedere |
| **Pattern ricorrente** | Si — l'AI tende a implementare il percorso funzionale minimo senza considerare autonomamente implicazioni di business/UX legate a sicurezza e prevenzione errori utente |
| **Gravità** | Alta — rischio concreto di perdita dati in produzione senza possibilità di annullare l'azione |

### [015] Mancata prevenzione memory leak con subscribe annidati
| Campo | Dettaglio |
|---|---|
| **Data** | Sessione corrente |
| **Categoria** | PAT — Pattern sbagliato |
| **Contesto** | Prima implementazione di onDelete in EmployeeList, prima ancora della modale di conferma |
| **Output errato** | L'AI ha scritto due subscribe annidati (delete seguito da getAll) come prima soluzione, senza usare operatori RxJS per concatenare gli Observable |
| **Perché è sbagliato** | I subscribe annidati sono un antipattern noto in RxJS — creano memory leak perché le sottoscrizioni interne non vengono gestite/chiuse correttamente. È una best practice fondamentale di Angular/RxJS che l'AI dovrebbe applicare di default, non dopo segnalazione dell'utente |
| **Correzione** | Sostituito con pipe(concatMap()) e un solo subscribe finale |
| **Pattern ricorrente** | Si — l'AI tende a scrivere la soluzione più diretta e leggibile a prima vista, senza applicare automaticamente pattern RxJS corretti per gestione asincrona a catena |
| **Gravità** | Alta — memory leak in produzione, individuato solo grazie alla review critica dell'utente |

### [016] DatePipe non importata nel componente
| Campo | Dettaglio |
|---|---|
| **Data** | Sessione corrente |
| **Categoria** | CMP — Codice non compilabile |
| **Contesto** | Uso della pipe date nel template di EmployeeDetail per formattare la data delle competenze |
| **Output errato** | L'AI ha usato `{{ competency.date | date:'dd/MM/yyyy' }}` nel template senza importare DatePipe nell'array imports del componente standalone |
| **Perché è sbagliato** | In Angular con standalone components ogni pipe usata nel template deve essere esplicitamente importata nel componente — senza l'import il template non compila |
| **Correzione** | Creato un file shared/common-imports.ts con le pipe comuni (DatePipe, CurrencyPipe, DecimalPipe) da importare nei componenti che ne hanno bisogno |
| **Pattern ricorrente** | Si — l'AI dimentica di includere le dipendenze necessarie (pipe, moduli) nell'array imports dei componenti standalone |
| **Gravità** | Media — causa errore di compilazione ma facile da individuare e correggere |

### [017] Scarsa cura del design UI nel componente EmployeeDetail
| Campo | Dettaglio |
|---|---|
| **Data** | Sessione corrente |
| **Categoria** | UND — Under-engineering |
| **Contesto** | Costruzione del componente EmployeeDetail — form e tabella competenze |
| **Output errato** | L'AI ha prodotto un layout minimo — elementi impilati senza card, senza feedback di validazione per campo, spaziatura generica, sottoutilizzo dei componenti PrimeNG disponibili (p-card, p-message) |
| **Perché è sbagliato** | Un componente in produzione deve avere una gerarchia visiva chiara e feedback utente adeguato, specialmente su un form. L'AI si è concentrata sulla funzionalità (resolver, validazione logica, salvataggio) trascurando la qualità dell'esperienza utente |
| **Correzione** | Da applicare — ricostruzione del componente con card, validazione visibile, gerarchia visiva curata |
| **Pattern ricorrente** | Si — quando la richiesta non enfatizza esplicitamente il design, l'AI tende a dare priorità alla logica funzionante e trattare la UI come scaffold minimo |
| **Gravità** | Media — non blocca la funzionalità ma richiede rework per essere production-ready |

### [018] Dati stantii dopo navigazione — uso di snapshot invece di Observable reattivo
| Campo | Dettaglio |
|---|---|
| **Data** | Sessione corrente |
| **Categoria** | PAT — Pattern sbagliato |
| **Contesto** | Lettura dei dati del resolver in EmployeeList tramite route.snapshot.data |
| **Output errato** | L'AI ha usato route.snapshot.data per inizializzare il signal — una lettura statica, one-time, del valore del resolver |
| **Perché è sbagliato** | Con dati che cambiano frequentemente (creazione/cancellazione), lo snapshot può non riflettere lo stato più recente se il componente non viene ricreato in ogni navigazione. Va usato route.data con toSignal per una sottoscrizione reattiva |
| **Correzione** | Sostituito route.snapshot.data con toSignal(route.data.pipe(map(...))) per garantire aggiornamento automatico |
| **Pattern ricorrente** | Si — l'AI introduce il pattern resolver + snapshot senza considerare i casi con dati mutabili frequentemente (liste con CRUD) |
| **Gravità** | Media — causa un bug di UX reale, dati non aggiornati visibili all'utente |

### [019] Soluzione precedente inefficace — bug non risolto al primo tentativo
| Campo | Dettaglio |
|---|---|
| **Data** | Sessione corrente |
| **Categoria** | UND — Under-engineering |
| **Contesto** | Correzione del bug "dati non aggiornati dopo creazione" in EmployeeList |
| **Output errato** | L'AI ha proposto toSignal(route.data) come soluzione, ma il problema reale non era la lettura statica dello snapshot bensì la mancata riesecuzione del resolver quando si naviga verso una route già visitata (route reuse strategy di Angular) |
| **Perché è sbagliato** | La correzione non ha risolto il bug segnalato dall'utente — serviva un'analisi più approfondita del comportamento del router invece di una correzione superficiale basata su un sospetto plausibile ma non verificato |
| **Correzione** | Abbandonato il resolver per la lista, sostituito con caricamento esplicito nel costruttore tramite service — approccio che non dipende dai meccanismi di route reuse del router |
| **Pattern ricorrente** | Si — l'AI può proporre correzioni plausibili ma non verificate quando non ha modo di testare concretamente il comportamento runtime |
| **Gravità** | Alta — due tentativi di correzione falliti prima di arrivare alla soluzione robusta, con perdita di tempo per l'utente |

### [021] Mancata applicazione di takeUntilDestroyed sulle subscribe
| Campo | Dettaglio |
|---|---|
| **Data** | Sessione corrente |
| **Categoria** | ASS — Assunzione non dichiarata / mancata applicazione istruzioni progetto |
| **Contesto** | Creazione di ProjectList — chiamate a loadProjects() e onDelete() |
| **Output errato** | L'AI ha scritto subscribe() senza applicare takeUntilDestroyed(this.destroyRef), nonostante sia una regola esplicita e vincolante indicata nelle istruzioni del progetto |
| **Perché è sbagliato** | Le istruzioni del progetto richiedono che ogni sottoscrizione a Observable di lunga durata usi takeUntilDestroyed per evitare memory leak quando il componente viene distrutto — regola non opzionale |
| **Correzione** | Aggiunto DestroyRef via inject(), applicato takeUntilDestroyed(this.destroyRef) su entrambe le subscribe |
| **Pattern ricorrente** | Si — rischio concreto che l'AI non applichi in automatico regole personalizzate definite nelle istruzioni di progetto ad ogni singolo output, specialmente su codice generato velocemente |
| **Gravità** | Alta — viola una regola esplicita del progetto, potenziale memory leak in produzione |

### [022] Endpoint di scrittura mancante per EmployeeCompetencyAssessment
| Campo | Dettaglio |
|---|---|
| **Data** | Sessione corrente |
| **Categoria** | INC — Incompleto |
| **Contesto** | Costruzione del flusso completo per l'algoritmo di matching (competenze richieste vs competenze dipendenti) |
| **Output errato** | L'AI ha costruito la lettura delle competenze del dipendente (EmployeeDetailDto) fin dall'inizio del progetto, ma non ha mai realizzato né segnalato l'assenza di un endpoint per la scrittura (associare una competenza a un dipendente con relativo livello) |
| **Perché è sbagliato** | Senza questo endpoint il sistema è strutturalmente incompleto — l'algoritmo di matching costruito successivamente non ha modo di ricevere dati reali, e il gap è stato individuato solo dall'utente in fase di test, non segnalato proattivamente dall'AI durante la costruzione del modulo Employee |
| **Correzione** | Aggiunto endpoint POST /api/employees/{id}/competencies e DELETE /api/employees/competencies/{assessmentId}, con relativa UI in EmployeeDetail |
| **Pattern ricorrente** | Si — l'AI può costruire il percorso di lettura di una feature CRUD senza completare il percorso di scrittura, specialmente quando l'attenzione si sposta su feature successive (algoritmo di matching) prima che il modulo precedente sia davvero completo |
| **Gravità** | Alta — bloccava funzionalmente l'intero algoritmo di matching, il cuore del progetto |

### [023] Campo Id dell'assessment mancante in EmployeeCompetencyDto
| Campo | Dettaglio |
|---|---|
| **Data** | Sessione corrente |
| **Categoria** | INC — Incompleto |
| **Contesto** | Costruzione dell'endpoint di eliminazione di una competenza dal profilo dipendente |
| **Output errato** | L'AI ha suggerito di usare competencyId come identificativo per l'eliminazione, ma l'endpoint backend elimina per Id dell'assessment (EmployeeCompetencyAssessment.Id), non per CompetencyId — e quel campo non era mai stato esposto nel DTO |
| **Perché è sbagliato** | competencyId identifica la competenza in generale (es. "Angular"), non la specifica valutazione di quel dipendente su quella competenza in quella data. Un dipendente potrebbe avere più valutazioni della stessa competenza nel tempo (storico) — usare competencyId per la delete avrebbe eliminato l'assessment sbagliato o fallito silenziosamente |
| **Correzione** | Aggiunto campo Id a EmployeeCompetencyDto sia nel backend che nel modello TypeScript, aggiornato il mapping nel service e la chiamata nel template |
| **Pattern ricorrente** | Si — quando l'AI introduce un DTO di lettura, tende a non includere l'id della riga specifica se il DTO è pensato inizialmente solo per la visualizzazione, salvo poi scoprire che serve anche per operazioni di scrittura/eliminazione mirate |
| **Gravità** | Media — causava comportamento errato silenzioso (eliminazione della valutazione sbagliata in caso di storico multiplo) anziché un errore di compilazione immediato, rendendolo più insidioso da individuare |

### [024] Id e CompetencyId mancanti in RequirementCompetencyDto
| Campo | Dettaglio |
|---|---|
| **Data** | Sessione corrente |
| **Categoria** | INC — Incompleto |
| **Contesto** | Preparazione della funzionalità di modifica di una competenza richiesta già associata a un ruolo |
| **Output errato** | RequirementCompetencyDto esponeva solo dati di visualizzazione (nome competenza, livello, peso, tipo) senza l'Id della riga né il CompetencyId necessario a precompilare un form di modifica |
| **Perché è sbagliato** | Stesso pattern già tracciato in [023] — un DTO pensato solo per la lettura non basta appena serve un'azione mirata (modifica/eliminazione) su quella specifica riga |
| **Correzione** | Aggiunti i campi Id e CompetencyId al DTO e al relativo mapping |
| **Pattern ricorrente** | Si — seconda occorrenza dello stesso pattern, questa volta su un'entità diversa (ProjectRoleRequirementCompetency invece di EmployeeCompetencyAssessment). Da considerare una regola generale: ogni DTO di riga in una collezione deve sempre esporre il proprio Id |
| **Gravità** | Media — bloccava la funzionalità di modifica richiesta, individuato prima di scrivere codice frontend grazie al pattern già noto |

### [025] Nessuna interfaccia per la creazione di ProjectRole
| Campo | Dettaglio |
|---|---|
| **Data** | Sessione corrente |
| **Categoria** | INC — Incompleto |
| **Contesto** | Costruzione del dropdown di selezione ruolo in ProjectDetail, che presuppone l'esistenza di ProjectRole già create |
| **Output errato** | L'AI ha costruito il consumo dei ProjectRole (dropdown in ProjectDetail) senza costruire né segnalare l'assenza di un'interfaccia per crearli — il catalogo ruoli resta vuoto e non raggiungibile dalla UI |
| **Perché è sbagliato** | Stesso pattern già visto con le competenze dei dipendenti [022] — l'AI costruisce il lato "consumo" di una entity prima di garantire che il lato "creazione" esista, lasciando un vicolo cieco funzionale |
| **Correzione** | Da implementare — pagina dedicata alla gestione (lista + creazione) di ProjectRole |
| **Pattern ricorrente** | Si — terza occorrenza dello stesso pattern strutturale (dopo competenze employee e competenze requisito). Da trattare come regola: ogni volta che l'AI costruisce un dropdown/select che consuma una entity, deve verificare e garantire che esista un percorso di creazione per quella entity prima di considerare la feature completa |
| **Gravità** | Alta — occorrenza ricorrente dello stesso tipo di gap, segnala una lacuna sistematica nel modo in cui l'AI pianifica le feature CRUD correlate |

### [026] Icone PrimeIcons non renderizzate correttamente per pi-pencil e pi-times
| Campo | Dettaglio |
|---|---|
| **Data** | Sessione corrente |
| **Categoria** | CMP — Codice non compilabile / comportamento errato |
| **Contesto** | Icone di modifica ed eliminazione nel componente ProjectRoleList |
| **Output errato** | L'AI ha introdotto icon-btn con classi pi-pencil e pi-times senza verificare che il font-family personalizzato (Inter) applicato globalmente per lo stile Apple non entrasse in conflitto con il font icon-based di PrimeIcons su elementi <i> |
| **Perché è sbagliato** | Impostare font-family in modo troppo ampio nei fogli di stile globali o nei componenti rischia di sovrascrivere il font specifico richiesto dalle icone a font (PrimeIcons), causando il fallback al font di sistema del sistema operativo per quei glifi specifici |
| **Correzione** | Da verificare — aggiungere una regola esplicita che protegga font-family: 'primeicons' su tutti gli elementi con classe .pi, evitando che stili più generici la sovrascrivano |
| **Pattern ricorrente** | Da monitorare — non ancora confermato se sistemico o isolato a questo componente |
| **Gravità** | Bassa — problema visivo, non bloccante, ma incoerente con lo stile curato del resto dell'app |

### [027] DbUpdateException catturata nel layer Application, violando il disaccoppiamento da EF Core
| Campo | Dettaglio |
|---|---|
| **Data** | Sessione corrente |
| **Categoria** | PAT — Pattern sbagliato |
| **Contesto** | Gestione dell'errore di vincolo di integrità referenziale nella cancellazione di ProjectRole |
| **Output errato** | L'AI ha scritto un try/catch su DbUpdateException direttamente in ProjectRoleService, che vive nel layer Application — questo layer non dovrebbe avere alcuna dipendenza da Microsoft.EntityFrameworkCore, principio stabilito fin dall'inizio del progetto con il Repository pattern |
| **Perché è sbagliato** | Application deve rimanere agnostico rispetto alla tecnologia di persistenza — se domani si cambia ORM o database, il codice in Application non dovrebbe richiedere modifiche. Catturare un'eccezione specifica di EF Core in quel layer rompe questo isolamento |
| **Correzione** | Da applicare — spostare la gestione dell'eccezione in Infrastructure (nel repository), che può tradurla in un'eccezione di dominio generica (es. EntityInUseException) che Application può catturare senza conoscere EF Core |
| **Pattern ricorrente** | No, prima occorrenza di questo tipo specifico — ma da monitorare, perché è un errore facile da ripetere ogni volta che si introduce gestione di vincoli referenziali |
| **Gravità** | Media — non blocca la funzionalità, ma introduce un accoppiamento architetturale scorretto che va contro le convenzioni consolidate del progetto |

### [028] Diagnosi errata dell'icona come problema di font — causa reale era CSS mancante
| Campo | Dettaglio |
|---|---|
| **Data** | Sessione corrente |
| **Categoria** | UND — Under-engineering / diagnosi errata |
| **Contesto** | Bottoni modifica ed elimina in ProjectRoleList che mostravano stile "nativo Windows" |
| **Output errato** | L'AI ha ipotizzato ripetutamente cause legate al font PrimeIcons (cache, font-family in conflitto, file non caricato) prima di verificare l'ipotesi più semplice — che la classe CSS .icon-btn, definita come stile locale del componente in altri file (EmployeeList, ProjectList), non fosse mai stata replicata in ProjectRoleList |
| **Perché è sbagliato** | Gli stili nei componenti standalone Angular sono incapsulati per default (view encapsulation) — non condivisi automaticamente tra componenti. L'AI ha dato per scontato che una classe scritta in un componente fosse disponibile ovunque, errore di base sul funzionamento di Angular, e ha investigato piste sbagliate (font, cache) invece di verificare prima la causa più diretta: la classe non era definita nel componente in questione |
| **Correzione** | Aggiunta la definizione di .icon-btn e .icon-btn--danger negli styles di ProjectRoleList, come già presente negli altri componenti con lo stesso pattern visivo |
| **Pattern ricorrente** | Si — ogni volta che l'AI introduce un componente nuovo riusando classi CSS "standard" del progetto (icon-btn, page__header, ecc.), rischia di dimenticare che quegli stili non sono globali e vanno ridefiniti in ogni componente, oppure va creato un sistema di stili condivisi (SCSS mixin o classi globali in styles.scss) |
| **Gravità** | Media — difetto visivo che ha richiesto più cicli di diagnosi errata prima di arrivare alla causa reale, con perdita di tempo su piste sbagliate (font, cache, conflitti CSS) |

### [029] Stili duplicati manualmente in ogni componente invece di stili globali condivisi
| Campo | Dettaglio |
|---|---|
| **Data** | Sessione corrente |
| **Categoria** | OVE — Over-engineering (duplicazione) |
| **Contesto** | Costruzione progressiva di EmployeeList, ProjectList, ProjectRoleList, TeamProposals con lo stesso linguaggio visivo Apple |
| **Output errato** | L'AI ha riscritto lo stesso blocco di stili (page, page__header, icon-btn, empty-state, p-button, clean-table) in ogni singolo componente invece di riconoscere fin dal secondo componente che si trattava di stili di sistema condivisi da centralizzare in styles.scss |
| **Perché è sbagliato** | Duplicare CSS identico in più componenti è una violazione del principio DRY che ha causato un bug reale [028] — dimenticare di copiare .icon-btn in un nuovo componente ha rotto silenziosamente lo stile senza errore di compilazione. Ogni nuova pagina rischiava lo stesso problema |
| **Correzione** | Spostati tutti gli stili di sistema condivisi in styles.scss globale, rimosso ::ng-deep dove non più necessario (gli stili globali non sono incapsulati), lasciato negli styles dei singoli componenti solo il markup realmente specifico di quella pagina |
| **Pattern ricorrente** | Si — pattern strutturale di fondo: l'AI tende a trattare ogni componente come un'unità isolata anche quando il progetto ha già stabilito un design system implicito, invece di riconoscere e centralizzare le ripetizioni non appena emergono (idealmente al secondo componente con lo stesso stile, non al quinto) |
| **Gravità** | Media — non bloccante, ma fonte diretta del bug [028] e di rischio di manutenzione futura (modificare lo stile di un bottone richiederebbe N modifiche in N file invece di una sola) |

### [030] Nessun punto di accesso persistente ai team creati
| Campo | Dettaglio |
|---|---|
| **Data** | Sessione corrente |
| **Categoria** | INC — Incompleto |
| **Contesto** | Costruzione del flusso di generazione proposte e creazione team, seguita da TeamDetail |
| **Output errato** | L'AI aveva già identificato autonomamente questo gap in un checkpoint precedente ("Lista dei team per progetto") ma non lo ha implementato prima di considerare il flusso funzionalmente chiuso, lasciando l'utente senza modo di ritrovare un team dopo la navigazione iniziale |
| **Perché è sbagliato** | Anche quando un gap viene identificato correttamente in anticipo, se non viene effettivamente implementato nella sequenza di lavoro concordata il problema si presenta comunque nella pratica — la sola consapevolezza del gap non sostituisce la sua chiusura effettiva prima di passare al task successivo |
| **Correzione** | Aggiunta sezione "Team" in ProjectDetail che mostra lo storico dei team associati al progetto con navigazione diretta a ciascuno |
| **Pattern ricorrente** | Si — variante del pattern già visto in [022]/[025]: costruire il lato "creazione/consumo avanzato" di una feature (algoritmo di matching, TeamDetail) senza completare il lato "accesso/navigazione" di base, anche quando il gap è stato esplicitamente pianificato in anticipo |
| **Gravità** | Media — non blocca la funzionalità sottostante (i dati esistono e sono corretti), ma rende inutilizzabile nella pratica una feature già completa dal punto di vista tecnico |

### [031] Nessun vincolo sulla generazione di team multipli per lo stesso progetto
| Campo | Dettaglio |
|---|---|
| **Data** | Sessione corrente |
| **Categoria** | ASS — Assunzione non dichiarata |
| **Contesto** | Bottone "Genera proposte di team" in ProjectDetail, sempre visibile indipendentemente dai team già esistenti per quel progetto |
| **Output errato** | L'AI ha costruito il flusso di generazione team senza considerare né dichiarare la regola di business su quanti team un progetto possa avere attivi contemporaneamente, lasciando che l'utente potesse generare proposte multiple senza vincoli |
| **Perché è sbagliato** | Un progetto con un team già Attivo che permette la generazione di nuove proposte crea ambiguità operativa reale — quale team è quello "vero"? L'AI non ha sollevato autonomamente questa domanda di business prima che l'utente se ne accorgesse usando il sistema |
| **Correzione** | Aggiunta la regola: il bottone di generazione proposte è disabilitato se esiste un team con stato diverso da Closed per quel progetto, sostituito con un messaggio informativo che indirizza al team esistente |
| **Pattern ricorrente** | Si — variante dello stesso pattern osservato in [014] (conferma cancellazione) e [017] (design UI): l'AI tende a implementare il percorso funzionale principale senza esplorare autonomamente le implicazioni di business ai bordi del flusso (cosa succede se l'azione viene ripetuta, cosa succede in caso di stati multipli) |
| **Gravità** | Media — non causa perdita di dati, ma genera ambiguità operativa concreta se non corretta prima dell'uso reale del sistema |

### [033] Nessuna sincronizzazione tra stato del Team e stato del Project
| Campo | Dettaglio |
|---|---|
| **Data** | Sessione corrente |
| **Categoria** | ASS — Assunzione non dichiarata |
| **Contesto** | Creazione di un team da una proposta tramite CreateFromProposalAsync |
| **Output errato** | L'AI ha costruito Project.Status e Team.Status come due macchine a stati completamente indipendenti, senza considerare che nella logica di dominio il progetto dovrebbe riflettere l'esistenza di un team assegnato — un progetto con team attivo che resta "Draft" è uno stato incoerente che l'utente ha dovuto segnalare |
| **Perché è sbagliato** | Due entità del dominio strettamente correlate (Project e Team) devono avere invarianti di coerenza tra i loro stati quando esiste una relazione di business diretta tra loro, non solo una relazione tecnica (FK). L'AI non ha esplorato autonomamente questa implicazione al momento di introdurre TeamStatus |
| **Correzione** | Aggiunta sincronizzazione automatica: alla creazione di un team, se il progetto è in stato Draft, transita automaticamente ad Active |
| **Pattern ricorrente** | Si — variante dello stesso pattern di [031]/[032]: l'AI introduce entità con cicli di vita propri (enum di stato) senza esplorare le dipendenze di coerenza tra entità correlate del dominio |
| **Gravità** | Media — non causa perdita dati, ma lascia il sistema in uno stato di dominio logicamente inconsistente e visibile all'utente (progetto con team attivo etichettato come bozza) |

### [034] canGenerateTeam() non considerava lo stato del progetto, solo quello del team
| Campo | Dettaglio |
|---|---|
| **Data** | Sessione corrente |
| **Categoria** | ASS — Assunzione non dichiarata |
| **Contesto** | Introduzione del cambio di stato manuale del progetto (Completed/Cancelled), successiva a canGenerateTeam() già esistente |
| **Output errato** | Quando è stata introdotta la funzionalità di cambio stato manuale del progetto, l'AI non ha aggiornato la condizione canGenerateTeam() già esistente per riflettere anche questa nuova possibilità di stato — un progetto Completed poteva teoricamente ancora mostrare il bottone di generazione team se l'ultimo team era Closed |
| **Perché è sbagliato** | Quando si introduce una nuova dimensione di stato (stato manuale del progetto) che si sovrappone a una regola di business già esistente (vincolo sul team), le due vanno riconciliate esplicitamente — l'AI ha trattato le due funzionalità come indipendenti anche se agiscono sullo stesso bottone/sezione della UI |
| **Correzione** | Estesa canGenerateTeam() con un controllo preliminare sullo stato del progetto (Completed/Cancelled → sempre false), differenziato il messaggio informativo in base alla causa reale del blocco |
| **Pattern ricorrente** | Si — quarta occorrenza della stessa famiglia di problema ([031], [032], [034]): quando l'AI introduce una nuova regola di stato/business, non verifica sistematicamente le interazioni con regole di stato già esistenti sulla stessa UI |
| **Gravità** | Bassa — scenario limite (richiede una sequenza specifica di stati per manifestarsi), ma stesso pattern strutturale da correggere alla radice più che caso per caso |

### [035] Transizione manuale Draft → Active in conflitto con la transizione automatica già esistente
| Campo | Dettaglio |
|---|---|
| **Data** | Sessione corrente |
| **Categoria** | INC — Inconsistenza |
| **Contesto** | Introduzione del controllo manuale di stato del progetto [034], in aggiunta alla sincronizzazione automatica Draft→Active già costruita in [033] |
| **Output errato** | L'AI ha definito le transizioni manuali consentite (incluso Draft→Active) senza verificare che questa si sovrapponesse e contraddicesse l'invariante di dominio appena introdotta con la sincronizzazione automatica — un progetto avrebbe potuto diventare Active senza alcun team associato, semplicemente tramite azione manuale dell'HR |
| **Perché è sbagliato** | Quando si introduce un meccanismo automatico che stabilisce un'invariante di dominio (progetto Active implica team esistente), qualsiasi meccanismo manuale introdotto successivamente sulla stessa entità deve essere verificato contro quell'invariante, non trattato come indipendente |
| **Correzione** | Rimossa la transizione manuale Draft→Active dal set di transizioni consentite sia nel backend (dizionario allowedTransitions) che nel frontend (statusTransitions) — quel passaggio avviene esclusivamente tramite creazione di un team |
| **Pattern ricorrente** | Si — quinta occorrenza della stessa famiglia strutturale ([031],[032],[034],[035]): l'AI introduce nuove regole di stato senza verificare sistematicamente le interazioni con invarianti di dominio già stabilite in precedenza sulla stessa entità |
| **Gravità** | Alta — a differenza delle occorrenze precedenti (scenari limite di UI), questa avrebbe permesso di rompere concretamente e facilmente un'invariante di dominio centrale con una singola azione manuale dell'utente, non uno scenario limite |

### [036] GetById mancante in ProjectRolesController
| Campo | Dettaglio |
|---|---|
| **Data** | Sessione corrente |
| **Categoria** | INC — Incompleto |
| **Contesto** | Costruzione iniziale di ProjectRolesController, quando la UI consumava solo la lista senza pagina di dettaglio dedicata |
| **Output errato** | L'AI ha costruito GetAll, Create, Update, Delete ma non GetById fin dall'inizio, assumendo implicitamente che la UI sarebbe rimasta a lista con editing inline senza mai richiedere il caricamento di un singolo ruolo per id |
| **Perché è sbagliato** | Un controller REST completo per un'entità con Update dovrebbe esporre GetById come pratica standard, indipendentemente da come la UI lo consumerà nell'immediato — è un endpoint a basso costo che evita esattamente questo tipo di rilavorazione quando il design della UI cambia |
| **Correzione** | Aggiunto GetById a IProjectRoleService, ProjectRoleService e ProjectRolesController |
| **Pattern ricorrente** | No, prima occorrenza specifica di GetById mancante — ma imparentata con la famiglia di pattern INC già tracciata (endpoint costruiti solo per il caso d'uso immediato invece che per la completezza dell'entità) |
| **Gravità** | Bassa — gap facilmente colmabile, nessun impatto su dati esistenti |

### [037] Sincronizzazione Project→Active agganciata al momento sbagliato del ciclo di vita del Team
| Campo | Dettaglio |
|---|---|
| **Data** | Sessione corrente |
| **Categoria** | PAT — Pattern sbagliato |
| **Contesto** | Sincronizzazione tra Team.Status e Project.Status, introdotta in [033] |
| **Output errato** | L'AI ha agganciato la transizione Project Draft→Active al momento della creazione del team (stato iniziale Proposed), invece che al momento in cui l'HR conferma esplicitamente la proposta (transizione a Approved) |
| **Perché è sbagliato** | Semanticamente "Proposed" rappresenta ancora una proposta algoritmica non confermata da nessuno — considerare il progetto "Active" sulla base di una proposta non ancora accettata anticipa una decisione che l'HR non ha ancora preso, rompendo il significato reale dello stato Active |
| **Correzione** | Rimossa la sincronizzazione da CreateFromProposalAsync, spostata in UpdateStatusAsync condizionata al passaggio esplicito a TeamStatus.Approved |
| **Pattern ricorrente** | Si — variante dello stesso pattern [033]/[035]: l'AI introduce logica di sincronizzazione tra entità correlate senza analizzare a fondo quale specifico momento del ciclo di vita rappresenti realmente la transizione semantica corretta, scegliendo il punto di innesto più immediato/ovvio (creazione) invece di quello semanticamente corretto (conferma) |
| **Gravità** | Alta — errore di modellazione del dominio individuato dall'utente, non dall'AI, nonostante fosse un'area già segnalata come sensibile da precedenti correzioni sullo stesso argomento |

### [038] Stato TeamStatus.Approved introdotto senza necessità reale, mai distinto operativamente da Active
| Campo | Dettaglio |
|---|---|
| **Data** | Sessione corrente |
| **Categoria** | OVE — Over-engineering |
| **Contesto** | Definizione iniziale dell'enum TeamStatus nel modello di dominio, primi messaggi della conversazione |
| **Output errato** | L'AI ha introdotto 4 stati (Proposed, Approved, Active, Closed) senza validare con l'utente se la distinzione tra Approved e Active corrispondesse a un passaggio realmente distinto nel processo HR, assumendo una granularità di workflow non richiesta esplicitamente |
| **Perché è sbagliato** | Un enum con uno stato che nella pratica non viene mai usato distintamente introduce complessità di codice (transizioni da gestire, casi da testare, rami di logica) senza alcun beneficio — l'AI ha modellato un processo aziendale ipotetico invece di verificarlo con chi conosce il processo reale, causando due round di correzione sulla sincronizzazione Project↔Team ([037] e questa) prima di scoprire che la distinzione alla base non serviva |
| **Correzione** | Rimosso TeamStatus.Approved, il flusso si semplifica a Proposed → Active → Closed, la sincronizzazione con Project avviene sulla transizione a Active |
| **Pattern ricorrente** | Si — imparentato con la famiglia [033]/[035]/[037] sulla modellazione degli stati, ma questa è la causa radice delle due correzioni precedenti: il vero problema non era dove agganciare la sincronizzazione, ma il fatto che il modello di stati sottostante avesse più granularità di quanta ne servisse realmente |
| **Gravità** | Alta — ha causato due cicli di correzione a cascata ([037] e la presente) prima che la vera causa radice (over-engineering del modello di stati) venisse identificata; una domanda di chiarimento fatta all'inizio, quando l'enum è stato introdotto, avrebbe evitato entrambi i round di correzione successivi |

### [039] EmployeeCompetencyDto già segnalato per campo Id mancante — pattern confermato anche in EmployeeTeamHistoryDto
| Campo | Dettaglio |
|---|---|
| **Data** | Sessione corrente |
| **Categoria** | INC — Incompleto |
| **Contesto** | Prima stesura di EmployeeTeamHistoryDto, senza ProjectId necessario per la navigazione |
| **Output errato** | L'AI ha costruito EmployeeTeamHistoryDto pensando solo alla visualizzazione (progetto, ruolo, stato, data) senza anticipare che l'utente avrebbe voluto cliccare la riga per navigare al team, nonostante lo stesso identico pattern fosse già stato tracciato tre volte in precedenza ([022],[023],[024]) su DTO diversi |
| **Perché è sbagliato** | Nonostante il pattern fosse già esplicitamente riconosciuto come ricorrente e persino formalizzato come regola generale nell'entry [024] ("ogni DTO di riga in una collezione deve sempre esporre il proprio Id/riferimenti di navigazione"), l'AI lo ha ripetuto una quarta volta su un nuovo DTO, segno che la regola generale identificata non viene applicata proattivamente ai DTO successivi, solo retroattivamente dopo la segnalazione |
| **Correzione** | Aggiunto ProjectId a EmployeeTeamHistoryDto fin dalla prima stesura di questo messaggio, non in una correzione successiva |
| **Pattern ricorrente** | Si — quarta occorrenza dello stesso pattern strutturale, nonostante fosse già stato esplicitamente formalizzato come regola generale in [024]. Indica un limite reale: identificare un pattern ricorrente in una sessione non garantisce che l'AI lo applichi automaticamente a tutti i casi futuri simili nella stessa conversazione, va ri-verificato attivamente ogni volta |
| **Gravità** | Bassa in questo caso specifico (corretto proattivamente nello stesso messaggio prima di essere consegnato, non dopo un errore dell'utente) — ma la gravità concettuale del pattern ricorrente resta alta per quello che rivela sull'affidabilità delle "regole generali" auto-identificate |

### [040] Query N+1 in GetColleaguesWithAffinityAsync
| Campo | Dettaglio |
|---|---|
| **Data** | Sessione corrente |
| **Categoria** | PAT — Pattern sbagliato |
| **Contesto** | Recupero delle coppie di colleghi con relativa affinità in EmployeeAffinityService.GetColleaguesWithAffinityAsync |
| **Output errato** | L'AI ha scritto un foreach sulle coppie di colleghi che esegue una query GetByEmployeePairAsync separata per ogni singola coppia, invece di caricare tutte le affinità rilevanti in un'unica query e incrociarle in memoria |
| **Perché è sbagliato** | È il classico antipattern N+1: con N coppie di colleghi, il metodo esegue N+1 query al database invece di 2 (una per le coppie, una per tutte le affinità). Con un numero di dipendenti realistico in un contesto aziendale, il costo di questa scelta cresce linearmente e diventa un collo di bottiglia misurabile, mentre con i pochi dati di test usati finora il problema resta invisibile |
| **Correzione** | Da applicare — caricare tutte le EmployeeAffinity esistenti in una sola query prima del ciclo, e cercare la corrispondenza in memoria con un dizionario invece di interrogare il database ad ogni iterazione |
| **Pattern ricorrente** | No, prima occorrenza esplicita di N+1 in questo progetto — ma è una classe di errore ben nota in EF Core, da tenere d'occhio ogni volta che si scrive un ciclo che contiene una chiamata a repository al suo interno |
| **Gravità** | Media — nessun impatto visibile con i dati di test attuali, ma diventerebbe un problema di performance concreto con l'uso reale del sistema in produzione |

### [041] Placeholder alert() sostituiti con sistema di notifiche centralizzato
| Campo | Dettaglio |
|---|---|
| **Data** | Sessione corrente |
| **Categoria** | UND — Under-engineering (risolto) |
| **Contesto** | ProjectRoleList e ProjectRoleDetail usavano alert() nativo del browser per segnalare l'errore 409 di eliminazione bloccata — introdotto esplicitamente come placeholder temporaneo, segnalato dall'AI stessa al momento dell'introduzione |
| **Output errato** | alert() del browser è visivamente incoerente con lo stile curato costruito in tutto il resto dell'applicazione (font di sistema, blocca il thread UI, nessuna possibilità di styling) |
| **Perché è sbagliato** | Un'interfaccia costruita con attenzione a font, palette, bottoni pill-shaped e componenti PrimeNG non dovrebbe interrompersi con un popup di sistema nativo per la gestione degli errori — rompe la coerenza visiva ed è un'esperienza utente scadente rispetto al resto dell'app |
| **Correzione** | Introdotto NotificationService centralizzato basato su MessageService/p-toast di PrimeNG, montato una sola volta in AppComponent, stilizzato coerentemente con il resto dell'app. Sostituiti tutti gli alert() esistenti ed esteso il pattern anche ai flussi di successo (creazione, modifica) che finora navigavano via senza alcun feedback visivo |
| **Pattern ricorrente** | No — era un debito tecnico già segnalato esplicitamente al momento della sua introduzione, non un errore silenzioso scoperto in retrospettiva |
| **Gravità** | Bassa — non bloccante, miglioramento di coerenza UX pianificato fin dall'inizio |

### [042] takeUntilDestroyed posizionato prima di concatMap invece che come ultimo operatore
| Campo | Dettaglio |
|---|---|
| **Data** | Sessione corrente |
| **Categoria** | PAT — Pattern sbagliato |
| **Contesto** | Pipe di cancellazione competenza in CompetencyList — delete seguito da concatMap(getAll) |
| **Output errato** | L'AI ha posizionato takeUntilDestroyed(this.destroyRef) come primo operatore della pipe, prima di concatMap, invece che come ultimo operatore immediatamente prima di subscribe() come raccomandato dalla documentazione ufficiale Angular |
| **Perché è sbagliato** | Posizionare takeUntilDestroyed a metà catena invece che alla fine rischia di non propagare correttamente la disiscrizione attraverso l'intera catena di operatori concatenati (in particolare con concatMap, che introduce una subscription interna separata) — se il componente viene distrutto mentre la chiamata interna è ancora in corso, potrebbe continuare a essere eseguita inutilmente invece di essere cancellata |
| **Correzione** | Spostato takeUntilDestroyed(this.destroyRef) come ultimo operatore della pipe, dopo concatMap, immediatamente prima di subscribe() |
| **Pattern ricorrente** | Si — probabile che lo stesso errore di posizionamento sia presente in altre pipe con concatMap già scritte in precedenza (EmployeeList.onDelete, ProjectList.onDelete) che seguono lo stesso schema — da verificare e correggere sistematicamente, non solo in questo file |
| **Gravità** | Media — comportamento scorretto solo in uno scenario limite specifico (distruzione del componente durante l'esecuzione della seconda chiamata concatenata), difficile da notare durante test manuali ma reale come rischio di richiesta HTTP orfana |

### [043] Script SQL di test con Source come stringa, contraddicendo la configurazione EF Core discussa in precedenza
| Campo | Dettaglio |
|---|---|
| **Data** | Sessione corrente |
| **Categoria** | INC — Inconsistenza |
| **Contesto** | Script SQL di inserimento dati di test per l'algoritmo di matching |
| **Output errato** | L'AI ha scritto lo script SQL usando valori stringa ('HRAssessment') per la colonna Source, assumendo che la conversione HasConversion<string>() configurata in EF Core mesi prima nella stessa conversazione fosse stata applicata al database reale, senza verificare lo schema effettivo della colonna prima di scrivere l'INSERT |
| **Perché è sbagliato** | L'AI ha fatto affidamento sulla configurazione discussa a livello di codice C# invece di verificare lo stato effettivo dello schema del database, che può divergere dal codice se la migration non è stata rigenerata dopo quella specifica configurazione, o se la configurazione non è mai stata effettivamente completata durante lo sviluppo |
| **Correzione** | Sostituiti tutti i valori stringa di Source con il valore intero corrispondente (0 per HRAssessment), in attesa di verificare con una query INFORMATION_SCHEMA lo stato reale della colonna e correggere eventualmente la configurazione EF Core con una migration dedicata |
| **Pattern ricorrente** | No, prima occorrenza — ma segnala un limite di fondo simile a [044]: l'AI ragiona sullo stato "atteso" del sistema basandosi sulla cronologia della conversazione, non sullo stato verificato del database reale, che può divergere per ragioni non tracciate nella chat (migration non generate, configurazioni modificate e mai applicate) |
| **Gravità** | Media — errore bloccante per l'esecuzione dello script di test, ma facilmente diagnosticabile e corretto una volta individuata la causa |

### [044] Dati di test per affinità inseriti senza relazione TeamMember, invisibili all'endpoint /colleagues per design
| Campo | Dettaglio |
|---|---|
| **Data** | Sessione corrente |
| **Categoria** | CTX — Contesto perso |
| **Contesto** | Script SQL di dati di test — inserimento diretto in EmployeeAffinities per coppie di dipendenti mai assegnati allo stesso Team |
| **Output errato** | L'AI ha inserito righe in EmployeeAffinities per testare l'algoritmo, segnalando esplicitamente che si trattava di un'eccezione alla regola di dominio (affinità solo dopo aver condiviso un team), ma non ha verificato che GetColleaguesWithAffinityAsync — l'endpoint che l'utente avrebbe naturalmente usato per vedere quei dati — filtra esclusivamente sulle coppie derivate da TeamMember, rendendo quei dati di test invisibili dall'interfaccia utente nonostante esistano correttamente nel database |
| **Perché è sbagliato** | L'AI aveva già in memoria (avendola scritta poche interazioni prima) la logica esatta di GetColleaguesWithAffinityAsync basata su TeamMember, ma non ha collegato le due informazioni — l'eccezione consapevole introdotta nello script e il comportamento del metodo che quei dati sarebbero dovuti alimentare — nello stesso momento in cui costruiva lo script di test |
| **Correzione** | Da chiarire con l'utente: o si crea prima un Team con quei membri per rispettare il presupposto reale dell'endpoint, oppure si usa direttamente il servizio TeamMatchingService (che legge da EmployeeAffinityRepository.GetForEmployeesAsync, non da GetColleaguePairsAsync) per verificare che il bonus di affinità funzioni nell'algoritmo di matching, indipendentemente dalla visibilità nella pagina "Affinità" |
| **Pattern ricorrente** | Si — variante del pattern [044]/[043] di questa sessione: l'AI ragiona su porzioni di sistema (script di test, endpoint di lettura) senza incrociarle sistematicamente con la logica di business già scritta in precedenza nella stessa conversazione, anche quando quella logica è direttamente rilevante e nota |
| **Gravità** | Media — nessun dato corrotto, ma ha causato confusione nel test (l'endpoint sembrava rotto quando in realtà si comportava correttamente rispetto al design) e richiesto un ciclo di debug per chiarire |

### [045] Ordinamento non deterministico dei candidati causa risultati imprevedibili in caso di pareggio di punteggio
| Campo | Dettaglio |
|---|---|
| **Data** | Sessione corrente |
| **Categoria** | PAT — Pattern sbagliato |
| **Contesto** | TeamMatchingService — ScoreCandidates e GreedyAssign, in particolare in presenza di candidati con punteggio identico per ruoli diversi |
| **Output errato** | Nessuna query coinvolta nel calcolo dei candidati (GetAllWithCompetenciesAsync) ha un ORDER BY esplicito — l'AI non ha considerato che, con chiavi primarie GUID generate casualmente, l'assenza di ordinamento esplicito rende l'ordine di restituzione delle righe imprevedibile, e che questo si propaga fino alla decisione del greedy assignment in caso di punteggi identici tra candidati |
| **Perché è sbagliato** | Un algoritmo di matching usato per decisioni di business (a chi viene assegnato un ruolo) deve essere deterministico e riproducibile — stesso input, stesso output, sempre. Con l'ordinamento implicito e non garantito del database, la stessa identica richiesta di generazione proposte potrebbe restituire risultati diversi in esecuzioni diverse, anche senza alcuna modifica ai dati, semplicemente per come SQL Server decide di materializzare le righe quella volta specifica |
| **Correzione** | Da applicare — aggiungere un ordinamento deterministico esplicito come criterio di tie-break secondario, sia nella query dei dipendenti sia nell'ordinamento finale dei candidati nell'algoritmo (es. OrderByDescending(score).ThenBy(employeeId) o ThenBy(lastName)) |
| **Pattern ricorrente** | No, prima occorrenza di questo tipo specifico — ma è un errore di categoria nota e ben documentata (query senza ORDER BY con comportamento non deterministico), che avrebbe dovuto essere prevenuto per default in qualsiasi query il cui risultato alimenta una logica di business sensibile all'ordine, non solo scoperto tramite test |
| **Gravità** | Alta — il cuore del progetto (l'algoritmo di matching) può produrre risultati diversi e imprevedibili a parità di dati, compromettendo l'affidabilità delle proposte generate e rendendo i test difficili da riprodurre in modo consistente |

### [046] Algoritmo greedy senza priorità di scarsità causa slot non riempiti quando un candidato "jolly" ha punteggio pari su più ruoli
| Campo | Dettaglio |
|---|---|
| **Data** | Sessione corrente |
| **Categoria** | PAT — Pattern sbagliato |
| **Contesto** | GreedyAssign in TeamMatchingService, terza iterazione del test con Elena come unica candidata idonea per Tech Lead ma con punteggio pari anche su Frontend e Backend, dove esistevano alternative |
| **Output errato** | L'AI ha corretto in [045] solo la mancanza di determinismo (stesso input → stesso output), ma non aveva identificato il problema algoritmico più profondo sottostante: un greedy che processa le coppie (slot, candidato) ordinate solo per punteggio, senza considerare quanti candidati alternativi esistono per ciascuno slot, può assegnare un candidato versatile al primo slot che incontra nell'ordinamento, anche se quello slot aveva alternative valide, lasciando scoperto uno slot che invece non ne aveva nessuna |
| **Perché è sbagliato** | È un limite noto della programmazione greedy applicata a problemi di assegnamento (assignment problem) — ottimizzare localmente ad ogni passo (il punteggio più alto disponibile) non garantisce l'ottimo globale (il team completo con il punteggio totale più alto, o quantomeno completo). L'AI aveva già introdotto in [045] un correttivo parziale (tie-break per determinismo) scambiandolo per la soluzione completa, senza verificare se il risultato prodotto fosse effettivamente corretto oltre che riproducibile |
| **Correzione** | Riscritto GreedyAssign per processare prima gli slot con il minor numero di candidati idonei disponibili (scarcity-first), garantendo che i ruoli senza alternative vengano riempiti prima che i loro unici candidati idonei vengano assegnati altrove |
| **Pattern ricorrente** | Si — variante specifica del pattern [044]/[045] di questa sessione: una correzione tecnica (determinismo) è stata presentata e verificata come se risolvesse il problema di fondo, quando in realtà copriva solo un sintomo collaterale (risultati diversi tra esecuzioni) lasciando intatta la causa reale (logica di assegnazione subottimale) |
| **Gravità** | Alta — anche con dati realistici (non solo nello scenario di test costruito ad hoc), uno scenario di "candidato eccellente su più ruoli con alternative asimmetriche" è plausibile in un contesto HR reale, e produrrebbe proposte incomplete o subottimali senza che l'utente abbia modo di capire perché, se non validando manualmente ogni singola proposta come fatto qui |


### [047] Eliminazione fisica del progetto consentita indipendentemente dallo stato
| Campo | Dettaglio |
|---|---|
| **Data** | Sessione corrente |
| **Categoria** | ASS — Assunzione non dichiarata |
| **Contesto** | Bottone "Elimina progetto" in ProjectDetail, introdotto nella prima stesura del componente e mai rivisto dopo l'introduzione delle transizioni di stato (Draft/Active/Completed/Cancelled) |
| **Output errato** | L'AI ha costruito l'eliminazione fisica del progetto come azione sempre disponibile indipendentemente dallo stato, senza considerare che una volta introdotto un ciclo di vita con stati distinti (in particolare Cancelled come "via distruttiva morbida"), l'eliminazione fisica e il cambio di stato a Cancelled diventano due azioni sovrapposte con lo stesso intento ma conseguenze molto diverse (perdita permanente dello storico vs preservazione) |
| **Perché è sbagliato** | Un progetto Active o Completed può avere team, storico di affinità e altre relazioni di dominio significative — eliminarlo fisicamente cancella tutta quella storia, mentre lo stato Cancelled la preserva. L'AI non ha esplorato questa sovrapposizione quando ha introdotto ProjectStatus, lasciando che l'utente se ne accorgesse riflettendo autonomamente sul flusso |
| **Correzione** | Bloccata l'eliminazione fisica lato backend per progetti non più in Draft, nascosto il bottone lato frontend nella stessa condizione, differenziato il messaggio di errore tra le due cause di blocco possibili (stato non Draft vs vincolo di integrità referenziale) |
| **Pattern ricorrente** | Si — ennesima variante della famiglia di pattern sulle transizioni di stato ([031],[032],[034],[035],[037],[038]): l'introduzione di un ciclo di vita a stati non è stata accompagnata da una revisione sistematica di tutte le azioni CRUD esistenti sulla stessa entità per verificarne la coerenza con gli stati introdotti successivamente |
| **Gravità** | Media — nessuna perdita di dati già avvenuta segnalata dall'utente, ma il rischio era concreto e strutturale, non uno scenario limite raro |

### [048] Vincolo di eliminazione applicato solo in ProjectDetail, non in ProjectList
| Campo | Dettaglio |
|---|---|
| **Data** | Sessione corrente |
| **Categoria** | INC — Incompleto |
| **Contesto** | Correzione [047] applicata al bottone "Elimina progetto" in ProjectDetail |
| **Output errato** | L'AI ha corretto il vincolo di eliminazione solo nel punto in cui la richiesta era stata sollevata (form di dettaglio), senza verificare autonomamente che lo stesso identico bottone/azione esistesse duplicato in un altro componente (la tabella di ProjectList), lasciando lì la falla esattamente come prima della correzione |
| **Perché è sbagliato** | Quando un'azione CRUD (in questo caso l'eliminazione) è esposta in più punti dell'interfaccia per la stessa entità — pattern già noto e ricorrente in questo progetto (liste + pagine di dettaglio hanno sempre entrambe un punto di eliminazione) — una correzione di business logic va sistematicamente verificata su tutti i punti di accesso equivalenti, non solo su quello esplicitamente segnalato |
| **Correzione** | Applicato lo stesso vincolo (visibilità del bottone elimina solo per progetti in stato Draft) anche in ProjectList, con gestione dell'errore coerente tramite toast |
| **Pattern ricorrente** | Si — variante diretta di [032] (stesso identico pattern: una regola corretta in un punto non viene automaticamente propagata al punto gemello nella lista) applicata questa volta all'eliminazione invece che alla modifica dei requisiti. Indica che il pattern "lista + dettaglio hanno azioni duplicate da sincronizzare" non è ancora stato interiorizzato come checklist automatica |
| **Gravità** | Media — la falla di business logic segnalata in [047] sarebbe rimasta comunque sfruttabile dalla lista, vanificando parzialmente la correzione appena fatta |

### [049] Algoritmo di matching non escludeva dipendenti già impegnati in team attivi su altri progetti
| Campo | Dettaglio |
|---|---|
| **Data** | Sessione corrente |
| **Categoria** | ASS — Assunzione non dichiarata |
| **Contesto** | TeamMatchingService.GenerateProposalsAsync — selezione del pool di candidati eleggibili |
| **Output errato** | L'AI ha costruito l'algoritmo di matching considerando come candidati tutti i dipendenti esistenti nel sistema, senza verificare la loro disponibilità reale rispetto a impegni già assunti in team attivi su altri progetti — un dipendente poteva quindi comparire come candidato per un nuovo progetto anche se già pienamente coinvolto altrove |
| **Perché è sbagliato** | Un sistema che genera proposte di team per decisioni HR reali deve riflettere la capacità e disponibilità effettiva delle persone, non solo la loro idoneità tecnica — proporre un dipendente già impegnato altrove genera proposte che nella pratica l'HR non potrebbe mai accettare, riducendo l'affidabilità e l'utilità reale dello strumento |
| **Correzione** | Aggiunta esclusione a monte dei dipendenti membri di un team con stato Active su qualunque progetto, prima ancora del calcolo dei punteggi di eleggibilità |
| **Pattern ricorrente** | Si — variante della famiglia di pattern sulle assunzioni di business non esplorate autonomamente dall'AI (vedi [014] conferma cancellazione, [031]-[035] transizioni di stato): un vincolo di capacità/disponibilità delle risorse è un requisito HR fondamentale che avrebbe dovuto essere sollevato proattivamente al momento della progettazione dell'algoritmo, non solo dopo che l'utente lo ha notato |
| **Gravità** | Alta — è un limite che tocca il cuore del sistema (l'algoritmo di matching) e la sua affidabilità pratica in un contesto d'uso reale, non uno scenario limite raro |

### [050] EmployeeTeamHistoryDto.teamStatus tipizzato come string invece di TeamStatus
| Campo | Dettaglio |
|---|---|
| **Data** | Sessione corrente |
| **Categoria** | ASS — Assunzione non dichiarata |
| **Contesto** | Definizione di EmployeeTeamHistoryDto in [039], per la sezione "Storico team" del dettaglio dipendente |
| **Output errato** | L'AI ha tipizzato il campo teamStatus come string generico invece che come l'enum TeamStatus già esistente e usato altrove nel progetto (team.model.ts), nonostante la regola "mai string quando esiste un enum di dominio corrispondente" fosse già stata stabilita esplicitamente in [008] diverse interazioni prima |
| **Perché è sbagliato** | È la stessa violazione di una regola già identificata e formalizzata in precedenza nella stessa conversazione — la type safety che l'enum garantirebbe (impossibilità di passare un valore non valido, autocompletamento, refactoring sicuro) viene persa, e il problema si manifesta solo più tardi, quando un'altra funzione tipizzata correttamente (teamStatusLabel) rifiuta di accettare il valore troppo generico |
| **Correzione** | Cambiato il tipo del campo teamStatus da string a TeamStatus in EmployeeTeamHistoryDto, permettendo l'uso diretto delle utility condivise teamStatusLabel/teamStatusSeverity |
| **Pattern ricorrente** | Si — seconda occorrenza esplicita della stessa regola violata (la prima in [008] su CompetencyDto.Type), a conferma che una regola identificata come pattern ricorrente in un punto della conversazione non viene automaticamente applicata a tutti i DTO scritti successivamente, va riverificata attivamente ogni volta che si introduce un nuovo DTO con un campo che rappresenta uno stato enumerato |
| **Gravità** | Media — errore di compilazione bloccante ma di causa chiara, con impatto limitato al singolo file una volta corretto |
---

## Statistiche

| Categoria | Occorrenze |
|---|---|
| VER — Versione errata | 1 |
| ASS — Assunzione non dichiarata | 1 |
| PAT — Pattern sbagliato | 1 |
| **Totale** | **3** |

---

## Pattern ricorrenti identificati

### Pattern 1 — Mismatch versioni librerie
**Frequenza:** 1 occorrenza  
**Descrizione:** L'AI suggerisce pacchetti senza specificare la versione compatibile con il framework target.  
**Regola di verifica:** Quando si installa un pacchetto NuGet verificare sempre che la major version corrisponda alla versione di .NET usata nel progetto.

---

### Pattern 2 — Campi aggiunti per completezza senza validazione dominio
**Frequenza:** 1 occorrenza  
**Descrizione:** L'AI aggiunge campi alle entity basandosi su pattern generici invece che sui requisiti specifici del dominio.  
**Regola di verifica:** Per ogni campo aggiunto dall'AI chiedersi: "Chi popola questo campo? Quando? Ha senso nel mio dominio specifico?"

---

### Pattern 3 — Semplificazione delle relazioni contestuali
**Frequenza:** 1 occorrenza  
**Descrizione:** L'AI tende a modellare relazioni al livello più alto possibile, perdendo la flessibilità contestuale.  
**Regola di verifica:** Per ogni relazione chiedersi: "Questo valore è sempre uguale o cambia per contesto?"

---

## Note metodologiche

### Come usare questo documento
1. Durante ogni sessione di sviluppo, analizza criticamente ogni output dell'AI prima di applicarlo
2. Se qualcosa non torna, fai la domanda — spesso rivela un errore o un'assunzione nascosta
3. Documenta immediatamente l'errore con tutti i campi della struttura
4. Aggiorna le statistiche e i pattern ricorrenti
5. Usa i pattern identificati come checklist di verifica per gli output futuri

### Domande utili da porsi su ogni output AI
- Questo codice compila?
- È coerente con le decisioni architetturali prese in precedenza?
- L'AI ha fatto assunzioni che non ho validato?
- Ogni campo/classe/metodo ha un motivo preciso nel mio dominio?
- La versione delle librerie è compatibile con il mio stack?
- Questo pattern è corretto per la Clean Architecture che stiamo usando?

