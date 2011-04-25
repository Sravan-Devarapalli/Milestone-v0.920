Type.registerNamespace("PraticeManagement.Controls.Generic.SearchHighlighting");

PraticeManagement.Controls.Generic.SearchHighlighting.SearchHighlightingBehavior = function(element) {
    PraticeManagement.Controls.Generic.SearchHighlighting.SearchHighlightingBehavior.initializeBase(this, [element]);
}

PraticeManagement.Controls.Generic.SearchHighlighting.SearchHighlightingBehavior.prototype = {
    initialize: function () {
        PraticeManagement.Controls.Generic.SearchHighlighting.SearchHighlightingBehavior.callBaseMethod(this, 'initialize');
        this._highlightOnLoad();
    },

    dispose: function () {
        // Cleanup code 
        PraticeManagement.Controls.Generic.SearchHighlighting.SearchHighlightingBehavior.callBaseMethod(this, 'dispose');
    },

    _highlightOnLoad: function () {
        // Get search string
        var searchString = this._getSearchString();

        if (searchString.length > 1) {
            var textContainerNode = document.getElementById(this.searchInsideBlockId);

            var regex = new RegExp(">([^<]*)?(" + searchString + ")([^>]*)?<", "ig");
            this._highlightTextNodes(textContainerNode, regex);
        }
    },

    // Pull the search string out of the URL
    _getSearchString: function () {
        return this.get_element().value;
    },

    _highlightTextNodes: function (element, regex) {
        var tempinnerHTML = element.innerHTML;
        // Do regex replace
        // Inject span with class of 'highlighted termX' for Google style highlighting
        element.innerHTML = tempinnerHTML.replace(regex, '>$1<span class="highlighted">$2</span>$3<');
    }
}

PraticeManagement.Controls.Generic.SearchHighlighting.SearchHighlightingBehavior.registerClass('PraticeManagement.Controls.Generic.SearchHighlighting.SearchHighlightingBehavior', AjaxControlToolkit.BehaviorBase);
