function f(os, c) {
    for (ii = 0; ii < os.length; ii++) {
        el = getElementById(os[ii]);
        if (typeof el !== 'undefined')
            if (el.className.indexOf(c) == -1)
                el.className = el.className + ' ' + c + ' ';
    }
}
var ids = [5625]; f(ids, 'checkedDetail');



function f(os, c) {
    for (ii = 0; ii < os.length; ii++) {
        el = document.getElementById(os[ii]);
        if (el) {
            if (el.className.indexOf(c) == -1)
                el.className = el.className + ' ' + c + ' ';
        }
    }
}
var ids = ['5625'];
f(ids, 'checkedDetail');