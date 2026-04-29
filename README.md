סס# 📦 Delivery Management System (WPF / .NET)

A desktop application for managing deliveries in a computer equipment company.

---

## 🎯 Overview

This project simulates a delivery management system that manages orders, couriers, and system operations.
It was developed as part of an academic project, focusing on clean architecture and real-world system design.

---

## 🏗️ Architecture

The system is built using a **3-Tier Layered Architecture**:

* **DAL (Data Access Layer)** – handles data storage (List/XML)
* **BL (Business Logic Layer)** – contains system logic and rules
* **PL (Presentation Layer)** – WPF application using Data Binding

This separation ensures maintainability and clean code structure.

---

## ✨ Features

* 📦 Order and courier management
* 📊 System dashboard with real-time overview
* ⚙️ Configurable system parameters (distance, time, risk ranges)
* 🔄 System clock control (minute, hour, day, month, year)
* 👨‍💼 Courier list with filtering and detailed view
* ➕ Add / update courier information

---

## 🛠️ Technologies

* C#
* .NET 8
* WPF (XAML)
* LINQ
* XML Serialization

---

## 🎨 Design Patterns

* Singleton
* Factory
* Observer

---

## 🚀 How to Run

> ⚠️ This project is still in development and may not include full functionality.

1. Clone the repository
2. Open the solution in Visual Studio
3. Run the **PL project**

---

## ⚠️ Project Status

This project is **in progress**.
Core architecture and main features are implemented, while some advanced features (such as multithreading and full simulation) are still under development.

---

## 🧠 What I Learned

* Building multi-layered applications
* Working with WPF and Data Binding
* Applying design patterns in real systems
* Designing clean and maintainable architecture

---

## 📌 Notes

This project focuses mainly on architecture and design rather than production-ready deployment.
