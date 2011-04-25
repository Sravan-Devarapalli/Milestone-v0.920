Type.registerNamespace("PraticeManagement.Controls.Opportunities.ViewProjectExtender");

PraticeManagement.Controls.Opportunities.ViewProjectExtender.ViewProjectBehavior = function (element) {
    PraticeManagement.Controls.Opportunities.ViewProjectExtender.ViewProjectBehavior.initializeBase(this, [element]);
    this.controlToShowProjectLinkId = null;
    this.emptyValue = "-1";
    this.returnToUrl = "";
    this._isControlWasShown = false;
}

PraticeManagement.Controls.Opportunities.ViewProjectExtender.ViewProjectBehavior.prototype = {
    initialize: function () {
        PraticeManagement.Controls.Opportunities.ViewProjectExtender.ViewProjectBehavior.callBaseMethod(this, 'initialize');

        // Initalization code
        $addHandler(this.get_element(), 'change', Function.createDelegate(this, this._onchange));
        this._onchange();
    },

    dispose: function () {
        // Cleanup code
        PraticeManagement.Controls.Opportunities.ViewProjectExtender.ViewProjectBehavior.callBaseMethod(this, 'dispose');
    },

    // Property accessors
    get_ControlToShowProjectLinkID: function () {
        return this.controlToShowProjectLinkId;
    },

    set_ControlToShowProjectLinkID: function (value) {
        this.controlToShowProjectLinkId = value;
    },

    get_EmptyValue: function () {
        return this.emptyValue;
    },

    set_EmptyValue: function (value) {
        this.emptyValue = value;
    },
    get_ReturnToUrl: function () {
        return this.returnToUrl;
    },

    set_ReturnToUrl: function (value) {
        this.returnToUrl = value;
    },

    _onchange: function (sender, args) {
        var targetElementId = this.get_element();
        var returnToUrl = this.returnToUrl;
        if (returnToUrl == "")
            returnToUrl = escape(window.location);
        var selectedValue = targetElementId.options[targetElementId.selectedIndex].value;
        var hlProject = document.getElementById(this.controlToShowProjectLinkId);
        if (selectedValue != this.emptyValue && selectedValue != "") {
            hlProject.style.display = "block";
            hlProject.href = "ProjectDetail.aspx?id=" + selectedValue + '&returnTo=' + returnToUrl;
        }
        else {
            if (targetElementId[targetElementId.selectedIndex].text == "Select project...")
                hlProject.style.display = "none";
        }
    }
}

PraticeManagement.Controls.Opportunities.ViewProjectExtender.ViewProjectBehavior.registerClass('PraticeManagement.Controls.Opportunities.ViewProjectExtender.ViewProjectBehavior', AjaxControlToolkit.BehaviorBase);
