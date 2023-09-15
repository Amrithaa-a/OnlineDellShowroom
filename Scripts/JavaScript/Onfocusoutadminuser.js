window.setTimeout(function () {
    $(".alert").fadeTo(500, 0).slideUp(500, function () {
        $(this).remove();
    });
}, 1000);
$(document).ready(function () {
    // Function to validate username on focusout
    $("#Username").focusout(function () {
        var username = $("#Username").val().trim();
        if (username === "") {
            showError("usernameError", "Please enter username.");
        } else if (!isValidEmail(username)) {
            showError("usernameError", "Username must be in email format");
        } else {
            hideError("usernameError");
        }
    });

    // Function to validate password on focusout
    $("#Password").focusout(function () {
        var password = $("#Password").val().trim();
        if (password === "") {
            showError("passwordError", "Please enter the password.");
        } else if (!isValidPassword(password)) {
            showError("passwordError", "Password must be at least 8 characters long and contain at least one uppercase letter, one lowercase letter, and one number.");
        } else {
            hideError("passwordError");
        }
    });

    // Function to check if the given email is valid
    function isValidEmail(email) {
        var emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
        return emailRegex.test(email);
    }

    // Function to check if the given password is strong
    function isValidPassword(password) {
        var passwordRegex = /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$/;
        return passwordRegex.test(password);
    }

    // Function to show error message
    function showError(elementId, errorMessage) {
        $("#" + elementId).text(errorMessage);
    }

    // Function to hide error message
    function hideError(elementId) {
        $("#" + elementId).text("");
    }

    // Function to handle button click
    $("#submitButton").click(function (event) {
        var username = $("#Username").val().trim();
        var password = $("#Password").val().trim();

        if (username === "") {
            showError("usernameError", "Please enter the username.");
            event.preventDefault(); // Prevent form submission
            return false;
        } else if (!isValidEmail(username)) {
            showError("usernameError", "Please enter a valid username");
            event.preventDefault(); // Prevent form submission
            return false;
        }

        if (password === "") {
            showError("passwordError", "Please enter the password.");
            event.preventDefault(); // Prevent form submission
            return false;
        } else if (!isValidPassword(password)) {
            showError("passwordError", "Password must be at least 8 characters long and contain at least one uppercase letter, one lowercase letter, and one number.");
            event.preventDefault(); // Prevent form submission
            return false;
        }

        // Form is valid, allow submission
        return true;
    });
});