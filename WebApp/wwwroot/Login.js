window.addLoginKeyListener = function () {
    const passwordInput = document.querySelector('#password');

    passwordInput.addEventListener('keydown', function (event) {
        if (event.key === 'Enter') {
            event.preventDefault();
            document.querySelector('#login-button').click();
        }
    });
};

window.addLoginButtonListener = function () {
    const loginButton = document.querySelector('#login-button');

    loginButton.addEventListener('click', function (event) {
        event.preventDefault();
        document.querySelector('form').submit();
    });
};