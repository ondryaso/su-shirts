function setCookie() {
    let contents = JSON.stringify(localStorage);
    contents = encodeURIComponent(contents);
    document.cookie = "cart=" + contents;
}

function updateCart() {
    let total = 0;

    $.each(localStorage, function (key, value) {
        if (!isNaN(key) && !isNaN(value)) {
            total += Number(value);
        }
    });

    $(".cart-text").text(total);
    if (total > 0) {
        $(".cart-btn").removeClass("disabled");
    } else {
        $(".cart-btn").addClass("disabled");
    }

    return total;
}

function updateItem(item, delta) {
    const itemId = item.data("sid");
    const currentItemVariantId = item.find(".variant-select").val();

    let storageItem = localStorage.getItem(currentItemVariantId);
    let currentValue = 0;

    if (storageItem) {
        currentValue = Number(storageItem) + delta;
        if (currentValue === 0) {
            localStorage.removeItem(currentItemVariantId);
        } else {
            localStorage.setItem(currentItemVariantId, currentValue);
        }
    } else {
        if (delta > 0) {
            currentValue = delta;
            localStorage.setItem(currentItemVariantId, currentValue);
        }
    }

    const variantSelector = $(".variant-select[data-sid='" + itemId + "']");

    variantSelector.each(function () {
        const selectedOption = $(this).find(":selected");

        if ($(this).val() === currentItemVariantId) {
            let left = selectedOption.data("left");
            left = left - currentValue;

            const thisItem = $(this).closest(".shirt-item");

            if (currentValue === 0) {
                thisItem.find(".minus-btn").addClass("disabled");
            } else {
                thisItem.find(".minus-btn").removeClass("disabled");
            }

            if (left === 0) {
                thisItem.find(".plus-btn").addClass("disabled");
            } else {
                thisItem.find(".plus-btn").removeClass("disabled");
            }

            const price = selectedOption.data("price");

            thisItem.find(".price-label").text(price + " Kč");
            thisItem.find(".left-label").text(left);
            thisItem.find(".items-counter").text(currentValue);
        }
    });

    updateCart();
}

$(function () {
    $(".minus-btn").on("click", function () {
        const item = $(this).closest(".shirt-item");
        updateItem(item, -1);
    });

    $(".plus-btn").on("click", function () {
        const item = $(this).closest(".shirt-item");
        updateItem(item, 1);
    });

    $(".variant-select").on("change", function () {
        const item = $(this).closest(".shirt-item");
        updateItem(item, 0);
    });

    $(".shirt-item").each(function () {
        const item = $(this);
        updateItem(item, null);
    });

    $(".cart-btn").on("click", function () {
        const items = updateCart();
        if (items > 0) {
            setCookie();
            window.location.href = makeReservationUrl;
        }
    });
});
