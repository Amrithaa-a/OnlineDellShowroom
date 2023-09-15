//order form
$(document).ready(function () {
    // Function to validate the Name field on focusout
    $("#Name").on("focusout", function () {
        var name = $("#Name").val().trim();
        var nameRegex = /^[A-Za-z]+$/;

        if (name === "") {
            $("#NameError").text("Please enter the name.");
        } else if (!nameRegex.test(name)) {
            $("#NameError").text("Name should contain only alphabets.");
        } else {
            $("#NameError").text("");
        }
    });

    // Function to validate the Address field on focusout
    $("#Address").on("focusout", function () {
        var address = $("#Address").val().trim();
        if (address === "") {
            $("#AddressError").text("Please enter the address.");
        } else {
            $("#AddressError").text("");
        }
    });

    // Function to validate the ContactNumber field on focusout
    $("#ContactNumber").on("focusout", function () {
        var contactNumber = $("#ContactNumber").val().trim();
        if (contactNumber === "") {
            $("#ContactNumberError").text("Please enter the contact number.");
        } else if (!/^[6-9]\d{9}$/.test(contactNumber)) {
            $("#ContactNumberError").text("Invalid contact number. Please enter a valid 10-digit number starting with 6, 7, 8, or 9.");
        } else {
            $("#ContactNumberError").text("");
        }
    });

    // Function to validate the Pincode field on focusout
    $("#Pincode").on("focusout", function () {
        var pincode = $("#Pincode").val().trim();
        if (pincode === "") {
            $("#PincodeError").text("Please enter the pincode.");
        } else if (!/^\d{6}$/.test(pincode)) {
            $("#PincodeError").text("Invalid pincode. Please enter a valid 6-digit number.");
        } else {
            $("#PincodeError").text("");
        }
    });

    // Prevent entering non-numeric characters
    $("#ContactNumber, #Pincode").on("input", function () {
        var value = $(this).val();
        value = value.replace(/\D/g, ''); // Remove non-numeric characters
        $(this).val(value);
    });


});

//order history
function showConfirmation(orderId) {
    console.log("Confirmation function called for orderId:", orderId);

    // Ask for confirmation before canceling the order
    if (confirm("Are you sure you want to cancel this order?")) {
        // If user confirms, send a POST request to the CancelOrder action method
        var token = $('[name=__RequestVerificationToken]').val(); // Fetch the Anti-Forgery Token value

        var data = {
            orderId: orderId,
            __RequestVerificationToken: token // Include the Anti-Forgery Token in the request data
        };

        $.ajax({
            url: "CancelOrder", 
            type: "POST",
            data: data,
            success: function (result) {
                console.log("CancelOrder success response:", result);
                if (result.success) {
                    window.location.reload();
                } else {
                    alert("Failed to cancel the order. Please try again.");
                }
            },
            error: function (xhr, textStatus, errorThrown) {
                console.log("CancelOrder error response:", xhr.status, xhr.responseText, textStatus, errorThrown);
                alert("Failed to cancel the order. Please try again.");
            },
        });
    }
}

$(document).ready(function () {
 

    console.log("Script loaded");

    // Handle "Buy Now" button click
    $('#buyNowBtn').on('click', function (e) {
        // Check if any items are selected for buying
        var selectedItems = $('.item-checkbox:checked');
        if (selectedItems.length === 0) {
            alert('Please select at least one item to buy.');
            e.preventDefault(); // Prevent the default link behavior
        } else {
            updateSelectedTotalPrice();
        }
    });

    // Function to calculate subtotal and update the total price
    function updateSubtotal() {
        var total = 0;
        $(".cart-item-row").each(function () {
            var $row = $(this);
            var quantity = parseInt($row.find(".quantity").val());
            var price = parseFloat($row.find(".price").data("price"));
            var subtotal = quantity * price;
            total += subtotal;
            $row.find(".subtotal").text("₹" + subtotal.toFixed(2));
        });
        updateSelectedTotalPrice(); // Update the selected total as well
    }

    // Function to update selected total price
    function updateSelectedTotalPrice() {
        var selectedTotal = 0;

        // Iterate through checked checkboxes and update selected total
        $(".item-checkbox:checked").each(function () {
            var $row = $(this).closest(".cart-item-row");
            var price = parseFloat($row.find(".price").data("price"));
            var quantity = parseInt($row.find(".quantity").val());
            selectedTotal += price * quantity;
        });

        if (selectedTotal === 0) {
            $("#selectedTotalPrice").text("₹0.00");
        } else {
            $("#selectedTotalPrice").text("₹" + selectedTotal.toFixed(2));

            // Store the selected total price in a session variable
            sessionStorage.setItem("selectedTotalPrice", selectedTotal.toFixed(2));
        }
    }

    // Call updateSelectedTotalPrice initially
    updateSelectedTotalPrice();


    // Call updateSubtotal initially
    updateSubtotal();

    // Update subtotal when quantity input changes
    $(".quantity").on("input", function () {
        updateSubtotal();
    });

    // Update selected total when checkboxes change
    $(".item-checkbox").on("change", function () {
        updateSelectedTotalPrice();
    });

    // Clear selected items button click event
    $(".clear-selected-items-btn").on("click", function () {
        var selectedLaptopIds = $(".item-checkbox:checked").map(function () {
            return parseInt($(this).val()); // Parse as integer
        }).get();

        if (selectedLaptopIds.length === 0) {
            alert("Please select at least one item to delete.");
            return;
        }

        $.post("/User/RemoveSelectedCartItems", { laptopIds: selectedLaptopIds }, function (data) {
            if (data.success) {
                $(".item-checkbox:checked").closest(".cart-item-row").remove();
                updateSubtotal();
                updateSelectedTotalPrice();
            }
        });
    });


});






