﻿Type.registerNamespace("PraticeManagement.Controls.Generic.DuplicateOptionsRemove");
var WorkTypeDropDownListOptions = new Array();
PraticeManagement.Controls.Generic.DuplicateOptionsRemove.DuplicateOptionsRemoveBehavior = function (element) {
    PraticeManagement.Controls.Generic.DuplicateOptionsRemove.DuplicateOptionsRemoveBehavior.initializeBase(this, [element]);
}
PraticeManagement.Controls.Generic.DuplicateOptionsRemove.DuplicateOptionsRemoveBehavior.prototype = {
    initialize: function () {
        PraticeManagement.Controls.Generic.DuplicateOptionsRemove.DuplicateOptionsRemoveBehavior.callBaseMethod(this, 'initialize');
        var controlIds = this.getControlIdList();
        for (i = 0; i < controlIds.length; i++) {
            var control = document.getElementById(controlIds[i]);
            if (control) {
                $addHandlers(
                                control,
                                {
                                    'change': this._onChange
                                },
                                this);
            }
        }
        this.saveOptions();
        this.update();
    },
    dispose: function () {
        // Cleanup code
        PraticeManagement.Controls.Generic.DuplicateOptionsRemove.DuplicateOptionsRemoveBehavior.callBaseMethod(this, 'dispose');
    },
    _onChange: function () {
        this.update();
    },
    saveOptions: function () {
        var controlIds = this.getControlIdList();
        for (i = 0; i < controlIds.length; i++) {
            var control = document.getElementById(controlIds[i]);
            if (control) {
                var items = control.getElementsByTagName("option");
                var ddlOptionList = new Array();
                for (i = 0; i < items.length; i++) {
                    var opt = new Option(items[i].text, items[i].value);
                    Array.add(ddlOptionList, opt);
                }
                ddlOptionList.sort(this.compareOptionText);
                var target = this.get_element();
                WorkTypeDropDownListOptions[target.id] = ddlOptionList;
            }
        }
    },
    update: function () {
        var ddlSelectedValList = new Array();
        var controlIds = this.getControlIdList();
        for (i = 0; i < controlIds.length; i++) {
            var control = document.getElementById(controlIds[i]);
            if (control && control.value > -1) {
                Array.add(ddlSelectedValList, control.value);
            }
        }
        for (i = 0; i < controlIds.length; i++) {
            var control = document.getElementById(controlIds[i]);
            if (control) {
                var selectedVal = control.value;
                control.options.length = 0;
                var target = this.get_element();
                var optionList = WorkTypeDropDownListOptions[target.id];
                for (var j = 0; j < optionList.length; j++) {
                    var addOption = true;
                    for (var k = 0; k < ddlSelectedValList.length; k++) {
                        if (ddlSelectedValList[k] == optionList[j].value && ddlSelectedValList[k] != selectedVal) {
                            addOption = false;
                            break;
                        }
                    }
                    if (addOption) {
                        control.add(new Option(optionList[j].text, optionList[j].value));
                    }
                }
                control.value = selectedVal;
            }
        }
    },
    compareOptionText: function (a, b) {
        return a.text != b.text ? a.text < b.text ? -1 : 1 : 0;
    },
    sortOptions: function (list) {
        var items = list.options.length;
        // create array and make copies of options in list
        var tmpArray = new Array(items);
        for (i = 0; i < items; i++)
            tmpArray[i] = new Option(list.options[i].text, list.options[i].value);
        // sort options using given function
        tmpArray.sort(this.compareOptionText);
        // make copies of sorted options back to list
        for (i = 0; i < items; i++)
            list.options[i] = new Option(tmpArray[i].text, tmpArray[i].value);
    },
    getControlIdList: function () {
        var contolsToCheckList = this.controlsToCheck;
        if (contolsToCheckList)
            return contolsToCheckList.split(';');
        else
            return [];
    }
}
PraticeManagement.Controls.Generic.DuplicateOptionsRemove.DuplicateOptionsRemoveBehavior.registerClass('PraticeManagement.Controls.Generic.DuplicateOptionsRemove.DuplicateOptionsRemoveBehavior', AjaxControlToolkit.BehaviorBase);

