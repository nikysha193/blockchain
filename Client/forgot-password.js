const forgotForm = document.getElementById("forgotForm"); // Получаем Forgot Password форму

const emailInput = document.getElementById("email"); // Получаем Email

const emailLabel = document.getElementById("emailLabel"); // Получаем Email label

const resetButton = document.getElementById("resetButton"); // Получаем Continue

const message = document.getElementById("message"); // Получаем сообщения

const title = document.getElementById("title"); // Получаем заголовок

const subtitle = document.getElementById("subtitle"); // Получаем подзаголовок

const securityText = document.getElementById("securityText"); // Получаем Secure Recovery

const languageSelect = document.getElementById("languageSelect"); // Получаем языки

const themeButton = document.getElementById("themeButton"); // Получаем Theme

const backToLogin = document.getElementById("backToLogin"); // Получаем Back to Login


const translations = { // Переводы

    en: { // English

        title: "Reset Password", // Заголовок

        subtitle: "Enter your email to recover access", // Подзаголовок

        security: "SECURE RECOVERY", // Badge

        email: "Email", // Email

        emailPlaceholder: "Enter your email", // Placeholder

        continue: "Continue", // Continue

        back: "← Back to Login", // Назад

        darkTheme: "Dark theme", // Dark

        lightTheme: "Light theme", // Light

        sent: "Recovery system will be connected to the server later." // Пока временно

    },


    ru: { // Русский

        title: "Восстановление пароля", // Заголовок

        subtitle: "Введите email для восстановления доступа", // Подзаголовок

        security: "БЕЗОПАСНОЕ ВОССТАНОВЛЕНИЕ", // Badge

        email: "Электронная почта", // Email

        emailPlaceholder: "Введите ваш email", // Placeholder

        continue: "Продолжить", // Continue

        back: "← Назад ко входу", // Назад

        darkTheme: "Тёмная тема", // Dark

        lightTheme: "Светлая тема", // Light

        sent: "Позже восстановление будет подключено к серверу." // Пока временно

    },


    he: { // Иврит

        title: "איפוס סיסמה", // Заголовок

        subtitle: "הכנס אימייל כדי לשחזר גישה", // Подзаголовок

        security: "שחזור מאובטח", // Badge

        email: "אימייל", // Email

        emailPlaceholder: "הכנס אימייל", // Placeholder

        continue: "המשך", // Continue

        back: "חזרה להתחברות ←", // Назад

        darkTheme: "מצב כהה", // Dark

        lightTheme: "מצב בהיר", // Light

        sent: "מערכת השחזור תחובר לשרת בהמשך." // Пока временно

    }

};


let currentLanguage = localStorage.getItem("language") || "en"; // Загружаем язык

let darkMode = localStorage.getItem("theme") === "dark"; // Загружаем тему


function updateLanguage() { // Обновляем интерфейс

    const text = translations[currentLanguage]; // Получаем перевод

    title.textContent = text.title; // Заголовок

    subtitle.textContent = text.subtitle; // Подзаголовок

    securityText.textContent = text.security; // Badge

    emailLabel.textContent = text.email; // Email

    emailInput.placeholder = text.emailPlaceholder; // Placeholder

    resetButton.textContent = text.continue; // Continue

    backToLogin.textContent = text.back; // Back

    themeButton.textContent = darkMode ? text.lightTheme : text.darkTheme; // Theme

    languageSelect.value = currentLanguage; // Выбранный язык


    if (currentLanguage === "he") { // Если Hebrew

        document.body.classList.add("rtl"); // RTL

        document.documentElement.dir = "rtl"; // RTL

        document.documentElement.lang = "he"; // Hebrew

    } else { // Остальные

        document.body.classList.remove("rtl"); // Убираем RTL

        document.documentElement.dir = "ltr"; // LTR

        document.documentElement.lang = currentLanguage; // Язык

    }

}


function updateTheme() { // Меняем тему

    document.body.classList.toggle("dark", darkMode); // Dark on/off

    localStorage.setItem("theme", darkMode ? "dark" : "light"); // Сохраняем

    updateLanguage(); // Обновляем кнопку Theme

}


languageSelect.addEventListener("change", function () { // Меняем язык

    currentLanguage = languageSelect.value; // Новый язык

    localStorage.setItem("language", currentLanguage); // Сохраняем

    message.textContent = ""; // Очищаем сообщение

    updateLanguage(); // Обновляем страницу

});


themeButton.addEventListener("click", function () { // Меняем тему

    darkMode = !darkMode; // Переключаем

    updateTheme(); // Применяем

});


forgotForm.addEventListener("submit", function (event) { // Нажимаем Continue

    event.preventDefault(); // Не перезагружаем страницу

    message.textContent = translations[currentLanguage].sent; // Пока показываем временное сообщение

});


updateTheme(); // Загружаем тему

updateLanguage(); // Загружаем язык