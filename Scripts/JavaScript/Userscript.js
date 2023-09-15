window.setTimeout(function () {
    $(".alert").fadeTo(500, 0).slideUp(500, function () {
        $(this).remove();
    });
}, 4000);


    $(document).ready(function () {
        // Get the "Dateofbirth" input element
        var dateOfBirthInput = $("#Dateofbirth");

        // Disable future dates in the date picker
        dateOfBirthInput.attr("max", formatDate(new Date()));

        // Validate age
        dateOfBirthInput.on("change", function () {
            var dateOfBirth = new Date($(this).val());
            var eighteenYearsAgo = new Date();
            eighteenYearsAgo.setFullYear(eighteenYearsAgo.getFullYear() - 18);

            if (dateOfBirth > eighteenYearsAgo) {
                $(this).addClass("is-invalid");
                $("#ageError").html("Age must be at least 18 years.");
            } else {
                $(this).removeClass("is-invalid");
                $("#ageError").html("");
            }
        });

        // Helper function to format date as yyyy-mm-dd
        function formatDate(date) {
            var year = date.getFullYear();
            var month = ("0" + (date.getMonth() + 1)).slice(-2);
            var day = ("0" + date.getDate()).slice(-2);
            return year + "-" + month + "-" + day;
        }
    });
   
function incrementQuantity(inputId) {
    var quantityField = document.getElementById(inputId);
    var currentQuantity = parseInt(quantityField.value);
    quantityField.value = currentQuantity + 1;
}

function decrementQuantity(inputId) {
    var quantityField = document.getElementById(inputId);
    var currentQuantity = parseInt(quantityField.value);
    if (currentQuantity > 1) {
        quantityField.value = currentQuantity - 1;
    }
}

function deleteLaptop(laptopId) {
    if (confirm("Are you sure you want to delete this laptop?")) {
        $.post('@Url.Action("DeleteLaptop", "Admin")', { id: laptopId })
            .done(function (data) {
                window.location.reload();
            })
            .fail(function () {
                alert("Failed to delete laptop. Please try again.");
            });
    }
}




// Userscript.js (Place this script in your Scripts folder and reference it in the ViewCart.cshtml)
function updateCartItemQuantity(laptopId, newQuantity) {
    // Make an AJAX call to update the quantity in the server-side session
    $.ajax({
        type: 'POST',
        url: '@Url.Action("UpdateCartItemQuantity", "User")',
        data: { laptopId: laptopId, quantity: newQuantity },
        success: function (result) {
            // On success, reload the page to reflect the changes
            location.reload();
        },
        error: function () {
            // Handle error if needed
            alert("Error updating quantity. Please try again.");
        }
    });
}


// Function to fetch cities based on the selected state
// Function to fetch cities based on the selected state
$(document).ready(function () {
    $("#SelectedStateId").change(function () {
        var stateId = $(this).val();
        $.ajax({
            type: "GET",
            url: "/User/GetCities", // This URL should point to the correct route for the 'GetCities' action method in the 'UserController'
            data: { stateId: stateId },
            dataType: "json",
            success: function (data) {
                var citiesDropdown = $("#SelectedCityId");
                citiesDropdown.empty(); // Clear existing options
                if (data.length > 0) {
                    // If there are cities available, add them to the dropdown
                    $.each(data, function (index, option) {
                        citiesDropdown.append(new Option(option.Text, option.Value, option.Value === "@Model.SelectedCityId", option.Disabled));

                    });
                } else {
                    // If no cities available, show a message
                    citiesDropdown.append(new Option("-- No cities available --", ""));
                }
            },
            error: function (error) {
                console.log(error);
            }
        });
    });
});

//Add laptop page validation
$(document).ready(function () {
    $("#laptopNameInput").on("focusout", function () {
        var laptopName = this.value.trim();
        if (laptopName === "") {
            $("#laptopNameError").text("Please enter the laptop name.");
        } else if (!isNaN(laptopName)) {
            $("#laptopNameError").text("Laptop name cannot be just numbers.");
        } else {
            $("#laptopNameError").text("");
        }
    });

    $("#descriptionInput").on("focusout", function () {
        var description = this.value.trim();
        if (description === "") {
            $("#descriptionError").text("Please enter the laptop description.");
        } else {
            $("#descriptionError").text("");
        }
    });

    $("#imageFileInput").on("focusout", function () {
        var fileInput = this;
        if (fileInput.files.length > 0) {
            var file = fileInput.files[0];
            var fileType = file.type.toLowerCase();
            if (!fileType.startsWith("image/")) {
                // Non-image file selected, show error message
                $("#fileError").text("Please select an image file.");
                fileInput.value = ""; // Clear the selected file
            } else {
                // Image file selected, clear the error message
                $("#fileError").text("");
            }
        } else {
            // No file selected, clear the error message
            $("#fileError").text("");
        }
    });

    $("#priceInput").on("focusout", function () {
        var price = parseFloat(this.value.trim());
        if (price === 0) {
            $("#priceError").text("Price cannot be zero.");
        } else if (isNaN(price)) {
            $("#priceError").text("Price must be a valid number.");
        } else {
            $("#priceError").text("");
        }
    });

    // Function to validate the form before submission
    $("#myForm").submit(function (event) {
        var laptopName = $("#laptopNameInput").val().trim();
        if (laptopName === "") {
            $("#laptopNameError").text("Please enter the laptop name.");
            event.preventDefault();
        } else if (!isNaN(laptopName)) {
            $("#laptopNameError").text("Laptop name cannot be just numbers.");
            event.preventDefault();
        }

        var description = $("#descriptionInput").val().trim();
        if (description === "") {
            $("#descriptionError").text("Please enter the laptop description.");
            event.preventDefault();
        }

        var fileInput = $("#imageFileInput")[0];
        if (fileInput.files.length > 0) {
            var file = fileInput.files[0];
            var fileType = file.type.toLowerCase();
            if (!fileType.startsWith("image/")) {
                // Non-image file selected, show error message
                $("#fileError").text("Please select an image file.");
                fileInput.value = ""; // Clear the selected file
                event.preventDefault();
            }
        } else {
            // No file selected, show error message
            $("#fileError").text("Please select an image file.");
            event.preventDefault();
        }

        var price = parseFloat($("#priceInput").val().trim());
        if (price === 0) {
            $("#priceError").text("Price cannot be zero.");
            event.preventDefault();
        } else if (isNaN(price)) {
            $("#priceError").text("Price must be a valid number.");
            event.preventDefault();
        }
    });
});