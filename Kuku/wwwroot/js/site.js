// Write your JavaScript code.
$(document).ready(function () {
    $('a.show-more').on('click', function () {
        $($(this).data('target')).dropdown('toggle');
        return false;
    });
    $('.filter-product .dropdown-menu').on('show.bs.dropdown', function () { $(this).off('click') });
    $('.filter-product .dropdown-menu a.dropdown-item').on('click', function () {
        window.location = $(this).attr('href');
    });
});