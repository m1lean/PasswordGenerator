# PassGen — ASP.NET Core MVC Password Generator

Полноценное MVC-приложение на .NET 8 с генератором паролей,
историей (SQLite) и экспортом в TXT/CSV.

## Структура проекта

```
PasswordGenerator/
├── Controllers/
│   ├── HomeController.cs       # Генерация паролей (GET/POST)
│   └── HistoryController.cs    # История, удаление, экспорт
├── Models/
│   ├── PasswordEntry.cs        # EF-сущность (таблица БД)
│   ├── GenerateRequest.cs      # Форма запроса (входные данные)
│   └── ViewModels.cs           # HomeViewModel, HistoryViewModel, GenerateResult
├── Services/
│   ├── PasswordService.cs      # Генерация (CSPRNG, энтропия, надёжность)
│   ├── HistoryService.cs       # CRUD истории через EF Core
│   └── ExportService.cs        # Экспорт TXT и CSV
├── Data/
│   └── AppDbContext.cs         # EF Core DbContext (SQLite)
├── Views/
│   ├── Home/Index.cshtml       # Главная страница — генератор
│   ├── History/Index.cshtml    # История паролей
│   └── Shared/_Layout.cshtml  # Общий макет
├── wwwroot/
│   ├── css/site.css            # Стили (dark theme)
│   └── js/site.js              # JS-хелперы
├── Program.cs                  # DI, EF, middleware
├── appsettings.json
└── PasswordGenerator.csproj
```

## Запуск

### Требования
- [.NET 8 SDK](https://dotnet.microsoft.com/download)

### Команды

```bash
cd PasswordGenerator

# Восстановить пакеты и запустить
dotnet run
```

Приложение запустится на `http://localhost:5000` (или `https://localhost:5001`).

База данных `passwords.db` (SQLite) создаётся автоматически при первом запуске.

## Функции

| Функция | Описание |
|---|---|
| Генератор | Длина 4–128, кол-во 1–100, 6 наборов символов |
| CSPRNG | `RandomNumberGenerator` без модульного смещения |
| Энтропия | Расчёт в битах: `length × log₂(charset_size)` |
| Надёжность | 5 уровней: очень слабый → очень сильный |
| Без повторений | Shuffle-based генерация |
| История | Сохранение в SQLite через EF Core |
| Поиск/сортировка | По дате, длине, надёжности |
| Пагинация | 20 записей на страницу |
| Экспорт TXT | Подробный текстовый отчёт |
| Экспорт CSV | Таблица для Excel/Sheets |
| CSRF защита | `[ValidateAntiForgeryToken]` на всех POST |

## Архитектура

```
Browser → Controller → Service → Repository (EF) → SQLite
              ↓
           ViewModel → View (Razor) → HTML
```

- **MVC** — чёткое разделение Model / View / Controller
- **DI** — все сервисы через `IServiceCollection`
- **EF Core** — ORM с Code-First подходом
- **Repository pattern** — `IHistoryService` абстрагирует БД
- **Interface segregation** — `IPasswordService`, `IHistoryService`, `IExportService`
