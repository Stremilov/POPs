# POPs — Учёт контингента студентов

Монорепозиторий на C#: backend (ASP.NET Core Web API) + frontend (Blazor WebAssembly).

Приложение реализовано по диаграмме классов, вариантов использования и активности из лабораторной работы.

## Видеообзор

Короткий обзор системы (вход, дашборд, студенты, группы, курсы, отчёты, роль студента).

<video controls width="100%">
  <source src="docs/Obzor-uchet-kontingenta.mp4" type="video/mp4">
</video>

[Смотреть / скачать MP4](https://github.com/Stremilov/POPs/raw/main/docs/Obzor-uchet-kontingenta.mp4)

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

## Запуск одной командой

Скрипт сам скачает .NET 8 (если его нет), восстановит пакеты и поднимет API + сайт.

### macOS / Linux

```bash
git clone https://github.com/Stremilov/POPs.git
cd POPs
chmod +x start.sh
./start.sh
```

Если репозиторий уже скачан:

```bash
./start.sh
```

### Windows (PowerShell или двойной клик по `start.cmd`)

```powershell
git clone https://github.com/Stremilov/POPs.git
cd POPs
.\start.cmd
```

или:

```powershell
.\start.ps1
```

Сайт: http://localhost:5081  
Swagger: http://localhost:5080/swagger

### Демо-логины

| Роль            | Логин     | Пароль   |
|-----------------|-----------|----------|
| Администратор   | `admin`   | `admin`  |
| Студент         | `student` | `student`|

## Git / GitHub

Репозиторий: https://github.com/Stremilov/POPs.git
