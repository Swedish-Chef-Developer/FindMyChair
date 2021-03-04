$(document).ready(function () {
	$('#filtersortform').submit(function () {
		$.ajax({
			type: this.method,
			url: this.action,
			data: $(this).serialize(),
			dataType: 'html',
			success: function (response) {
				$('#aa #list-content').html(response);
			},
			error: function (error) {
				console.log(error);
			}
		});
		return false;
	});
});