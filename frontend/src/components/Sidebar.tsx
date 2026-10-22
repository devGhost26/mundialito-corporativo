"use client";

import Link from "next/link";
import { usePathname } from "next/navigation";

const icons = {
  table: <path d="M3 5h18M3 12h18M3 19h18M8 5v14" />,
  ball: (
    <>
      <circle cx="12" cy="12" r="9" />
      <path d="m12 7 4 3-1.5 4.5h-5L8 10z" />
    </>
  ),
  calendar: (
    <>
      <rect x="3" y="5" width="18" height="16" rx="2" />
      <path d="M3 10h18M8 3v4M16 3v4" />
    </>
  ),
  users: (
    <>
      <circle cx="9" cy="8" r="3.5" />
      <path d="M2.5 20a6.5 6.5 0 0 1 13 0M16 4.5a3.5 3.5 0 0 1 0 7M18 14a6 6 0 0 1 3.5 6" />
    </>
  ),
};

const links = [
  { href: "/standings", label: "Tabla de posiciones", icon: icons.table },
  { href: "/scorers", label: "Goleadores", icon: icons.ball },
  { href: "/matches", label: "Partidos", icon: icons.calendar },
  { href: "/teams", label: "Equipos", icon: icons.users },
];

export function Sidebar() {
  const pathname = usePathname();

  return (
    <aside className="sidebar">
      <div className="brand">
        <div className="brand-logo" aria-hidden="true">
          <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
            {icons.ball}
          </svg>
        </div>
        <div>
          <div className="brand-name">Mundialito</div>
          <div className="brand-sub">Torneo corporativo</div>
        </div>
      </div>

      <nav className="nav" aria-label="Secciones">
        <div className="nav-label">Torneo</div>
        {links.map((link) => (
          <Link key={link.href} href={link.href} className={pathname.startsWith(link.href) ? "active" : undefined}>
            <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="1.8" strokeLinecap="round" strokeLinejoin="round" aria-hidden="true">
              {link.icon}
            </svg>
            {link.label}
          </Link>
        ))}
      </nav>

      <div className="sidebar-footer">API .NET 10 · Next.js</div>
    </aside>
  );
}
