// Client Management JavaScript Module
const ClientModule = (function () {
    'use strict';

    let clientId = 0;
    let translations = {};

    // Store full data for each section
    let fullWeightHistory = [];
    let fullBiochemicalTests = [];
    let fullDrugsSupplements = [];
    let fullMedicalHistory = [];

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
                    } else {
                        // If weight/height was updated, refresh the weight history section
                        fullWeightHistory = []; // Clear cache

                        $.ajax({
                            type: 'GET',
                            url: '/Client/GetWeightHistories',
                            data: { clientId: clientId },
                            success: function (historyData) {
                                fullWeightHistory = historyData;

                                // Show only first 5 records
                                refreshWeightHistoryTable(historyData.slice(0, 5));

                                // Update badge count
                                $('#weightHistoryCollapse').prev().find('.badge').text(historyData.length);

                                // Show/hide pagination button
                                if (historyData.length > 5) {
                                    $('#weightHistoryPagination').show();
                                } else {
                                    $('#weightHistoryPagination').hide();
                                }

                                // Expand the weight history section
                                $('#weightHistoryCollapse').collapse('show');
                            }
                        });
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
    // WEIGHT HISTORY - PAGINATION ONLY
    // =============================

    function loadMoreWeightHistory() {
        if (fullWeightHistory.length > 0) {
            // Already loaded, just toggle display
            refreshWeightHistoryTable(fullWeightHistory);
            $('#weightHistoryPagination').hide();
        } else {
            // Fetch all data from server
            $.ajax({
                type: 'GET',
                url: '/Client/GetWeightHistories',
                data: { clientId: clientId },
                success: function (data) {
                    fullWeightHistory = data;
                    refreshWeightHistoryTable(data);
                    $('#weightHistoryPagination').hide();
                },
                error: function () {
                    showMessage(t('ErrorOccurred'), false);
                }
            });
        }
    }

    function showLessWeightHistory() {
        refreshWeightHistoryTable(fullWeightHistory.slice(0, 5));
        $('#weightHistoryPagination').show();
        $('#weightHistoryShowLess').hide();
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
                </tr>
            `;
            tbody.append(row);
        });

        // Show "Show Less" button if displaying all records
        if (fullWeightHistory.length > 5 && data.length === fullWeightHistory.length) {
            $('#weightHistoryShowLess').show();
        } else {
            $('#weightHistoryShowLess').hide();
        }
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
                    fullBiochemicalTests = result.data; // Store full data
                    refreshBiochemicalTestTable(result.data.slice(0, 5));
                    $('#newBiochemicalTest').val('');
                    showMessage(result.msg || t('BiochemicalTestAdded'));

                    // Update badge count
                    $('#biochemicalTestCollapse').prev().find('.badge').text(result.data.length);

                    // Show pagination if more than 5 records
                    if (result.data.length > 5) {
                        $('#biochemicalTestPagination').show();
                    } else {
                        $('#biochemicalTestPagination').hide();
                    }

                    // Expand the section to show the new data
                    $('#biochemicalTestCollapse').collapse('show');
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
                    fullBiochemicalTests = result.data; // Store full data
                    refreshBiochemicalTestTable(result.data.slice(0, 5));
                    showMessage(result.msg || t('BiochemicalTestDeleted'));

                    // Update badge count
                    $('#biochemicalTestCollapse').prev().find('.badge').text(result.data.length);

                    // Update pagination visibility
                    if (result.data.length > 5) {
                        $('#biochemicalTestPagination').show();
                    } else {
                        $('#biochemicalTestPagination').hide();
                    }
                } else {
                    showMessage(result.message, false);
                }
            },
            error: function () {
                showMessage(t('ErrorOccurred'), false);
            }
        });
    }

    function loadMoreBiochemicalTests() {
        if (fullBiochemicalTests.length > 0) {
            refreshBiochemicalTestTable(fullBiochemicalTests);
            $('#biochemicalTestPagination').hide();
        } else {
            $.ajax({
                type: 'GET',
                url: '/Client/GetBiochemicalTests',
                data: { clientId: clientId },
                success: function (data) {
                    fullBiochemicalTests = data;
                    refreshBiochemicalTestTable(data);
                    $('#biochemicalTestPagination').hide();
                },
                error: function () {
                    showMessage(t('ErrorOccurred'), false);
                }
            });
        }
    }

    function showLessBiochemicalTests() {
        refreshBiochemicalTestTable(fullBiochemicalTests.slice(0, 5));
        $('#biochemicalTestPagination').show();
        $('#biochemicalTestShowLess').hide();
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

        // Show "Show Less" button if displaying all records
        if (fullBiochemicalTests.length > 5 && data.length === fullBiochemicalTests.length) {
            $('#biochemicalTestShowLess').show();
        } else {
            $('#biochemicalTestShowLess').hide();
        }
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
                    fullDrugsSupplements = result.data; // Store full data
                    refreshDrugsSupplementTable(result.data.slice(0, 5));
                    $('#newDrugSupplement').val('');
                    showMessage(result.msg || t('DrugsSupplementAdded'));

                    // Update badge count
                    $('#drugsSupplementCollapse').prev().find('.badge').text(result.data.length);

                    if (result.data.length > 5) {
                        $('#drugsSupplementPagination').show();
                    } else {
                        $('#drugsSupplementPagination').hide();
                    }

                    // Expand the section to show the new data
                    $('#drugsSupplementCollapse').collapse('show');
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
                    fullDrugsSupplements = result.data; // Store full data
                    refreshDrugsSupplementTable(result.data.slice(0, 5));
                    showMessage(result.msg || t('DrugsSupplementDeleted'));

                    // Update badge count
                    $('#drugsSupplementCollapse').prev().find('.badge').text(result.data.length);

                    if (result.data.length > 5) {
                        $('#drugsSupplementPagination').show();
                    } else {
                        $('#drugsSupplementPagination').hide();
                    }
                } else {
                    showMessage(result.message, false);
                }
            },
            error: function () {
                showMessage(t('ErrorOccurred'), false);
            }
        });
    }

    function loadMoreDrugsSupplements() {
        if (fullDrugsSupplements.length > 0) {
            refreshDrugsSupplementTable(fullDrugsSupplements);
            $('#drugsSupplementPagination').hide();
        } else {
            $.ajax({
                type: 'GET',
                url: '/Client/GetDrugsSupplements',
                data: { clientId: clientId },
                success: function (data) {
                    fullDrugsSupplements = data;
                    refreshDrugsSupplementTable(data);
                    $('#drugsSupplementPagination').hide();
                },
                error: function () {
                    showMessage(t('ErrorOccurred'), false);
                }
            });
        }
    }

    function showLessDrugsSupplements() {
        refreshDrugsSupplementTable(fullDrugsSupplements.slice(0, 5));
        $('#drugsSupplementPagination').show();
        $('#drugsSupplementShowLess').hide();
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

        // Show "Show Less" button if displaying all records
        if (fullDrugsSupplements.length > 5 && data.length === fullDrugsSupplements.length) {
            $('#drugsSupplementShowLess').show();
        } else {
            $('#drugsSupplementShowLess').hide();
        }
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
                    fullMedicalHistory = result.data; // Store full data
                    refreshMedicalHistoryTable(result.data.slice(0, 5));
                    $('#newMedicalHistory').val('');
                    showMessage(result.msg || t('MedicalHistoryAdded'));

                    // Update badge count
                    $('#medicalHistoryCollapse').prev().find('.badge').text(result.data.length);

                    if (result.data.length > 5) {
                        $('#medicalHistoryPagination').show();
                    } else {
                        $('#medicalHistoryPagination').hide();
                    }

                    // Expand the section to show the new data
                    $('#medicalHistoryCollapse').collapse('show');
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
                    fullMedicalHistory = result.data; // Store full data
                    refreshMedicalHistoryTable(result.data.slice(0, 5));
                    showMessage(result.msg || t('MedicalHistoryDeleted'));

                    // Update badge count
                    $('#medicalHistoryCollapse').prev().find('.badge').text(result.data.length);

                    if (result.data.length > 5) {
                        $('#medicalHistoryPagination').show();
                    } else {
                        $('#medicalHistoryPagination').hide();
                    }
                } else {
                    showMessage(result.message, false);
                }
            },
            error: function () {
                showMessage(t('ErrorOccurred'), false);
            }
        });
    }

    function loadMoreMedicalHistory() {
        if (fullMedicalHistory.length > 0) {
            refreshMedicalHistoryTable(fullMedicalHistory);
            $('#medicalHistoryPagination').hide();
        } else {
            $.ajax({
                type: 'GET',
                url: '/Client/GetMedicalHistories',
                data: { clientId: clientId },
                success: function (data) {
                    fullMedicalHistory = data;
                    refreshMedicalHistoryTable(data);
                    $('#medicalHistoryPagination').hide();
                },
                error: function () {
                    showMessage(t('ErrorOccurred'), false);
                }
            });
        }
    }

    function showLessMedicalHistory() {
        refreshMedicalHistoryTable(fullMedicalHistory.slice(0, 5));
        $('#medicalHistoryPagination').show();
        $('#medicalHistoryShowLess').hide();
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

        // Show "Show Less" button if displaying all records
        if (fullMedicalHistory.length > 5 && data.length === fullMedicalHistory.length) {
            $('#medicalHistoryShowLess').show();
        } else {
            $('#medicalHistoryShowLess').hide();
        }
    }

    // Public API
    return {
        init: init,
        createOrUpdate: createOrUpdate,
        loadMoreWeightHistory: loadMoreWeightHistory,
        showLessWeightHistory: showLessWeightHistory,
        addBiochemicalTest: addBiochemicalTest,
        deleteBiochemicalTest: deleteBiochemicalTest,
        loadMoreBiochemicalTests: loadMoreBiochemicalTests,
        showLessBiochemicalTests: showLessBiochemicalTests,
        addDrugsSupplement: addDrugsSupplement,
        deleteDrugsSupplement: deleteDrugsSupplement,
        loadMoreDrugsSupplements: loadMoreDrugsSupplements,
        showLessDrugsSupplements: showLessDrugsSupplements,
        addMedicalHistory: addMedicalHistory,
        deleteMedicalHistory: deleteMedicalHistory,
        loadMoreMedicalHistory: loadMoreMedicalHistory,
        showLessMedicalHistory: showLessMedicalHistory
    };
})();

// Make module globally accessible
window.ClientModule = ClientModule;