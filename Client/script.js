const loginForm = document.getElementById("loginForm"); // Получаем форму Login

const usernameInput = document.getElementById("username"); // Получаем поле Username

const passwordInput = document.getElementById("password"); // Получаем поле Password

const rememberMe = document.getElementById("rememberMe"); // Получаем checkbox Remember me

const message = document.getElementById("message"); // Получаем место для сообщений

const languageSelect = document.getElementById("languageSelect"); // Получаем выбор языка

const themeButton = document.getElementById("themeButton"); // Получаем кнопку темы

const title = document.getElementById("title"); // Получаем главный заголовок

const subtitle = document.getElementById("subtitle"); // Получаем подзаголовок

const usernameLabel = document.getElementById("usernameLabel"); // Получаем Username label

const passwordLabel = document.getElementById("passwordLabel"); // Получаем Password label

const loginButton = document.getElementById("loginButton"); // Получаем кнопку Login

const securityText = document.getElementById("securityText"); // Получаем Blockchain Secured

const rememberText = document.getElementById("rememberText"); // Получаем Remember me

const forgotPasswordLink = document.getElementById("forgotPasswordLink"); // Получаем Forgot password

const registerQuestion = document.getElementById("registerQuestion"); // Получаем текст перед Sign Up

const registerLink = document.getElementById("registerLink"); // Получаем Sign Up


const translations = { // Все переводы страницы

    en: { // Английский

        title: "Secure Documents", // Заголовок

        subtitle: "Protected access to your documents", // Подзаголовок

        security: "BLOCKCHAIN SECURED", // Blockchain badge

        username: "Username", // Username

        password: "Password", // Password

        usernamePlaceholder: "Enter username", // Placeholder Username

        passwordPlaceholder: "Enter password", // Placeholder Password

        remember: "Remember me", // Remember me

        forgotPassword: "Forgot password?", // Forgot password

        login: "Login", // Login

        registerQuestion: "Don't have an account?", // Нижний текст

        register: "Sign Up", // Sign Up

        darkTheme: "Dark theme", // Dark theme

        lightTheme: "Light theme", // Light theme

        wrongLogin: "Incorrect username or password", // Неверный Login

        serverError: "Server error", // Ошибка Server

        cannotConnect: "Cannot connect to server" // Ошибка соединения

    },


    ru: { // Русский

        title: "Безопасные документы", // Заголовок

        subtitle: "Защищённый доступ к вашим документам", // Подзаголовок

        security: "ЗАЩИЩЕНО BLOCKCHAIN", // Blockchain badge

        username: "Имя пользователя", // Username

        password: "Пароль", // Password

        usernamePlaceholder: "Введите имя пользователя", // Placeholder Username

        passwordPlaceholder: "Введите пароль", // Placeholder Password

        remember: "Запомнить меня", // Remember me

        forgotPassword: "Забыли пароль?", // Forgot password

        login: "Войти", // Login

        registerQuestion: "Нет аккаунта?", // Нижний текст

        register: "Регистрация", // Sign Up

        darkTheme: "Тёмная тема", // Dark theme

        lightTheme: "Светлая тема", // Light theme

        wrongLogin: "Неверное имя пользователя или пароль", // Неверный Login

        serverError: "Ошибка сервера", // Server error

        cannotConnect: "Не удалось подключиться к серверу" // Ошибка соединения

    },


    he: { // Иврит

        title: "מסמכים מאובטחים", // Заголовок

        subtitle: "גישה מאובטחת למסמכים שלך", // Подзаголовок

        security: "מאובטח באמצעות BLOCKCHAIN", // Blockchain badge

        username: "שם משתמש", // Username

        password: "סיסמה", // Password

        usernamePlaceholder: "הכנס שם משתמש", // Placeholder Username

        passwordPlaceholder: "הכנס סיסמה", // Placeholder Password

        remember: "זכור אותי", // Remember me

        forgotPassword: "שכחת סיסמה?", // Forgot password

        login: "התחבר", // Login

        registerQuestion: "אין לך חשבון?", // Нижний текст

        register: "הרשמה", // Sign Up

        darkTheme: "מצב כהה", // Dark theme

        lightTheme: "מצב בהיר", // Light theme

        wrongLogin: "שם המשתמש או הסיסמה שגויים", // Неверный Login

        serverError: "שגיאת שרת", // Server error

        cannotConnect: "לא ניתן להתחבר לשרת" // Ошибка соединения

    }

};


