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

    // Helper function to get CSRF token
    function getCsrfToken() {
        return document.querySelector('input[name="__RequestVerificationToken"]')?.value || '';
    }

    // Helper function to format date
    function formatDate(dateString) {
        const date = new Date(dateString);
        return date.toLocaleString();
    }

    // Helper function to show toast/alert
    function showMessage(message, isSuccess = true) {
        // You can replace this with a proper toast library like toastr
        if (isSuccess) {
            alert(message);
        } else {
            alert('Error: ' + message);
        }
    }

    // =============================
    // WEIGHT HISTORY
    // =============================

    async function addWeightHistory() {
        const weight = document.getElementById('newWeight').value;
        const height = document.getElementById('newHeight').value;

        if (!weight && !height) {
            alert(t('EnterWeight') + ' ' + t('Required'));
            return;
        }

        const data = {
            clientId: clientId,
            weight: weight ? parseFloat(weight) : null,
            height: height ? parseFloat(height) : null
        };

        try {
            const response = await fetch('/Client/AddWeightHistory', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'RequestVerificationToken': getCsrfToken()
                },
                body: JSON.stringify(data)
            });

            const result = await response.json();
            if (result.success) {
                refreshWeightHistoryTable(result.data);
                document.getElementById('newWeight').value = '';
                document.getElementById('newHeight').value = '';
                showMessage(result.msg || t('WeightHistoryAdded'));
            } else {
                handleValidationErrors(result.brokenRules);
            }
        } catch (error) {
            showMessage(t('ErrorOccurred'), false);
        }
    }

    async function deleteWeightHistory(id) {
        if (!confirm(t('ConfirmDelete'))) return;

        try {
            const response = await fetch('/Client/DeleteWeightHistory', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/x-www-form-urlencoded',
                },
                body: `id=${id}&clientId=${clientId}`
            });

            const result = await response.json();
            if (result.success) {
                refreshWeightHistoryTable(result.data);
                showMessage(result.msg || t('WeightHistoryDeleted'));
            } else {
                showMessage(result.message, false);
            }
        } catch (error) {
            showMessage(t('ErrorOccurred'), false);
        }
    }

    function refreshWeightHistoryTable(data) {
        const tbody = document.getElementById('weightHistoryBody');
        tbody.innerHTML = '';
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
            tbody.innerHTML += row;
        });
    }

    // =============================
    // BIOCHEMICAL TESTS
    // =============================

    async function addBiochemicalTest() {
        const medicalData = document.getElementById('newBiochemicalTest').value;

        if (!medicalData) {
            alert(t('EnterMedicalTestData') + ' ' + t('Required'));
            return;
        }

        const data = {
            clientId: clientId,
            medicalData: medicalData
        };

        try {
            const response = await fetch('/Client/AddBiochemicalTest', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'RequestVerificationToken': getCsrfToken()
                },
                body: JSON.stringify(data)
            });

            const result = await response.json();
            if (result.success) {
                refreshBiochemicalTestTable(result.data);
                document.getElementById('newBiochemicalTest').value = '';
                showMessage(result.msg || t('BiochemicalTestAdded'));
            } else {
                handleValidationErrors(result.brokenRules);
            }
        } catch (error) {
            showMessage(t('ErrorOccurred'), false);
        }
    }

    async function deleteBiochemicalTest(id) {
        if (!confirm(t('ConfirmDelete'))) return;

        try {
            const response = await fetch('/Client/DeleteBiochemicalTest', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/x-www-form-urlencoded',
                },
                body: `id=${id}&clientId=${clientId}`
            });

            const result = await response.json();
            if (result.success) {
                refreshBiochemicalTestTable(result.data);
                showMessage(result.msg || t('BiochemicalTestDeleted'));
            } else {
                showMessage(result.message, false);
            }
        } catch (error) {
            showMessage(t('ErrorOccurred'), false);
        }
    }

    function refreshBiochemicalTestTable(data) {
        const tbody = document.getElementById('biochemicalTestBody');
        tbody.innerHTML = '';
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
            tbody.innerHTML += row;
        });
    }

    // =============================
    // DRUGS/SUPPLEMENTS
    // =============================

    async function addDrugsSupplement() {
        const drug = document.getElementById('newDrugSupplement').value;

        if (!drug) {
            alert(t('EnterDrugSupplementName') + ' ' + t('Required'));
            return;
        }

        const data = {
            clientId: clientId,
            drug: drug
        };

        try {
            const response = await fetch('/Client/AddDrugsSupplement', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'RequestVerificationToken': getCsrfToken()
                },
                body: JSON.stringify(data)
            });

            const result = await response.json();
            if (result.success) {
                refreshDrugsSupplementTable(result.data);
                document.getElementById('newDrugSupplement').value = '';
                showMessage(result.msg || t('DrugsSupplementAdded'));
            } else {
                handleValidationErrors(result.brokenRules);
            }
        } catch (error) {
            showMessage(t('ErrorOccurred'), false);
        }
    }

    async function deleteDrugsSupplement(id) {
        if (!confirm(t('ConfirmDelete'))) return;

        try {
            const response = await fetch('/Client/DeleteDrugsSupplement', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/x-www-form-urlencoded',
                },
                body: `id=${id}&clientId=${clientId}`
            });

            const result = await response.json();
            if (result.success) {
                refreshDrugsSupplementTable(result.data);
                showMessage(result.msg || t('DrugsSupplementDeleted'));
            } else {
                showMessage(result.message, false);
            }
        } catch (error) {
            showMessage(t('ErrorOccurred'), false);
        }
    }

    function refreshDrugsSupplementTable(data) {
        const tbody = document.getElementById('drugsSupplementBody');
        tbody.innerHTML = '';
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
            tbody.innerHTML += row;
        });
    }

    // =============================
    // MEDICAL HISTORY
    // =============================

    async function addMedicalHistory() {
        const medicalNote = document.getElementById('newMedicalHistory').value;

        if (!medicalNote) {
            alert(t('EnterMedicalNote') + ' ' + t('Required'));
            return;
        }

        const data = {
            clientId: clientId,
            medicalNote: medicalNote
        };

        try {
            const response = await fetch('/Client/AddMedicalHistory', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'RequestVerificationToken': getCsrfToken()
                },
                body: JSON.stringify(data)
            });

            const result = await response.json();
            if (result.success) {
                refreshMedicalHistoryTable(result.data);
                document.getElementById('newMedicalHistory').value = '';
                showMessage(result.msg || t('MedicalHistoryAdded'));
            } else {
                handleValidationErrors(result.brokenRules);
            }
        } catch (error) {
            showMessage(t('ErrorOccurred'), false);
        }
    }

    async function deleteMedicalHistory(id) {
        if (!confirm(t('ConfirmDelete'))) return;

        try {
            const response = await fetch('/Client/DeleteMedicalHistory', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/x-www-form-urlencoded',
                },
                body: `id=${id}&clientId=${clientId}`
            });

            const result = await response.json();
            if (result.success) {
                refreshMedicalHistoryTable(result.data);
                showMessage(result.msg || t('MedicalHistoryDeleted'));
            } else {
                showMessage(result.message, false);
            }
        } catch (error) {
            showMessage(t('ErrorOccurred'), false);
        }
    }

    function refreshMedicalHistoryTable(data) {
        const tbody = document.getElementById('medicalHistoryBody');
        tbody.innerHTML = '';
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
            tbody.innerHTML += row;
        });
    }

    // =============================
    // VALIDATION HANDLING
    // =============================

    function handleValidationErrors(brokenRules) {
        if (!brokenRules || brokenRules.length === 0) return;

        let errorMessage = '';
        brokenRules.forEach(rule => {
            errorMessage += `${rule.propertyName}: ${t(rule.message) || rule.message}\n`;
        });

        showMessage(errorMessage, false);
    }

    // =============================
    // FORM SUBMISSION (Create/Edit)
    // =============================

    function submitClientForm(formId, url) {
        const form = document.getElementById(formId);
        const formData = new FormData(form);

        // Convert FormData to JSON
        const jsonData = {};
        formData.forEach((value, key) => {
            jsonData[key] = value;
        });

        fetch(url, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'RequestVerificationToken': getCsrfToken()
            },
            body: JSON.stringify(jsonData)
        })
        .then(response => response.json())
        .then(result => {
            if (result.isSuccess) {
                showMessage(result.msg);
                if (result.redirectUrl) {
                    window.location.href = result.redirectUrl;
                }
            } else {
                handleValidationErrors(result.brokenRules);
            }
        })
        .catch(error => {
            showMessage(t('ErrorOccurred'), false);
        });

        return false; // Prevent default form submission
    }

    // Public API
    return {
        init: init,
        addWeightHistory: addWeightHistory,
        deleteWeightHistory: deleteWeightHistory,
        addBiochemicalTest: addBiochemicalTest,
        deleteBiochemicalTest: deleteBiochemicalTest,
        addDrugsSupplement: addDrugsSupplement,
        deleteDrugsSupplement: deleteDrugsSupplement,
        addMedicalHistory: addMedicalHistory,
        deleteMedicalHistory: deleteMedicalHistory,
        submitClientForm: submitClientForm
    };
})();

// Make module globally accessible
window.ClientModule = ClientModule;