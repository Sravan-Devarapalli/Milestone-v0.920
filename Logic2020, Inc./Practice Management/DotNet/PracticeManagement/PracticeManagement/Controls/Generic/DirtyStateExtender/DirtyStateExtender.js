Type.registerNamespace("PraticeManagement.Controls.Generic.DirtyStateExtender");

PraticeManagement.Controls.Generic.DirtyStateExtender.DirtyStateBehavior = function (element) {
    PraticeManagement.Controls.Generic.DirtyStateExtender.DirtyStateBehavior.initializeBase(this, [element]);

    this._NoteIdValue = null;
    this._HiddenNoteIdValue = null;
    this._ActualHoursIdValue = null;
    this._HiddenActualHoursIdValue = null;
    this._HorizontalTotalCalculatorExtenderIdValue = null;
    this._VerticalTotalCalculatorExtenderIdValue = null;
    this._SpreadSheetExtenderIdValue = null;
    this._IsNoteRequired = null;
    this._ApprovedManagersIdValue = null;
    this._HiddenApprovedManagersIdValue = null;
}

PraticeManagement.Controls.Generic.DirtyStateExtender.DirtyStateBehavior.prototype = {
    _initBase: function () {
        PraticeManagement.Controls.Generic.DirtyStateExtender.DirtyStateBehavior.callBaseMethod(this, 'initialize');
    },
    initialize: function () {
        this._initBase();

        //  On clicking the label, call the corresponding function
        $addHandler($get(this.get_ActualHoursIdValue()), 'change', Function.createDelegate(this, this._setDirty));
    },
    dispose: function () {
        //Add custom dispose actions here
        PraticeManagement.Controls.Generic.DirtyStateExtender.DirtyStateBehavior.callBaseMethod(this, 'dispose');
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
        this._upDateTotals();

        this._setBackground('gold');

        var note = new String($get(this.get_NoteIdValue()).value);
        var hours = new String($get(this.get_ActualHoursIdValue()).value);
        var approvedManagerId = this.get_ApprovedManagersIdValue() == null ? '' : ($get(this.get_ApprovedManagersIdValue()) == null ? '' : new String($get(this.get_ApprovedManagersIdValue()).value));

        var stuff = new String(note + hours + approvedManagerId);

        if (stuff.trim().length == 0)
            this.get_element().value = "";
        else
            this.get_element().value = "dirty";
    },
    _upDateTotals: function () {

        var spreadSheetExtenderId = $get(this.get_SpreadSheetExtenderIdValue()).value;
        if (spreadSheetExtenderId != '') {

            var spreadSheetExtenderIdArray = spreadSheetExtenderId.split(';');
            for (var i = 0; i < spreadSheetExtenderIdArray.length; i++) {
                var clientId = spreadSheetExtenderIdArray[i];
                if (clientId != '')
                    $find(clientId).update();
            }

        }

    },
    _checkDirty: function () {
        var note = new String($get(this.get_NoteIdValue()).value);
        var hours = new String($get(this.get_ActualHoursIdValue()).value);
        var approvedManagerId = this.get_ApprovedManagersIdValue() == null ? '' : ($get(this.get_ApprovedManagersIdValue()) == null ? '' : new String($get(this.get_ApprovedManagersIdValue()).value));
        var hdnApprovedManagerId = this.get_HiddenApprovedManagersIdValue() == null ? '' : ($get(this.get_HiddenApprovedManagersIdValue()) == null ? '' : new String($get(this.get_HiddenApprovedManagersIdValue()).value));

        var stuff = new String(note + hours + approvedManagerId);

        if (stuff.trim().length == 0)
            this.get_element().value = "";
        else if (note == $get(this.get_HiddenNoteIdValue()).value && approvedManagerId == hdnApprovedManagerId) {

            this.get_element().value = "";
        }
        else {
            this._setBackground('gold');
            this.get_element().value = "dirty";
        }
    },
    checkDirty: function () {
        this._checkDirty();
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
    set_IsNoteRequired: function (value) {
        this._IsNoteRequired = value;
    },
    get_IsNoteRequired: function () {
        return this._IsNoteRequired;
    },

    get_ApprovedManagersIdValue: function () {
        return this._ApprovedManagersIdValue;
    },

    set_ApprovedManagersIdValue: function (value) {
        this._ApprovedManagersIdValue = value;
    },

    get_HiddenApprovedManagersIdValue: function () {
        return this._HiddenApprovedManagersIdValue;
    },

    set_HiddenApprovedManagersIdValue: function (value) {
        this._HiddenApprovedManagersIdValue = value;
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
        var actualHours = $get(this.get_ActualHoursIdValue());
        if (!actualHours.disabled)
            actualHours.style.background = bgcolor;
    },

    _clearData: function () {
        var actualHours = $get(this.get_ActualHoursIdValue());
        var isPTO = actualHours.getAttribute('IsPTO') == null ? false : actualHours.getAttribute('IsPTO').toString().toLowerCase();
        var isLockoutDelete = actualHours.getAttribute('IsLockoutDelete');
        
        if(isLockoutDelete == 1){
            
        }
        else if ((!$get(this.get_NoteIdValue()).disabled && !actualHours.disabled) || (isPTO && !actualHours.disabled)) {

            actualHours.value = '';
            $get(this.get_NoteIdValue()).value = '';
            if (this.get_ApprovedManagersIdValue() != null && $get(this.get_ApprovedManagersIdValue()) != null) {
                $get(this.get_ApprovedManagersIdValue())[0].selected = true;
            }

            var horizontalExtenderId = $get(this.get_HorizontalTotalCalculatorExtenderIdValue()).value;
            var verticalExtenderId = $get(this.get_VerticalTotalCalculatorExtenderIdValue()).value;


            if (horizontalExtenderId != '') {
                $find(horizontalExtenderId).update();
            }

            if (verticalExtenderId != '') {
                $find(verticalExtenderId).update();
            }

            this._upDateTotals();

            if ($get(this.get_HiddenActualHoursIdValue()).value != '') {
                this.get_element().value = "dirty";
                setDirty();
                EnableSaveButton(true);
            }
            else {
                this.get_element().value = '';
            }
            this.cancelBackground();
            this._setBackground('white');
        }
    },

    _makeDirty: function () {
        var approvedManager = this.get_ApprovedManagersIdValue() == null ? '' : ($get(this.get_ApprovedManagersIdValue()) == null ? '' : $get(this.get_ApprovedManagersIdValue()).value);

        if ($get(this.get_HiddenActualHoursIdValue()).value == ''
            && $get(this.get_ActualHoursIdValue()).value == ''
            && $get(this.get_NoteIdValue()).value == ''
            && approvedManager == '') {
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

            if (this.get_ApprovedManagersIdValue() != null && this.get_ApprovedManagersIdValue() != "" && $get(this.get_ApprovedManagersIdValue())) {
                if ($get(this.get_HiddenApprovedManagersIdValue()).value == '') {
                    $get(this.get_ApprovedManagersIdValue())[0].selected = true;
                }
                else {
                    $get(this.get_ApprovedManagersIdValue()).value = $get(this.get_HiddenApprovedManagersIdValue()).value;
                }
            }

            if ($get(this.get_ActualHoursIdValue()).value == '' && $get(this.get_HiddenNoteIdValue()).value == '') this.cancelBackground();

        }
    }
}
PraticeManagement.Controls.Generic.DirtyStateExtender.DirtyStateBehavior.registerClass('PraticeManagement.Controls.Generic.DirtyStateExtender.DirtyStateBehaviour', AjaxControlToolkit.BehaviorBase);

