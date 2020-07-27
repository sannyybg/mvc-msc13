$(function () {
    $("ul img").mouseover(function () {
        $(this).animate({ height: '+=10', width: '+=10' })
    });
});

$(function () {
    $("ul img").mouseout(function () {
        $(this).animate({ height: '-=10', width: '-=10' })
    });
});

$(function () {
    $(".add-picture input").change(function () {
        var previewId = $(this).siblings('img').attr('id');

        if (this.files && this.files[0]) {
            var reader = new FileReader();
            reader.onload = function (e) {
                $('#' + previewId).attr('src', e.target.result);
            }

            reader.readAsDataURL(this.files[0]);
        }
    });
});



$(function () {
    $("#users a").click(function () {
        var userId = $(this).attr('id');
        $.post('User/ChangeRole', { id: userId }, function (data) {
            $("#users #" + userId).text(data);
        });

    });
});


$(function () {
    $("#add-artist-form").on('submit', function (e) {

        e.preventDefault();
        var files = $('#artinputphoto').get(0).files;

        var data = new FormData;
        data.append('Photo', files[0]);
        data.append('Name', $('#add-artist-form input[name="Name"]').val());
        data.append('BasicInfo', $('#add-artist-form input[name="BasicInfo"]').val());

        $.ajax({
            url: "/StoreManager/AddArtists1",
            data: data,
            type: "POST",
            contentType: false,
            processData: false,
            error: function (error) {
                alert(error.statusText);
            },
            success: function (result) {
                var list = $('#ArtistId');
                var option = '<option value="' + result.ArtistId + '">' + result.Name + '</option>';

                list.append(option);

                alert("Artist Added");

                $('.close').click();
            }

        });
    });
});


$(function () {
    $("#album-search").keyup(function () {
        $('#search-results').html('');
        var searchField = $(this).val();

        var url = '/StoreManager/AlbumList?parameter=' + searchField;


        $.getJSON(url, function (data) {
            $.each(data, function (i, field) {

                var appendData = '<li class="list-group-item"><img class="image-thumbnail" src="' + field.AlbumUrl + ' " width="50"/>' + field.Title + '</li>'
                $('#search-results').append(appendData);

            });
        });

    });
});



$(function () {
    $("#user-search").keyup(function () {
        $('#search-results').html('');
        var searchField = $(this).val();

        var url = '/User/UserList?parameter=' + searchField;


        $.getJSON(url, function (data) {
            $.each(data, function (i, field) {

                var appendData = '<li class="list-group-item">' + field.Username + '</li>'
                $('#search-results').append(appendData);

            });
        });

    });
});