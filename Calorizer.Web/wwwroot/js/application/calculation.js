// Calculation Module
const CalculationModule = (function () {
    'use strict';

    let clientId = 0;
    let genderId = 0;
    let translations = {};
    let clientName = '';
    let notes = [];
    let showingAllNotes = false;
    const NOTES_PAGE_SIZE = 5;

    // Initialize module
    function init(id, gender, localization, notesHistory, name) {
        clientId = id;
        genderId = gender;
        translations = localization || {};
        notes = notesHistory || [];
        clientName = name || '';

        // Add event listeners for Total Calories and macronutrient inputs
        $('#totalCalories, #carbsPercentage, #proteinPercentage, #fatPercentage').on('input', function () {
            calculateMacronutrientGrams();
        });

        // Food exchange: recalculate the row + totals while typing the serving
        $(document).on('input change', '.food-serving', calculateFoodExchange);

        renderNotes();
    }

    // Escape user text before injecting into HTML
    function escapeHtml(text) {
        return $('<div>').text(text == null ? '' : text).html();
    }

    function formatDate(dateString) {
        const d = new Date(dateString);
        const pad = n => String(n).padStart(2, '0');
        return `${pad(d.getDate())}/${pad(d.getMonth() + 1)}/${d.getFullYear()} ${pad(d.getHours())}:${pad(d.getMinutes())}`;
    }

    // Trim trailing zeros: 120.00 -> 120, 7.50 -> 7.5
    function formatNumber(value) {
        return parseFloat(value.toFixed(2));
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

    // Calculate Food Exchange: each row = serving x per-serving value, then totals
    function calculateFoodExchange() {
        let totalCHO = 0;
        let totalProtein = 0;
        let totalFat = 0;
        let totalCalories = 0;

        $('.food-serving').each(function () {
            const $input = $(this);
            const $row = $input.closest('tr');
            const serving = parseFloat($input.val()) || 0;

            const cho = (parseFloat($input.data('cho')) || 0) * serving;
            const protein = (parseFloat($input.data('protein')) || 0) * serving;
            const fat = (parseFloat($input.data('fat')) || 0) * serving;
            const calories = (parseFloat($input.data('calories')) || 0) * serving;

            $row.find('.row-cho').val(formatNumber(cho));
            $row.find('.row-protein').val(formatNumber(protein));
            $row.find('.row-fat').val(formatNumber(fat));
            $row.find('.row-calories').val(formatNumber(calories));

            totalCHO += cho;
            totalProtein += protein;
            totalFat += fat;
            totalCalories += calories;
        });

        $('#totalCHO').val(formatNumber(totalCHO));
        $('#totalProtein').val(formatNumber(totalProtein));
        $('#totalFat').val(formatNumber(totalFat));
        $('#totalCaloriesFood').val(formatNumber(totalCalories));
    }

    // ===================== Notes (history) =====================

    function addNote() {
        const note = ($('#newNote').val() || '').trim();
        if (!note) {
            showMessage(t('NoteRequired'), false);
            return;
        }

        $.ajax({
            type: 'POST',
            url: '/Calculation/AddNote',
            data: JSON.stringify({ ClientId: clientId, Note: note }),
            contentType: 'application/json',
            success: function (result) {
                if (result.success) {
                    notes = result.data || [];
                    $('#newNote').val('');
                    renderNotes();
                } else {
                    showMessage(result.message || t('ErrorOccurred'), false);
                }
            },
            error: function () {
                showMessage(t('ErrorOccurred'), false);
            }
        });
    }

    function deleteNote(id) {
        if (!confirm(t('ConfirmDelete'))) return;

        $.ajax({
            type: 'POST',
            url: '/Calculation/DeleteNote',
            data: { id: id, clientId: clientId },
            success: function (result) {
                if (result.success) {
                    notes = result.data || [];
                    renderNotes();
                } else {
                    showMessage(result.message || t('ErrorOccurred'), false);
                }
            },
            error: function () {
                showMessage(t('ErrorOccurred'), false);
            }
        });
    }

    function renderNotes() {
        const $tbody = $('#notesBody');
        $tbody.empty();
        $('#notesCount').text(notes.length);

        if (notes.length === 0) {
            $tbody.append(`<tr><td colspan="3" class="text-center text-muted">${escapeHtml(t('NoNotes'))}</td></tr>`);
        }

        const visible = showingAllNotes ? notes : notes.slice(0, NOTES_PAGE_SIZE);
        visible.forEach(item => {
            $tbody.append(`
                <tr>
                    <td>${formatDate(item.createdOn)}</td>
                    <td dir="auto" style="white-space: pre-wrap;">${escapeHtml(item.note)}</td>
                    <td class="text-nowrap">
                        <button type="button" class="btn btn-sm btn-outline-primary" title="${escapeHtml(t('Print'))}" onclick="CalculationModule.printNotes(${item.id})">
                            <i class="bi bi-printer"></i>
                        </button>
                        <button type="button" class="btn btn-sm btn-danger" title="${escapeHtml(t('Delete'))}" onclick="CalculationModule.deleteNote(${item.id})">
                            <i class="bi bi-trash"></i>
                        </button>
                    </td>
                </tr>
            `);
        });

        const hasMore = notes.length > NOTES_PAGE_SIZE;
        $('#notesShowMore').toggle(hasMore && !showingAllNotes);
        $('#notesShowLess').toggle(hasMore && showingAllNotes);
    }

    function showAllNotes() {
        showingAllNotes = true;
        renderNotes();
    }

    function showLessNotes() {
        showingAllNotes = false;
        renderNotes();
    }

    // Print notes only (not the whole page).
    // printNotes()   -> all saved notes
    // printNotes(id) -> one note
    function printNotes(noteId) {
        const toPrint = noteId ? notes.filter(n => n.id === noteId) : notes;
        if (toPrint.length === 0) {
            showMessage(t('NoNotes'), false);
            return;
        }

        $('#notesPrintArea').remove();

        const isRtl = document.documentElement.dir === 'rtl' || $('html').attr('lang') === 'ar';
        const itemsHtml = toPrint.map(n => `
            <div class="print-note">
                <div class="print-note-date">${formatDate(n.createdOn)}</div>
                <div class="print-note-text" dir="auto">${escapeHtml(n.note)}</div>
            </div>
        `).join('');

        $('body').append(`
            <div id="notesPrintArea" dir="${isRtl ? 'rtl' : 'ltr'}">
                <h3>${escapeHtml(t('Notes'))}</h3>
                <div class="print-meta">
                    ${escapeHtml(t('ClientName'))}: ${escapeHtml(clientName)} &nbsp;|&nbsp;
                    ${escapeHtml(t('ClientID'))}: ${clientId} &nbsp;|&nbsp;
                    ${formatDate(new Date())}
                </div>
                ${itemsHtml}
            </div>
        `);

        $('body').addClass('printing-notes');
        window.print();
        $('body').removeClass('printing-notes');
        $('#notesPrintArea').remove();
    }

    // Public API
    return {
        init: init,
        calculateEnergy: calculateEnergy,
        calculateFoodExchange: calculateFoodExchange,
        addNote: addNote,
        deleteNote: deleteNote,
        showAllNotes: showAllNotes,
        showLessNotes: showLessNotes,
        printNotes: printNotes
    };
})();

// Make module globally accessible
window.CalculationModule = CalculationModule;