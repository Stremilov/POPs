# POPs — Учёт контингента студентов

Монорепозиторий на C#: backend (ASP.NET Core Web API) + frontend (Blazor WebAssembly).

Приложение реализовано по диаграмме классов, вариантов использования и активности из лабораторной работы.

## Структура

```
POPs.sln
src/
  POPs.Shared/   — общие модели и DTO
  POPs.Api/      — бэкенд (сервисы + REST API + SQLite)
  POPs.Web/      — фронтенд (Blazor WASM)
```

## Что внутри

**Сущности:** `Administrator`, `Student`, `Group`, `Course`

**Сервисы (как на диаграмме классов):**
- `StudentService` — добавить / изменить / удалить / найти / по группе / отчислить
- `GroupService` — создать / изменить / удалить / куратор / зачисление
- `CourseService` — создать / изменить / привязать группу / перевод на следующий курс
- `ReportService` — контингент, статистика, отчёт по группе

**Роли:**
- Администратор — студенты, группы, курсы, отчёты
- Студент — вход и редактирование персональных данных

## Утилиты

- **Entity Framework Core + SQLite** — БД без отдельного сервера
- **Swagger** — документация API (`/swagger`)
- **BCrypt** — хэш паролей
- **Blazor WASM** — фронт на C#

## Запуск локально

Нужен [.NET 8 SDK](https://dotnet.microsoft.com/download).

Терминал 1 — API:

```bash
cd src/POPs.Api
dotnet run
```

API: http://localhost:5080  
Swagger: http://localhost:5080/swagger

Терминал 2 — Web:

```bash
cd src/POPs.Web
dotnet run
```

Web: http://localhost:5081

### Демо-логины

| Роль            | Логин     | Пароль   |
|-----------------|-----------|----------|
| Администратор   | `admin`   | `admin`  |
| Студент         | `student` | `student`|

## Git / GitHub

Репозиторий: https://github.com/Stremilov/POPs.git

```bash
git add .
git commit -m "Добавлено приложение учёта контингента студентов"
git push -u origin main
```
