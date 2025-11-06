# Proyecto ETL Worker (.NET 8 + SQLite)

Este proyecto implementa un **proceso ETL (Extract, Transform, Load)** automatizado usando un **Worker Service en .NET 8**, diseñado para ejecutarse de forma continua y extraer datos de múltiples fuentes:

- 🧾 CSV local  
- 🌐 API REST  
- 🗄️ Base de datos SQLite local (staging)

---

## 🚀 Características principales

- Extracción simultánea de múltiples fuentes (`IExtractor`).
- Guardado temporal en archivos `.json` (`Data/staging/`).
- Almacenamiento persistente en una base de datos local `SQLite` (`Database/etl_data.db`).
- Logging detallado con `ILogger`.
- Código modular y mantenible con arquitectura por capas.

---

## 🧠 Tecnologías utilizadas

| Tecnología | Uso principal |
|-------------|----------------|
| .NET 8 | Plataforma base |
| Worker Service | Ejecución en segundo plano |
| Dapper | Acceso rápido a datos |
| SQLite | Almacenamiento local sin configuración |
| CsvHelper | Lectura de archivos CSV |
| HttpClient | Consumo de API REST |
| Microsoft.Extensions.Logging | Registro de eventos |

---

## ⚙️ Instalación y ejecución

### 1️⃣ Clonar el repositorio

```bash
git clone https://github.com/JoseLB220/ETL.Worker
cd ETL.Worker
