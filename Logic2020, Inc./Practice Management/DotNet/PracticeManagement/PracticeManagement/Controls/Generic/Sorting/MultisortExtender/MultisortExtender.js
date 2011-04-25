Type.registerNamespace('PraticeManagement.Controls.Generic.Sorting.MultisortExtender');

// Array to store all extenders on the page
var allExtenders = new Array();

//  Update particular extender
function updateSingleExtender(element, index, array) {
    element.updateExtender();
}

//  Update all extenders
function UpdateAllExtenders() {
    Array.forEach(allExtenders, updateSingleExtender);
}

PraticeManagement.Controls.Generic.Sorting.MultisortExtender.MultisortBehavior = function(element) {

    PraticeManagement.Controls.Generic.Sorting.MultisortExtender.MultisortBehavior.initializeBase(this, [element]);

    this._SynchronizerIDValue = null;   //  Id of the syncronizer control
    this._AscendingTextValue = null;    //  Text used for the ascending values
    this._DescendingTextValue = null;   //  Text used for the descending values
    this._NoSortingTextValue = null;    //  Text used for no sorting values
    this._SortExpressionValue = null;   //  The value used to generate general sort expression
    this._InitialLabel =
        this.get_element().innerHTML; //  The value that was initially on the label

    this._ItemsSeparator = ",";         //  Character used to separate items in sync string
    this._DirectionSeparator = ":";     //  Character used to separate sort expression from direction

    this._ASC = "ASC";                  //  Ascending direction serialization string
    this._DESC = "DESC";                //  Descending direction serialization string
    this._NONE = "NONE";                //  No sorting direction serialization string

    this._sortDirectionOrder =          //  Order of changing sort directions
        [this._ASC, this._DESC, this._NONE];

    // Add particular extender to the array if it's not there yet
    if (!Array.contains(allExtenders, this))
        Array.add(allExtenders, this);
}

