import type { Metadata } from "next";
import Link from "next/link";
import "./globals.css";

export const metadata: Metadata = {
  title: "Mundialito Corporativo",
  description: "Control del torneo de fútbol corporativo",
};

export default function RootLayout({ children }: { children: React.ReactNode }) {
  return (
    <html lang="es">
      <body>
        <nav>
          <Link href="/standings">Tabla</Link>
          <Link href="/scorers">Goleadores</Link>
          <Link href="/matches">Partidos</Link>
          <Link href="/teams">Equipos</Link>
        </nav>
        <main>{children}</main>
      </body>
    </html>
  );
}
