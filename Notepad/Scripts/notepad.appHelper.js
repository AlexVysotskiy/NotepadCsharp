// helper for common operations
var appHelper = {
    _currentOrder: {
        order: 'DESC',
        field: 'Id'
    },
    defaultCount: 3,
    loaderText: '<span class="glyphicon glyphicon-refresh loader" aria-hidden="true"></span>'
};

// render new rows by passed informations and add to page
appHelper.renderEntryRows = function (rowsData, order, checkExisted) {

    if (checkExisted == null) {
        checkExisted = true;
    }

    var searchFields = ['Name', 'Lastname', 'BirthYear', 'PhoneNumber'];
    var list = [];

    var template = $('#template-row').clone();
    template.removeClass('hidden');

    for (var key in rowsData) {

        var rowData = rowsData[key];

        var isPrinted = $.inArray(rowData.Id, entryList.ids) != -1;

        if (!checkExisted || !isPrinted) {

            if (!isPrinted) {
                entryList.ids.push(rowData.Id);
            }

            var row = template.clone();
            row.attr('id', 'notepad-entry-' + rowData.Id).find('[type="checkbox"]').val(rowData.Id);
            row.data('itemid', rowData.Id);

            for (var i in searchFields) {

                var field = searchFields[i];
                row.find('.' + field).html(rowData[field]);
            }

            list.push(row);
        }
    }

    var holder = $('#list-holder');

    if (!order || order == 'after') {
        holder.append(list);
    }
    else if (order == 'clear') {
        holder.find('.notepad-entry').remove();
        holder.prepend(list);
    }
    else {
        holder.prepend(list);
    }
};

// request for new entries by passed parameters
appHelper.loadNewItems = function (url, page, count, callbackSuccess) {

    var currentOrder = appHelper.getCurrentOrder();

    var params = {
        page: page != null ? page : appHelper.getPage(),
        count: count != null ? count : appHelper.defaultCount,
        order: currentOrder.order,
        by: currentOrder.field
    };

    var renderParams = []
    for (var key in params) {
        renderParams.push(key + '=' + params[key]);
    }

    url += '?' + renderParams.join('&') + '&' + $('#search-form').serialize();

    $.getJSON(url, function (data) {

        var loadMoreContainer = $('#load-more')

        if (!data.hasMore) {
            loadMoreContainer.hide(1);
        }
        else {
            loadMoreContainer.show(1);
        }

        callbackSuccess.call(this, data);

    });
}

// change order
appHelper.updateOrder = function (order, field) {

    appHelper._currentOrder = { order: order, field: field };

    return appHelper._currentOrder;
}

// get current order direction / field
appHelper.getCurrentOrder = function () {

    if (!appHelper.hasOwnProperty('_currentOrder')) {
        appHelper._currentOrder = {
            order: 'DESC',
            field: 'Id'
        };
    }

    return appHelper._currentOrder;
}

// current viewing page
appHelper.getPage = function () {
    if (!appHelper.hasOwnProperty('_page')) {
        appHelper._page = 0;
    }
    return appHelper._page;
}

// set current viewing page
appHelper.setPage = function (page) {

    appHelper._page = parseInt(page);
}

// change viewing page
appHelper.incrPage = function () {
    appHelper._page++;
}

appHelper.addLocationHash = function (field, value)
{
    var hash = location.hash.toString().replace('#', '').split('|');

    var newHash = [];
    
    for (var key in hash)
    {
        if (hash[key] != "" && hash[key].search(field) == -1 && hash[key] != '#')
        {
            newHash.push(hash[key])
        }
    }

    newHash.push(field + ':' + value);
    location.hash = newHash.join('|');
}

appHelper.removeFromLocationHash = function (field) {

    var hash = location.hash.toString().replace('#', '').split('|');

    var newHash = [];

    console.log(hash);

    for (var key in hash) {
        if (hash[key] != "" && hash[key].search(field) == -1 && hash[key] != '#') {
            newHash.push(hash[key])
        }
    }

    location.hash = newHash.join('|');
}