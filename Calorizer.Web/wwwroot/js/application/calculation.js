// Calculation Module
const CalculationModule = (function () {
    'use strict';

    let clientId = 0;
    let genderId = 0;
    let translations = {};

    // Food exchange nutritional values
    const foodNutrition = {
        SkimmedMilk: { cho: 12, protein: 8, fat: 0, calories: 80 },
        LowFatMilk: { cho: 12, protein: 8, fat: 5, calories: 125 },
        FullFatMilk: { cho: 12, protein: 8, fat: 8, calories: 152 },
        Fruit: { cho: 15, protein: 0, fat: 0, calories: 60 },
        Vegetable: { cho: 5, protein: 2, fat: 0, calories: 25 },
        Sugar: { cho: 5, protein: 0, fat: 0, calories: 20 },
        Starch: { cho: 15, protein: 3, fat: 0, calories: 80 },
        LeanMeat: { cho: 0, protein: 7, fat: 3, calories: 55 },
        MediumFatMeat: { cho: 0, protein: 7, fat: 5, calories: 75 },
        HighFatMeat: { cho: 0, protein: 7, fat: 8, calories: 100 },
        Fat: { cho: 0, protein: 0, fat: 5, calories: 45 }
    };

    // Initialize module
    function init(id, gender, localization) {
        clientId = id;
        genderId = gender;
        translations = localization || {};

        // Add event listeners for Total Calories and macronutrient inputs
        $('#totalCalories, #carbsPercentage, #proteinPercentage, #fatPercentage').on('input', function () {
            calculateMacronutrientGrams();
        });
    }

    // Helper function to get localized text
    function t(key) {
        return translations[key] || key;
    }

    // Helper function to show toast/alert (only for errors)
    function showMessage(message, isSuccess = true) {
        const alertClass = isSuccess ? 'alert-success' : 'alert-danger';
        const alertHtml = `
            <div class="alert ${alertClass} alert-dismissible fade show" role="alert">
                ${message}
                <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
            </div>
        `;

        const cardBody = document.querySelector('.card-body');
        if (cardBody) {
            const existingAlert = cardBody.querySelector('.alert');
            if (existingAlert) existingAlert.remove();

            cardBody.insertAdjacentHTML('afterbegin', alertHtml);

            // Scroll to top to see the message
            window.scrollTo({ top: 0, behavior: 'smooth' });

            setTimeout(() => {
                const alert = cardBody.querySelector('.alert');
                if (alert) alert.remove();
            }, 5000);
        }
    }

    // Calculate Total Energy Expenditure
    function calculateEnergy() {
        const height = parseFloat($('#height').val()) || 0;
        const weight = parseFloat($('#weight').val()) || 0;
        const age = parseInt($('#age').val()) || 0;
        const physicalActivityFactor = parseFloat($('#physicalActivityFactor').val()) || 0;

        if (!physicalActivityFactor) {
            showMessage(t('PhysicalActivityFactor') + ' ' + t('Required'), false);
            return;
        }

        const data = {
            ClientId: clientId,
            Height: height,
            Weight: weight,
            Age: age,
            GenderId: genderId,
            PhysicalActivityFactor: physicalActivityFactor
        };

        $.ajax({
            type: 'POST',
            url: '/Calculation/Calculate',
            data: JSON.stringify(data),
            contentType: 'application/json',
            success: function (result) {
                if (result.success) {
                    const calcData = result.data;

                    // Update Total Energy Expenditure
                    $('#totalEnergyExpenditure').val(calcData.totalEnergyExpenditure.toFixed(2));
                    $('#totalCalories').val(calcData.totalCalories.toFixed(2));

                    // Auto-calculate macronutrients if values are already entered
                    calculateMacronutrientGrams();
                } else {
                    showMessage(result.message || t('ErrorOccurred'), false);
                }
            },
            error: function () {
                showMessage(t('ErrorOccurred'), false);
            }
        });
    }

    // Calculate Macronutrient Grams
    // Formulas:
    // CHO grams = (CHO value × Total Calories) / 4
    // Protein grams = (Protein value × Total Calories) / 4
    // Fat grams = (Fat value × Total Calories) / 9
    function calculateMacronutrientGrams() {
        const totalCalories = parseFloat($('#totalCalories').val()) || 0;

        if (totalCalories === 0) {
            // Clear all gram fields if total calories is 0
            $('#carbsGrams').val('');
            $('#proteinGrams').val('');
            $('#fatGrams').val('');
            return;
        }

        // Get input values
        const carbsValue = parseFloat($('#carbsPercentage').val()) || 0;
        const proteinValue = parseFloat($('#proteinPercentage').val()) || 0;
        const fatValue = parseFloat($('#fatPercentage').val()) || 0;

        // Calculate grams using the formulas
        // CHO: (CHO × Total Calories) / 4
        const carbsGrams = (carbsValue * totalCalories) / 4;

        // Protein: (Protein × Total Calories) / 4
        const proteinGrams = (proteinValue * totalCalories) / 4;

        // Fat: (Fat × Total Calories) / 9
        const fatGrams = (fatValue * totalCalories) / 9;

        // Update the disabled fields
        $('#carbsGrams').val(carbsGrams.toFixed(2));
        $('#proteinGrams').val(proteinGrams.toFixed(2));
        $('#fatGrams').val(fatGrams.toFixed(2));
    }

    // Calculate Food Exchange Totals
    function calculateFoodExchange() {
        let totalCHO = 0;
        let totalProtein = 0;
        let totalFat = 0;
        let totalCalories = 0;

        // Loop through all food groups
        Object.keys(foodNutrition).forEach(function (foodGroup) {
            const serving = parseFloat($(`input[data-group="${foodGroup}"]`).val()) || 0;
            const nutrition = foodNutrition[foodGroup];

            totalCHO += nutrition.cho * serving;
            totalProtein += nutrition.protein * serving;
            totalFat += nutrition.fat * serving;
            totalCalories += nutrition.calories * serving;
        });

        // Update totals
        $('#totalCHO').val(totalCHO.toFixed(2));
        $('#totalProtein').val(totalProtein.toFixed(2));
        $('#totalFat').val(totalFat.toFixed(2));
        $('#totalCaloriesFood').val(totalCalories.toFixed(2));
    }

    // Auto-calculate when serving values change
    $(document).on('change', '.food-serving', function () {
        calculateFoodExchange();
    });

    // Public API
    return {
        init: init,
        calculateEnergy: calculateEnergy,
        calculateFoodExchange: calculateFoodExchange
    };
})();

// Make module globally accessible
window.CalculationModule = CalculationModule;