let currentLanguage = localStorage.getItem("language") || "en"; // Загружаем сохранённый язык

let darkMode = localStorage.getItem("theme") === "dark"; // Загружаем сохранённую тему


function updateLanguage() { // Функция обновляет весь интерфейс

    const text = translations[currentLanguage]; // Получаем нужный язык

    title.textContent = text.title; // Меняем заголовок

    subtitle.textContent = text.subtitle; // Меняем подзаголовок

    securityText.textContent = text.security; // Меняем Blockchain badge

    usernameLabel.textContent = text.username; // Меняем Username

    passwordLabel.textContent = text.password; // Меняем Password

    usernameInput.placeholder = text.usernamePlaceholder; // Меняем placeholder Username

    passwordInput.placeholder = text.passwordPlaceholder; // Меняем placeholder Password

    rememberText.textContent = text.remember; // Меняем Remember me

    forgotPasswordLink.textContent = text.forgotPassword; // Меняем Forgot password

    loginButton.textContent = text.login; // Меняем Login

    registerQuestion.textContent = text.registerQuestion; // Меняем нижний текст

    registerLink.textContent = text.register; // Меняем Sign Up

    themeButton.textContent = darkMode ? text.lightTheme : text.darkTheme; // Меняем название Theme


    languageSelect.value = currentLanguage; // Показываем выбранный язык в select


    if (currentLanguage === "he") { // Если выбран иврит

        document.body.classList.add("rtl"); // Включаем RTL

        document.documentElement.dir = "rtl"; // Направление справа налево

        document.documentElement.lang = "he"; // Язык документа

    } else { // Если русский или английский

        document.body.classList.remove("rtl"); // Убираем RTL

        document.documentElement.dir = "ltr"; // Направление слева направо

        document.documentElement.lang = currentLanguage; // Устанавливаем язык

    }

}


function updateTheme() { // Функция применяет тему

    document.body.classList.toggle("dark", darkMode); // Добавляем или удаляем dark

    localStorage.setItem("theme", darkMode ? "dark" : "light"); // Сохраняем тему

    updateLanguage(); // Обновляем текст кнопки Theme

}


languageSelect.addEventListener("change", function () { // Когда меняем язык

    currentLanguage = languageSelect.value; // Получаем новый язык

    localStorage.setItem("language", currentLanguage); // Сохраняем язык

    message.textContent = ""; // Очищаем старое сообщение

    updateLanguage(); // Меняем интерфейс

});


themeButton.addEventListener("click", function () { // Когда нажимаем Theme

    darkMode = !darkMode; // Переключаем тему

    updateTheme(); // Применяем тему

});


const rememberedUsername = localStorage.getItem("rememberedUsername"); // Проверяем сохранённый Username


if (rememberedUsername) { // Если Username был сохранён

    usernameInput.value = rememberedUsername; // Автоматически вставляем Username

    rememberMe.checked = true; // Ставим галочку Remember me

}


loginForm.addEventListener("submit", async function (event) { // Нажатие Login

    event.preventDefault(); // Не перезагружаем страницу

    const username = usernameInput.value; // Получаем Username

    const password = passwordInput.value; // Получаем Password


    if (rememberMe.checked) { // Если стоит Remember me

        localStorage.setItem("rememberedUsername", username); // Сохраняем только Username

    } else { // Если галочка выключена

        localStorage.removeItem("rememberedUsername"); // Удаляем сохранённый Username

    }


    try { // Пытаемся подключиться к C# Server

        const response = await fetch("http://localhost:5000/api/login", { // Отправляем запрос

            method: "POST", // POST запрос

            headers: { // HTTP headers

                "Content-Type": "application/json" // Отправляем JSON

            },

            body: JSON.stringify({ // Создаём JSON

                username: username, // Username

                password: password // Password

            })

        });


        if (response.ok) { // Если Login успешный

            const data = await response.json(); // Получаем ответ Server

            message.textContent = data.message; // Показываем ответ

        } else if (response.status === 401) { // Если логин неправильный

            message.textContent = translations[currentLanguage].wrongLogin; // Показываем ошибку

        } else { // Другая ошибка сервера

            message.textContent = translations[currentLanguage].serverError; // Показываем Server error

        }

    } catch (error) { // Если Server недоступен

        console.error(error); // Показываем ошибку в Console

        message.textContent = translations[currentLanguage].cannotConnect; // Показываем ошибку пользователю

    }

});


updateTheme(); // Загружаем сохранённую тему

updateLanguage(); // Загружаем сохранённый язык