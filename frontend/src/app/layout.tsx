import type { Metadata } from "next";
import { Sidebar } from "@/components/Sidebar";
import "./globals.css";

export const metadata: Metadata = {
  title: "Mundialito Corporativo",
  description: "Control del torneo de fútbol corporativo",
};

export default function RootLayout({ children }: { children: React.ReactNode }) {
  return (
    <html lang="es">
      <body>
        <div className="app">
          <Sidebar />
          <main className="main">{children}</main>
        </div>
      </body>
    </html>
  );
}
