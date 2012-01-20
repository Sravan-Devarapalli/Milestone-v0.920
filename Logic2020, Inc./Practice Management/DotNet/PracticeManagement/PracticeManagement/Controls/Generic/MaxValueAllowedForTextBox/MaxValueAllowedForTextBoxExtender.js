Type.registerNamespace("PraticeManagement.Controls.Generic.MaxValueAllowedForTextBox");

PraticeManagement.Controls.Generic.MaxValueAllowedForTextBox.MaxValueAllowedForTextBoxExtender = function (element) {
    PraticeManagement.Controls.Generic.MaxValueAllowedForTextBox.MaxValueAllowedForTextBoxExtender.initializeBase(this, [element]);
}

PraticeManagement.Controls.Generic.MaxValueAllowedForTextBox.MaxValueAllowedForTextBoxExtender.prototype = {
    initialize: function () {
        PraticeManagement.Controls.Generic.MaxValueAllowedForTextBox.MaxValueAllowedForTextBoxExtender.callBaseMethod(this, 'initialize');
        var controlIds = this.getControlIdList();
        for (i = 0; i < controlIds.length; i++) {
            var control = document.getElementById(controlIds[i]);
            if (control) {
                $addHandlers(
                     control,
                     {
                         'keypress': this._keypress
                     },
                     this);
            }
        }
    },

    dispose: function () {
        // Cleanup code
        PraticeManagement.Controls.Generic.MaxValueAllowedForTextBox.MaxValueAllowedForTextBoxExtender.callBaseMethod(this, 'dispose');
    },
    _keypress: function (eventElement) {
        this.validate(eventElement)
    },
    validate: function (eventElement) {
        var txtbox = eventElement.target;
        var previousVal = txtbox.value.toString();
        txtbox.setAttribute('previousvalue', previousVal);
        var character = String.fromCharCode(eventElement.charCode);
        if (isNaN(Number(character))) {
            if (character == ".") {

                for (var i = 0; i < previousVal.length; i++) {
                    if (previousVal[i] == ".") {
                        eventElement.preventDefault();
                        return;
                    }
                }

            }
            
        }
    },
    getControlIdList: function () {
        var contolsToCheckList = this.controlsToCheck;
        if (contolsToCheckList)
            return contolsToCheckList.split(';');
        else
            return [];
    },
    getMaximumValue: function () {
        return this.maximumValue;
    }
}

PraticeManagement.Controls.Generic.MaxValueAllowedForTextBox.MaxValueAllowedForTextBoxExtender.registerClass('PraticeManagement.Controls.Generic.MaxValueAllowedForTextBox.MaxValueAllowedForTextBoxExtender', AjaxControlToolkit.BehaviorBase);

