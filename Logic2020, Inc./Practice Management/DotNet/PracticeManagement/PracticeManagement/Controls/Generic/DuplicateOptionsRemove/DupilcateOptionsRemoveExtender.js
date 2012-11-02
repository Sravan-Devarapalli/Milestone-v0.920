Type.registerNamespace("PraticeManagement.Controls.Generic.DuplicateOptionsRemove");
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
        this.showOrHideApprovedBy();
    },
    dispose: function () {
        // Cleanup code
        PraticeManagement.Controls.Generic.DuplicateOptionsRemove.DuplicateOptionsRemoveBehavior.callBaseMethod(this, 'dispose');
    },
    _onChange: function () {
        this.update();
        this.showOrHideApprovedBy();
    },
    showOrHideApprovedBy: function () {

        var controlIds = this.getControlIdList();
        for (i = 0; i < controlIds.length; i++) {
            var control = document.getElementById(controlIds[i]);
            if (control) {
                var optionsList = control.options;
                for (var j = 0; j < optionsList.length; j++) {
                    var opt = optionsList[j];
                    if (opt.value == control.value) {
                        var jsonApprovedByClientIdsString = control.getAttribute('JsonApprovedByClientIds');
                        var approvedByClientIdsJson = jQuery.parseJSON(jsonApprovedByClientIdsString);

                        if (approvedByClientIdsJson != null) {
                            if (opt.getAttribute('IsORT') != null && opt.getAttribute('IsORT').toLowerCase() == 'true') {

                                for (var k = 0; k < approvedByClientIdsJson.length; k++) {
                                    (document.getElementById(approvedByClientIdsJson[k])).style.display = "";
                                }
                            }
                            else {
                                for (var k = 0; k < approvedByClientIdsJson.length; k++) {
                                    (document.getElementById(approvedByClientIdsJson[k])).style.display = "none";
                                }
                            }
                        }

                        break;
                    }
                }
            }
        }

    },
    saveOptions: function () {

        var controlIds = this.getControlIdList();
        for (i = 0; i < controlIds.length; i++) {
            var control = document.getElementById(controlIds[i]);
            if (control) {
                var items = control.getElementsByTagName("option");
                var inActiveOptionValue = control.getAttribute('selectedInActiveWorktypeid');
                var ddlOptionList = new Array();
                for (i = 0; i < items.length; i++) {
                    var opt = new Option(items[i].text, items[i].value);

                    if (items[i].getAttribute('IsORT') != null) {
                        opt.setAttribute('IsORT', items[i].getAttribute('IsORT'))
                    }

                    if (items[i].getAttribute('isW2HourlyTimeType') != null) {
                        opt.setAttribute('isW2HourlyTimeType', items[i].getAttribute('isW2HourlyTimeType'))
                    }

                    if (items[i].getAttribute('isW2SalaryTimeType') != null) {
                        opt.setAttribute('isW2SalaryTimeType', items[i].getAttribute('isW2SalaryTimeType'))
                    }

                    if (isNaN(inActiveOptionValue) || inActiveOptionValue == null) {
                        Array.add(ddlOptionList, opt);
                    } else {
                        if (inActiveOptionValue != items[i].value) {
                            Array.add(ddlOptionList, opt);
                        }
                    }
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
        var target = this.get_element();
        for (i = 0; i < controlIds.length; i++) {
            var control = document.getElementById(controlIds[i]);
            if (control && control.value > -1) {
                Array.add(ddlSelectedValList, control.value);
            }
        }
        for (i = 0; i < controlIds.length; i++) {
            var control = document.getElementById(controlIds[i]);
            if (control) {
                var inActiveOptionText = control.getAttribute('selectedInActiveWorktypeName');
                var inActiveOptionValue = control.getAttribute('selectedInActiveWorktypeid');

                var selectedVal = control.value;
                control.options.length = 0;

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
                        var opt = new Option(optionList[j].text, optionList[j].value);
                        if (optionList[j].getAttribute('IsORT') != null) {
                            opt.setAttribute('IsORT', optionList[j].getAttribute('IsORT'));
                        }
                        if (optionList[j].getAttribute('isW2HourlyTimeType') != null) {
                            opt.setAttribute('isW2HourlyTimeType', optionList[j].getAttribute('isW2HourlyTimeType'));
                        }
                        if (optionList[j].getAttribute('isW2SalaryTimeType') != null) {
                            opt.setAttribute('isW2SalaryTimeType', optionList[j].getAttribute('isW2SalaryTimeType'));
                        }
                        control.add(opt);
                    }
                }
                if (!isNaN(inActiveOptionValue) && inActiveOptionValue != '' && inActiveOptionValue != null) {
                    var opt = new Option(inActiveOptionText, inActiveOptionValue);
                    if (control.getAttribute('InActiveWTIsORT') != null) {
                        opt.setAttribute('IsORT', control.getAttribute('InActiveWTIsORT'));
                    }
                    if (control.getAttribute('InActiveWTIsW2Hourly') != null) {
                        opt.setAttribute('isW2HourlyTimeType', control.getAttribute('InActiveWTIsW2Hourly'));
                    }
                    if (control.getAttribute('InActiveWTIsW2Salary') != null) {
                        opt.setAttribute('isW2SalaryTimeType', control.getAttribute('InActiveWTIsW2Salary'));
                    }
                    control.add(opt);
                }
                control.value = selectedVal;
            }
        }
        this.setDisplayForPlusButton();
    },
    setDisplayForPlusButton: function () {
        var controlIds = this.getControlIdList();
        if (controlIds.length > 0) {
            var control = document.getElementById(controlIds[0]);
            var count = this.getCountOfUnSelectedControls();
            if (control.value > 0)
                count++;
            var plusButtonClientID = document.getElementById(this.plusButtonClientID);
            if (plusButtonClientID != null) {
                var controlItems = control.getElementsByTagName("option");
                if (controlItems.length == 1 || controlItems.length == count + 1) {
                    plusButtonClientID.style.display = 'none';
                } else {
                    plusButtonClientID.style.display = 'block';
                }
            }
        }
    },
    getCountOfUnSelectedControls: function () {
        var controlIds = this.getControlIdList();
        var count = 0;
        for (i = 0; i < controlIds.length; i++) {
            var control = document.getElementById(controlIds[i]);
            if (control && control.value < 0) {
                count = count + 1;
            }
        }
        return count;
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

