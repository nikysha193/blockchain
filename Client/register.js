const registerForm = document.getElementById("registerForm"); // Получаем форму Register

const usernameInput = document.getElementById("username"); // Получаем Username

const emailInput = document.getElementById("email"); // Получаем Email

const passwordInput = document.getElementById("password"); // Получаем Password

const confirmPasswordInput = document.getElementById("confirmPassword"); // Получаем Confirm Password

const message = document.getElementById("message"); // Получаем сообщения

const languageSelect = document.getElementById("languageSelect"); // Получаем выбор языка

const themeButton = document.getElementById("themeButton"); // Получаем Theme

const title = document.getElementById("title"); // Получаем заголовок

const subtitle = document.getElementById("subtitle"); // Получаем подзаголовок

const securityText = document.getElementById("securityText"); // Получаем Blockchain badge

const usernameLabel = document.getElementById("usernameLabel"); // Username label

const emailLabel = document.getElementById("emailLabel"); // Email label

const passwordLabel = document.getElementById("passwordLabel"); // Password label

const confirmPasswordLabel = document.getElementById("confirmPasswordLabel"); // Confirm Password label

const registerButton = document.getElementById("registerButton"); // Sign Up

const loginQuestion = document.getElementById("loginQuestion"); // Нижний текст

const loginLink = document.getElementById("loginLink"); // Login link


const translations = { // Все переводы

    en: { // English

        title: "Create Account", // Заголовок

        subtitle: "Create your secure identity", // Подзаголовок

        security: "BLOCKCHAIN SECURED", // Badge

        username: "Username", // Username

        email: "Email", // Email

        password: "Password", // Password

        confirmPassword: "Confirm Password", // Confirm Password

        usernamePlaceholder: "Enter username", // Placeholder

        emailPlaceholder: "Enter email", // Placeholder

        passwordPlaceholder: "Enter password", // Placeholder

        confirmPasswordPlaceholder: "Repeat password", // Placeholder

        register: "Sign Up", // Sign Up

        loginQuestion: "Already have an account?", // Нижний текст

        login: "Login", // Login

        darkTheme: "Dark theme", // Dark Theme

        lightTheme: "Light theme", // Light Theme

        passwordsNotMatch: "Passwords do not match", // Ошибка

        ready: "Registration form is ready" // Пока временное сообщение

    },


    ru: { // Русский

        title: "Создать аккаунт", // Заголовок

        subtitle: "Создайте защищённую учётную запись", // Подзаголовок

        security: "ЗАЩИЩЕНО BLOCKCHAIN", // Badge

        username: "Имя пользователя", // Username

        email: "Электронная почта", // Email

        password: "Пароль", // Password

        confirmPassword: "Повторите пароль", // Confirm Password

        usernamePlaceholder: "Введите имя пользователя", // Placeholder

        emailPlaceholder: "Введите email", // Placeholder

        passwordPlaceholder: "Введите пароль", // Placeholder

        confirmPasswordPlaceholder: "Повторите пароль", // Placeholder

        register: "Зарегистрироваться", // Sign Up

        loginQuestion: "Уже есть аккаунт?", // Нижний текст

        login: "Войти", // Login

        darkTheme: "Тёмная тема", // Dark Theme

        lightTheme: "Светлая тема", // Light Theme

        passwordsNotMatch: "Пароли не совпадают", // Ошибка

        ready: "Форма регистрации готова" // Пока временное сообщение

    },


    he: { // Иврит

        title: "יצירת חשבון", // Заголовок

        subtitle: "צור חשבון מאובטח", // Подзаголовок

        security: "מאובטח באמצעות BLOCKCHAIN", // Badge

        username: "שם משתמש", // Username

        email: "אימייל", // Email

        password: "סיסמה", // Password

        confirmPassword: "אימות סיסמה", // Confirm Password

        usernamePlaceholder: "הכנס שם משתמש", // Placeholder

        emailPlaceholder: "הכנס אימייל", // Placeholder

        passwordPlaceholder: "הכנס סיסמה", // Placeholder

        confirmPasswordPlaceholder: "הכנס שוב את הסיסמה", // Placeholder

        register: "הרשמה", // Sign Up

        loginQuestion: "כבר יש לך חשבון?", // Нижний текст

        login: "התחברות", // Login

        darkTheme: "מצב כהה", // Dark Theme

        lightTheme: "מצב בהיר", // Light Theme

        passwordsNotMatch: "הסיסמאות אינן תואמות", // Ошибка

        ready: "טופס ההרשמה מוכן" // Временное сообщение

    }

};


let currentLanguage = localStorage.getItem("language") || "en"; // Загружаем язык

let darkMode = localStorage.getItem("theme") === "dark"; // Загружаем тему


function updateLanguage() { // Обновляем интерфейс

    const text = translations[currentLanguage]; // Получаем перевод

    title.textContent = text.title; // Заголовок

    subtitle.textContent = text.subtitle; // Подзаголовок

    securityText.textContent = text.security; // Blockchain badge

    usernameLabel.textContent = text.username; // Username

    emailLabel.textContent = text.email; // Email

    passwordLabel.textContent = text.password; // Password

    confirmPasswordLabel.textContent = text.confirmPassword; // Confirm Password

    usernameInput.placeholder = text.usernamePlaceholder; // Placeholder

    emailInput.placeholder = text.emailPlaceholder; // Placeholder

    passwordInput.placeholder = text.passwordPlaceholder; // Placeholder

    confirmPasswordInput.placeholder = text.confirmPasswordPlaceholder; // Placeholder

    registerButton.textContent = text.register; // Register

    loginQuestion.textContent = text.loginQuestion; // Нижний текст

    loginLink.textContent = text.login; // Login

    themeButton.textContent = darkMode ? text.lightTheme : text.darkTheme; // Theme

    languageSelect.value = currentLanguage; // Показываем выбранный язык


    if (currentLanguage === "he") { // Если иврит

        document.body.classList.add("rtl"); // RTL

        document.documentElement.dir = "rtl"; // RTL

        document.documentElement.lang = "he"; // Hebrew

    } else { // Остальные языки

        document.body.classList.remove("rtl"); // Убираем RTL

        document.documentElement.dir = "ltr"; // LTR

        document.documentElement.lang = currentLanguage; // Язык

    }

}


function updateTheme() { // Применяем тему

    document.body.classList.toggle("dark", darkMode); // Dark on/off

    localStorage.setItem("theme", darkMode ? "dark" : "light"); // Сохраняем

    updateLanguage(); // Обновляем кнопку Theme

}


languageSelect.addEventListener("change", function () { // Смена языка

    currentLanguage = languageSelect.value; // Новый язык

    localStorage.setItem("language", currentLanguage); // Сохраняем

    message.textContent = ""; // Очищаем сообщение

    updateLanguage(); // Обновляем страницу

});


themeButton.addEventListener("click", function () { // Смена темы

    darkMode = !darkMode; // Переключаем

    updateTheme(); // Применяем

});


registerForm.addEventListener("submit", function (event) { // Нажатие Sign Up

    event.preventDefault(); // Не перезагружаем страницу

    const password = passwordInput.value; // Password

    const confirmPassword = confirmPasswordInput.value; // Confirm Password


    if (password !== confirmPassword) { // Проверяем совпадение

        message.textContent = translations[currentLanguage].passwordsNotMatch; // Ошибка

        return; // Останавливаем функцию

    }


    message.textContent = translations[currentLanguage].ready; // Временно показываем успех

});


updateTheme(); // Загружаем тему

updateLanguage(); // Загружаем язык