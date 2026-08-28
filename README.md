# 📦 Delivery Management System

### A desktop logistics control center for a computer-equipment retailer — built with WPF / .NET 8 on a clean 3-tier architecture.

<p align="center">
  <em>A role-based (Admin / Courier) delivery management desktop app with a live dashboard, real-time observer-driven updates, and swappable XML / in-memory persistence.</em>
</p>

---

## 📸 Screenshots & Demo

> _Placeholder slots — drop your captures in `docs/` and update the links._

| Login Screen | Admin Dashboard | Courier Dashboard |
|---|---|---|
| `![Login](docs/login.png)` | `![Dashboard](docs/dashboard.png)` | `![Courier](docs/courier.png)` |

| Order List | Order Details | Courier List | Delivery History |
|---|---|---|---|
| `![Orders](docs/orders.png)` | `![OrderDetails](docs/order-details.png)` | `![Couriers](docs/couriers.png)` | `![History](docs/history.png)` |

🎥 **Demo GIF placeholder:** `![Demo](docs/demo.gif)`

---

## 🎯 Project Overview

**Delivery Manager** is a desktop application that simulates the end-to-end delivery operation of a computer-equipment company. It manages **orders**, **couriers**, and **deliveries**, and exposes two distinct experiences through a single login screen:

- **Admin / Manager** — full operational control: a system dashboard with a configurable clock, live orders summary with filtering, system configuration, and CRUD over couriers and orders.
- **Courier** — a focused self-service workspace: view/edit personal details, pick an available order, handle an in-progress delivery, and review past delivery history.

The project was developed as an academic exercise focused on **clean architecture, design patterns, and real-world system design** — layered separation, the Observer pattern for live UI synchronization, thread-safe updates, and a pluggable data-access layer.

---

## 🏗️ Architecture & Design Patterns

The solution follows a strict **3-Tier Layered Architecture** with a shared facade contract. Each layer depends only on the abstraction of the layer below it.

```
┌──────────────────────────────────────────────┐
│   PL  — Presentation Layer (WPF / XAML)      │  Data binding · commands · observers
├──────────────────────────────────────────────┤
│   BL  — Business Logic Layer (.NET 8)        │  Validation · rules · notifications
├──────────────────────────────────────────────┤
│   DalFacade — DAL contract (interfaces)      │  ICrud<T> · IDal · factory · config
├──────────────────────────────────────────────┤
│   DalList / DalXml — DAL implementations     │  In-memory list  ·  XML serialization
└──────────────────────────────────────────────┘
```

| Layer | Project | Responsibility |
|---|---|---|
| **PL** | `PL/` | WPF windows, data binding, converters, observer-driven refresh |
| **BL** | `BL/` | Business rules, DTOs (`BO/`), manager helpers, observer hubs |
| **DalFacade** | `DalFacade/` | DAL interfaces (`ICrud<T>`, `IDal`), DO entities, `Factory`, exceptions |
| **DalList** | `DalList/` | In-memory implementation with seeded data |
| **DalXml** | `DalXml/` | XML-file persistence (serialization) |
| **DalTest** | `DalTest/` | Unit/integration tests for the DAL |
| **BITest** | `BITest/` | Backwards-compatibility / integration tests |

The active DAL is chosen at runtime via `xml/dal-config.xml` (`<dal>xml</dal>` → DalXml, `list` → DalList) and instantiated through the **Factory** pattern, so the BL never knows which storage engine is running.

### Design Patterns Used

- **Singleton** — `Factory.Get()` returns a single shared `IBl` / `IDal` instance.
- **Factory** — `BlApi.Factory` and `DalApi.Factory` decouple creation from implementation.
- **Observer** — the BL exposes `AddObserver / RemoveObserver`; PL windows subscribe and refresh only the affected data (clock, config, orders, couriers) — no polling.
- **Adapter / Facade** — `DalFacade` presents a uniform CRUD interface over two very different backends.
- **Dependency Injection via interfaces** — every layer programs to interfaces (`IBl`, `ICourier`, `IOrder`, `IAdmin`, `ICrud<T>`).
- **Thread-safe observer dispatch** — `ObserverMutex` + `Dispatcher.BeginInvoke` marshal background notifications onto the UI thread and prevent re-entrant refresh storms.

### Concurrency & Reliability

- Background observer callbacks are marshaled to the UI thread via the WPF `Dispatcher`.
- A dedicated `ObserverMutex` guards each refresh path against re-entrancy and schedules a re-run if a notification arrives mid-refresh.
- Password verification accepts both hashed and legacy plaintext stored values, so seeded demo data logs in cleanly.

---

## ✨ Core Features

### 🛡️ Admin / Manager
- **System Clock** — advance the simulated clock by minute / hour / day / month / year; the UI updates live.
- **System Configuration** — tune max delivery distance, max delivery time, risk range, and inactivity time.
- **Orders Summary** — filter the live order count by _Delivery Done Type_ and _Schedule Status_, then jump straight to the filtered order list.
- **Courier Management** — list, filter by transport, add, update, and delete couriers.
- **Order Management** — list, filter, sort, add, update, and cancel orders; inspect every delivery attached to an order.
- **Database Tools** — initialize (re-seed) or reset the database from the dashboard.

