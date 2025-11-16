# Agri-Energy Connect

Agri-Energy Connect is a web-based platform designed to bridge the gap between South African farmers and green energy technology providers. 
The system enables sustainable agriculture by supporting collaboration, resource sharing, product visibility, and access to environmentally friendly energy solutions.

This project forms part of a Portfolio of Evidence (PoE) an academic project that is focused on promoting eco-friendly farming practices and enabling a digital ecosystem for agricultural and renewable energy integration.

## Objective

The primary objective of Agri-Energy Connect is to:

- Support farmers in showcasing and managing their agricultural products.
- Provide access to renewable energy solutions that enhance agricultural productivity.
- Allow employees to oversee and manage farmer registrations and product data.
- Promote collaboration and education around sustainable and energy-efficient farming.

## Key Features

### 1. **Role-Based Access Control**
The platform includes three user roles:
| Role | Description |
|------|-------------|
| **Admin** | System-level oversight, manage farmer records and view all products. |
| **Employee** | Manages farmer records and views all products. |
| **Farmer** | Manages their own products and personal profile. |

### 2. **Farmer Product Management**
Farmers can:
- Add, update, and view their own products.
- Track details such as category, production date, and organic status.

### 3. **Produce & Renewable Energy Product Support**
Products are categorized into two main types:
| Type | Examples |
|------|----------|
| **Produce** | Fruits, vegetables, eggs, honey, grains, meat, herbs, etc. |
| **Energy Solutions** | Solar irrigation kits, wind turbines, cold-room solar systems, biogas digesters, battery banks, etc. |

### 4. **Employee Farmer Management**
Employees can:
- Register new farmers.
- View all registered farmers.
- View products associated with each farmer.

### 5. **Seeded Realistic Sample Data**
The database is seeded with:
- Example farmers across different provinces.
- A dedicated **energy solutions vendor** (with Farmer role).
- A diverse catalog of produce and renewable technology solutions.

## Technology Stack

| Layer | Technology |
|-------|------------|
| Front-End | ASP.NET Core MVC, Razor Views, Bootstrap 5 |
| Back-End | ASP.NET Core 8, C# |
| Database | Microsoft SQL Server, Entity Framework Core |
| Authentication | ASP.NET Core Identity with Roles |

## Default Seeded Accounts

| Role | Email | Password |
|------|--------|----------|
| **Admin** | `admin@agriconnect.co.za` | `Password123!` |
| **Employee** | `employee@agriconnect.co.za` | `Password123!` |
| **Farmer (Produce)** | `johnvanwyk@agrifarm.com` | `Password123!` |
| **Farmer (Produce)** | `mariamkhwanazi@greenfields.com` | `Password123!` |
| **Farmer (Produce)** | `davidwilson@agrifields.com` | `Password123!` |
| **Farmer (Energy Vendor)** | `gabrieldaw@greentech.co.za` | `Password123!` |

## Future Enhancements (Optional)

- Product detail pages with gallery and pricing breakdown.
- Marketplace matching farmers to renewable service providers.
- Knowledge Resource Hub (Sustainable farming & energy education).
- Stock taking system
- Purchase feature
- Mobile-first responsive UI refinements.


## License

This project is for educational and portfolio demonstration purposes.  
It may be adapted for real-world agricultural innovation initiatives.


**Agri-Energy Connect** — Supporting sustainable farming for a greener future.
