function CollapseOrExpandAll(btnExpandOrCollapseAllClientId, hdnCollapsedClientId, hdncpeExtendersIds) {
    var btn = btnExpandOrCollapseAllClientId;
    var hdnCollapsed = hdnCollapsedClientId;
    var isExpand = false;
    if (btn != null) {
        if (btn.value == "Expand All") {
            isExpand = true;
            btn.value = "Collapse All";
            btn.title = "Collapse All";
            hdnCollapsed.value = 'false';
        }
        else {
            btn.value = "Expand All";
            btn.title = "Expand All";
            hdnCollapsed.value = 'true';
        }

        var projectPanelskvPair = jQuery.parseJSON(hdncpeExtendersIds.value);
        ExpandOrCollapsePanels(projectPanelskvPair, isExpand);
    }
    return false;
}




function ExpandOrcollapseExtender(cpe, isExpand) {
    if (cpe != null) {
        if (isExpand) {
            ExpandPanel(cpe)
        }
        else {
            var isCollapsed = cpe.get_Collapsed();
            if (!isCollapsed)
                cpe._doClose();
        }
    }
}


function ExpandPanel(cpe) {
    var isCollapsed = cpe.get_Collapsed();
    if (isCollapsed) {
        cpe.expandPanel();
    }
}


function ExpandOrCollapsePanels(ids, isExpand) {
    for (var i = 0; i < ids.length; i++) {
        var cpe = $find(ids[i]);
        ExpandOrcollapseExtender(cpe, isExpand);
    }
}

