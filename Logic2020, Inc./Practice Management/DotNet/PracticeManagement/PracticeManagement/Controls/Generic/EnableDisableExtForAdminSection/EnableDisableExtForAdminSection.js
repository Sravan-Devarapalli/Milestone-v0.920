Type.registerNamespace("PraticeManagement.Controls.Generic.EnableDisableExtForAdminSection");

PraticeManagement.Controls.Generic.EnableDisableExtForAdminSection.EnableDisableExtForAdminSectionBehavior = function (element) {
    PraticeManagement.Controls.Generic.EnableDisableExtForAdminSection.EnableDisableExtForAdminSectionBehavior.initializeBase(this, [element]);
    this.controlToDisableId = null;
    this._isControlWasDisabled = false;
}

PraticeManagement.Controls.Generic.EnableDisableExtForAdminSection.EnableDisableExtForAdminSectionBehavior.prototype = {
    initialize: function () {
        PraticeManagement.Controls.Generic.EnableDisableExtForAdminSection.EnableDisableExtForAdminSectionBehavior.callBaseMethod(this, 'initialize');

        var hoursControlIds = this.getControlIdList(this.hoursControlsToCheck);
        for (var i = 0; i < hoursControlIds.length; i++) {
            var control = document.getElementById(hoursControlIds[i]);
            if (control) {
                $addHandlers(
                     control,
                     {
                         'change': this._onChange
                     },
                     this);
            }
        }

        var notesControlIds = this.getControlIdList(this.notesControlsToCheck);
        for (var i = 0; i < notesControlIds.length; i++) {
            var control = document.getElementById(notesControlIds[i]);
            if (control) {
                $addHandlers(
                     control,
                     {
                         'change': this._onChange
                     },
                     this);
            }
        }

        var managersControlIds = this.getControlIdList(this.managersControlsToCheck);
        for (var i = 0; i < managersControlIds.length; i++) {
            var control = document.getElementById(managersControlIds[i]);
            if (control) {
                $addHandlers(
                control,
                {
                    'change': this._onChange
                },
                this);
            }
        }

        var deleteControlIds = this.getControlIdList(this.deleteControlsToCheck);
        for (var i = 0; i < deleteControlIds.length; i++) {
            var control = document.getElementById(deleteControlIds[i]);
            if (control) {
                $addHandlers(
                     control,
                     {
                         'mousedown': this._onMousedown
                     },
                     this);
            }
        }

        var closeControlIds = this.getControlIdList(this.closeControlsToCheck);
        for (var i = 0; i < closeControlIds.length; i++) {
            var control = document.getElementById(closeControlIds[i]);
            if (control) {
                $addHandlers(
                     control,
                     {
                         'click': this._onCloseClick
                     },
                     this);
            }
        }


        this._onChange();
    },

    dispose: function () {
        // Cleanup code 
        PraticeManagement.Controls.Generic.EnableDisableExtForAdminSection.EnableDisableExtForAdminSectionBehavior.callBaseMethod(this, 'dispose');
    },

    getControlIdList: function (controlsToCheck) {
        var contolsToCheckList = controlsToCheck;
        if (contolsToCheckList)
            return contolsToCheckList.split(';');
        else
            return [];
    },
    isHoursControlId: function (targetId) {
        var hoursControlIds = this.getControlIdList(this.hoursControlsToCheck);
        for (var i = 0; i < hoursControlIds.length; i++) {
            if (hoursControlIds[i] == targetId) {
                return true;
            }
        }
        return false;
    },
    isManagersControlId: function (targetId) {
        var managersControlIds = this.getControlIdList(this.managersControlsToCheck);
        for (var i = 0; i < managersControlIds.length; i++) {
            if (managersControlIds[i] == targetId) {
                return true;
            }
        }
        return false;
    },
    getTargetControl: function () {
        var hoursControlIds = this.getControlIdList(this.hoursControlsToCheck);
        for (var i = 0; i < hoursControlIds.length; i++) {
            var targetAcutalHours = document.getElementById(hoursControlIds[i]);
            if (targetAcutalHours != null && targetAcutalHours.value != '') {
                return targetAcutalHours;
            }
        }

        var notesControlIds = this.getControlIdList(this.notesControlsToCheck);
        for (var i = 0; i < notesControlIds.length; i++) {
            var targetNotes = document.getElementById(notesControlIds[i]);
            if (targetNotes != null && targetNotes.value != '') {
                return targetNotes;
            }
        }

        var managersControlIds = this.getControlIdList(this.managersControlsToCheck);
        for (var i = 0; i < managersControlIds.length; i++) {
            var targetManagerList = document.getElementById(managersControlIds[i]);
            if (targetManagerList != null && targetManagerList.value != '') {
                return targetManagerList;
            }
        }
        return null;
    },
    getTargetControlIndex: function (targetId) {
        var hoursControlIds = this.getControlIdList(this.hoursControlsToCheck);
        for (var i = 0; i < hoursControlIds.length; i++) {
            if (hoursControlIds[i] == targetId) {
                return i;
            }
        }
        var notesControlIds = this.getControlIdList(this.notesControlsToCheck);
        for (var i = 0; i < notesControlIds.length; i++) {
            var targetNotes = document.getElementById(notesControlIds[i]);
            if (notesControlIds[i] == targetId) {
                return i;
            }
        }
        var managersControlIds = this.getControlIdList(this.managersControlsToCheck);
        for (var i = 0; i < managersControlIds.length; i++) {
            if (managersControlIds[i] == targetId) {
                return i;
            }
        }
        return null;
    },
    _onMousedown: function () {
        var target = this.getTargetControl();
        if (target != null) {
            this.enableAllColtrols();
        }
    },
    disableOtherColtrols: function (index) {
        var hoursControlIds = this.getControlIdList(this.hoursControlsToCheck);
        for (var i = 0; i < hoursControlIds.length; i++) {

            var targetAcutalHours = document.getElementById(hoursControlIds[i]);
            if (targetAcutalHours != null) {
                if (i != index) {
                    targetAcutalHours.setAttribute('readonly', 'readonly');
                }
                else {
                    targetAcutalHours.removeAttribute('readonly');
                }
            }
        }
        var notesControlIds = this.getControlIdList(this.notesControlsToCheck);
        for (var i = 0; i < notesControlIds.length; i++) {
            var targetNotes = document.getElementById(notesControlIds[i]);
            if (targetNotes != null) {
                if (i != index)
                    targetNotes.setAttribute('readonly', 'readonly');
                else
                    targetNotes.removeAttribute('readonly');
            }
        }
        var managersControlIds = this.getControlIdList(this.managersControlsToCheck);
        for (var i = 0; i < managersControlIds.length; i++) {
            var targetManagerList = document.getElementById(managersControlIds[i]);
            if (targetManagerList != null) {
                if (i != index)
                    targetManagerList.setAttribute('disabled', 'disabled');
                else
                    targetManagerList.removeAttribute('disabled');
            }
        }
        var deleteControlIds = this.getControlIdList(this.deleteControlsToCheck);
        for (var i = 0; i < deleteControlIds.length; i++) {
            var targetDelete = document.getElementById(deleteControlIds[i]);
            if (targetDelete != null) {
                if (i != index)
                    targetDelete.setAttribute('disabled', 'disabled');
                else
                    targetDelete.removeAttribute('disabled');
            }
        }


    },
    enableAllColtrols: function () {
        var hoursControlIds = this.getControlIdList(this.hoursControlsToCheck);
        for (var i = 0; i < hoursControlIds.length; i++) {
            var targetAcutalHours = document.getElementById(hoursControlIds[i]);
            if (targetAcutalHours != null) {
                targetAcutalHours.removeAttribute('readonly');
            }
        }
        var notesControlIds = this.getControlIdList(this.notesControlsToCheck);
        for (var i = 0; i < notesControlIds.length; i++) {
            var targetNotes = document.getElementById(notesControlIds[i]);
            if (targetNotes != null) {
                targetNotes.removeAttribute('readonly');
            }
        }
        var managersControlIds = this.getControlIdList(this.managersControlsToCheck);
        for (var i = 0; i < managersControlIds.length; i++) {
            var targetManagerList = document.getElementById(managersControlIds[i]);
            if (targetManagerList != null) {
                targetManagerList.removeAttribute('disabled');
            }
        }
        var deleteControlIds = this.getControlIdList(this.deleteControlsToCheck);
        for (var i = 0; i < deleteControlIds.length; i++) {
            var targetDelete = document.getElementById(deleteControlIds[i]);
            if (targetDelete != null) {
                targetDelete.removeAttribute('disabled');
            }
        }
    },
    _onCloseClick: function () {

        var target = this.getTargetControl();
        if (target != null) {
            var index = this.getTargetControlIndex(target.id);
            var hiddenNotesControlIds = this.getControlIdList(this.hiddenNotesControlsToCheck);
            var hiddenManagersControlIds = this.getControlIdList(this.hiddenManagersControlsToCheck);
            var hoursControlIds = this.getControlIdList(this.hoursControlsToCheck);
            var hiddenNotesControl = document.getElementById(hiddenNotesControlIds[index]);
            var hiddenManagerListControl = document.getElementById(hiddenManagersControlIds[index]);
            var hoursControl = document.getElementById(hoursControlIds[index]);

            if (hiddenNotesControl != null && hoursControl != null && hiddenManagerListControl != null) {
                if (hiddenNotesControl.value == '' && hoursControl.value == '' && hiddenManagerListControl.value == '') {
                    //notes not save roll back the onchange event and enable all  
                    this.enableAllColtrols();
                }
            }
        }

    },
    _onChange: function () {

        var target = this.getTargetControl();
        if (target != null) {

            var isHourTextBox = this.isHoursControlId(target.id);
            var isManagerControl = this.isManagersControlId(target.id);
            var targetAcutalHoursHiddenField = document.getElementById(this.targetAcutalHoursHiddenFieldId);
            var targetNotesHiddenField = document.getElementById(this.targetNotesHiddenFieldId);
            var targetManagersHiddenField = document.getElementById(this.targetManagersHiddenFieldId);

            var targetAcutalHours = null;
            var targetNotes = null;
            var targetManager = null;

            if (targetAcutalHoursHiddenField.value != '')
                var targetAcutalHours = document.getElementById(targetAcutalHoursHiddenField.value);

            if (targetNotesHiddenField.value != '')
                var targetNotes = document.getElementById(targetNotesHiddenField.value);

            if (targetManagersHiddenField.value != '')
                targetManager = document.getElementById(targetManagersHiddenField.value);

            if ((targetAcutalHours == null || targetAcutalHours == undefined) && isHourTextBox && !isManagerControl) {
                targetAcutalHoursHiddenField.value = target.id;
                targetAcutalHours = document.getElementById(target.id);
            }
            if ((targetNotes == null || targetNotes == undefined) && !isHourTextBox && !isManagerControl) {
                targetNotesHiddenField.value = target.id;
                targetNotes = document.getElementById(target.id);
            }
            if ((targetManager == null || targetManager == undefined) && !isHourTextBox && isManagerControl) {
                targetManagersHiddenField.value = target.id;
                targetManager = document.getElementById(target.id);
            }

            if (targetAcutalHours != null || targetNotes != null || targetManager != null) {
                var index = this.getTargetControlIndex(target.id);
                this.disableOtherColtrols(index);
            }
        }
    }

}

PraticeManagement.Controls.Generic.EnableDisableExtForAdminSection.EnableDisableExtForAdminSectionBehavior.registerClass('PraticeManagement.Controls.Generic.EnableDisableExtForAdminSection.EnableDisableExtForAdminSectionBehavior', AjaxControlToolkit.BehaviorBase);

