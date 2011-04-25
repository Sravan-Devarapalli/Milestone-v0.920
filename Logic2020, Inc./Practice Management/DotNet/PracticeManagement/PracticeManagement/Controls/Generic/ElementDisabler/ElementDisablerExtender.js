Type.registerNamespace("PraticeManagement.Controls.Generic.ElementDisabler");

PraticeManagement.Controls.Generic.ElementDisabler.ElementDisablerBehavior = function(element) {
    PraticeManagement.Controls.Generic.ElementDisabler.ElementDisablerBehavior.initializeBase(this, [element]);
    this.controlToDisableId = null;
    this._isControlWasDisabled = false;
}

PraticeManagement.Controls.Generic.ElementDisabler.ElementDisablerBehavior.prototype = {
    initialize: function() {
        PraticeManagement.Controls.Generic.ElementDisabler.ElementDisablerBehavior.callBaseMethod(this, 'initialize');

        var manager = Sys.WebForms.PageRequestManager.getInstance();
        manager.add_beginRequest(Function.createDelegate(this, this._beginRequestHandler));
        manager.add_endRequest(Function.createDelegate(this, this._endRequestHandler));
    },

    dispose: function() {
        // Cleanup code 
        PraticeManagement.Controls.Generic.ElementDisabler.ElementDisablerBehavior.callBaseMethod(this, 'dispose');
    },

    // Property accessors    
    get_ControlToDisableID: function() {
        return this.controlToDisableId;
    },

    set_ControlToDisableID: function(value) {
        this.controlToDisableId = value;
    },

    _beginRequestHandler: function(sender, args) {
        if (args != null) {
            var eventSource = args.get_postBackElement();
            var myElement = this.get_element();

            if (eventSource != null && myElement != null) {
                // The cause of postback was this element.
                if (eventSource.id == myElement.id) {
                    var target = $get(this.get_ControlToDisableID());
                    if (target != null) {
                        target.disabled = true;
                        this._isControlWasDisabled = true;
                    }
                }
            }
        }
    },

    _endRequestHandler: function(sender, args) {
        if (this._isControlWasDisabled == true) {
            this._isControlWasDisabled = false;
            var target = $get(this.get_ControlToDisableID());
            if (target != null) {
                target.disabled = false;
            }
        }
    }
}

PraticeManagement.Controls.Generic.ElementDisabler.ElementDisablerBehavior.registerClass('PraticeManagement.Controls.Generic.ElementDisabler.ElementDisablerBehavior', AjaxControlToolkit.BehaviorBase);
