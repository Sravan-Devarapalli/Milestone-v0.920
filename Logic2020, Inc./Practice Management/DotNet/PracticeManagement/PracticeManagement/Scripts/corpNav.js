$(document).ready(function () {
    var selectDefList;
    var hoverItem;
    var menuItem;
    var timer;
    var showMenu = function () {
        $('li#' + menuItem + ' a').addClass('liBG');
        $('li#' + menuItem).addClass('l1hover');
        $(selectDefList).slideDown(150);
    };
    $('.l1').hover(function () {
        hoverItem = $(this).attr('id');
        selectDefList = 'li#' + hoverItem + ' dl';
        this.children[0].style.color = '#2a4753';
        menuItem = hoverItem;
        timer = setTimeout(showMenu, 250);
    },
function () {
    clearTimeout(timer);
    $('li#' + hoverItem + ' a').removeClass('liBG');
    $('li#' + menuItem).removeClass('l1hover');
    $('li#' + hoverItem + ' a')[0].style.color = '';
    $(selectDefList).slideUp(150);
    return false;
});
    $('dd.l3').hover(function () {
        var selectL2 = $(this).attr('id');
        if ($('dd#' + selectL2 + ' .flyout').is(':hidden')) {
            $('dd#' + selectL2 + ' a').addClass('ddBG');
            $('dd#' + selectL2 + ' .flyout').show(0);
        }
        else {
            $('dd#' + selectL2 + ' a').removeClass('ddBG');
            $('dd#' + selectL2 + ' .flyout').hide();
        }
    }
);
});
