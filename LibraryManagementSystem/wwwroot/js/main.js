$(document).ready(function () {

    //Fields for custom filtering
    var searchtext = $('#searchtext');
    var searchprice = $('#searchprice');
    var rackid = $('#rackId');
    var shelfid = $('#shelfId');
    var searchavailable = $('#searchavailable');

    //Apply Custom filtering
    $('.btn-search').click(function (e) {

        var table = $('#bookstable').DataTable();
        table.draw();
        e.preventDefault();
    });

    //Clear Custom filtering
    $('.btn-cancel').click(function (e) {
        clearSearch();
        var table = $('#bookstable').DataTable();
        table.draw();
        e.preventDefault();
    });

    //Initilize books datatable 
    loadBooks();
    
    function loadBooks() {
        $("#bookstable").DataTable({
            "processing": true, // for show progress bar
            "serverSide": true, // for process server side
            "filter": true, // this is for disable filter (search box)
            "orderMulti": false, // for disable multiple column at once
            "ajax": {
                "url": "/Books/GetData",
                "type": "POST",
                "datatype": "json",
                "data": function (d) {
                    d.text = searchtext.val(),
                        d.price = searchprice.val(),
                        d.rackid = rackid.val(),
                        d.shelfid = shelfid.val(),
                        d.available = searchavailable.is(':checked')
                }
            },
            "columnDefs": [{
                "targets": [0],
                "visible": false,
                "searchable": true
            }],
            "columns": [
                { "data": "bookId", "name": "BookId", "autoWidth": true },
                { "data": "code", "name": "Code", "autoWidth": true },
                { "data": "name", "name": "Name", "autoWidth": true },
                { "data": "author", "name": "Author", "autoWidth": true },
                { "data": "price", "name": "Price", "autoWidth": true },
                {
                    "data": "isAvailable", "name": "Available", "autoWidth": true, "sortable": false, render: function (data, type, row) {
                        if (row.isAvailable == true)
                        {
                            return '<span class="px-2 rounded bg-success text-white">Available</span>';
                        }
                        else
                        {
                            return '<span class="px-2 rounded bg-danger text-white">Not available</span>';
                        }
                    }
                },
                { "data": "shelf", "name": "Shelf", "autoWidth": true },
                { "data": "shelfId", "name": "Shelf", "visible": false },
                {
                    data: null,
                    render: function (data, type, row) {
                        return "<a href='#' class='btn btn-sm btn-info mx-1 btn-view' >View</a><a href='#' class='btn btn-sm btn-primary mx-1 btn-edit' >Edit</a><a href='#' class='btn btn-sm btn-danger mx-1 btn-delete' data-id='" + row.bookId + "' >Delete</a>";
                    },
                    "sortable": false
                },
            ],
            "drawCallback": function (settings) {
                
                var response = settings.json;
                $('.total').html(response.totalPrice);
            },
            "dom":'Bfrtip',
            "buttons": [
                {
                text: 'Export PDF',
                    extend: 'pdfHtml5',
                    exportOptions: {
                        columns: [0, 1, 2, 3, 4, 5, 6]
                    }
                }
            ]
        });

    }

    //Event handler for book row edit button click
    $('body').on('click', '#bookstable tbody tr .btn-edit', function () {

        var table = $('#bookstable').DataTable();
        var row = table.row($(this).parents('tr')).data();

        $('#idEdit').val(row.bookId);
        $('#codeEdit').val(row.code);
        $('#nameEdit').val(row.name);
        $('#authorEdit').val(row.author);
        $('#priceEdit').val(row.price);
        $('#shelfEdit').val(row.shelfId);
        $('#availableEdit').prop('checked', row.isAvailable);
        $('#editModal').modal('show');

    });

    //Event handler for book row view button click
    $('body').on('click', '#bookstable tbody tr .btn-view', function () {

        var table = $('#bookstable').DataTable();
        var row = table.row($(this).parents('tr')).data();

        $('#idView').val(row.bookId);
        $('#codeView').val(row.code);
        $('#nameView').val(row.name);
        $('#authorView').val(row.author);
        $('#priceView').val(row.price);
        $('#shelfView').val(row.shelfId);
        $('#availableView').prop('checked', row.isAvailable);
        $('#viewModal').modal('show');

    });

    //Validate add new book form
    $('#addForm').bootstrapValidator({
        onSuccess: function (e) {
            addBook();
            e.preventDefault();
        },
        submitButtons: '.btn-add',
        feedbackIcons: {
            valid: 'fa fa-check',
            invalid: 'fa fa-remove',
            validating: 'fa fa-refresh fa-spin'
        },
        fields: {
            code: {
                validators: {
                    notEmpty: {
                        message: 'Enter book code'
                    }
                }
            },
            name: {
                validators: {
                    notEmpty: {
                        message: 'Enter book name'
                    }
                }
            },
            author: {
                validators: {
                    notEmpty: {
                        message: 'Enter book author'
                    }
                }
            },
            shelf: {
                validators: {
                    notEmpty: {
                        message: 'Select a shelf'
                    }
                }
            },
            price: {
                validators: {
                    notEmpty: {
                        message: 'Enter book price'
                    },
                    numeric: {
                        message: 'Price must be numeric'
                    }
                }
            }


        }
    });

    //Validate edit book form
    $('#updateForm').bootstrapValidator({
        onSuccess: function (e) {
            updateBook();
            e.preventDefault();
        },
        submitButtons: '.btn-update',
        feedbackIcons: {
            valid: 'fa fa-check',
            invalid: 'fa fa-remove',
            validating: 'fa fa-refresh fa-spin'
        },
        fields: {
            codeEdit: {
                validators: {
                    notEmpty: {
                        message: 'Enter book code'
                    }
                }
            },
            nameEdit: {
                validators: {
                    notEmpty: {
                        message: 'Enter book name'
                    }
                }
            },
            authorEdit: {
                validators: {
                    notEmpty: {
                        message: 'Enter book author'
                    }
                }
            },
            shelfEdit: {
                validators: {
                    notEmpty: {
                        message: 'Select a shelf'
                    }
                }
            },
            priceEdit: {
                validators: {
                    notEmpty: {
                        message: 'Enter book price'
                    },
                    numeric: {
                        message: 'Price must be numeric'
                    }
                }
            }


        }
    });

    //Add new book
    function addBook() {

        var book = {
            "Code": $('#code').val(),
            "Name": $('#name').val(),
            "Author": $('#author').val(),
            "Price": $('#price').val(),
            "ShelfId": $('#shelf').val(),
            "IsAvailable": $('#available').is(':checked')
        };

        $.ajax({
            url: "/Books/Add",
            type: "POST",
            data: JSON.stringify(book),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            error: function (response) {
                toastr.error(response.message, response.title);
            },
            success: function (response) {

                if (response.status == 'success') {
                    clearForm();
                    $('#addModal').modal('hide');
                    toastr.success(response.message, response.title);
                    var table = $('#bookstable').DataTable();
                    table.ajax.reload();
                }
                else {
                    toastr.error(response.message, response.title);
                }

            }
        });

    }

    //Edit book
    function updateBook() {

        var book = {
            "BookId": $('#idEdit').val(),
            "Code": $('#codeEdit').val(),
            "Name": $('#nameEdit').val(),
            "Author": $('#authorEdit').val(),
            "Price": $('#priceEdit').val(),
            "ShelfId": $('#shelfEdit').val(),
            "IsAvailable": $('#availableEdit').is(':checked')
        };

        $.ajax({
            url: "/Books/Update",
            type: "POST",
            data: JSON.stringify(book),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            error: function (response) {
                toastr.error(response.message, response.title);
            },
            success: function (response) {

                if (response.status == 'success') {

                    $('#editModal').modal('hide');
                    toastr.success(response.message, response.title);
                    var table = $('#bookstable').DataTable();
                    table.draw();
                }
                else {
                    toastr.error(response.message, response.title);
                }

            }
        });

    }

    //Event handler for book deleting button click
    $('body').on('click', '.btn-delete', function () {
        var bookId = $(this).data('id');
        if (confirm("Are you sure you want to delete this book?")) {
            var book = {
                "BookId": bookId
            }

            $.ajax({
                url: "/Books/Delete",
                type: "POST",
                data: JSON.stringify(book),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                error: function (response) {
                    toastr.error(response.message, response.title);
                },
                success: function (response) {

                    if (response.status == 'success') {


                        toastr.success(response.message, response.title);
                        var table = $('#bookstable').DataTable();
                        table.draw();
                    }
                    else {
                        toastr.error(response.message, response.title);
                    }

                }
            });


        } else {
            return false;
        }
    });

    //Clear the form for adding new book 
    function clearForm()
    {
            $('#code').val('');
            $('#name').val('');
            $('#author').val('');
            $('#price').val('');
            $('#shelf').val('');
            $('#available').prop('checked', false);
    }

    //Clear custom filtering fields
    function clearSearch() {
        $('#searchtext').val('');
        $('#searchprice').val('');
        $('#rackId').val('');
        $('#shelfId').val('');
        $('#searchavailable').prop('checked', false);
    }


});

(function ($) {
    "use strict";

    
    // Sticky Navbar
    $(window).scroll(function () {
        if ($(this).scrollTop() > 300) {
            $('.sticky-top').addClass('shadow-sm').css('top', '0px');
        } else {
            $('.sticky-top').removeClass('shadow-sm').css('top', '-100px');
        }
    });


    // Back to top button
    $(window).scroll(function () {
        if ($(this).scrollTop() > 300) {
            $('.back-to-top').fadeIn('slow');
        } else {
            $('.back-to-top').fadeOut('slow');
        }
    });
    $('.back-to-top').click(function () {
        $('html, body').animate({ scrollTop: 0 }, 1500, 'easeInOutExpo');
        return false;
    });


})(jQuery);