### 🚚 Courier
- **Personal Details** — view and update name, phone, email, transport, max distance, and delivery type.
- **Order In Progress** — see the active delivery (address, customer, phone, time windows) and finish handling with a result type.
- **Choose Order** — browse available orders and claim one.
- **Delivery History** — review every past delivery with handling time and result.

### 🔐 Authentication
- A single modern login screen routes users to the correct experience based on credentials.
- Inline validation feedback (no jarring message boxes for routine input errors).
- Show/hide password toggle, Enter-to-submit, role badges.

---

## 🛠️ Technologies

| | |
|---|---|
| **Language** | C# 12 |
| **Platform** | .NET 8 (WPF, `net8.0-windows`) |
| **UI** | WPF + XAML, data binding, value converters, shared styled theme |
| **Data** | XML serialization (`DalXml`) / in-memory lists (`DalList`) |
| **Patterns** | Singleton, Factory, Observer, Facade |
| **Query** | LINQ |
| **Tests** | xUnit-style DAL & integration tests (`DalTest`, `BITest`) |

---

## 🔑 Demo Credentials

> The app ships seeded with the data below. **Couriers are re-seeded with new random IDs/passwords whenever "Init DB" is run** — to look up live values, open `xml/couriers.xml`.

| Role | User ID | Password | Notes |
|---|---|---|---|
| **Admin / Manager** | `326205614` | `admin326` | Stored in `xml/data-config.xml` under `<Managers>`. |
| **Courier** | `300000000` | `Pass4645` | Example — read current values from `xml/couriers.xml`. |
| **Courier** | `300000001` | `Pass8740` | Example — read current values from `xml/couriers.xml`. |

---

## 🚀 Local Setup & Run

### Prerequisites
- **.NET 8 SDK** (with WPF / Windows desktop workload) — Windows required for the WPF UI.
- (Optional) Visual Studio 2022 for an IDE experience.

### 1. Clone
```bash
git clone <your-repo-url>
cd dotnet5786_5614_4320
```

### 2. Build
```bash
dotnet build dotnet5786_5614_4320.sln -c Debug
```
All projects share a common output directory: `bin/`.

### 3. Run
The startup project is **`PL`** (`StartupUri="LoginPage.xaml"`). The app must run from a directory whose **parent contains an `xml/` folder** (DalXml loads data from `..\xml\`). The shared `bin/` output satisfies this (`bin/../xml`).

```bash
# From the repo root:
dotnet bin/PL.dll
#   — or —
bin/PL.exe
```

Sign in with the **Admin** credentials above to reach the dashboard.

### 4. Switch the data backend (optional)
Edit `xml/dal-config.xml`:
```xml
<dal>xml</dal>   <!-- or: list -->
```

### 5. Run the tests (optional)
```bash
dotnet test DalTest/DalTest.csproj
dotnet test BITest/BITest.csproj
```

---

## 📦 Release Package

A self-contained, no-.NET-install-required build is published for Windows x64:

```bash
dotnet publish PL/PL.csproj -c Release -r win-x64 --self-contained true -o publish/app
# copy the data files so ..\xml\ resolves at runtime
cp -r xml publish/xml
```

The packaged archive **`DeliveryApp_v1.0.zip`** contains:
- `app/` — the self-contained executable and all runtime files,
- `xml/` — the data files (`orders.xml`, `deliveries.xml`, `couriers.xml`, `data-config.xml`, `dal-config.xml`).

Unzip anywhere and run `app/PL.exe`.

---

## 🗂️ Project Structure

```
dotnet5786_5614_4320/
├── PL/                     # WPF presentation layer (startup project)
│   ├── App.xaml            # App + shared modern theme (buttons/cards/inputs)
│   ├── LoginPage.xaml      # Modern role-aware login screen
│   ├── MainWindow.xaml     # Admin dashboard (clock, config, summary, actions)
│   ├── Courier/            # Courier windows (list, self, edit, history, choose-order)
│   ├── Order/              # Order windows (list, details)
│   ├── Converters/         # Value converters
│   └── Helpers/            # ObserverMutex, etc.
├── BL/                     # Business logic + BO DTOs + manager helpers
├── DalFacade/              # DAL interfaces & DO entities
├── DalList/                # In-memory DAL
├── DalXml/                 # XML-persistence DAL
├── DalTest/                # DAL tests
├── BITest/                 # Integration tests
├── xml/                    # Runtime data files
└── dotnet5786_5614_4320.sln
```

---

## 📝 Notes for Reviewers

- **Layering is enforced through interfaces**, not just folders — the BL references `DalFacade` (the contract), never a concrete DAL.
- **Live UI updates** are driven by the Observer pattern, not timers — a background change in the BL notifies only the subscribed windows.
- **The DAL is swappable at runtime** via a single config line, demonstrating the Factory + Facade patterns in practice.
- **UI consistency** is centralized: a shared theme in `App.xaml` (`AppPrimaryButton`, `AppCard`, `AppTextBox`, …) keeps every window visually coherent.

---

## 📄 License

Academic project — provided as-is for educational and portfolio purposes.