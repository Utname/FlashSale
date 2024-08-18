function ShowImagePreviewAnhBia(imageUploader, previewAnhBia) {
    if (imageUploader.files && imageUploader.files[0]) {
        var reader = new FileReader();
        reader.onload = function (e) {
            $(previewAnhBia).attr('src', e.target.result);
        }
        reader.readAsDataURL(imageUploader.files[0]);
    }
}

function ShowImagePreviewAnhDaiDien(imageUploader, previewAnhDaiDien) {
    if (imageUploader.files && imageUploader.files[0]) {
        var reader = new FileReader();
        reader.onload = function (e) {
            $(previewAnhDaiDien).attr('src', e.target.result);
        }
        reader.readAsDataURL(imageUploader.files[0]);
    }
}

function ShowImagePreviewImage(imageUploader, previewImage) {
    if (imageUploader.files && imageUploader.files[0]) {
        var reader = new FileReader();
        reader.onload = function (e) {
            $(previewImage).attr('src', e.target.result);
        }
        reader.readAsDataURL(imageUploader.files[0]);
    }
}






function UpdateStatusDel(id, statusDel, controller) {

    var jsonData = {
        id: id,
        statusDel: statusDel,
    }
    $.ajax({
        url: `/Admin/${controller}/UpdateStatusDel`,
        type: "GET",
        dataType: "json",
        data: jsonData,
        success: function (data) {
            // Model = data;
            location.reload();
        }
    })
}

function DownloadExcel(controller) {

    var fullUrl = `/Admin/${controller}/DownloadExcel`;
    // Redirect to the URL or perform any other action
    window.location.href = fullUrl;

}

$(document).ready(function () {
    $.fn.DataTable.ext.pager.numbers_length = 5;

    $('#example').DataTable({
        "paging": false, // Tắt phân trang mặc định của DataTables
        "info": false, // Tắt thông tin tổng số bản ghi (total records)
    });
});

function formatNumberCommon(input) {
    let value = input.value.replace(/,/g, ''); // Remove existing commas
    if (value) {
        input.value = parseFloat(value).toLocaleString('en-US'); // Format number with commas
    } else {
        input.value = '';
    }
}
