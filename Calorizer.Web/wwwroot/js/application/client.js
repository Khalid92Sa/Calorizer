// Client Management JavaScript Module
const ClientModule = (function () {
    'use strict';

    let clientId = 0;
    let translations = {};

    // Initialize module
    function init(id, localization) {
        clientId = id;
        translations = localization || {};
    }

    // Helper function to get localized text
    function t(key) {
        return translations[key] || key;
    }

    // Helper function to format date
    function formatDate(dateString) {
        const date = new Date(dateString);
        return date.toLocaleString();
    }

    // Helper function to show toast/alert
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

            setTimeout(() => {
                const alert = cardBody.querySelector('.alert');
                if (alert) alert.remove();
            }, 5000);
        }
    }

    // Clear all validation errors
    function clearValidationErrors() {
        $('.field-validation-valid').html('').hide();
        $('.is-invalid').removeClass('is-invalid');
    }

    // Display validation errors under fields
    function displayValidationErrors(brokenRules) {
        clearValidationErrors();

        if (!brokenRules || brokenRules.length === 0) return;

        for (let i = 0; i < brokenRules.length; i++) {
            const propertyName = brokenRules[i]["propertyName"];
            const message = brokenRules[i]["message"];

            // Find error span by data-valmsg-for attribute
            const errorElement = $("span[data-valmsg-for='" + propertyName + "']");
            if (errorElement.length) {
                errorElement.html(message).show();

                // Add is-invalid class to input
                const input = $("input[name='" + propertyName + "'], select[name='" + propertyName + "'], textarea[name='" + propertyName + "']");
                input.addClass('is-invalid');

                // Scroll to first error
                if (i === 0 && errorElement.is(":visible")) {
                    $("html, body").animate({
                        scrollTop: errorElement.offset().top - 100
                    }, 500);
                }
            }
        }

        $('.field-validation-valid').show();
    }

    // =============================
    // FORM SUBMISSION (CreateOrUpdate)
    // =============================

    function createOrUpdate() {
        clearValidationErrors();

        var formData = $('#clientForm').serialize();

        $.ajax({
            type: 'POST',
            url: $('#clientForm').attr('action'),
            data: formData,
            success: function (data) {
                if (data.resultCode == 422) {
                    displayValidationErrors(data.brokenRules);
                    showMessage(t('ValidationError'), false);
                }
                else if (data.resultCode == 200) {
                    showMessage(data.msg, true);

                    if (data.redirectUrl) {
                        setTimeout(function () {
                            window.location = data.redirectUrl;
                        }, 1500);
                    }
                }
                else {
                    showMessage(data.msg || t('ErrorOccurred'), false);
                }
            },
            error: function (e) {
                showMessage(t('ErrorOccurred'), false);
            }
        });
    }

    // =============================
    // WEIGHT HISTORY
    // =============================

    function addWeightHistory() {
        const weight = $('#newWeight').val();
        const height = $('#newHeight').val();

        if (!weight && !height) {
            showMessage(t('EnterWeight') + ' ' + t('Required'), false);
            return;
        }

        const data = {
            clientId: clientId,
            weight: weight ? parseFloat(weight) : null,
            height: height ? parseFloat(height) : null
        };

        $.ajax({
            type: 'POST',
            url: '/Client/AddWeightHistory',
            data: JSON.stringify(data),
            contentType: 'application/json',
            success: function (result) {
                if (result.success) {
                    refreshWeightHistoryTable(result.data);
                    $('#newWeight').val('');
                    $('#newHeight').val('');
                    showMessage(result.msg || t('WeightHistoryAdded'));
                } else {
                    if (result.brokenRules && result.brokenRules.length > 0) {
                        const errors = result.brokenRules.map(r => r.message).join('<br>');
                        showMessage(errors, false);
                    } else {
                        showMessage(result.message, false);
                    }
                }
            },
            error: function () {
                showMessage(t('ErrorOccurred'), false);
            }
        });
    }

    function deleteWeightHistory(id) {
        if (!confirm(t('ConfirmDelete'))) return;

        $.ajax({
            type: 'POST',
            url: '/Client/DeleteWeightHistory',
            data: { id: id, clientId: clientId },
            success: function (result) {
                if (result.success) {
                    refreshWeightHistoryTable(result.data);
                    showMessage(result.msg || t('WeightHistoryDeleted'));
                } else {
                    showMessage(result.message, false);
                }
            },
            error: function () {
                showMessage(t('ErrorOccurred'), false);
            }
        });
    }

    function refreshWeightHistoryTable(data) {
        const tbody = $('#weightHistoryBody');
        tbody.empty();

        data.forEach(item => {
            const row = `
                <tr>
                    <td>${formatDate(item.createdOn)}</td>
                    <td>${item.weight || ''}</td>
                    <td>${item.height || ''}</td>
                    <td>
                        <button type="button" class="btn btn-sm btn-danger" onclick="ClientModule.deleteWeightHistory(${item.id})">
                            <i class="bi bi-trash"></i>
                        </button>
                    </td>
                </tr>
            `;
            tbody.append(row);
        });
    }

    // =============================
    // BIOCHEMICAL TESTS
    // =============================

    function addBiochemicalTest() {
        const medicalData = $('#newBiochemicalTest').val();

        if (!medicalData) {
            showMessage(t('EnterMedicalTestData') + ' ' + t('Required'), false);
            return;
        }

        const data = {
            clientId: clientId,
            medicalData: medicalData
        };

        $.ajax({
            type: 'POST',
            url: '/Client/AddBiochemicalTest',
            data: JSON.stringify(data),
            contentType: 'application/json',
            success: function (result) {
                if (result.success) {
                    refreshBiochemicalTestTable(result.data);
                    $('#newBiochemicalTest').val('');
                    showMessage(result.msg || t('BiochemicalTestAdded'));
                } else {
                    if (result.brokenRules && result.brokenRules.length > 0) {
                        const errors = result.brokenRules.map(r => r.message).join('<br>');
                        showMessage(errors, false);
                    } else {
                        showMessage(result.message, false);
                    }
                }
            },
            error: function () {
                showMessage(t('ErrorOccurred'), false);
            }
        });
    }

    function deleteBiochemicalTest(id) {
        if (!confirm(t('ConfirmDelete'))) return;

        $.ajax({
            type: 'POST',
            url: '/Client/DeleteBiochemicalTest',
            data: { id: id, clientId: clientId },
            success: function (result) {
                if (result.success) {
                    refreshBiochemicalTestTable(result.data);
                    showMessage(result.msg || t('BiochemicalTestDeleted'));
                } else {
                    showMessage(result.message, false);
                }
            },
            error: function () {
                showMessage(t('ErrorOccurred'), false);
            }
        });
    }

    function refreshBiochemicalTestTable(data) {
        const tbody = $('#biochemicalTestBody');
        tbody.empty();

        data.forEach(item => {
            const row = `
                <tr>
                    <td>${formatDate(item.createdOn)}</td>
                    <td>${item.medicalData}</td>
                    <td>
                        <button type="button" class="btn btn-sm btn-danger" onclick="ClientModule.deleteBiochemicalTest(${item.id})">
                            <i class="bi bi-trash"></i>
                        </button>
                    </td>
                </tr>
            `;
            tbody.append(row);
        });
    }

    // =============================
    // DRUGS/SUPPLEMENTS
    // =============================

    function addDrugsSupplement() {
        const drug = $('#newDrugSupplement').val();

        if (!drug) {
            showMessage(t('EnterDrugSupplementName') + ' ' + t('Required'), false);
            return;
        }

        const data = {
            clientId: clientId,
            drug: drug
        };

        $.ajax({
            type: 'POST',
            url: '/Client/AddDrugsSupplement',
            data: JSON.stringify(data),
            contentType: 'application/json',
            success: function (result) {
                if (result.success) {
                    refreshDrugsSupplementTable(result.data);
                    $('#newDrugSupplement').val('');
                    showMessage(result.msg || t('DrugsSupplementAdded'));
                } else {
                    if (result.brokenRules && result.brokenRules.length > 0) {
                        const errors = result.brokenRules.map(r => r.message).join('<br>');
                        showMessage(errors, false);
                    } else {
                        showMessage(result.message, false);
                    }
                }
            },
            error: function () {
                showMessage(t('ErrorOccurred'), false);
            }
        });
    }

    function deleteDrugsSupplement(id) {
        if (!confirm(t('ConfirmDelete'))) return;

        $.ajax({
            type: 'POST',
            url: '/Client/DeleteDrugsSupplement',
            data: { id: id, clientId: clientId },
            success: function (result) {
                if (result.success) {
                    refreshDrugsSupplementTable(result.data);
                    showMessage(result.msg || t('DrugsSupplementDeleted'));
                } else {
                    showMessage(result.message, false);
                }
            },
            error: function () {
                showMessage(t('ErrorOccurred'), false);
            }
        });
    }

    function refreshDrugsSupplementTable(data) {
        const tbody = $('#drugsSupplementBody');
        tbody.empty();

        data.forEach(item => {
            const row = `
                <tr>
                    <td>${formatDate(item.createdOn)}</td>
                    <td>${item.drug}</td>
                    <td>
                        <button type="button" class="btn btn-sm btn-danger" onclick="ClientModule.deleteDrugsSupplement(${item.id})">
                            <i class="bi bi-trash"></i>
                        </button>
                    </td>
                </tr>
            `;
            tbody.append(row);
        });
    }

    // =============================
    // MEDICAL HISTORY
    // =============================

    function addMedicalHistory() {
        const medicalNote = $('#newMedicalHistory').val();

        if (!medicalNote) {
            showMessage(t('EnterMedicalNote') + ' ' + t('Required'), false);
            return;
        }

        const data = {
            clientId: clientId,
            medicalNote: medicalNote
        };

        $.ajax({
            type: 'POST',
            url: '/Client/AddMedicalHistory',
            data: JSON.stringify(data),
            contentType: 'application/json',
            success: function (result) {
                if (result.success) {
                    refreshMedicalHistoryTable(result.data);
                    $('#newMedicalHistory').val('');
                    showMessage(result.msg || t('MedicalHistoryAdded'));
                } else {
                    if (result.brokenRules && result.brokenRules.length > 0) {
                        const errors = result.brokenRules.map(r => r.message).join('<br>');
                        showMessage(errors, false);
                    } else {
                        showMessage(result.message, false);
                    }
                }
            },
            error: function () {
                showMessage(t('ErrorOccurred'), false);
            }
        });
    }

    function deleteMedicalHistory(id) {
        if (!confirm(t('ConfirmDelete'))) return;

        $.ajax({
            type: 'POST',
            url: '/Client/DeleteMedicalHistory',
            data: { id: id, clientId: clientId },
            success: function (result) {
                if (result.success) {
                    refreshMedicalHistoryTable(result.data);
                    showMessage(result.msg || t('MedicalHistoryDeleted'));
                } else {
                    showMessage(result.message, false);
                }
            },
            error: function () {
                showMessage(t('ErrorOccurred'), false);
            }
        });
    }

    function refreshMedicalHistoryTable(data) {
        const tbody = $('#medicalHistoryBody');
        tbody.empty();

        data.forEach(item => {
            const row = `
                <tr>
                    <td>${formatDate(item.createdOn)}</td>
                    <td>${item.medicalNote}</td>
                    <td>
                        <button type="button" class="btn btn-sm btn-danger" onclick="ClientModule.deleteMedicalHistory(${item.id})">
                            <i class="bi bi-trash"></i>
                        </button>
                    </td>
                </tr>
            `;
            tbody.append(row);
        });
    }

    // Public API
    return {
        init: init,
        createOrUpdate: createOrUpdate,
        addWeightHistory: addWeightHistory,
        deleteWeightHistory: deleteWeightHistory,
        addBiochemicalTest: addBiochemicalTest,
        deleteBiochemicalTest: deleteBiochemicalTest,
        addDrugsSupplement: addDrugsSupplement,
        deleteDrugsSupplement: deleteDrugsSupplement,
        addMedicalHistory: addMedicalHistory,
        deleteMedicalHistory: deleteMedicalHistory
    };
})();

// Make module globally accessible
window.ClientModule = ClientModule;