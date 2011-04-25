Type.registerNamespace("PraticeManagement.Controls.Generic.DirtyState");

// Array to store all single time entries on the page
var allSingleTEs = new Array();

PraticeManagement.Controls.Generic.DirtyState.DirtyBehavior = function (element) {
    PraticeManagement.Controls.Generic.DirtyState.DirtyBehavior.initializeBase(this, [element]);

    this._NoteIdValue = null;
    this._HiddenNoteIdValue = null;
    this._ActualHoursIdValue = null;
    this._HiddenActualHoursIdValue = null;
    this._IsChargeableIdValue = null;
    this._HiddenIsChargeableIdValue = null
    this._HiddenDefaultIsChargeableIdValue = null;
    this._IsCorrectIdValue = null;
    this._HiddenIsCorrectIdValue = null;
    this._HorizontalTotalCalculatorExtenderIdValue = null;
    this._VerticalTotalCalculatorExtenderIdValue = null;
    this._SpreadSheetExtenderIdValue = null;
    this._TimeTypeDropdownIdValue = null;
    this._ProjectMilestoneDropdownIdValue = null;

    // Add particular STE to the array if it's not there yet
    if (!Array.contains(allSingleTEs, this))
        Array.add(allSingleTEs, this);
}

PraticeManagement.Controls.Generic.DirtyState.DirtyBehavior.prototype = {
    _initBase: function () {
        PraticeManagement.Controls.Generic.DirtyState.DirtyBehavior.callBaseMethod(this, 'initialize');
    },
    initialize: function () {
        this._initBase();

        //  On clicking the label, call the corresponding function
        $addHandler($get(this.get_NoteIdValue()), 'change', Function.createDelegate(this, this._setDirty));
        $addHandler($get(this.get_ActualHoursIdValue()), 'change', Function.createDelegate(this, this._setDirty));
        $addHandler($get(this.get_IsCorrectIdValue()), 'change', Function.createDelegate(this, this._setDirty));

        var isCharg = $get(this.get_IsChargeableIdValue());
        if (isCharg)
            $addHandler(isCharg, 'change', Function.createDelegate(this, this._setDirty));
    },
    dispose: function () {
        //Add custom dispose actions here
        PraticeManagement.Controls.Generic.DirtyState.DirtyBehavior.callBaseMethod(this, 'dispose');
    },
    _note: function () {
        return new String($get(this.get_NoteIdValue()).value);
    },
    _hours: function () {
        return new String($get(this.get_ActualHoursIdValue()).value);
    },
    _isDirty: function () {
        if (this.get_element())
            return this.get_element().value != "";

        return false;
    },
    _setDirty: function () {
        this._setBackground('gold');

        var note = new String($get(this.get_NoteIdValue()).value);
        var hours = new String($get(this.get_ActualHoursIdValue()).value);

        var stuff = new String(note + hours);

        if (stuff.trim().length == 0)
            this.get_element().value = "";
        else
            this.get_element().value = "dirty";
    },
    get_NoteIdValue: function () {
        return this._NoteIdValue;
    },

    set_NoteIdValue: function (value) {
        this._NoteIdValue = value;
    },

    get_HiddenNoteIdValue: function () {
        return this._HiddenNoteIdValue;
    },

    set_HiddenNoteIdValue: function (value) {
        this._HiddenNoteIdValue = value;
    },

    get_IsChargeableIdValue: function () {
        return this._IsChargeableIdValue;
    },

    set_IsChargeableIdValue: function (value) {
        this._IsChargeableIdValue = value;
    },

    get_HiddenIsChargeableIdValue: function () {
        return this._HiddenIsChargeableIdValue;
    },

    set_HiddenIsChargeableIdValue: function (value) {
        this._HiddenIsChargeableIdValue = value;
    },

    get_HiddenDefaultIsChargeableIdValue: function () {
        return this._HiddenDefaultIsChargeableIdValue;
    },

    set_HiddenDefaultIsChargeableIdValue: function (value) {
        this._HiddenDefaultIsChargeableIdValue = value;
    },

    get_IsCorrectIdValue: function () {
        return this._IsCorrectIdValue;
    },

    set_IsCorrectIdValue: function (value) {
        this._IsCorrectIdValue = value;
    },
    get_HiddenIsCorrectIdValue: function () {
        return this._HiddenIsCorrectIdValue;
    },

    set_HiddenIsCorrectIdValue: function (value) {
        this._HiddenIsCorrectIdValue = value;
    },

    get_ActualHoursIdValue: function () {
        return this._ActualHoursIdValue;
    },

    set_ActualHoursIdValue: function (value) {
        this._ActualHoursIdValue = value;
    },

    get_HiddenActualHoursIdValue: function () {
        return this._HiddenActualHoursIdValue;
    },

    set_HiddenActualHoursIdValue: function (value) {
        this._HiddenActualHoursIdValue = value;
    },

    get_HorizontalTotalCalculatorExtenderIdValue: function () {
        return this._HorizontalTotalCalculatorExtenderIdValue;
    },

    set_HorizontalTotalCalculatorExtenderIdValue: function (value) {
        this._HorizontalTotalCalculatorExtenderIdValue = value;
    },

    get_VerticalTotalCalculatorExtenderIdValue: function () {
        return this._VerticalTotalCalculatorExtenderIdValue;
    },

    set_VerticalTotalCalculatorExtenderIdValue: function (value) {
        this._VerticalTotalCalculatorExtenderIdValue = value;
    },

    get_SpreadSheetExtenderIdValue: function () {
        return this._SpreadSheetExtenderIdValue;
    },

    set_SpreadSheetExtenderIdValue: function (value) {
        this._SpreadSheetExtenderIdValue = value;
    },

    get_NoteIdValue: function () {
        return this._NoteIdValue;
    },

    set_NoteIdValue: function (value) {
        this._NoteIdValue = value;
    },

    get_TimeTypeDropdownIdValue: function () {
        return this._TimeTypeDropdownIdValue;
    },
    set_TimeTypeDropdownIdValue: function (value) {
        this._TimeTypeDropdownIdValue = value;
    },
    get_TimeTypeDropdown: function () {
        return $get(this._TimeTypeDropdownIdValue);
    },
    get_ProjectMilestoneDropdownIdValue: function () {
        return this._ProjectMilestoneDropdownIdValue;
    },

    set_ProjectMilestoneDropdownIdValue: function (value) {
        this._ProjectMilestoneDropdownIdValue = value;
    },
    get_ProjectMilestoneDropdown: function () {
        return $get(this._ProjectMilestoneDropdownIdValue);
    },

    isValidNote: function () {
        if (!this._isDirty())
            return true;
        var note = this._note();
        if (note.trim().length == 0) {
            if (this._hours().trim().length == 0)
                return true;
            else
                return false;
        }

        return note.length >= 3 && note.length <= 1000;
    },

    isValidHours: function () {
        if (!this._isDirty())
            return true;
        var hours = this._hours();

        if (hours.trim().length == 0) {
            if (this._note().trim().length == 0)
                return true;
            else
                return false;
        }

        var parsedHrs = Number.parseInvariant(hours);
        return parsedHrs >= 0.0 && parsedHrs <= 24.00;
    },

    showErrorMessageColor: function () {
        this._setBackground('red');
    },

    clearData: function () {
        this._clearData();
    },

    clearNotes: function () {
        this._clearNotes();
    },
    makeDirty: function () {
        this._makeDirty();
    },
    cancelBackground: function () {
        this._setBackground('none');
    },

    _setBackground: function (bgcolor) {
        $get(this.get_ActualHoursIdValue()).style.background = bgcolor;
    },

    _clearData: function () {
        if (!$get(this.get_NoteIdValue()).disabled) {

            $get(this.get_ActualHoursIdValue()).value = '';
            $get(this.get_NoteIdValue()).value = '';
            $get(this.get_IsChargeableIdValue()).checked = $get(this.get_HiddenDefaultIsChargeableIdValue()).value == 'true';
            $get(this.get_IsCorrectIdValue()).checked = false;

            var horizontalExtenderId = $get(this.get_HorizontalTotalCalculatorExtenderIdValue()).value;
            var verticalExtenderId = $get(this.get_VerticalTotalCalculatorExtenderIdValue()).value;
            var spreadSheetExtenderId = $get(this.get_SpreadSheetExtenderIdValue()).value;
            if (horizontalExtenderId != '') {
                $find(horizontalExtenderId).update();
            }
            $find(verticalExtenderId).update();
            $find(spreadSheetExtenderId).update();
            if ($get(this.get_HiddenActualHoursIdValue()).value != '') {
                this.get_element().value = "dirty";
                setDirty();
                EnableSaveButton(true);
            }
            else {
                this.get_element().value = '';
            }
            this.cancelBackground();
        }
    },

    _makeDirty: function () {
        if ($get(this.get_HiddenActualHoursIdValue()).value == ''
            && $get(this.get_ActualHoursIdValue()).value == ''
            && $get(this.get_NoteIdValue()).value == ''
            && $get(this.get_IsChargeableIdValue()).checked.toString() == $get(this.get_HiddenDefaultIsChargeableIdValue()).value
            && $get(this.get_IsCorrectIdValue()).checked.toString() != "true") {
            this.get_element().value = '';
        }
        else {
            this.get_element().value = "dirty";
            setDirty();
            EnableSaveButton(true);
        }
    },
    _clearNotes: function () {
        if (!$get(this.get_NoteIdValue()).disabled) {
            $get(this.get_NoteIdValue()).value = $get(this.get_HiddenNoteIdValue()).value;
            $get(this.get_IsChargeableIdValue()).checked = $get(this.get_HiddenIsChargeableIdValue()).value == "true";
            $get(this.get_IsCorrectIdValue()).checked = $get(this.get_HiddenIsCorrectIdValue()).value == "true";
            if ($get(this.get_ActualHoursIdValue()).value == '' && $get(this.get_HiddenNoteIdValue()).value == '') this.cancelBackground();
        }
    }
}
PraticeManagement.Controls.Generic.DirtyState.DirtyBehavior.registerClass('PraticeManagement.Controls.Generic.DirtyState.DirtyBehaviour', AjaxControlToolkit.BehaviorBase);

