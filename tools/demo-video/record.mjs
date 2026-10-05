import { chromium } from "playwright";
import { mkdirSync } from "fs";
import { dirname, join } from "path";
import { fileURLToPath } from "url";

const __dirname = dirname(fileURLToPath(import.meta.url));
const outDir = join(__dirname, "../../docs/demo-raw");
mkdirSync(outDir, { recursive: true });

const BASE = "http://localhost:5081";

async function caption(page, title, text) {
  await page.evaluate(({ title, text }) => {
    let bar = document.getElementById("demo-caption");
    if (!bar) {
      bar = document.createElement("div");
      bar.id = "demo-caption";
      bar.style.cssText = `
        position: fixed; left: 24px; right: 24px; bottom: 22px; z-index: 99999;
        padding: 14px 18px; border-radius: 16px;
        background: rgba(33,19,15,.88); color: #f0dfcb;
        font: 600 16px/1.35 "Segoe UI", sans-serif;
        border: 1px solid rgba(248,127,35,.45);
        box-shadow: 0 12px 30px rgba(0,0,0,.4);
        pointer-events: none;
      `;
      document.body.appendChild(bar);
    }
    bar.innerHTML = `<div style="color:#f87f23;font-size:12px;letter-spacing:.08em;text-transform:uppercase;margin-bottom:4px">${title}</div>${text}`;
  }, { title, text });
}

async function titleCard(page, kicker, heading, sub) {
  await page.setContent(`
    <html><body style="margin:0;height:100vh;display:grid;place-items:center;
      background:radial-gradient(900px 500px at 10% -10%, rgba(248,127,35,.4), transparent 55%),
                 radial-gradient(700px 480px at 100% 0, rgba(204,10,13,.32), transparent 50%),
                 linear-gradient(180deg,#3a1a10,#21130f);
      color:#f0dfcb;font-family:Segoe UI,sans-serif;">
      <div style="width:min(820px,90%);">
        <div style="color:#f87f23;letter-spacing:.16em;text-transform:uppercase;font-weight:700;margin-bottom:14px">${kicker}</div>
        <h1 style="font-size:48px;letter-spacing:-.04em;margin:0 0 16px">${heading}</h1>
        <p style="font-size:20px;color:#e4d6c6;margin:0;line-height:1.5">${sub}</p>
      </div>
    </body></html>`);
}

const browser = await chromium.launch({ headless: true, channel: "chrome" });
const context = await browser.newContext({
  viewport: { width: 1440, height: 900 },
  recordVideo: { dir: outDir, size: { width: 1440, height: 900 } }
});
const page = await context.newPage();
page.setDefaultTimeout(20000);

await titleCard(
  page,
  "Лабораторная работа · C# монорепозиторий",
  "Учёт контингента студентов",
  "Обзор системы по диаграммам классов, вариантов использования и активности"
);
await page.waitForTimeout(3500);

await page.goto(BASE, { waitUntil: "networkidle" });
await page.waitForSelector("h1");
await caption(page, "Вход в систему", "Окно авторизации до главной. Роли: администратор и студент.");
await page.waitForTimeout(2800);

await page.getByRole("button", { name: "Войти" }).click();
await page.waitForURL("**/");
await page.waitForSelector("text=Студентов");
await caption(page, "Главная · дашборд", "После входа администратор видит сводку: студенты, группы, курсы.");
await page.waitForTimeout(3200);

await page.getByRole("link", { name: "Студенты" }).click();
await page.getByRole("heading", { name: "Управление студентами" }).waitFor();
await page.getByRole("cell", { name: "Стремилов Лев" }).waitFor();
await caption(page, "Управление студентами", "Поиск, добавление, редактирование и отчисление — прецедент администратора.");
await page.waitForTimeout(2200);
await page.getByPlaceholder("Поиск...").fill("Стремилов");
await page.getByRole("button", { name: "Найти" }).click();
await page.waitForTimeout(1600);
await page.getByRole("button", { name: "Добавить" }).click();
await page.waitForSelector("text=Добавление студента");
await caption(page, "Добавление студента", "Форма персональных данных: ФИО, группа, контакты, статус.");
await page.waitForTimeout(2800);

await page.getByRole("link", { name: "Группы" }).click();
await page.getByRole("heading", { name: "Управление группами" }).waitFor();
await page.getByRole("cell", { name: "ИСП-101" }).waitFor();
await caption(page, "Управление группами", "Создание группы, куратор и зачисление студента в группу.");
await page.waitForTimeout(3000);

await page.getByRole("link", { name: "Курсы" }).click();
await page.getByRole("heading", { name: "Управление курсами" }).waitFor();
await caption(page, "Управление курсами", "Курсы, привязка группы и перевод студентов на следующий курс.");
await page.waitForTimeout(3000);

await page.getByRole("link", { name: "Отчёты" }).click();
await page.getByRole("heading", { name: "Формирование отчётов" }).waitFor();
await page.getByRole("button", { name: "Сформировать" }).waitFor();
await caption(page, "Формирование отчётов", "Диаграмма активности: выбрать группу → сформировать → просмотреть.");
await page.waitForTimeout(1800);
await page.getByRole("button", { name: "Сформировать" }).click();
await page.getByText("Просмотреть отчёт").waitFor();
await page.waitForTimeout(2200);
await page.getByRole("button", { name: "Показать" }).click();
await page.getByRole("cell", { name: "Стремилов Лев" }).waitFor();
await page.waitForTimeout(2800);

await page.getByRole("button", { name: "Выйти" }).click();
await page.waitForSelector("text=Авторизация");
await page.locator('input').nth(0).fill("student");
await page.locator('input[type="password"]').fill("student");
await caption(page, "Роль студента", "Студент входит отдельно и заполняет персональные данные.");
await page.waitForTimeout(2200);
await page.getByRole("button", { name: "Войти" }).click();
await page.waitForURL("**/");
await page.getByRole("navigation").getByRole("link", { name: "Мои данные" }).click();
await page.waitForSelector("text=Внесение персональных данных");
await caption(page, "Профиль студента", "Прецедент «Внесение персональных данных».");
await page.waitForTimeout(3200);

await titleCard(
  page,
  "Стек",
  "ASP.NET Core API + Blazor WASM",
  "SQLite · сервисы из диаграммы классов · GitHub: Stremilov/POPs"
);
await page.waitForTimeout(3800);

const video = page.video();
await context.close();
const webm = await video.path();
await browser.close();
console.log(webm);
