# 🎬 Demo Video Script — Delivery Hub v1.0

**Target length:** 60–90 seconds · **Audience:** tech recruiters / engineering managers
**Goal:** show a working app *and* the engineering thinking behind it.

> Tip: record at 1080p. Capture the window only (not your whole desktop).
> Save the final clip as `docs/assets/demo.gif` (or attach an MP4 to the release).

---

## 🎙️ Narration outline (EN) + on-screen bullets (HE)

### 0:00–0:10 — Hook & Login
**EN (voiceover):** "This is a delivery-management desktop app for a computer-store chain, built on a clean 3-tier architecture in WPF and .NET 8."
**HE (on-screen text):** מערכת ניהול משלוחים — ארכיטקטורה בת 3 שכבות ב-WPF ו-.NET 8.
**Show:** the modern login screen. Type the admin ID `204857392` / password `admin123`, click the 👁 show-password toggle, press **Sign In**.

### 0:10–0:30 — Admin: Order Management
**EN:** "As the admin, I land on the dashboard. I can advance the simulated system clock, tune configuration, and see a live orders summary."
**HE:** דשבורד מנהל: הזזת שעון מערכת, הגדרות, סיכום הזמנות חי.
**Show:** click **Order List** → open `order_management.png` view. Use the filter/sort dropdowns, then double-click a row to open **Order Details** (show the deliveries sub-grid). Click **+ Add Order**, fill a couple of fields, save.

### 0:30–0:55 — Courier: Assignment & Status Update
**EN:** "Now logging in as a courier — a focused self-service view. I pick an available order, which assigns it to me, then I handle the in-progress delivery and record the result."
**HE:** כניסה כשליח: בחירת הזמנה זמינה → השלמת טיפול → עדכון תוצאה.
**Show:** (log out →) log in as courier `312458962` / `courier123`. Open **Choose Order**, select an order → it appears in **Order In Progress**. Choose a finish type, click **Finish Handling**. Open **Delivery History** to show the recorded delivery.

### 0:55–1:20 — Architecture recap
**EN:** "Under the hood: a presentation layer over a business layer over a swappable data layer — XML or in-memory, chosen by one config line. The UI updates live through the Observer pattern, not polling."
**HE:** מתחת למכסה: PL ← BL ← DAL ניתן להחלפה (XML / זיכרון). העדכונים בזמן אמת דרך Observer, ללא polling.
**Show:** cut to a slide of the architecture diagram (or the README section), then back to the app. End on the login screen.

---

## ✅ Checklist before recording
- [ ] Database initialized (so there's data to show): Admin → **Init DB**.
- [ ] Have a courier with a known password ready (check `xml/couriers.xml`).
- [ ] Close other windows so the recording is clean.
- [ ] Window size ~1100×720 for list windows.

## 📝 Suggested closing line
**EN:** "Delivery Hub — a delivery-management platform for computer-equipment retailers, engineered around clean architecture, design patterns, and real-world UI. Full source and a self-contained build are linked in the README."
**HE:** Delivery Hub — מערכת ניהול משלוחים לקמעונאי ציוד מחשוב, מבוססת ארכיטקטורה נקייה, תבניות עיצוב ו-UI מקצועי. הקוד המלא וגרסה עצמאית להורדה ב-README.