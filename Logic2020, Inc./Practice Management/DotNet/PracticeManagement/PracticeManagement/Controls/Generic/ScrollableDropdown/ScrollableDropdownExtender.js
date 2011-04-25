Type.registerNamespace("PraticeManagement.Controls.Generic.ScrollableDropdown");

PraticeManagement.Controls.Generic.ScrollableDropdown.ScrollableDropdownBehavior = function (element) {
    PraticeManagement.Controls.Generic.ScrollableDropdown.ScrollableDropdownBehavior.initializeBase(this, [element]);

    this._labelValue = null;
    this._IsScrollingDropdownClicked = false;
}

PraticeManagement.Controls.Generic.ScrollableDropdown.ScrollableDropdownBehavior.prototype = {
    initialize: function () {

        PraticeManagement.Controls.Generic.ScrollableDropdown.ScrollableDropdownBehavior.callBaseMethod(this, 'initialize');

        //  On clicking the Dropdown, call the corresponding function
        $addHandler($get(this.get_labelValue()), 'click', Function.createDelegate(this, this.showOrHideScrollingDropdown));
        $addHandler(this.get_scrollingDropdown(), 'click', Function.createDelegate(this, this.scrollingDropdownClicked));
        $addHandler(document.body, 'click', Function.createDelegate(this, this.documentBodyClicked));
        $addHandler(window, 'scroll', Function.createDelegate(this, this.hideScrollingDropdown));

        this.hideScrollingDropdown();
    },

    get_scrollingDropdown: function () {
        if (this.get_element()) {
            return this.get_element().parentNode;
        }
    },

    get_labelValue: function () {
        return this._labelValue;
    },

    set_labelValue: function (value) {
        this._labelValue = value;
    },

    get_isScrollingDropdownClicked: function () {
        return this._IsScrollingDropdownClicked;
    },

    set_isScrollingDropdownClicked: function (value) {
        this._IsScrollingDropdownClicked = value;
    },

    showOrHideScrollingDropdown: function () {

        var scrollingDropdown = this.get_scrollingDropdown();
        var labelControl = $get(this.get_labelValue());

        //labelControl.disabled = 'disabled';

        if (scrollingDropdown.style.display == 'none') {
            scrollingDropdown.style.display = '';
            this._setCheckBoxListPosition();
        }
        else {
            scrollingDropdown.style.display = 'none';
        }

        //labelControl.disabled = '';
        this.set_isScrollingDropdownClicked(true);
    },

    hideScrollingDropdown: function () {

        var scrollingDropdown = this.get_scrollingDropdown();
        if (scrollingDropdown) {
            scrollingDropdown.style.display = 'none';
            scrollingDropdown.style.left = '0px';
            scrollingDropdown.style.top = '0px';
            scrollingDropdown.style.position = 'fixed';
        }
    },

    documentBodyClicked: function () {


        if (!this.get_isScrollingDropdownClicked()) {
            this.hideScrollingDropdown();
        }
        else {
            this.set_isScrollingDropdownClicked(false);
        }
    },

    scrollingDropdownClicked: function () {

        this.set_isScrollingDropdownClicked(true);
    },

    dispose: function () {
        // Cleanup code 
        PraticeManagement.Controls.Generic.ScrollableDropdown.ScrollableDropdownBehavior.callBaseMethod(this, 'dispose');
    },

    _setCheckBoxListPosition: function () {

        var scrollingDropdown = this.get_scrollingDropdown();
        var labelControlPosition = this.getOffset($get(this.get_labelValue()));

        if (labelControlPosition != null && scrollingDropdown) {
            scrollingDropdown.style.left = labelControlPosition.x + 'px';
            scrollingDropdown.style.top = labelControlPosition.y + labelControlPosition.h + 'px';
            if (Number(document.documentElement.scrollTop) > 0) {
                window.scrollTo(0, 0);
            }
        }
    },

    getOffset: function (object) {
        for (var r = { x: object.offsetLeft, y: object.offsetTop, h: object.offsetHeight, w: object.offsetWidth }; object = object.offsetParent; r.x += object.offsetLeft, r.y += object.offsetTop);
        return r;
    }
}

PraticeManagement.Controls.Generic.ScrollableDropdown.ScrollableDropdownBehavior.registerClass('PraticeManagement.Controls.Generic.ScrollableDropdown.ScrollableDropdownBehavior', AjaxControlToolkit.BehaviorBase);

