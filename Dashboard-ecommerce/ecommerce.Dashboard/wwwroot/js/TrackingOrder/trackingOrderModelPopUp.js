$(document).on("click", ".open-AddBookDialog", function () {
    var TrackingId = $(this).data('id');
    $(".modal-body #trackingId").val(TrackingId);

    var orderId = $(this).data('orderid');
    $(".modal-body #orderId").val(orderId);

    var trackingStatus = $(this).data('trackingstatus');
    $(".modal-body #trackingStatus").val(trackingStatus);

    var courierTrackingNumber = $(this).data('couriertrackingnumber');
    $(".modal-body #courierTrackingNumber").val(courierTrackingNumber);

    var curierCopmany = $(this).data('curiercopmany');
    $(".modal-body #curierCopmany").val(curierCopmany);

    var trackingUrl = $(this).data('trackingurl');
    $(".modal-body #trackingUrl").val(trackingUrl);

    var email = $(this).data('email');
    $(".modal-body #email").val(email);

    var expectedArrivalDate = $(this).data('expectedarrivaldate');
    $(".modal-body #expectedArrivalDate").val(expectedArrivalDate);

    $('#addBookDialog').modal('show');
});