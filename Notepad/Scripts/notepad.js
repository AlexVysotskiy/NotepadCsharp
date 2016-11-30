// list of rendered entires
var entryList = {};

// main method, add all handlers
var initNotepadApp = function (appParams) {

    var mainContainer = $('#content-holder.notepad'),
        loadMoreContainer = $('#load-more'),
        removeTrigger = $('#remove-entry'),
        mainModal = $('#main-modal');

    // define prinded entries
    entryList.ids = [];
    for (var key in appParams.entryList) {
        entryList.ids.push(appParams.entryList[key].Id);
    }

    // hanlde change order. Click by column title change the ordering on entries. It's necessary to request for new set of entries
    mainContainer.find('.change-order').on('click', function (event) {

        event.preventDefault();

        // define curent order
        var currentOrder = appHelper.getCurrentOrder();

        // return to basic order
        if ($(this).hasClass('remove')) {

            currentOrder.order = 'DESC';
            currentOrder.field = 'Id';

            mainContainer.find('.change-order.active').removeClass("active asc desc");

            appHelper.removeFromLocationHash('o');
            appHelper.removeFromLocationHash('b');
        }
        else {

            // define new order direction
            if (!$(this).hasClass('active')) {

                mainContainer.find('.change-order.active').removeClass("active asc desc");
            }

            if ($(this).hasClass('asc')) {

                $(this).removeClass('asc').addClass('active desc');
                currentOrder.order = 'DESC';
            }
            else {

                $(this).removeClass('desc').addClass('active asc');
                currentOrder.order = 'ASC';
            }

            // define new order field
            currentOrder.field = $(this).attr('id');

            appHelper.addLocationHash('o', currentOrder.order);
            appHelper.addLocationHash('b', currentOrder.field);
        }

        // save new order
        appHelper.updateOrder(currentOrder.order, currentOrder.field);

        // request for new entries. Request for the same count as was already printed
        appHelper.loadNewItems(appParams.listUrl, 0, mainContainer.find('#list-holder > .notepad-entry').length, function (data) {

            entryList.ids = [];
            appHelper.renderEntryRows(data.list, 'clear', false);
        });

    });

    // handle load more button
    loadMoreContainer.find('.ajax-loader').on('click', function (event) {

        var trigger = $(this);

        event.preventDefault();

        // prevent multiple clicks
        if (!trigger.data('inProgress')) {

            trigger.data('prevText', trigger.html());
            trigger.html(appHelper.loaderText);
            trigger.data('inProgress', 1);

            // request new entries with new offset
            appHelper.loadNewItems(appParams.listUrl, appHelper.getPage() + 1, appParams.viewCount, function (data) {

                // change viewing page
                appHelper.incrPage();

                // render row
                appHelper.renderEntryRows(data.list);

                trigger.data('inProgress', 0);
                trigger.html(trigger.data('prevText'));

                appHelper.addLocationHash('p',appHelper.getPage());
            });
        }

    });

    // handle click on remove button
    removeTrigger.on('click', function (event) {

        event.preventDefault();

        // collect all ids, which will be removed
        var ids = [];
        $('input.remove[type="checkbox"]:checked').each(function () {

            ids.push($(this).val());
        });

        if (ids.length > 0) {

            removeTrigger.data('prevText', removeTrigger.html());
            removeTrigger.html(appHelper.loaderText);

            // make remove request 
            $.ajax({
                type: "POST",
                url: appParams.removeUrl,
                data: { ids: ids },
                dataType: 'json',
                success: function (data) {

                    if (data.success) {

                        var selector = [];
                        for (var i in ids) {
                            selector.push('#notepad-entry-' + ids[i]);
                        }

                        // remove it on frontend
                        $(selector.join(', ')).remove();
                    }

                    removeTrigger.html(removeTrigger.data('prevText'));
                    removeTrigger.data('active', 0);
                    removeTrigger.attr('disabled', 'disabled');

                    // update observing page
                    appHelper.setPage(Math.ceil(mainContainer.find('#list-holder > .notepad-entry').length / appParams.viewCount));

                }
            });
        }

    });

    // handle click on selection-checkbox
    mainContainer.on('click', 'input.remove[type="checkbox"]', function (event) {

        // remove or add disable attr on remove-button
        if ($(this).is(':checked')) {

            var selected = parseInt(removeTrigger.data('active')) + 1;
            removeTrigger.data('active', selected);

            if (selected == 1) {
                removeTrigger.removeAttr('disabled');
            }
        }
        else {

            var selected = parseInt(removeTrigger.data('active')) - 1;
            removeTrigger.data('active', selected);
            if (!selected) {
                removeTrigger.attr('disabled', 'disabled');
            }
        }

    });

    // hanlder for submiting form in modal window
    mainModal.on('click', '.save', function (event) {

        var form = mainModal.find('form');

        var trigger = $(this);
        trigger.attr('disabled', 'disabled');

        if (form.length) {

            form.trigger('clearErrors');

            // serialize data and send request
            var data = form.serialize();

            $.ajax({
                type: form.attr('method'),
                url: form.attr('action'),
                data: data,
                success: function (data) {

                    if (data.success) {

                        // if everything is OK trigger this event (we define it every time fir each form)
                        mainModal.trigger('successSubmit', [data]);
                        mainModal.modal('hide');
                    }
                    else {
                        form.trigger('addErrors', [data.errors]);
                    }
                    trigger.removeAttr('disabled');
                }
            });
        }

    });

    // handler for success edited entity
    var editEntrySuccess = function (data) {

        // update info on frontend
        var row = $('#notepad-entry-' + data.entry.Id);
        var searchFields = ['Name', 'Lastname', 'BirthYear', 'PhoneNumber'];
        for (var i in searchFields) {

            var field = searchFields[i];
            row.find('.' + field).html(data.entry[field]);
        }
    };

    // show editing form in modal for entry
    mainContainer.on('click', '.edit-entry', function (event) {

        event.preventDefault();

        var trigger = $(this);

        // request form from server
        $.ajax({
            type: "GET",
            url: appParams.editUrl + '?id=' + trigger.parents('.notepad-entry').first().data('itemid'),
            success: function (data) {

                mainModal.find('.modal-title').html('Редактировать запись');
                mainModal.find('.modal-body').html(data);
                mainModal.find('.modal-footer').html('<button type="button" class="btn btn-success save">Сохранить</button>');

                // add success submit handler
                mainModal.off('successSubmit').on('successSubmit', function (event, data) {
                    editEntrySuccess.call(this, data);
                });

                mainModal.modal('show');
            }
        });

    });

    // handler for success edded entity
    var addNewEntrySuccess = function (data) {
        appHelper.renderEntryRows([data.entry], 'before');
    };

    // show form in modal for new entry
    $('#add-new').on('click', function (event) {

        event.preventDefault();

        var trigger = $(this);

        $.ajax({
            type: "GET",
            url: appParams.createUrl,
            success: function (data) {

                mainModal.find('.modal-title').html('Создать запись');
                mainModal.find('.modal-body').html(data);
                mainModal.find('.modal-footer').html('<button type="button" class="btn btn-success save">Сохранить</button>');

                mainModal.off('successSubmit').on('successSubmit', function (event, data) {
                    addNewEntrySuccess.call(this, data);
                });

                mainModal.modal('show');
            }
        });

    });

    // send request to save entries to storage
    $('#save-all').on('click', function (event) {
        
        event.preventDefault();

        var trigger = $(this);

        $.ajax({
            type: "GET",
            url: appParams.saveUrl,
            success: function (data) {
            }
        });
    });
   
    // handle submit search form
    $('#search-form').on('submit', function (event) {

        event.preventDefault();

        var search = $(this).find('[name="search"]').val().trim();

        if (search) {

            var trigger = $(this);

            if (!trigger.data('inProgress')) {

                trigger.data('inProgress', 1);

                // load new entries by search conditions
                appHelper.loadNewItems(appParams.listUrl, 0, appParams.viewCount, function (data) {

                    appHelper.setPage(0);
                    entryList.ids = [];
                    appHelper.renderEntryRows(data.list, 'clear');

                    trigger.data('inProgress', 0);

                    appHelper.addLocationHash('p', appHelper.getPage());
                    appHelper.addLocationHash('s', search);
                    appHelper.addLocationHash('f', $('[name="field"]').val());
                });
            }

        }

    });

    // handle reset search form event
    $('#search-form .clear').on('click', function (event) {

        event.preventDefault();
        $('#search-form').find('[name="search"]').val('');

        appHelper.loadNewItems(appParams.listUrl, 0, appParams.viewCount, function (data) {

            appHelper.setPage(0);
            entryList.ids = [];
            appHelper.renderEntryRows(data.list, 'clear');

            appHelper.addLocationHash('p', appHelper.getPage());
            appHelper.removeFromLocationHash('s');
            appHelper.removeFromLocationHash('f');
        });
    });

    $('body').on('clearErrors', 'form', function () {

        $(this).find('.has-error').removeClass('has-error');
        $(this).find('.bg-danger').remove();

    }).on('addErrors', 'form', function (event, errorFields) {

        for (var fieldName in errorFields) {

            var field = $(this).find('[name="' + fieldName + '"]');
            field.parent().addClass('has-error');
            field.after($('<div class="bg-danger">').html(errorFields[fieldName].join('<br/>')));
        }
    });

    // all actions are saved to url hash. It allows as to share some result by links
    // Here we parse url hash wher page is loaded
    var initHash = function ()
    {
        var currentHash = location.hash.replace('#', '').split('|');

        var needLoad = false;
        var hashConfig = {};
        
        for (var k in currentHash)
        {
            var opt = currentHash[k];
            if(opt != '' && opt != '#')
            {              
                if(/[a-z]\:[a-zа-я0-9]+/i.test(opt))
                {
                    opt = opt.split(':');
                    hashConfig[opt[0]] = opt[1];
                }
            }
        }

        //  fill search form
        if (hashConfig.hasOwnProperty('s') && hashConfig.hasOwnProperty('f'))
        {
            needLoad = true;

            $('[name="search"]').val(hashConfig.s);
            $('[name="field"]').val(hashConfig.f);
        }

        //  define base order
        if (hashConfig.hasOwnProperty('o') && hashConfig.hasOwnProperty('b'))
        {
            needLoad = true;

            var link = $('#' + hashConfig.b + '.change-order');
            link.addClass('active ' + hashConfig.o.toLowerCase())

            appHelper.updateOrder(hashConfig.o.toUpperCase(), hashConfig.b);
        }

        var page = hashConfig.hasOwnProperty('p') ? parseInt(hashConfig.p) : 0;
        
        if (needLoad || page != 0)
        {
            appHelper.loadNewItems(appParams.listUrl, 0, (page + 1) * appParams.viewCount, function (data) {

                entryList.ids = [];
                appHelper.renderEntryRows(data.list, 'clear', false);
                appHelper.setPage(page);

            });
        }
    }


    initHash();
}
