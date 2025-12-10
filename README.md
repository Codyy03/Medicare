# MediCare

**MediCare** is a project that supports the operation of medical facilities.

The system enables the management of patients, doctors, rooms, and appointments, and also provides an administration panel and statistics.

---

## Technologies
- **Backend:** ASP.NET Core, Entity Framework Core, PostgreSQL, JWT
- **Frontend:** React, TypeScript, Bootstrap, Axios, Vite
- **Tests:** xUnit (unit + integration)
- **DevOps:** Docker, GitHub Actions, Swagger

---

## Project structure
- `medicare.client` – frontend application (React + TS)
- `MediCare.Server` – backend API (ASP.NET Core)
- `MediCare.ServerTests` – unit and integration tests

- ## Run locally
### Backend
```bash
cd MediCare.Server
dotnetbuild
dotnet runes
```

Access:
- Frontend: http://localhost:5187
- Backend API: https://localhost:7014/swagger

- ## Launch in Docker
```
git clone https://github.com/Codyy03/Medicare.git
cd Medicare
docker compose up -d
```

Access:
- Frontend: http://localhost:3000
- Backend API: https://localhost:7014/swagger
- pgAdmin: http://localhost:8080

## Database
![Entity Diagram](docs/diagrams/er-diagram.png)

## Security
- JWT-based authorization (access + refresh tokens)
- User roles: Admin, Doctor, Patient
- Data validation on the backend and frontend

## Tests
- Unit tests – data validation (passwords, PESEL, work hours)
- Integration tests – full API scenarios (patients, appointments, doctors)
- Test infrastructure – EmptyDbFactory, SeededDbFactory, TestJwtTokenHelper

## Application Appearance
Below are sample MediCare system screens:

- ## User not logged in
![Login screen](docs/screens/login.png)
![Appointment booking panel](docs/screens/appointments.png)
![Doctor overview panel](docs/screens/doctorsList.png)

- ## Patient logged in
![Patient appointment booking panel](docs/screens/booking-visit.png)
![Patient profile editing panel](docs/screens/profile-edit.png)
![Patient visit panel](docs/screens/visit-list.png)

- ## Doctor
![Doctor visit panel](docs/screens/doctors-visits-all.png)
![Data editing panel Doctor](docs/screens/doctor-profile-edit.png)

- ## Admin
![Admin dashboard](docs/screens/admin-panel.png)
![News management panel](docs/screens/admin-news.png)
![News addition panel](docs/screens/admin-news-add.png)
![News editing panel](docs/screens/admin-news-edit.png)
![News editing panel](docs/screens/admin-stats.png)
