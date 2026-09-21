import { Component } from '@angular/core';
import { RouterOutlet, RouterLink, RouterLinkActive } from '@angular/router';
import { Loader } from './shared/components/loader';
import { ConfirmDialog } from './shared/components/confirm-dialog';
import { Toast } from './shared/components/toast';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, RouterLink, RouterLinkActive, Loader, ConfirmDialog, Toast],
  template: `
    <app-loader />
    <app-confirm-dialog />
    <app-toast />

    <header class="navbar">
      <div class="navbar__inner">
        <a routerLink="/employees">
        <img src="/logo-wordmark.svg" alt="TeamCraft" class="navbar__logo" /> 
      </a>

        <nav class="navbar__links">
          <a routerLink="/employees" routerLinkActive="is-active">Dipendenti</a>
          <a routerLink="/competencies" routerLinkActive="is-active">Competenze</a>
          <a routerLink="/project-roles" routerLinkActive="is-active">Ruoli</a>
          <a routerLink="/projects" routerLinkActive="is-active">Progetti</a>
          <a routerLink="/affinities" routerLinkActive="is-active">Affinità</a>
        </nav>
      </div>
    </header>

    <main>
      <router-outlet />
    </main>
  `,
  styles: [`
    .navbar {
      position: sticky;
      top: 0;
      z-index: 100;
      background: rgba(255, 255, 255, 0.72);
      backdrop-filter: saturate(180%) blur(20px);
      -webkit-backdrop-filter: saturate(180%) blur(20px);
      border-bottom: 1px solid rgba(0, 0, 0, 0.06);
    }

    .navbar__inner {
      max-width: 1100px;
      margin: 0 auto;
      height: 52px;
      display: flex;
      align-items: center;
      justify-content: space-between;
      padding: 0 24px;
    }

    .navbar__logo {
    width: auto;
    height: 40px;
    margin-right: 8px;
    vertical-align: middle;
    }

    .navbar__links {
      display: flex;
      gap: 28px;
    }

    .navbar__links a {
      font-size: 13px;
      font-weight: 500;
      color: var(--color-text-secondary);
      text-decoration: none;
      padding: 6px 0;
      position: relative;
      transition: color 0.2s ease;
    }

    .navbar__links a:hover {
      color: var(--color-text-primary);
    }

    .navbar__links a.is-active {
      color: var(--color-text-primary);
    }

    .navbar__links a.is-active::after {
      content: '';
      position: absolute;
      bottom: -17px;
      left: 0;
      right: 0;
      height: 2px;
      background: var(--color-accent);
      border-radius: 2px;
    }

    main {
      min-height: calc(100vh - 52px);
    }

    @media (max-width: 640px) {
      .navbar__links {
        gap: 16px;
      }

      .navbar__links a {
        font-size: 12px;
      }
    }
  `]
})
export class App { }