Type.registerNamespace("PraticeManagement.Controls.Generic.SelectCutOff");

PraticeManagement.Controls.Generic.SelectCutOff.SelectCutOffBehavior = function(element) {
    PraticeManagement.Controls.Generic.SelectCutOff.SelectCutOffBehavior.initializeBase(this, [element]);
}

PraticeManagement.Controls.Generic.SelectCutOff.SelectCutOffBehavior.prototype = {
    initialize: function() {
        PraticeManagement.Controls.Generic.SelectCutOff.SelectCutOffBehavior.callBaseMethod(this, 'initialize');

        $addHandlers(this.get_element(),
                     { 'mousedown': this._onFocus,
                         'blur': this._onBlur,
                         'change': this._onBlur
                     },
                     this);
    },

    dispose: function() {
        // Cleanup code 
        PraticeManagement.Controls.Generic.SelectCutOff.SelectCutOffBehavior.callBaseMethod(this, 'dispose');
    },

    _onFocus: function() {
        var target = this.get_element();
        if (target != null) {
            target.setAttribute("class", this.extendedCssClass);
        }
    },

    _onBlur: function() {
        var target = this.get_element();
        if (target != null) {
            target.setAttribute("class", this.normalCssClass);
        }
    }
}

PraticeManagement.Controls.Generic.SelectCutOff.SelectCutOffBehavior.registerClass('PraticeManagement.Controls.Generic.SelectCutOff.SelectCutOffBehavior', AjaxControlToolkit.BehaviorBase);
