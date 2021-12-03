// Call the dataTables jQuery plugin
$(document).ready(function() {
    $('#indexTable').DataTable();
});




$('#deleteModal').on('show.bs.modal', function (event) {
    var button = $(event.relatedTarget)
    var id = button.data('id')
    var name = button.data('name')   

    var modal = $(this)
    modal.find('.modal-title').text('Delete ' + name)
    modal.find('.modal-body-content').text('Are you sure to delete  ' + name + ' ?')
    modal.find('.modal-body input').val(id)
})