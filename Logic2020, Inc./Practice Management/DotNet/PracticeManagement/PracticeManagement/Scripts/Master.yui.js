function checkDirtyWithRedirect(url) {
    if ((document.location.pathname.match("/OpportunityDetail.aspx") != null) ||
                showDialod()) {
        __doPostBack("__Page", url);
        return false;
    }

    return true;
}

function showInProcessImage(divWait, divContainer) {
    // get the bounds of both the container and the image  
    var containerBounds = Sys.UI.DomElement.getBounds(divContainer);
    var imgBounds = Sys.UI.DomElement.getBounds(divWait);

    // figure out where to position the element (the center of the container)  
    var x = containerBounds.x +
					Math.round(containerBounds.width / 2) - Math.round(imgBounds.width / 2);
    var y = containerBounds.y +
					Math.round(containerBounds.height / 2) - Math.round(imgBounds.height / 2);

    // set the position of the in progress image  
    Sys.UI.DomElement.setLocation(divWait, x, y);

    // finally un-hide it  
    divWait.style.visibility = 'visible';
    divWait.style.display = 'block';
}

function hideInProcessImage(divWait) {
    divWait.style.visibility = 'hidden';
    divWait.style.display = 'none';
}
function getCookie(c_name) {
    var i, x, y, ARRcookies = document.cookie.split(";");
    for (i = 0; i < ARRcookies.length; i++) {
        x = ARRcookies[i].substr(0, ARRcookies[i].indexOf("="));
        y = ARRcookies[i].substr(ARRcookies[i].indexOf("=") + 1);
        x = x.replace(/^\s+|\s+$/g, "");
        if (x == c_name) {
            return unescape(y);
        }
    }
}
