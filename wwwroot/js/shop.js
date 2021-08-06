function updateVariant(variantId, itemId, delta) {
    let storageItem = localStorage.getItem(variantId);
    let currentValue = 0;

    if (storageItem) {
        currentValue = Number(storageItem) + delta;
        if (currentValue === 0) {
            localStorage.removeItem(variantId);
        } else {
            localStorage.setItem(variantId, currentValue);
        }
    } else {
        if (delta > 0) {
            currentValue = delta;
            localStorage.setItem(variantId, currentValue);
        }
    }

    const item = $("#item_" + itemId);
    const selectedOption = $("#variants_" + itemId).find(":selected");
    let left = selectedOption.data("left");
    left = left - delta;
    selectedOption.data("left", left);

    if (currentValue === 0) {
        item.find(".minus-btn").addClass("disabled");
    } else {
        item.find(".minus-btn").removeClass("disabled");
    }

    if (left === 0) {
        item.find(".plus-btn").addClass("disabled");
    } else {
        item.find(".plus-btn").removeClass("disabled");
    }

    updateItem(itemId);
}

function updateItem(itemId) {
    const item = $("#item_" + itemId);
    
    const selectedOption = $("#variants_" + itemId).find(":selected");
    const price = selectedOption.data("price");
    const left = selectedOption.data("left");

    item.find(".price-label").text(price + " Kč");
    item.find(".left-label").text(left);
}

function getCurrentVariant(itemId) {
    return $("#variants_" + itemId).val();
}

$(function () {
    $(".minus-btn").on("click", function () {
        const itemId = $(this).data("sid");
        const variantId = getCurrentVariant(itemId);
        updateVariant(variantId, itemId, -1);
    });

    $(".plus-btn").on("click", function () {
        const itemId = $(this).data("sid");
        const variantId = getCurrentVariant(itemId);
        updateVariant(variantId, itemId, 1);
    });
});
