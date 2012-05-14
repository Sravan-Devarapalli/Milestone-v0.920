﻿Type.registerNamespace("PraticeManagement.Controls.Generic.EnableDisableExtender");
var obj = null;
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

        this._initializeEnableDisableTextBoxes();
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
    showPopUp: function () {
        $find(this.popUpBehaviourId).show();
    },

    _initializeEnableDisableTextBoxes: function () {
        var target = this.get_element();
        if (target != null) {
            if (target.value == -1) {
                this._disableTextBoxes();
            }
            else {
                var controlIds = this.getControlIdList();
                for (var i = 0; i < controlIds.length; i++) {
                    var control = document.getElementById(controlIds[i]);
                    if (control != null) {
                        var isHourlyRevenueDisable = 0;
                        var isChargeCodeTurnOffDisable = 0;
                        var isTerminationDateDisable = 0;
                        var isPersonSalaryTypeDisable = 0;
                        var isHireDateDisable = 0;
                        if (control != null) {
                            if (control.getAttribute('isHourlyRevenueDisable') != undefined && control.getAttribute('isHourlyRevenueDisable') != null) {
                                isHourlyRevenueDisable = control.getAttribute('isHourlyRevenueDisable');
                            }
                            if (control.getAttribute('isChargeCodeTurnOffDisable') != undefined && control.getAttribute('isChargeCodeTurnOffDisable') != null) {
                                isChargeCodeTurnOffDisable = control.getAttribute('isChargeCodeTurnOffDisable');
                            }
                            if (control.getAttribute('IsHireDateDisable') != undefined && control.getAttribute('IsHireDateDisable') != null) {
                                isHireDateDisable = control.getAttribute('IsHireDateDisable');
                            }
                            if (control.getAttribute('IsTerminationDateDisable') != undefined && control.getAttribute('IsTerminationDateDisable') != null) {
                                isTerminationDateDisable = control.getAttribute('IsTerminationDateDisable');
                            }
                            if (control.getAttribute('IsPersonSalaryTypeDisable') != undefined && control.getAttribute('IsPersonSalaryTypeDisable') != null) {
                                isPersonSalaryTypeDisable = control.getAttribute('IsPersonSalaryTypeDisable');
                            }
                        }
                        if (isHourlyRevenueDisable == 0 && isChargeCodeTurnOffDisable == 0 && isHireDateDisable == 0 && isTerminationDateDisable == 0 && isPersonSalaryTypeDisable == 0) {
                            if (control.getAttribute('disabled') == 'disabled') {
                                control.removeAttribute('disabled');
                                control.style.backgroundColor = 'white';
                            }
                        } else {
                            control.setAttribute('disabled', 'disabled');
                            if (control.value == '')
                                control.style.backgroundColor = 'gray';
                        }
                    }
                }
            }
        }
    },
    _checkEnableDisableTextBoxes: function (isChargeCodeTurnOffJsonString) {
        var isChargeCodeTurnOffJson = jQuery.parseJSON(isChargeCodeTurnOffJsonString);
        var showPopup = false;
        var controlIds = obj.getControlIdList();
        var isNonBillable = true;
        if (controlIds.length >= 14) {
            isNonBillable = false;
        }
        var j = 0;
        for (var i = 0; i < 7; i++) {
            var isChargeCodeTurnOffJsonObject = isChargeCodeTurnOffJson[i];
            var isNewchargeCodeTurnOff = isChargeCodeTurnOffJsonObject.IschargeCodeTurnOff == "true";
            if (isNonBillable) {
                var nonBillableControl = document.getElementById(controlIds[i]);
                var nonBillableControlHasValue = nonBillableControl.value != '';
                var nonBillableControlIsDirty = nonBillableControl.style.backgroundColor != 'white' && nonBillableControl.style.backgroundColor != 'gray';
                if (isNewchargeCodeTurnOff && (nonBillableControlIsDirty || nonBillableControlHasValue)) {
                    showPopup = true;
                }
            }
            else {
                var billableControl = document.getElementById(controlIds[j]);
                var nonBillableControl = document.getElementById(controlIds[j + 1]);
                var billableControlHasValue = billableControl.value != '';
                var billableControlIsDirty = billableControl.style.backgroundColor != 'white' && billableControl.style.backgroundColor != 'gray';
                var nonBillableControlHasValue = nonBillableControl.value != '';
                var nonBillableControlIsDirty = nonBillableControl.style.backgroundColor != 'white' && nonBillableControl.style.backgroundColor != 'gray';
                if (isNewchargeCodeTurnOff && (billableControlHasValue || billableControlHasValue || nonBillableControlIsDirty || nonBillableControlHasValue)) {
                    showPopup = true;
                }
                j = j + 2;
            }
        }
        if (!showPopup) {
            obj._enableTextBoxes(isChargeCodeTurnOffJson);
        } else {
            var target = obj.get_element();
            var previousId = target.getAttribute('previousId');
            target.value = previousId;
            obj.showPopUp();
        }
    },
    _enableTextBoxes: function (isChargeCodeTurnOffJson) {
        var controlIds = this.getControlIdList();
        var isNonBillable = true;
        if (controlIds.length >= 14) {
            isNonBillable = false;
        }
        var j = 0;
        for (var i = 0; i < 7; i++) {
            var isChargeCodeTurnOffJsonObject = isChargeCodeTurnOffJson[i];
            var ischargeCodeTurnOff = isChargeCodeTurnOffJsonObject.IschargeCodeTurnOff == "true";
            if (isNonBillable) {
                var nonBillableControl = document.getElementById(controlIds[i]);
                if (ischargeCodeTurnOff) {
                    nonBillableControl.setAttribute('isChargeCodeTurnOffDisable', 1);
                }
                else {
                    nonBillableControl.setAttribute('isChargeCodeTurnOffDisable', 0);
                }
            }
            else {
                var billableControl = document.getElementById(controlIds[j]);
                var nonBillableControl = document.getElementById(controlIds[j + 1]);
                j = j + 2;
                if (ischargeCodeTurnOff) {
                    billableControl.setAttribute('isChargeCodeTurnOffDisable', 1);
                    nonBillableControl.setAttribute('isChargeCodeTurnOffDisable', 1);
                }
                else {
                    billableControl.setAttribute('isChargeCodeTurnOffDisable', 0);
                    nonBillableControl.setAttribute('isChargeCodeTurnOffDisable', 0);
                }

            }
        }

        for (var i = 0; i < controlIds.length; i++) {
            var control = document.getElementById(controlIds[i]);

            if (control != null) {
                var isHourlyRevenueDisable = 0;
                var isChargeCodeTurnOffDisable = 0;
                var isTerminationDateDisable = 0;
                var isPersonSalaryTypeDisable = 0;
                var isHireDateDisable = 0;
                if (control != null) {
                    if (control.getAttribute('isHourlyRevenueDisable') != undefined && control.getAttribute('isHourlyRevenueDisable') != null) {
                        isHourlyRevenueDisable = control.getAttribute('isHourlyRevenueDisable');
                    }
                    if (control.getAttribute('isChargeCodeTurnOffDisable') != undefined && control.getAttribute('isChargeCodeTurnOffDisable') != null) {
                        isChargeCodeTurnOffDisable = control.getAttribute('isChargeCodeTurnOffDisable');
                    }
                    if (control.getAttribute('IsHireDateDisable') != undefined && control.getAttribute('IsHireDateDisable') != null) {
                        isHireDateDisable = control.getAttribute('IsHireDateDisable');
                    }
                    if (control.getAttribute('IsTerminationDateDisable') != undefined && control.getAttribute('IsTerminationDateDisable') != null) {
                        isTerminationDateDisable = control.getAttribute('IsTerminationDateDisable');
                    }
                    if (control.getAttribute('IsPersonSalaryTypeDisable') != undefined && control.getAttribute('IsPersonSalaryTypeDisable') != null) {
                        isPersonSalaryTypeDisable = control.getAttribute('IsPersonSalaryTypeDisable');
                    }
                }
                if (isHourlyRevenueDisable == 0 && isChargeCodeTurnOffDisable == 0 && isHireDateDisable == 0 && isTerminationDateDisable == 0 && isPersonSalaryTypeDisable == 0) {
                    if (control.getAttribute('disabled') == 'disabled') {
                        control.removeAttribute('disabled');
                        var controlIsDirty = control.style.backgroundColor != 'white' && control.style.backgroundColor != 'gray';
                        if (!controlIsDirty) {
                            control.style.backgroundColor = 'white';
                        }
                    }
                } else {
                    control.setAttribute('disabled', 'disabled');
                    if (control.value == '')
                        control.style.backgroundColor = 'gray';
                }
            }
        }
        var target = obj.get_element();
        target.setAttribute('previousId', target.value);
    },
    _disableTextBoxes: function () {
        var controlIds = this.getControlIdList();
        for (i = 0; i < controlIds.length; i++) {
            var control = document.getElementById(controlIds[i]);

            if (control) {
                control.setAttribute('disabled', 'disabled');
                var controlIsDirty = control.style.backgroundColor.toLowerCase() != 'white' && control.style.backgroundColor.toLowerCase() != 'gray';
                if (!controlIsDirty) {
                    control.style.backgroundColor = 'gray';
                }
            }
        }
        var target = this.get_element();
        target.setAttribute('previousId', target.value);
    },
    _getChargeCodeTurnOffDates: function (timeEntryId) {

        var urlVal = "TimeEntryHandler.ashx?accountId=" + this.accountId
                                            + "&projectId=" + this.projectId
                                            + "&businessUnitId=" + this.businessUnitId
                                            + "&timeEntryId=" + timeEntryId
                                            + "&weekStartDate=" + this.weekStartDate
                                            + "&personId=" + this.personId;
        obj = this;

        $.post(urlVal, this._checkEnableDisableTextBoxes);
    },
    _onChange: function () {

        var target = this.get_element();
        if (target != null) {
            if (target.value == -1) {
                this._disableTextBoxes();
            } else {
                this._getChargeCodeTurnOffDates(target.value);
            }

        }

    }
}

PraticeManagement.Controls.Generic.EnableDisableExtender.EnableDisableExtenderBehavior.registerClass('PraticeManagement.Controls.Generic.EnableDisableExtender.EnableDisableExtenderBehavior', AjaxControlToolkit.BehaviorBase);

