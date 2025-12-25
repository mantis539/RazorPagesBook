## API Документація

Проєкт надає стандартизований REST API для взаємодії з каталогом книг та системою відгуків.

* **Специфікація:** [openapi.yaml](RazorPagesBook/docs/api/openapi.yaml) — повний опис контрактів, моделей даних (DTO) та статусів відповідей.
* **Стандарт:** OpenAPI 3.0.3.
* **Обробка помилок:** API повертає уніфікований формат помилок із `requestId` для кореляції запитів.

### Візуалізація інтерфейсу Swagger
Нижче наведено скріншот документації, згенерованої на основі нашого контракту:

![Swagger UI Screenshot](RazorPagesBook/docs/api/swagger_screenshot.png)

---

### Основні Endpoints:
| Метод | Шлях | Опис |
| :--- | :--- | :--- |
| `GET` | `/api/books` | Отримання списку всіх книг |
| `POST` | `/api/books` | Додавання нової книги (підтримує Idempotency-Key) |
| `GET` | `/api/books/{id}` | Детальна інформація про книгу |
| `DELETE` | `/api/books/{id}` | Видалення книги (тільки для Admin) |
| `GET` | `/health` | Перевірка стану системи (повертає JSON з таймаутом 1с) |


Для перевірки докера було відкрито командний рядок та вписано команду docker run hello-world, після чого крмандеий рядок показав такий результат:
![hello-world](RazorPagesBook/RazorPagesBook/wwwroots/uploads/books/docker.png)
