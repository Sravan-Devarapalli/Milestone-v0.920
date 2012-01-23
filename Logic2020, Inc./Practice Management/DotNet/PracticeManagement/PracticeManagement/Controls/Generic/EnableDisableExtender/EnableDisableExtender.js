Type.registerNamespace("PraticeManagement.Controls.Generic.EnableDisableExtender");

PraticeManagement.Controls.Generic.EnableDisableExtender.EnableDisableExtenderBehavior = function (element) {
    PraticeManagement.Controls.Generic.EnableDisableExtender.EnableDisableExtenderBehavior.initializeBase(this, [element]);
}

PraticeManagement.Controls.Generic.EnableDisableExtender.EnableDisableExtenderBehavior.prototype = {
    initialize: function () {
        PraticeManagement.Controls.Generic.EnableDisableExtender.EnableDisableExtenderBehavior.callBaseMethod(this, 'initialize');

        $addHandlers(this.get_element(),
                     {
                         'change': this._onChange
                     },
                     this);
        this._onChange();

    },

    dispose: function () {
        // Cleanup code 
        PraticeManagement.Controls.Generic.EnableDisableExtender.EnableDisableExtenderBehavior.callBaseMethod(this, 'dispose');
    },
     getControlIdList: function () {
        var contolsToCheckList = this.controlsToCheck;
        if (contolsToCheckList)
            return contolsToCheckList.split(';');
        else
            return [];
    },

    setEnableDisableTextBox: function (disable) {
        var controlIds = this.getControlIdList();
        for (i = 0; i < controlIds.length; i++) {
            var control = document.getElementById(controlIds[i]);
            var Isdisable = 0;
            if (control != null && control.getAttribute('disable') != undefined && control.getAttribute('disable') != null) {
                Isdisable = control.getAttribute('disable');
            }
            if (control != null) {
                if (disable == 1) {
                    control.setAttribute('disabled', 'disabled');
                    if (control.value == '')
                        control.style.backgroundColor = 'gray';
                }
                else {
                    if (Isdisable == 0) {
                        control.removeAttribute('disabled');
                        control.style.backgroundColor = 'white';
                    } else {
                        control.setAttribute('disabled', 'disabled');
                        if (control.value == '')
                            control.style.backgroundColor = 'gray';
                    }

                }
            }
        }
    },
    _onChange: function () {
        var target = this.get_element();
        if (target != null) {
            if (target.value == -1) {
                this.setEnableDisableTextBox(1);
            } else {
                this.setEnableDisableTextBox(0);
            }
        }
    }
}

PraticeManagement.Controls.Generic.EnableDisableExtender.EnableDisableExtenderBehavior.registerClass('PraticeManagement.Controls.Generic.EnableDisableExtender.EnableDisableExtenderBehavior', AjaxControlToolkit.BehaviorBase);
