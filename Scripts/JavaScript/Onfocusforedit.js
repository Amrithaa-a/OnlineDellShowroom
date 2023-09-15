$(document).ready(function () {
    // Function to validate Firstname on focusout
    $("#Firstname").focusout(function () {
        var firstname = $("#Firstname").val().trim();
        if (firstname === "") {
            $("#firstnameError").text("Please enter your firstname.");
        } else if (!isValidName(firstname)) {
            $("#firstnameError").text("Firstname should contain only alphabetic characters.");
        } else {
            $("#firstnameError").text("");
        }
    });

    // Function to validate Lastname on focusout
    $("#Lastname").focusout(function () {
        var lastname = $("#Lastname").val().trim();
        if (lastname === "") {
            $("#lastnameError").text("Please enter your lastname.");
        } else if (!isValidName(lastname)) {
            $("#lastnameError").text("Lastname should contain only alphabetic characters.");
        } else {
            $("#lastnameError").text("");
        }
    });

    // Function to check if the given name contains only alphabetic characters
    function isValidName(name) {
        var nameRegex = /^[a-zA-Z]+$/;
        return nameRegex.test(name);
    }
});
$(document).ready(function () {
    $("#Dateofbirth").on("focusout", function () {
        var dob = new Date($("#Dateofbirth").val());
        var today = new Date();
        var age = today.getFullYear() - dob.getFullYear();

        if (today.getMonth() < dob.getMonth() ||
            (today.getMonth() === dob.getMonth() && today.getDate() < dob.getDate())) {
            age--;
        }

        if (age < 18) {
            $("#ageError").text("Age must be 18 years or older.");
        } else {
            $("#ageError").text("");
        }
    });
});

$(document).ready(function () {
    // Function to validate mobile number on focusout
    $("#Mobilenumber").on("focusout", function () {
        validateMobileNumber();
    });

    // Function to validate mobile number
    function validateMobileNumber() {
        var mobilenumber = $("#Mobilenumber").val().trim();
        var mobileNumberRegex = /^[0-9]{10}$/;

        if (mobilenumber === "") {
            $("#mobilenumberError").text("Please enter the mobile number.");
        } else if (!mobileNumberRegex.test(mobilenumber)) {
            $("#mobilenumberError").text("Please enter a valid 10-digit numeric mobile number.");
        } else {
            $("#mobilenumberError").text("");
        }
    }
});
$(document).ready(function () {
    // Function to validate email on focusout
    $("#Email").on("focusout", function () {
        validateEmail();
    });

    // Function to validate email
    function validateEmail() {
        var email = $("#Email").val().trim();
        var emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;

        if (email === "") {
            $("#emailError").text("Please enter the email.");
        } else if (!emailRegex.test(email)) {
            $("#emailError").text("Please enter a valid email address.");
        } else {
            $("#emailError").text("");
        }
    }
});
$(document).ready(function () {
    // Function to validate address on focusout
    $("#Address").on("focusout", function () {
        validateAddress();
    });

    // Function to validate address
    function validateAddress() {
        console.log("Validating Address...");
        var address = $("#Address").val().trim();

        if (address === "") {
            $("#addressError").text("Please enter your address.");
        } else {
            $("#addressError").text("");
        }
    }
});
$(document).ready(function () {
    // Function to retrieve cities based on the selected state
    function getCitiesByState(stateId) {
        $.ajax({
            url: '/User/GetCitiesByState',
            type: 'GET',
            data: { stateId: stateId },
            success: function (data) {
                // Clear existing options and add new options to the city dropdown
                var cityDropdown = $('#CityDropdown');
                cityDropdown.empty();
                $.each(data, function (index, item) {
                    cityDropdown.append($('<option></option>').val(item.Value).text(item.Text));
                });
            },
            
        });
    }

    // Event handler for state dropdown change
    $('#StateDropdown').on('change', function () {
        var selectedStateId = $(this).val();
        // Set the selected stateId to the hidden field
        $('#StateId').val(selectedStateId);
        // Fetch cities for the selected state
        getCitiesByState(selectedStateId);
    });

    // On page load, trigger the state dropdown change event to populate cities if a state is already selected
    var selectedStateId = $('#StateDropdown').val();
    $('#StateId').val(selectedStateId);
    getCitiesByState(selectedStateId);
});



$(document).ready(function () {
    // Function to validate username on focusout
    $("#Username").on("focusout", function () {
        validateUsername();
    });

    // Function to validate username
    function validateUsername() {
        var username = $("#Username").val().trim();
        var emailPattern = /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/;

        if (username === "") {
            $("#usernameError").text("Please enter the username.");
        } else if (!emailPattern.test(username)) {
            $("#usernameError").text("Please enter a valid email address.");
        } else {
            $("#usernameError").text("");
        }
    }
});

$(document).ready(function () {
    // Function to validate username on focusout
    $("#Username").on("focusout", function () {
        validateUsername();
    });

    // Function to validate username
    function validateUsername() {
        var username = $("#Username").val().trim();
        var emailPattern = /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/;

        if (username === "") {
            $("#usernameError").text("Please enter the username.");
        } else if (!emailPattern.test(username)) {
            $("#usernameError").text("Please enter a valid email address.");
        } else {
            $("#usernameError").text("");

            // Make an AJAX call to check if the username already exists
            $.ajax({
                type: "POST",
                url: "/UserController/CheckUsernameExists", 
                data: { username: username },
                success: function (data) {
                    if (data.exists) {
                        $("#usernameError").text("This username already exists. Please choose a different one.");
                    } else {
                        $("#usernameError").text("");
                    }
                },
                
            });
        }
    }
});

$(document).ready(function () {
    // Function to validate password on focusout
    $("#Password").on("focusout", function () {
        var password = $("#Password").val().trim();
        var strongPasswordPattern = /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$/;

        if (password === "") {
            $("#passwordError").text("Please enter the password.");
        } else if (!strongPasswordPattern.test(password)) {
            $("#passwordError").text("Password must be at least 8 characters long and contain at least one uppercase letter, one lowercase letter, and one number.");
        } else {
            $("#passwordError").text("");
        }
    });
});
$(document).ready(function () {
    // Function to validate password on focusout
    $("#Password").on("focusout", function () {
        validatePassword();
        validateConfirmPassword();
    });

    // Function to validate confirm password on focusout
    $("#ConfirmPassword").on("focusout", function () {
        validateConfirmPassword();
    });

    // Function to validate the form before submission
    $("#myForm").submit(function (event) {
        if (!validatePassword() || !validateConfirmPassword()) {
            event.preventDefault(); // Prevent form submission if any validation fails
        }
    });

    // Function to validate password
    function validatePassword() {
        var password = $("#Password").val().trim();
        var strongPasswordPattern = /^(?=.*\d)(?=.*[a-z])(?=.*[A-Z]).{8,}$/;

        if (password === "") {
            $("#passwordError").text("Please enter the password.");
            return false;
        } else if (!strongPasswordPattern.test(password)) {
            $("#passwordError").text("Password must have at least 8 characters, including one uppercase letter, one lowercase letter, and one digit.");
            return false;
        } else {
            $("#passwordError").text("");
            return true;
        }
    }
})