PraticeManagement.Controls.Generic.Sorting.MultisortExtender.MultisortBehavior.prototype = {

    //  Initialize extender
    initialize: function() {
        PraticeManagement.Controls.Generic.Sorting.MultisortExtender.MultisortBehavior.callBaseMethod(this, 'initialize');

        //  On clicking the label, call the corresponding function
        $addHandler(this.get_element(), 'click',
            Function.createDelegate(this, this._onclick));

        // On mouseover and mouseout change the cursor
        $addHandler(this.get_element(), 'mouseover',
            Function.createDelegate(this, this._onmouseover));
        $addHandler(this.get_element(), 'mouseout',
            Function.createDelegate(this, this._onmouseout));

        //  Inilialize the label on page load
        this.updateExtender();
    },

    dispose: function() {
        // Cleanup code 

        PraticeManagement.Controls.Generic.Sorting.MultisortExtender.MultisortBehavior.callBaseMethod(this, 'dispose');
    },

    //  ----------------------------------------------------------
    //      Property accessors - start
    //  ----------------------------------------------------------
    get_InitialLabel: function() {
        return this._InitialLabel;
    },

    get_SortExpressionValue: function() {
        return this._SortExpressionValue;
    },

    set_SortExpressionValue: function(value) {
        this._SortExpressionValue = value;
    },

    get_SynchronizerIDValue: function() {
        return this._SynchronizerIDValue;
    },

    set_SynchronizerIDValue: function(value) {
        this._SynchronizerIDValue = value;
    },

    get_AscendingTextValue: function() {
        return this._AscendingTextValue;
    },

    set_AscendingTextValue: function(value) {
        this._AscendingTextValue = value;
    },

    get_DescendingTextValue: function() {
        return this._DescendingTextValue;
    },

    set_DescendingTextValue: function(value) {
        this._DescendingTextValue = value;
    },

    get_NoSortingTextValue: function() {
        return this._NoSortingTextValue;
    },

    set_NoSortingTextValue: function(value) {
        this._NoSortingTextValue = value;
    },
    //  ----------------------------------------------------------
    //      Property accessors - end
    //  ----------------------------------------------------------

    // Returns sync string as array of items like <SortExpression>:<SortDirection>
    _getSyncArray: function() {
        var e = $get(this.get_SynchronizerIDValue());
        if (e) {
            return e.value.split(this._ItemsSeparator);
        }
        return null;
    },

    //  Returns current item state as sort direction
    _getState: function() {
        // Get sync as array
        var syncArr = this._getSyncArray();

        // if it's empty, return default sorting order
        if (syncArr == null)
            return this._NONE;

        // if it's not empty, look for direction
        for (i = 0; i < syncArr.length; i++) {
            var kv = syncArr[i].split(this._DirectionSeparator)
            if (kv[0] == this.get_SortExpressionValue())
                return kv[1];
        }

        // return default if nothing was found
        return this._NONE;
    },

    //  Returns current item index in a sync string
    _getIndex: function() {
        // Get sync as array
        var syncArr = this._getSyncArray();

        // if it's empty, return default sorting order
        if (syncArr == null)
            return 0;

        // if it's not empty, look for direction
        for (i = 0; i < syncArr.length; i++) {
            var kv = syncArr[i].split(this._DirectionSeparator)
            if (kv[0] == this.get_SortExpressionValue())
                return i;
        }

        // return default if nothing was found
        return 0;
    },

    // Returns user-friendly text by serialized direction value
    _getTextByDirection: function(direction) {
        if (direction == this._ASC)
            return this.get_AscendingTextValue();

        if (direction == this._DESC)
            return this.get_DescendingTextValue();

        return this.get_NoSortingTextValue();
    },

    // Returns user-friendly text by current state
    _getStateText: function() {
        var state = this._getState();
        var singleItem = this._getTextByDirection(state);
        var text = singleItem;

        if (state != this._NONE) {
            var index = this._getIndex();
            for (i = 0; i < index; i++)
                text += singleItem;
        }

        return text;
    },

    // Sets label text according to the current sync string
    updateExtender: function() {
        if (this.get_element())
            this.get_element().innerHTML = this.get_InitialLabel() + ' ' + this._getStateText();
        else
            this.get_element.innerHTML = this.get_InitialLabel() + ' ' + this._getStateText();
    },
    
    // Removes unnecessary item separators from the sync string if any
    _trimSyncString: function() {
        var syncString = $get(this.get_SynchronizerIDValue()).value;

        if (syncString.indexOf(this._ItemsSeparator) == 0) {
            syncString = syncString.substring(1);
            $get(this.get_SynchronizerIDValue()).value = syncString;
        }

        var syncLen = syncString.length;
        if (syncString.lastIndexOf(this._ItemsSeparator) == syncLen - 1) {
            syncString = syncString.substring(0, syncLen - 1);
            $get(this.get_SynchronizerIDValue()).value = syncString;
        }

        $get(this.get_SynchronizerIDValue()).value =
            syncString.replace(this._ItemsSeparator + this._ItemsSeparator, this._ItemsSeparator);
    },

    // Changes sync string on client click
    _onclick: function() {
        // Get current direction
        var state = this._getState();
        // Get next direction based on current
        var nextState = this._sortDirectionOrder[(Array.indexOf(this._sortDirectionOrder, state) + 1) % 3];

        // Generate current selialized state
        var currSerialized = this.get_SortExpressionValue() + this._DirectionSeparator + state;
        // Generate next serialized state
        var nextSerialized = this.get_SortExpressionValue() + this._DirectionSeparator + nextState;

        var syncString = $get(this.get_SynchronizerIDValue()).value;
        if (nextState == this._NONE) {
            //  If the next state is no sorting, remove it from there
            $get(this.get_SynchronizerIDValue()).value = syncString.replace(currSerialized, "");
        }
        else {
            // If the next state is ASC or DESC, add corresponding value to the sync string

            if (syncString.indexOf(currSerialized) < 0) {
                // If current sort expression is not in the sync string, add it there
                var separator = syncString.length == 0 ? "" : this._ItemsSeparator;
                $get(this.get_SynchronizerIDValue()).value += separator + nextSerialized;
            }
            else {
                // If current sort expression is already in the sync string, replace it with the new one
                $get(this.get_SynchronizerIDValue()).value = syncString.replace(currSerialized, nextSerialized);
            }
        }

        // Trim the sync string
        this._trimSyncString();
        // Update labels on all extenders
        UpdateAllExtenders()
    },

    // Change cursor to the pointer on mouse over the label
    _onmouseover: function() {
        this.get_element().style.cursor = 'pointer';
    },

    // Change cursor to the default one on mouse out of the label
    _onmouseout: function() {
        this.get_element().style.cursor = 'auto';
    }
}

PraticeManagement.Controls.Generic.Sorting.MultisortExtender.MultisortBehavior.registerClass('PraticeManagement.Controls.Generic.Sorting.MultisortExtender.MultisortBehavior', AjaxControlToolkit.BehaviorBase)
